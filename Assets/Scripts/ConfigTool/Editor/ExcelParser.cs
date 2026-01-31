using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Data;
using ExcelDataReader;
using System.Text;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Excel文件解析器
/// 负责读取Excel文件并解析为配置数据结构
/// </summary>
public static class ExcelParser
{
    /// <summary>
    /// 日志回调委托
    /// </summary>
    public delegate void LogCallback(string message, LogType logType = LogType.Log);

    #region 常量定义
    
    // 行索引定义（基于0）
    private const int DESCRIPTION_ROW = 0;  // 描述行
    private const int FIELD_NAME_ROW = 1;   // 字段名行
    private const int FIELD_TYPE_ROW = 2;   // 字段类型行
    private const int DATA_START_ROW = 3;   // 数据开始行
    
    // 类型识别正则
    private static readonly Regex ARRAY_TYPE_REGEX = new Regex(@"^(.+)\[\]$");
    private static readonly Regex COMPLEX_TYPE_REGEX = new Regex(@"^[{].*[}]$");
    
    #endregion

    #region 文件访问辅助方法

    /// <summary>
    /// 尝试打开Excel文件，支持多种打开模式
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="logCallback">日志回调</param>
    /// <returns>文件流</returns>
    private static FileStream TryOpenExcelFile(string filePath, LogCallback logCallback = null)
    {
        // 方案1: 尝试使用共享读取模式
        try
        {
            LogMessage(logCallback, "尝试使用共享读取模式打开Excel文件...", LogType.Log);
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        catch (Exception ex1)
        {
            LogMessage(logCallback, $"共享读取模式失败: {ex1.Message}", LogType.Warning);
        }

        // 方案2: 复制到临时文件
        try
        {
            LogMessage(logCallback, "尝试复制文件到临时位置...", LogType.Log);
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"ConfigTool_{Guid.NewGuid()}.xlsx");
            
            // 使用File.Copy复制文件
            File.Copy(filePath, tempFilePath, true);
            LogMessage(logCallback, $"文件已复制到临时位置: {tempFilePath}", LogType.Log);
            
            // 返回临时文件的流，并标记为删除
            var tempStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.DeleteOnClose);
            return tempStream;
        }
        catch (Exception ex2)
        {
            LogMessage(logCallback, $"复制文件失败: {ex2.Message}", LogType.Warning);
        }

        // 方案3: 使用只读模式重试
        try
        {
            LogMessage(logCallback, "尝试使用只读模式打开文件...", LogType.Log);
            return File.OpenRead(filePath);
        }
        catch (Exception ex3)
        {
            LogMessage(logCallback, $"只读模式失败: {ex3.Message}", LogType.Error);
            throw new IOException($"无法打开Excel文件: {filePath}\n" +
                                 $"请确保:\n" +
                                 $"1. 文件存在且有读取权限\n" +
                                 $"2. 如果文件被Excel打开，请关闭Excel后重试\n" +
                                 $"3. 或者另存为一个新文件进行导入\n" +
                                 $"错误详情: {ex3.Message}");
        }
    }

    /// <summary>
    /// 检查文件是否被其他程序占用
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>是否被占用</returns>
    public static bool IsFileInUse(string filePath)
    {
        try
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                return false;
            }
        }
        catch (IOException)
        {
            return true;
        }
    }

    #endregion

    #region 公共接口

    /// <summary>
    /// 解析Excel文件
    /// </summary>
    /// <param name="filePath">Excel文件路径</param>
    /// <param name="logCallback">日志回调函数</param>
    /// <returns>解析后的表格信息列表</returns>
    public static List<ExcelSheetInfo> ParseExcelFile(string filePath, LogCallback logCallback = null)
    {
        if (!File.Exists(filePath))
        {
            LogMessage(logCallback, $"Excel文件不存在：{filePath}", LogType.Error);
            throw new FileNotFoundException($"Excel文件不存在：{filePath}");
        }

        var sheetInfos = new List<ExcelSheetInfo>();

        try
        {
            // 尝试使用共享读取模式，允许其他程序同时访问文件
            using (var stream = TryOpenExcelFile(filePath, logCallback))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = false // 不使用第一行作为列名
                        }
                    });

                    // 遍历所有工作表
                    foreach (DataTable table in dataSet.Tables)
                    {
                        try
                        {
                            var sheetInfo = ParseWorksheet(table, logCallback);
                            if (sheetInfo != null && sheetInfo.Fields.Count > 0)
                            {
                                sheetInfos.Add(sheetInfo);
                                LogMessage(logCallback, $"成功解析工作表：{sheetInfo.SheetName}");
                            }
                        }
                        catch (Exception e)
                        {
                            LogMessage(logCallback, $"解析工作表 {table.TableName} 失败：{e.Message}", LogType.Warning);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            LogMessage(logCallback, $"读取Excel文件失败：{e.Message}", LogType.Error);
            throw new Exception($"Excel文件读取错误：{e.Message}。请确保已安装ExcelDataReader相关依赖包。");
        }

        return sheetInfos;
    }

    /// <summary>
    /// 日志输出辅助方法
    /// </summary>
    private static void LogMessage(LogCallback logCallback, string message, LogType logType = LogType.Log)
    {
        if (logCallback != null)
        {
            logCallback(message, logType);
        }
        else
        {
            // 如果没有回调，使用默认的Debug.Log
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 解析单个工作表
    /// </summary>
    /// <param name="table">数据表</param>
    /// <param name="logCallback">日志回调函数</param>
    /// <returns>表格信息</returns>
    private static ExcelSheetInfo ParseWorksheet(DataTable table, LogCallback logCallback = null)
    {
        if (table.Rows.Count < DATA_START_ROW + 1)
        {
            LogMessage(logCallback, $"工作表 {table.TableName} 数据行不足，跳过解析", LogType.Warning);
            return null;
        }

        var sheetInfo = new ExcelSheetInfo
        {
            SheetName = SanitizeClassName(table.TableName)
        };

        // 解析字段信息
        ParseFieldInfo(table, sheetInfo, logCallback);

        if (sheetInfo.Fields.Count == 0)
        {
            LogMessage(logCallback, $"工作表 {table.TableName} 没有有效字段，跳过解析", LogType.Warning);
            return null;
        }

        // 解析数据行
        ParseDataRows(table, sheetInfo, logCallback);

        sheetInfo.FieldCount = sheetInfo.Fields.Count;
        sheetInfo.DataRowCount = sheetInfo.DataRows.Count;

        return sheetInfo;
    }

    /// <summary>
    /// 解析字段信息
    /// </summary>
    /// <param name="table">数据表</param>
    /// <param name="sheetInfo">表格信息</param>
    /// <param name="logCallback">日志回调函数</param>
    private static void ParseFieldInfo(DataTable table, ExcelSheetInfo sheetInfo, LogCallback logCallback = null)
    {
        var descRow = table.Rows[DESCRIPTION_ROW];
        var nameRow = table.Rows[FIELD_NAME_ROW];
        var typeRow = table.Rows[FIELD_TYPE_ROW];

        int columnCount = table.Columns.Count;

        for (int col = 0; col < columnCount; col++)
        {
            // 获取字段信息
            string description = GetCellValue(descRow, col);
            string fieldName = GetCellValue(nameRow, col);
            string fieldType = GetCellValue(typeRow, col);

            // 跳过空的字段
            if (string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(fieldType))
            {
                continue;
            }

            // 验证字段名格式
            if (!IsValidFieldName(fieldName))
            {
                LogMessage(logCallback, $"无效的字段名：{fieldName}，跳过此字段", LogType.Warning);
                continue;
            }

            var fieldInfo = new FieldInfo
            {
                Name = fieldName.Trim(),
                Type = ParseFieldType(fieldType.Trim()),
                Description = string.IsNullOrWhiteSpace(description) ? fieldName : description.Trim()
            };

            // 分析字段特性
            AnalyzeFieldType(fieldInfo);

            sheetInfo.Fields.Add(fieldInfo);
        }
    }

    /// <summary>
    /// 解析数据行
    /// </summary>
    /// <param name="table">数据表</param>
    /// <param name="sheetInfo">表格信息</param>
    /// <param name="logCallback">日志回调函数</param>
    private static void ParseDataRows(DataTable table, ExcelSheetInfo sheetInfo, LogCallback logCallback = null)
    {
        int fieldCount = sheetInfo.Fields.Count;

        for (int row = DATA_START_ROW; row < table.Rows.Count; row++)
        {
            var dataRow = table.Rows[row];
            var rowData = new Dictionary<string, object>();

            bool hasValidData = false;

            for (int col = 0; col < fieldCount && col < table.Columns.Count; col++)
            {
                var field = sheetInfo.Fields[col];
                string cellValue = GetCellValue(dataRow, col);

                // 解析单元格数据
                object parsedValue = ParseCellValue(cellValue, field, logCallback);
                rowData[field.Name] = parsedValue;

                if (parsedValue != null && !string.IsNullOrEmpty(parsedValue.ToString()))
                {
                    hasValidData = true;
                }
            }

            // 只添加包含有效数据的行
            if (hasValidData)
            {
                sheetInfo.DataRows.Add(rowData);
            }
        }
    }

    /// <summary>
    /// 获取单元格值
    /// </summary>
    /// <param name="row">数据行</param>
    /// <param name="columnIndex">列索引</param>
    /// <returns>单元格字符串值</returns>
    private static string GetCellValue(DataRow row, int columnIndex)
    {
        if (columnIndex >= row.Table.Columns.Count)
        {
            return string.Empty;
        }

        var value = row[columnIndex];
        if (value == null || value == DBNull.Value)
        {
            return string.Empty;
        }
        
        return value.ToString().Trim();
    }

    /// <summary>
    /// 解析字段类型
    /// </summary>
    /// <param name="typeString">类型字符串</param>
    /// <returns>标准化的类型字符串</returns>
    private static string ParseFieldType(string typeString)
    {
        if (string.IsNullOrWhiteSpace(typeString))
        {
            return "string";
        }

        typeString = typeString.ToLower().Trim();

        // 类型映射
        switch (typeString)
        {
            case "int":
            case "integer":
            case "number":
                return "int";
            case "string":
            case "text":
                return "string";
            case "float":
            case "double":
                return "float";
            case "bool":
            case "boolean":
                return "bool";
            default:
                // 数组类型检查
                if (typeString.EndsWith("[]"))
                {
                    return typeString;
                }
                // 复杂类型（JSON格式）
                if (typeString.StartsWith("{") && typeString.EndsWith("}"))
                {
                    return "object";
                }
                return "string"; // 默认为字符串类型
        }
    }

    /// <summary>
    /// 分析字段类型特性
    /// </summary>
    /// <param name="fieldInfo">字段信息</param>
    private static void AnalyzeFieldType(FieldInfo fieldInfo)
    {
        // 检查是否为数组类型
        if (fieldInfo.Type.EndsWith("[]"))
        {
            fieldInfo.IsArray = true;
        }

        // 检查是否为复杂类型
        if (fieldInfo.Type == "object")
        {
            fieldInfo.IsComplex = true;
        }
    }

    /// <summary>
    /// 解析单元格值
    /// </summary>
    /// <param name="cellValue">单元格字符串值</param>
    /// <param name="field">字段信息</param>
    /// <param name="logCallback">日志回调函数</param>
    /// <returns>解析后的值</returns>
    private static object ParseCellValue(string cellValue, FieldInfo field, LogCallback logCallback = null)
    {
        if (string.IsNullOrWhiteSpace(cellValue))
        {
            return GetDefaultValue(field);
        }

        try
        {
            // 数组类型处理
            if (field.IsArray)
            {
                return ParseArrayValue(cellValue, field, logCallback);
            }

            // 复杂类型处理
            if (field.IsComplex)
            {
                return ParseComplexValue(cellValue, logCallback);
            }

            // 基础类型处理
            return ParseBasicValue(cellValue, field.Type, logCallback);
        }
        catch (Exception e)
        {
            LogMessage(logCallback, $"解析单元格值失败：{cellValue}, 字段：{field.Name}, 错误：{e.Message}", LogType.Warning);
            return GetDefaultValue(field);
        }
    }

    /// <summary>
    /// 解析数组值
    /// </summary>
    /// <param name="cellValue">单元格值</param>
    /// <param name="field">字段信息</param>
    /// <param name="logCallback">日志回调函数</param>
    /// <returns>数组对象</returns>
    private static object ParseArrayValue(string cellValue, FieldInfo field, LogCallback logCallback = null)
    {
        string content = cellValue;
        
        // 支持两种格式：
        // 1. 带花括号格式：{element1, element2, element3}（向后兼容）
        // 2. 不带花括号格式：element1, element2, element3（新格式）
        if (cellValue.StartsWith("{") && cellValue.EndsWith("}"))
        {
            // 去掉花括号
            content = cellValue.Substring(1, cellValue.Length - 2);
        }

        string[] elements = content.Split(',');
        
        string elementType = field.Type.Replace("[]", "");
        var resultList = new List<object>();

        foreach (string element in elements)
        {
            string trimmedElement = element.Trim();
            if (!string.IsNullOrEmpty(trimmedElement))
            {
                object parsedElement = ParseBasicValue(trimmedElement, elementType, logCallback);
                resultList.Add(parsedElement);
            }
        }

        return resultList.ToArray();
    }

    /// <summary>
    /// 解析复杂值（JSON格式）
    /// </summary>
    /// <param name="cellValue">单元格值</param>
    /// <param name="logCallback">日志回调函数</param>
    /// <returns>JSON对象</returns>
    private static object ParseComplexValue(string cellValue, LogCallback logCallback = null)
    {
        try
        {
            // 优先解析为JObject，用于代码生成阶段的类型识别
            if (cellValue.Trim().StartsWith("{") && cellValue.Trim().EndsWith("}"))
            {
                return Newtonsoft.Json.Linq.JObject.Parse(cellValue);
            }
            
            // 使用Newtonsoft.Json解析
            return Newtonsoft.Json.JsonConvert.DeserializeObject(cellValue);
        }
        catch (Exception e)
        {
            LogMessage(logCallback, $"JSON解析失败：{cellValue}, 错误：{e.Message}", LogType.Warning);
            return new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// 解析基础类型值
    /// </summary>
    /// <param name="cellValue">单元格值</param>
    /// <param name="type">类型</param>
    /// <param name="logCallback">日志回调函数</param>
    /// <returns>解析后的值</returns>
    private static object ParseBasicValue(string cellValue, string type, LogCallback logCallback = null)
    {
        switch (type.ToLower())
        {
            case "int":
                return int.TryParse(cellValue, out int intValue) ? intValue : 0;
            case "float":
                return float.TryParse(cellValue, out float floatValue) ? floatValue : 0f;
            case "bool":
                return bool.TryParse(cellValue, out bool boolValue) ? boolValue : false;
            case "string":
            default:
                return cellValue;
        }
    }

    /// <summary>
    /// 获取字段的默认值
    /// </summary>
    /// <param name="field">字段信息</param>
    /// <returns>默认值</returns>
    private static object GetDefaultValue(FieldInfo field)
    {
        if (field.IsArray)
        {
            return new object[0];
        }

        if (field.IsComplex)
        {
            return new Dictionary<string, object>();
        }

        switch (field.Type.ToLower())
        {
            case "int":
                return 0;
            case "float":
                return 0f;
            case "bool":
                return false;
            case "string":
            default:
                return string.Empty;
        }
    }

    /// <summary>
    /// 验证字段名是否有效
    /// </summary>
    /// <param name="fieldName">字段名</param>
    /// <returns>是否有效</returns>
    private static bool IsValidFieldName(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            return false;
        }

        // C#标识符验证
        var regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
        return regex.IsMatch(fieldName.Trim());
    }

    /// <summary>
    /// 清理类名，确保符合C#命名规范
    /// </summary>
    /// <param name="name">原始名称</param>
    /// <returns>清理后的名称</returns>
    private static string SanitizeClassName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "DefaultConfig";
        }

        // 移除非字母数字字符
        var regex = new Regex(@"[^a-zA-Z0-9_]");
        string sanitized = regex.Replace(name.Trim(), "");

        // 确保以字母开头
        if (string.IsNullOrEmpty(sanitized) || (!char.IsLetter(sanitized[0]) && sanitized[0] != '_'))
        {
            sanitized = "Config" + sanitized;
        }

        return sanitized;
    }

    #endregion
}