using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 配置数据生成器
/// 将Excel表格数据转换为JSON格式供运行时加载
/// </summary>
public class ConfigDataGenerator
{
    #region 公共接口

    /// <summary>
    /// 生成JSON数据
    /// </summary>
    /// <param name="sheetInfo">表格信息</param>
    /// <returns>JSON字符串</returns>
    public string GenerateJsonData(ExcelSheetInfo sheetInfo)
    {
        try
        {
            var configItems = new List<Dictionary<string, object>>();
            
            // 转换每一行数据
            foreach (var dataRow in sheetInfo.DataRows)
            {
                var configItem = ConvertDataRow(dataRow, sheetInfo.Fields);
                if (configItem.Count > 0)
                {
                    configItems.Add(configItem);
                }
            }
            
            // 创建包装对象
            var wrapperObject = new Dictionary<string, object>
            {
                ["items"] = configItems
            };
            
            // 序列化为JSON
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include
            };
            
            string jsonContent = JsonConvert.SerializeObject(wrapperObject, jsonSettings);
            
            Debug.Log($"成功生成JSON数据：{sheetInfo.SheetName}，共{configItems.Count}条记录");
            
            return jsonContent;
        }
        catch (Exception e)
        {
            Debug.LogError($"生成JSON数据失败：{e.Message}");
            throw;
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 转换数据行
    /// </summary>
    /// <param name="dataRow">原始数据行</param>
    /// <param name="fields">字段定义</param>
    /// <returns>转换后的数据字典</returns>
    private Dictionary<string, object> ConvertDataRow(Dictionary<string, object> dataRow, List<FieldInfo> fields)
    {
        var configItem = new Dictionary<string, object>();
        
        foreach (var field in fields)
        {
            if (dataRow.TryGetValue(field.Name, out var rawValue))
            {
                object convertedValue = ConvertFieldValue(rawValue, field);
                
                // 特殊处理ID字段：在JSON中使用原始字段名，但映射到IDValue属性
                if (field.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) ||
                    field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                {
                    configItem[field.Name + "Value"] = convertedValue;
                }
                else
                {
                    configItem[field.Name] = convertedValue;
                }
            }
            else
            {
                // 如果字段没有值，设置默认值
                if (field.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) ||
                    field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                {
                    configItem[field.Name + "Value"] = GetDefaultValue(field);
                }
                else
                {
                    configItem[field.Name] = GetDefaultValue(field);
                }
            }
        }
        
        return configItem;
    }

    /// <summary>
    /// 转换字段值
    /// </summary>
    /// <param name="rawValue">原始值</param>
    /// <param name="field">字段信息</param>
    /// <returns>转换后的值</returns>
    private object ConvertFieldValue(object rawValue, FieldInfo field)
    {
        if (rawValue == null)
        {
            return GetDefaultValue(field);
        }

        try
        {
            // 数组类型处理
            if (field.IsArray)
            {
                return ConvertArrayValue(rawValue, field);
            }

            // 复杂类型处理
            if (field.IsComplex)
            {
                return ConvertComplexValue(rawValue);
            }

            // 基础类型处理
            return ConvertBasicValue(rawValue, field.Type);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"转换字段值失败：{rawValue}, 字段：{field.Name}, 错误：{e.Message}");
            return GetDefaultValue(field);
        }
    }

    /// <summary>
    /// 转换数组值
    /// </summary>
    /// <param name="rawValue">原始值</param>
    /// <param name="field">字段信息</param>
    /// <returns>转换后的数组</returns>
    private object ConvertArrayValue(object rawValue, FieldInfo field)
    {
        if (rawValue is object[] array)
        {
            string elementType = field.Type.Replace("[]", "");
            var convertedArray = new List<object>();
            
            foreach (var element in array)
            {
                var convertedElement = ConvertBasicValue(element, elementType);
                convertedArray.Add(convertedElement);
            }
            
            return convertedArray.ToArray();
        }
        
        // 如果不是数组，尝试作为字符串解析
        string stringValue = rawValue?.ToString() ?? "";
        if (!string.IsNullOrEmpty(stringValue))
        {
            string content = stringValue;
            
            // 支持两种格式：
            // 1. 带花括号格式：{1,2,3}（向后兼容）
            // 2. 不带花括号格式：1,2,3（新格式）
            if (stringValue.StartsWith("{") && stringValue.EndsWith("}"))
            {
                // 去掉花括号
                content = stringValue.Substring(1, stringValue.Length - 2);
            }
            
            string[] elements = content.Split(',');
            
            string elementType = field.Type.Replace("[]", "");
            var resultArray = new List<object>();
            
            foreach (string element in elements)
            {
                string trimmedElement = element.Trim();
                if (!string.IsNullOrEmpty(trimmedElement))
                {
                    object convertedElement = ConvertBasicValue(trimmedElement, elementType);
                    resultArray.Add(convertedElement);
                }
            }
            
            return resultArray.ToArray();
        }
        
        return new object[0];
    }

    /// <summary>
    /// 转换复杂值
    /// </summary>
    /// <param name="rawValue">原始值</param>
    /// <returns>转换后的对象</returns>
    private object ConvertComplexValue(object rawValue)
    {
        if (rawValue is JObject jobj)
        {
            return jobj.ToObject<Dictionary<string, object>>();
        }
        
        if (rawValue is Dictionary<string, object> dict)
        {
            return dict;
        }
        
        // 尝试解析JSON字符串
        string jsonString = rawValue?.ToString() ?? "";
        if (!string.IsNullOrWhiteSpace(jsonString))
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"JSON解析失败：{jsonString}, 错误：{e.Message}");
            }
        }
        
        return new Dictionary<string, object>();
    }

    /// <summary>
    /// 转换基础类型值
    /// </summary>
    /// <param name="rawValue">原始值</param>
    /// <param name="type">目标类型</param>
    /// <returns>转换后的值</returns>
    private object ConvertBasicValue(object rawValue, string type)
    {
        if (rawValue == null)
        {
            return GetDefaultValueForType(type);
        }

        string stringValue = rawValue.ToString();
        
        switch (type.ToLower())
        {
            case "int":
                if (int.TryParse(stringValue, out int intValue))
                    return intValue;
                return 0;
                
            case "float":
                if (float.TryParse(stringValue, out float floatValue))
                    return floatValue;
                return 0f;
                
            case "bool":
                if (bool.TryParse(stringValue, out bool boolValue))
                    return boolValue;
                // 额外的布尔值判断
                string lowerValue = stringValue.ToLower();
                return lowerValue == "1" || lowerValue == "yes" || lowerValue == "true";
                
            case "string":
            default:
                return stringValue;
        }
    }

    /// <summary>
    /// 获取字段的默认值
    /// </summary>
    /// <param name="field">字段信息</param>
    /// <returns>默认值</returns>
    private object GetDefaultValue(FieldInfo field)
    {
        if (field.IsArray)
        {
            return new object[0];
        }
        
        if (field.IsComplex)
        {
            return new Dictionary<string, object>();
        }
        
        return GetDefaultValueForType(field.Type);
    }

    /// <summary>
    /// 根据类型获取默认值
    /// </summary>
    /// <param name="type">类型字符串</param>
    /// <returns>默认值</returns>
    private object GetDefaultValueForType(string type)
    {
        switch (type.ToLower())
        {
            case "int":
                return 0;
            case "float":
                return 0f;
            case "bool":
                return false;
            case "string":
            default:
                return "";
        }
    }

    #endregion

    #region 数据验证

    /// <summary>
    /// 验证生成的JSON数据
    /// </summary>
    /// <param name="jsonContent">JSON内容</param>
    /// <param name="sheetInfo">表格信息</param>
    /// <returns>验证结果</returns>
    public ValidationResult ValidateJsonData(string jsonContent, ExcelSheetInfo sheetInfo)
    {
        var result = new ValidationResult();
        
        try
        {
            // 解析JSON
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
            
            if (!jsonObject.ContainsKey("items"))
            {
                result.AddError("JSON结构错误：缺少items字段");
                return result;
            }
            
            var items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(
                jsonObject["items"].ToString());
            
            // 验证每一条记录
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                ValidateConfigItem(item, sheetInfo.Fields, i, result);
            }
            
            result.IsValid = result.Errors.Count == 0;
            
            Debug.Log($"JSON数据验证完成：{(result.IsValid ? "通过" : "失败")}，错误数：{result.Errors.Count}");
        }
        catch (Exception e)
        {
            result.AddError($"JSON验证异常：{e.Message}");
        }
        
        return result;
    }

    /// <summary>
    /// 验证单个配置项
    /// </summary>
    /// <param name="item">配置项</param>
    /// <param name="fields">字段定义</param>
    /// <param name="index">项目索引</param>
    /// <param name="result">验证结果</param>
    private void ValidateConfigItem(Dictionary<string, object> item, List<FieldInfo> fields, 
        int index, ValidationResult result)
    {
        foreach (var field in fields)
        {
            if (!item.ContainsKey(field.Name))
            {
                result.AddError($"记录{index}：缺少字段{field.Name}");
                continue;
            }
            
            var value = item[field.Name];
            
            // 验证字段类型
            if (!ValidateFieldType(value, field))
            {
                result.AddError($"记录{index}：字段{field.Name}类型不匹配，期望：{field.Type}");
            }
        }
    }

    /// <summary>
    /// 验证字段类型
    /// </summary>
    /// <param name="value">字段值</param>
    /// <param name="field">字段信息</param>
    /// <returns>是否匹配</returns>
    private bool ValidateFieldType(object value, FieldInfo field)
    {
        if (value == null)
        {
            return true; // null值总是有效的
        }
        
        if (field.IsArray)
        {
            return value is Array || value is List<object>;
        }
        
        if (field.IsComplex)
        {
            return value is Dictionary<string, object> || value is JObject;
        }
        
        switch (field.Type.ToLower())
        {
            case "int":
                return value is int || value is long;
            case "float":
                return value is float || value is double;
            case "bool":
                return value is bool;
            case "string":
                return value is string;
            default:
                return true;
        }
    }

    #endregion
}

#region 辅助类

/// <summary>
/// 验证结果
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; } = true;
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
    
    public void AddError(string error)
    {
        Errors.Add(error);
        IsValid = false;
    }
    
    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }
}

#endregion 