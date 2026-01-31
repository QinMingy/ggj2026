using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Excel配置表导出工具
/// 将Excel文件转换为C#配置数据类和JSON数据文件
/// </summary>
public class ConfigTableExporter : EditorWindow
{
    #region 常量定义
    
    private const string EXCEL_FILTER = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls";
    private const string CONFIG_NAMESPACE = "ConfigData";
    private const string CONFIG_OUTPUT_PATH = "Assets/Scripts/ConfigTool/Generated";
    private const string JSON_OUTPUT_PATH = "Assets/Resources/ConfigData";
    
    #endregion

    #region 私有字段
    
    private string _selectedExcelPath = "";
    private List<ExcelSheetInfo> _sheetInfos = new List<ExcelSheetInfo>();
    private Vector2 _scrollPosition;
    private bool _showAdvancedOptions = false;
    
    // 日志显示相关
    private List<string> _logMessages = new List<string>();
    private Vector2 _logScrollPosition;
    private bool _showLogPanel = true;
    
    #endregion

    #region Unity Editor菜单
    
    [MenuItem("工具/配置表工具/Excel配置导出器")]
    public static void ShowWindow()
    {
        var window = GetWindow<ConfigTableExporter>("Excel配置导出器");
        window.AddLog("欢迎使用Excel配置导出工具！");
        window.AddLog("提示：已修复dynamic关键字编译错误，请重新导出配置表", LogType.Warning);
    }
    
    [MenuItem("工具/配置表工具/一键导出所有配置")]
    public static void QuickExportAll()
    {
        var window = GetWindow<ConfigTableExporter>("Excel配置导出器");
        window.ExportAllConfigsQuick();
    }
    
    [MenuItem("工具/配置表工具/修复已生成的配置文件")]
    public static void FixGeneratedConfigs()
    {
        var window = GetWindow<ConfigTableExporter>("Excel配置导出器");
        window.AddLog("正在修复已生成的配置文件...");
        
        // 检查是否有已选择的Excel文件
        if (string.IsNullOrEmpty(window._selectedExcelPath))
        {
            EditorUtility.DisplayDialog("提示", 
                "请先在配置导出器窗口中选择Excel文件，然后重新导出配置表来修复dynamic关键字编译错误。", 
                "确定");
            return;
        }
        
        window.ParseExcelFile();
        window.ExportAllConfigs();
    }
    
    #endregion

    #region Unity Editor GUI

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Excel配置表导出工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        DrawFileSelection();
        EditorGUILayout.Space();
        
        DrawSheetInfo();
        EditorGUILayout.Space();
        
        DrawExportOptions();
        EditorGUILayout.Space();
        
        DrawExportButtons();
        EditorGUILayout.Space();
        
        DrawLogPanel();
    }

    private void DrawFileSelection()
    {
        EditorGUILayout.LabelField("1. 选择Excel文件", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("文件路径：", GUILayout.Width(60));
        EditorGUILayout.TextField(_selectedExcelPath);
        
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            SelectExcelFile();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSheetInfo()
    {
        if (_sheetInfos.Count == 0) return;
        
        EditorGUILayout.LabelField("2. 表格信息预览", EditorStyles.boldLabel);
        
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(200));
        
        foreach (var sheetInfo in _sheetInfos)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"表格：{sheetInfo.SheetName}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"数据行数：{sheetInfo.DataRowCount}");
            EditorGUILayout.LabelField($"字段数量：{sheetInfo.FieldCount}");
            
            if (sheetInfo.Fields.Count > 0)
            {
                EditorGUILayout.LabelField("字段信息：");
                foreach (var field in sheetInfo.Fields)
                {
                    EditorGUILayout.LabelField($"  {field.Name} ({field.Type}) - {field.Description}");
                }
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawExportOptions()
    {
        EditorGUILayout.LabelField("3. 导出选项", EditorStyles.boldLabel);
        
        _showAdvancedOptions = EditorGUILayout.Foldout(_showAdvancedOptions, "高级选项");
        
        if (_showAdvancedOptions)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox($"C#文件输出路径：{CONFIG_OUTPUT_PATH}", MessageType.Info);
            EditorGUILayout.HelpBox($"命名空间：{CONFIG_NAMESPACE}", MessageType.Info);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("支持的ID类型：", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• int - 整数类型ID（如：1, 2, 3）");
            EditorGUILayout.LabelField("• string - 字符串类型ID（如：\"item_001\", \"weapon_sword\"）");
            EditorGUILayout.LabelField("• 工具会自动检测ID字段的类型并生成相应代码");
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("修复说明：", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("• 已移除dynamic关键字依赖，解决编译错误");
            EditorGUILayout.LabelField("• 使用类型安全的泛型约束和类型转换");
            EditorGUILayout.LabelField("• 如遇到编译错误，请重新导出配置表");
            
            EditorGUI.indentLevel--;
        }
    }

    private void DrawExportButtons()
    {
        EditorGUILayout.LabelField("4. 导出操作", EditorStyles.boldLabel);
        
        GUI.enabled = !string.IsNullOrEmpty(_selectedExcelPath);
        
        if (GUILayout.Button("解析Excel文件", GUILayout.Height(30)))
        {
            ParseExcelFile();
        }
        
        GUI.enabled = _sheetInfos.Count > 0;
        
        if (GUILayout.Button("生成配置代码和数据", GUILayout.Height(30)))
        {
            ExportAllConfigs();
        }
        
        GUI.enabled = true;
    }

    private void DrawLogPanel()
    {
        EditorGUILayout.BeginHorizontal();
        _showLogPanel = EditorGUILayout.Foldout(_showLogPanel, "执行日志", true);
        if (GUILayout.Button("清空日志", GUILayout.Width(80)))
        {
            _logMessages.Clear();
        }
        EditorGUILayout.EndHorizontal();
        
        if (_showLogPanel)
        {
            EditorGUILayout.BeginVertical("box");
            
            _logScrollPosition = EditorGUILayout.BeginScrollView(_logScrollPosition, GUILayout.Height(150));
            
            if (_logMessages.Count == 0)
            {
                EditorGUILayout.LabelField("暂无日志信息", EditorStyles.centeredGreyMiniLabel);
            }
            else
            {
                foreach (var message in _logMessages)
                {
                    // 根据消息类型设置不同的样式
                    GUIStyle style = EditorStyles.label;
                    if (message.Contains("[错误]"))
                    {
                        style = new GUIStyle(EditorStyles.label) { normal = { textColor = Color.red } };
                    }
                    else if (message.Contains("[警告]"))
                    {
                        style = new GUIStyle(EditorStyles.label) { normal = { textColor = Color.yellow } };
                    }
                    else if (message.Contains("[成功]"))
                    {
                        style = new GUIStyle(EditorStyles.label) { normal = { textColor = Color.green } };
                    }
                    
                    EditorGUILayout.LabelField(message, style);
                }
            }
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
    
    private void AddLog(string message, LogType logType = LogType.Log)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        string prefix = "";
        
        switch (logType)
        {
            case LogType.Error:
                prefix = "[错误]";
                break;
            case LogType.Warning:
                prefix = "[警告]";
                break;
            case LogType.Log:
                prefix = "[信息]";
                break;
        }
        
        string logMessage = $"{timestamp} {prefix} {message}";
        _logMessages.Add(logMessage);
        
        // 限制日志数量，避免内存过多占用
        if (_logMessages.Count > 100)
        {
            _logMessages.RemoveAt(0);
        }
        
        // 自动滚动到最新消息
        _logScrollPosition.y = float.MaxValue;
        
        // 刷新窗口
        Repaint();
        
        // 同时输出到Unity Console
        switch (logType)
        {
            case LogType.Error:
                Debug.LogError($"[配置表工具] {message}");
                break;
            case LogType.Warning:
                Debug.LogWarning($"[配置表工具] {message}");
                break;
            default:
                Debug.Log($"[配置表工具] {message}");
                break;
        }
    }
    
    private void AddSuccessLog(string message)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        string logMessage = $"{timestamp} [成功] {message}";
        _logMessages.Add(logMessage);
        
        // 限制日志数量
        if (_logMessages.Count > 100)
        {
            _logMessages.RemoveAt(0);
        }
        
        // 自动滚动到最新消息
        _logScrollPosition.y = float.MaxValue;
        
        // 刷新窗口
        Repaint();
        
        // 同时输出到Unity Console
        Debug.Log($"[配置表工具] {message}");
    }

    #endregion

    #region 文件操作

    private void SelectExcelFile()
    {
        string path = EditorUtility.OpenFilePanel("选择Excel文件", "", "xlsx,xls");
        if (!string.IsNullOrEmpty(path))
        {
            _selectedExcelPath = path;
            _sheetInfos.Clear();
            
            // 检查文件是否被占用
            if (ExcelParser.IsFileInUse(path))
            {
                AddLog($"⚠️ 文件可能被Excel程序打开: {Path.GetFileName(path)}", LogType.Warning);
                AddLog("💡 导表工具将尝试使用多种方式读取文件，如遇问题请关闭Excel后重试", LogType.Log);
            }
            else
            {
                AddLog($"✅ 文件状态正常: {Path.GetFileName(path)}", LogType.Log);
            }
        }
    }

    private void ParseExcelFile()
    {
        if (string.IsNullOrEmpty(_selectedExcelPath))
        {
            EditorUtility.DisplayDialog("错误", "请先选择Excel文件", "确定");
            return;
        }

        // 检查文件状态
        if (ExcelParser.IsFileInUse(_selectedExcelPath))
        {
            AddLog($"⚠️ 检测到文件被占用，将尝试多种方式读取...", LogType.Warning);
        }

        AddLog($"开始解析Excel文件：{_selectedExcelPath}");

        try
        {
            _sheetInfos = ExcelParser.ParseExcelFile(_selectedExcelPath, AddLog);
            AddLog($"成功解析Excel文件，共找到{_sheetInfos.Count}个有效表格");
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("解析失败", $"解析Excel文件时发生错误：\n{e.Message}", "确定");
            AddLog($"解析Excel文件失败：{e}", LogType.Error);
        }
    }

    #endregion

    #region 导出功能

    private void ExportAllConfigs()
    {
        if (_sheetInfos.Count == 0)
        {
            EditorUtility.DisplayDialog("错误", "没有可导出的表格数据", "确定");
            AddLog("导出失败：没有可导出的表格数据", LogType.Warning);
            return;
        }

        AddLog($"开始导出配置表，共{_sheetInfos.Count}个表格");

        try
        {
            // 确保输出目录存在
            EnsureDirectoriesExist();
            
            int successCount = 0;
            int totalCount = _sheetInfos.Count;
            
            foreach (var sheetInfo in _sheetInfos)
            {
                EditorUtility.DisplayProgressBar("导出配置", $"正在处理：{sheetInfo.SheetName}", 
                    (float)successCount / totalCount);
                
                AddLog($"正在处理表格：{sheetInfo.SheetName}");
                
                try
                {
                    // 只生成C#类文件（包含数据）
                    GenerateCSharpClass(sheetInfo);
                    
                    successCount++;
                    AddLog($"成功处理表格：{sheetInfo.SheetName}");
                }
                catch (Exception e)
                {
                    AddLog($"导出表格 {sheetInfo.SheetName} 失败：{e.Message}", LogType.Error);
                }
            }
            
            EditorUtility.ClearProgressBar();
            
            // 刷新项目
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("导出完成", 
                $"成功导出 {successCount}/{totalCount} 个配置表\n" +
                "配置数据已直接写入C#代码中，无需额外的数据文件。", "确定");
                
            AddSuccessLog($"配置表导出完成：{successCount}/{totalCount}");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("导出失败", $"导出过程中发生错误：\n{e.Message}", "确定");
            AddLog($"配置表导出失败：{e}", LogType.Error);
        }
    }

    private void ExportAllConfigsQuick()
    {
        // 快速导出功能的实现
        if (string.IsNullOrEmpty(_selectedExcelPath))
        {
            EditorUtility.DisplayDialog("提示", "请先在导出器窗口中选择Excel文件", "确定");
            return;
        }
        
        ParseExcelFile();
        ExportAllConfigs();
    }

    private void EnsureDirectoriesExist()
    {
        if (!Directory.Exists(CONFIG_OUTPUT_PATH))
        {
            Directory.CreateDirectory(CONFIG_OUTPUT_PATH);
        }
        
        // 不再需要JSON输出目录
        // if (!Directory.Exists(JSON_OUTPUT_PATH))
        // {
        //     Directory.CreateDirectory(JSON_OUTPUT_PATH);
        // }
    }

    private void GenerateCSharpClass(ExcelSheetInfo sheetInfo)
    {
        var codeGenerator = new ConfigCodeGenerator(AddLog);
        string classCode = codeGenerator.GenerateConfigClass(sheetInfo);
        
        string fileName = $"{sheetInfo.SheetName}.cs";
        string filePath = Path.Combine(CONFIG_OUTPUT_PATH, fileName);
        
        File.WriteAllText(filePath, classCode, Encoding.UTF8);
        
        AddLog($"生成C#类文件（包含数据）：{filePath}");
    }

    #endregion
}

#region 数据结构

/// <summary>
/// Excel表格信息
/// </summary>
[Serializable]
public class ExcelSheetInfo
{
    public string SheetName;
    public int DataRowCount;
    public int FieldCount;
    public List<FieldInfo> Fields = new List<FieldInfo>();
    public List<Dictionary<string, object>> DataRows = new List<Dictionary<string, object>>();
}

/// <summary>
/// 字段信息
/// </summary>
[Serializable]
public class FieldInfo
{
    public string Name;        // 字段名
    public string Type;        // 字段类型
    public string Description; // 字段描述
    public bool IsArray;       // 是否为数组
    public bool IsComplex;     // 是否为复杂类型
}

#endregion 