using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// 配置代码生成器
/// 根据Excel表格信息生成C#配置数据类
/// </summary>
public class ConfigCodeGenerator
{
    #region 常量定义
    
    private const string CONFIG_NAMESPACE = "ConfigData";
    private const string GENERATED_WARNING = "// 此文件由ConfigTableExporter自动生成，请勿手动修改！";
    
    #endregion

    #region 私有字段
    
    /// <summary>
    /// 日志回调委托
    /// </summary>
    public delegate void LogCallback(string message, LogType logType = LogType.Log);
    
    private LogCallback _logCallback;
    
    #endregion

    #region 构造函数
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logCallback">日志回调</param>
    public ConfigCodeGenerator(LogCallback logCallback = null)
    {
        _logCallback = logCallback;
    }
    
    #endregion

    #region 公共接口

    /// <summary>
    /// 生成配置类代码
    /// </summary>
    /// <param name="sheetInfo">表格信息</param>
    /// <returns>生成的C#代码</returns>
    public string GenerateConfigClass(ExcelSheetInfo sheetInfo)
    {
        var codeBuilder = new StringBuilder();
        
        // 文件头部
        GenerateFileHeader(codeBuilder);
        
        // 命名空间开始
        codeBuilder.AppendLine($"namespace {CONFIG_NAMESPACE}");
        codeBuilder.AppendLine("{");
        
        // 先分析和生成复杂类型的辅助类，更新字段类型信息
        GenerateComplexTypeClasses(codeBuilder, sheetInfo);
        
        // 生成主配置类（此时复杂字段类型已经更新）
        GenerateMainConfigClass(codeBuilder, sheetInfo);
        
        // 生成配置数据管理类
        GenerateConfigDataManagerClass(codeBuilder, sheetInfo);
        
        // 命名空间结束
        codeBuilder.AppendLine("}");
        
        return codeBuilder.ToString();
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 生成文件头部
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    private void GenerateFileHeader(StringBuilder codeBuilder)
    {
        codeBuilder.AppendLine(GENERATED_WARNING);
        codeBuilder.AppendLine("// 生成时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        codeBuilder.AppendLine();
        
        // using 语句
        codeBuilder.AppendLine("using System;");
        codeBuilder.AppendLine("using System.Collections.Generic;");
        codeBuilder.AppendLine("using UnityEngine;");
        codeBuilder.AppendLine("using Newtonsoft.Json;");
        codeBuilder.AppendLine();
    }

    /// <summary>
    /// 生成主配置类
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="sheetInfo">表格信息</param>
    private void GenerateMainConfigClass(StringBuilder codeBuilder, ExcelSheetInfo sheetInfo)
    {
        string className = sheetInfo.SheetName;
        
        // 查找ID字段以确定基类类型
        var idField = sheetInfo.Fields.FirstOrDefault(f => 
            f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) ||
            f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        
        string baseClass = "ConfigDataBase";
        if (idField != null)
        {
            string idType = GetCSharpType(idField);
            if (idType != "int")
            {
                baseClass = $"ConfigDataBase<{idType}>";
            }
        }
        
        // 类注释
        codeBuilder.AppendLine("    /// <summary>");
        codeBuilder.AppendLine($"    /// {className} 配置数据类");
        codeBuilder.AppendLine("    /// </summary>");
        
        // 类声明
        codeBuilder.AppendLine("    [Serializable]");
        codeBuilder.AppendLine($"    public class {className} : {baseClass}");
        codeBuilder.AppendLine("    {");
        
        // 生成字段属性
        GenerateProperties(codeBuilder, sheetInfo.Fields);
        
        // 生成ID属性重写
        GenerateIDProperty(codeBuilder, sheetInfo.Fields);
        
        // 生成构造函数
        GenerateConstructor(codeBuilder, className);
        
        // 生成数据验证方法
        GenerateValidationMethod(codeBuilder, sheetInfo.Fields);
        
        // 生成ToString方法
        GenerateToStringMethod(codeBuilder, className, sheetInfo.Fields);
        
        codeBuilder.AppendLine("    }");
        codeBuilder.AppendLine();
    }

    /// <summary>
    /// 生成属性
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="fields">字段列表</param>
    private void GenerateProperties(StringBuilder codeBuilder, List<FieldInfo> fields)
    {
        foreach (var field in fields)
        {
            // 跳过ID字段，因为它将作为重写属性单独处理
            if (field.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) ||
                field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // 属性注释
            codeBuilder.AppendLine("        /// <summary>");
            codeBuilder.AppendLine($"        /// {field.Description}");
            codeBuilder.AppendLine("        /// </summary>");
            
            // JSON属性标记
            codeBuilder.AppendLine($"        [JsonProperty(\"{field.Name}\")]");
            
            // 属性声明
            string propertyType = GetCSharpType(field);
            codeBuilder.AppendLine($"        public {propertyType} {field.Name} {{ get; set; }}");
            codeBuilder.AppendLine();
        }
    }

    /// <summary>
    /// 生成ID属性重写
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="fields">字段列表</param>
    private void GenerateIDProperty(StringBuilder codeBuilder, List<FieldInfo> fields)
    {
        // 查找ID字段
        var idField = fields.FirstOrDefault(f => 
            f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) ||
            f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        
        if (idField != null)
        {
            // 首先生成私有的ID数据字段
            codeBuilder.AppendLine("        /// <summary>");
            codeBuilder.AppendLine($"        /// {idField.Description}");
            codeBuilder.AppendLine("        /// </summary>");
            codeBuilder.AppendLine($"        [JsonProperty(\"{idField.Name}\")]");
            string propertyType = GetCSharpType(idField);
            codeBuilder.AppendLine($"        public {propertyType} {idField.Name}Value {{ get; set; }}");
            codeBuilder.AppendLine();

            // 然后生成重写的ID属性，类型与字段类型一致
            codeBuilder.AppendLine("        /// <summary>");
            codeBuilder.AppendLine("        /// 配置项唯一ID");
            codeBuilder.AppendLine("        /// </summary>");
            codeBuilder.AppendLine($"        public override {propertyType} ID => {idField.Name}Value;");
            codeBuilder.AppendLine();
        }
        else
        {
            // 如果没有找到ID字段，使用默认实现
            codeBuilder.AppendLine("        /// <summary>");
            codeBuilder.AppendLine("        /// 配置项唯一ID（默认实现）");
            codeBuilder.AppendLine("        /// 警告：Excel表格中没有找到ID字段，请添加ID列");
            codeBuilder.AppendLine("        /// </summary>");
            codeBuilder.AppendLine("        public override int ID => 0; // 请确保Excel表格中有ID字段");
            codeBuilder.AppendLine();
        }
    }

    /// <summary>
    /// 生成构造函数
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="className">类名</param>
    private void GenerateConstructor(StringBuilder codeBuilder, string className)
    {
        codeBuilder.AppendLine("        /// <summary>");
        codeBuilder.AppendLine($"        /// {className} 构造函数");
        codeBuilder.AppendLine("        /// </summary>");
        codeBuilder.AppendLine($"        public {className}()");
        codeBuilder.AppendLine("        {");
        codeBuilder.AppendLine("            // 可以在这里设置默认值");
        codeBuilder.AppendLine("        }");
        codeBuilder.AppendLine();
    }

    /// <summary>
    /// 生成数据验证方法
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="fields">字段列表</param>
    private void GenerateValidationMethod(StringBuilder codeBuilder, List<FieldInfo> fields)
    {
        codeBuilder.AppendLine("        /// <summary>");
        codeBuilder.AppendLine("        /// 验证配置数据的有效性");
        codeBuilder.AppendLine("        /// </summary>");
        codeBuilder.AppendLine("        /// <returns>如果数据有效返回true</returns>");
        codeBuilder.AppendLine("        public override bool IsValid()");
        codeBuilder.AppendLine("        {");
        
        // 基础验证
        codeBuilder.AppendLine("            if (!base.IsValid())");
        codeBuilder.AppendLine("            {");
        codeBuilder.AppendLine("                return false;");
        codeBuilder.AppendLine("            }");
        codeBuilder.AppendLine();
        
        // 添加字段特定的验证逻辑
        GenerateFieldValidation(codeBuilder, fields);
        
        codeBuilder.AppendLine("            return true;");
        codeBuilder.AppendLine("        }");
        codeBuilder.AppendLine();
    }

    /// <summary>
    /// 生成字段验证逻辑
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="fields">字段列表</param>
    private void GenerateFieldValidation(StringBuilder codeBuilder, List<FieldInfo> fields)
    {
        foreach (var field in fields)
        {
            // 为字符串字段生成非空验证
            if (field.Type == "string" && field.Name.Contains("Name"))
            {
                codeBuilder.AppendLine($"            if (string.IsNullOrWhiteSpace({field.Name}))");
                codeBuilder.AppendLine("            {");
                codeBuilder.AppendLine($"                Debug.LogWarning(\"配置字段 {field.Name} 不能为空\");");
                codeBuilder.AppendLine("                return false;");
                codeBuilder.AppendLine("            }");
                codeBuilder.AppendLine();
            }
            
            // 为数组字段生成非空验证
            if (field.IsArray)
            {
                codeBuilder.AppendLine($"            if ({field.Name} == null)");
                codeBuilder.AppendLine("            {");
                codeBuilder.AppendLine($"                Debug.LogWarning(\"配置数组字段 {field.Name} 不能为null\");");
                codeBuilder.AppendLine("                return false;");
                codeBuilder.AppendLine("            }");
                codeBuilder.AppendLine();
            }
        }
    }

    /// <summary>
    /// 生成ToString方法
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="className">类名</param>
    /// <param name="fields">字段列表</param>
    private void GenerateToStringMethod(StringBuilder codeBuilder, string className, List<FieldInfo> fields)
    {
        codeBuilder.AppendLine("        /// <summary>");
        codeBuilder.AppendLine("        /// 返回配置信息的字符串表示");
        codeBuilder.AppendLine("        /// </summary>");
        codeBuilder.AppendLine("        public override string ToString()");
        codeBuilder.AppendLine("        {");
        
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"$\"{className}[ID={{ID}}");
        
        // 添加主要字段到ToString
        var mainFields = fields.Take(3).ToList(); // 只显示前3个字段
        foreach (var field in mainFields)
        {
            if (!field.Name.Equals("ID", StringComparison.OrdinalIgnoreCase))
            {
                stringBuilder.Append($", {field.Name}={{{field.Name}}}");
            }
        }
        
        stringBuilder.Append("]\"");
        
        codeBuilder.AppendLine($"            return {stringBuilder};");
        codeBuilder.AppendLine("        }");
    }

    /// <summary>
    /// 生成复杂类型的辅助类
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="sheetInfo">表格信息</param>
    private void GenerateComplexTypeClasses(StringBuilder codeBuilder, ExcelSheetInfo sheetInfo)
    {
        // 检查所有字段，不只是标记为IsComplex的字段
        var allComplexTypes = new List<ComplexTypeInfo>();
        
        foreach (var field in sheetInfo.Fields)
        {
            // 基于实际数据内容检查是否为复杂类型
            if (HasComplexData(field, sheetInfo.DataRows))
            {
                // 分析复杂类型的结构
                var complexTypes = AnalyzeComplexType(field, sheetInfo.DataRows);
                allComplexTypes.AddRange(complexTypes);
                
                // 标记字段为复杂类型（用于后续生成代码时判断）
                field.IsComplex = true;
                
                // 更新字段类型为主要复杂类型的名称
                if (complexTypes.Count > 0)
                {
                    field.Type = complexTypes[0].Name;
                    LogMessage($"字段 {field.Name} 被识别为复杂类型: {field.Type}", LogType.Log);
                }
            }
        }
        
        // 为每个复杂类型生成类（去重）
        var uniqueComplexTypes = new Dictionary<string, ComplexTypeInfo>();
        foreach (var complexType in allComplexTypes)
        {
            var structureKey = GenerateStructureKey(complexType);
            if (!uniqueComplexTypes.ContainsKey(structureKey))
            {
                uniqueComplexTypes[structureKey] = complexType;
                LogMessage($"添加唯一复杂类型: {complexType.Name} (结构键: {structureKey})", LogType.Log);
            }
            else
            {
                LogMessage($"跳过重复的复杂类型结构: {complexType.Name}", LogType.Log);
            }
        }
        
        LogMessage($"总共将生成 {uniqueComplexTypes.Count} 个唯一的复杂类型类", LogType.Log);
        
        // 用于跟踪已生成的类型，避免重复生成
        var generatedTypes = new HashSet<string>();
        
        foreach (var complexType in uniqueComplexTypes.Values)
        {
            if (!generatedTypes.Contains(complexType.Name))
            {
                GenerateComplexTypeClass(codeBuilder, complexType);
                generatedTypes.Add(complexType.Name);
                LogMessage($"生成主复杂类型: {complexType.Name}", LogType.Log);
                
                // 递归生成嵌套类型
                GenerateNestedComplexTypes(codeBuilder, complexType, generatedTypes);
            }
            else
            {
                LogMessage($"跳过已生成的主类型: {complexType.Name}", LogType.Log);
            }
        }
    }
    
    /// <summary>
    /// 检查字段是否包含复杂数据
    /// </summary>
    /// <param name="field">字段信息</param>
    /// <param name="dataRows">数据行</param>
    /// <returns>是否包含复杂数据</returns>
    private bool HasComplexData(FieldInfo field, List<Dictionary<string, object>> dataRows)
    {
        foreach (var row in dataRows)
        {
            if (row.TryGetValue(field.Name, out var value) && value != null)
            {
                string stringValue = value.ToString();
                
                // 检查是否为JSON对象格式
                if (!string.IsNullOrWhiteSpace(stringValue) && 
                    stringValue.Trim().StartsWith("{") && 
                    stringValue.Trim().EndsWith("}"))
                {
                    try
                    {
                        // 尝试解析为JSON对象
                        var jobj = Newtonsoft.Json.Linq.JObject.Parse(stringValue);
                        if (jobj.HasValues)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        // JSON解析失败，不是复杂类型
                    }
                }
            }
        }
        return false;
    }
    
    /// <summary>
    /// 生成嵌套的复杂类型
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="complexType">复杂类型信息</param>
    /// <param name="generatedTypes">已生成的类型集合，用于避免重复生成</param>
    private void GenerateNestedComplexTypes(StringBuilder codeBuilder, ComplexTypeInfo complexType, HashSet<string> generatedTypes = null)
    {
        if (generatedTypes == null)
        {
            generatedTypes = new HashSet<string>();
        }
        
        foreach (var nestedType in complexType.NestedTypes)
        {
            // 检查是否已经生成过此类型
            if (!generatedTypes.Contains(nestedType.Name))
            {
                GenerateComplexTypeClass(codeBuilder, nestedType);
                generatedTypes.Add(nestedType.Name);
                LogMessage($"生成嵌套复杂类型: {nestedType.Name}", LogType.Log);
                
                // 递归处理更深层的嵌套
                GenerateNestedComplexTypes(codeBuilder, nestedType, generatedTypes);
            }
            else
            {
                LogMessage($"跳过已生成的嵌套类型: {nestedType.Name}", LogType.Log);
            }
        }
    }

    /// <summary>
    /// 分析复杂类型结构
    /// </summary>
    /// <param name="field">复杂字段</param>
    /// <param name="dataRows">数据行</param>
    /// <returns>复杂类型信息</returns>
    private List<ComplexTypeInfo> AnalyzeComplexType(FieldInfo field, List<Dictionary<string, object>> dataRows)
    {
        var allComplexTypes = new List<ComplexTypeInfo>();
        var typeCounter = new Dictionary<string, int>(); // 用于生成唯一的类名
        
        // 只分析第一行包含有效JSON数据的行
        foreach (var row in dataRows)
        {
            if (row.TryGetValue(field.Name, out var value) && value != null)
            {
                try
                {
                    JObject jobj = null;
                    
                    if (value is JObject directJobj)
                    {
                        jobj = directJobj;
                    }
                    else if (value is string stringValue && !string.IsNullOrWhiteSpace(stringValue))
                    {
                        // 尝试将字符串解析为JObject
                        if (stringValue.Trim().StartsWith("{") && stringValue.Trim().EndsWith("}"))
                        {
                            try
                            {
                                jobj = Newtonsoft.Json.Linq.JObject.Parse(stringValue);
                            }
                            catch
                            {
                                // 解析失败，继续寻找下一行
                                continue;
                            }
                        }
                    }
                    
                    if (jobj != null)
                    {
                        // 找到第一行有效数据，分析结构并返回
                        var mainType = AnalyzeJObjectStructureRecursive(jobj, field.Name, typeCounter, allComplexTypes);
                        allComplexTypes.Add(mainType);
                        
                        LogMessage($"字段 {field.Name} 基于第一行有效数据生成类型结构: {mainType.Name}", LogType.Log);
                        break; // 只分析第一行有效数据，然后退出循环
                    }
                }
                catch (Exception e)
                {
                    LogMessage($"分析复杂类型失败：{e.Message}", LogType.Warning);
                }
            }
        }
        
        return allComplexTypes;
    }

    /// <summary>
    /// 递归分析JObject结构
    /// </summary>
    /// <param name="jobj">JSON对象</param>
    /// <param name="baseName">基础类名</param>
    /// <param name="typeCounter">类型计数器</param>
    /// <param name="allTypes">所有类型列表</param>
    /// <returns>复杂类型信息</returns>
    private ComplexTypeInfo AnalyzeJObjectStructureRecursive(JObject jobj, string baseName, Dictionary<string, int> typeCounter, List<ComplexTypeInfo> allTypes)
    {
        var complexType = new ComplexTypeInfo
        {
            Name = $"{baseName}Data"
        };
        
        foreach (var property in jobj.Properties())
        {
            var propTypeInfo = new PropertyTypeInfo();
            
            switch (property.Value.Type)
            {
                case JTokenType.Integer:
                    propTypeInfo.TypeName = "int";
                    break;
                case JTokenType.Float:
                    propTypeInfo.TypeName = "float";
                    break;
                case JTokenType.String:
                    propTypeInfo.TypeName = "string";
                    break;
                case JTokenType.Boolean:
                    propTypeInfo.TypeName = "bool";
                    break;
                case JTokenType.Array:
                    propTypeInfo = AnalyzeArrayProperty(property.Value as JArray, property.Name, typeCounter, allTypes);
                    break;
                case JTokenType.Object:
                    // 递归处理嵌套对象
                    var nestedName = $"{property.Name}Info";
                    if (typeCounter.ContainsKey(nestedName))
                    {
                        typeCounter[nestedName]++;
                        nestedName = $"{property.Name}Info{typeCounter[nestedName]}";
                    }
                    else
                    {
                        typeCounter[nestedName] = 1;
                    }
                    
                    var nestedType = AnalyzeJObjectStructureRecursive(property.Value as JObject, nestedName, typeCounter, allTypes);
                    allTypes.Add(nestedType);
                    
                    propTypeInfo.TypeName = nestedType.Name;
                    propTypeInfo.NestedType = nestedType;
                    break;
                default:
                    propTypeInfo.TypeName = "object";
                    break;
            }
            
            complexType.Properties[property.Name] = propTypeInfo;
        }
        
        return complexType;
    }
    
    /// <summary>
    /// 分析数组属性
    /// </summary>
    /// <param name="jarray">JSON数组</param>
    /// <param name="propertyName">属性名</param>
    /// <param name="typeCounter">类型计数器</param>
    /// <param name="allTypes">所有类型列表</param>
    /// <returns>属性类型信息</returns>
    private PropertyTypeInfo AnalyzeArrayProperty(JArray jarray, string propertyName, Dictionary<string, int> typeCounter, List<ComplexTypeInfo> allTypes)
    {
        var propTypeInfo = new PropertyTypeInfo
        {
            IsArray = true
        };
        
        if (jarray.Count > 0)
        {
            var firstElement = jarray[0];
            switch (firstElement.Type)
            {
                case JTokenType.Integer:
                    propTypeInfo.TypeName = "int";
                    break;
                case JTokenType.Float:
                    propTypeInfo.TypeName = "float";
                    break;
                case JTokenType.String:
                    propTypeInfo.TypeName = "string";
                    break;
                case JTokenType.Boolean:
                    propTypeInfo.TypeName = "bool";
                    break;
                case JTokenType.Object:
                    // 数组中的复杂对象
                    var elementName = $"{propertyName}Element";
                    if (typeCounter.ContainsKey(elementName))
                    {
                        typeCounter[elementName]++;
                        elementName = $"{propertyName}Element{typeCounter[elementName]}";
                    }
                    else
                    {
                        typeCounter[elementName] = 1;
                    }
                    
                    var elementType = AnalyzeJObjectStructureRecursive(firstElement as JObject, elementName, typeCounter, allTypes);
                    allTypes.Add(elementType);
                    
                    propTypeInfo.TypeName = elementType.Name;
                    propTypeInfo.NestedType = elementType;
                    break;
                default:
                    propTypeInfo.TypeName = "object";
                    break;
            }
        }
        else
        {
            propTypeInfo.TypeName = "object";
        }
        
        return propTypeInfo;
    }
    
    /// <summary>
    /// 生成结构键用于去重
    /// </summary>
    /// <param name="complexType">复杂类型</param>
    /// <returns>结构键</returns>
    private string GenerateStructureKey(ComplexTypeInfo complexType)
    {
        var keys = new List<string>();
        foreach (var prop in complexType.Properties.OrderBy(p => p.Key))
        {
            var typeStr = prop.Value.IsArray ? $"{prop.Value.TypeName}[]" : prop.Value.TypeName;
            keys.Add($"{prop.Key}:{typeStr}");
        }
        return string.Join(",", keys);
    }

    /// <summary>
    /// 生成复杂类型类
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="complexType">复杂类型信息</param>
    private void GenerateComplexTypeClass(StringBuilder codeBuilder, ComplexTypeInfo complexType)
    {
        codeBuilder.AppendLine("    /// <summary>");
        codeBuilder.AppendLine($"    /// {complexType.Name} 复杂数据类型");
        codeBuilder.AppendLine("    /// </summary>");
        codeBuilder.AppendLine("    [Serializable]");
        codeBuilder.AppendLine($"    public class {complexType.Name}");
        codeBuilder.AppendLine("    {");
        
        // 生成属性
        foreach (var property in complexType.Properties)
        {
            codeBuilder.AppendLine("        /// <summary>");
            codeBuilder.AppendLine($"        /// {property.Key}");
            codeBuilder.AppendLine("        /// </summary>");
            codeBuilder.AppendLine($"        [JsonProperty(\"{property.Key}\")]");
            
            string propertyType = GetPropertyTypeString(property.Value);
            codeBuilder.AppendLine($"        public {propertyType} {property.Key} {{ get; set; }}");
            codeBuilder.AppendLine();
        }
        
        codeBuilder.AppendLine("    }");
        codeBuilder.AppendLine();
    }
    
    /// <summary>
    /// 获取属性类型字符串
    /// </summary>
    /// <param name="propertyType">属性类型信息</param>
    /// <returns>类型字符串</returns>
    private string GetPropertyTypeString(PropertyTypeInfo propertyType)
    {
        string baseType = propertyType.TypeName;
        return propertyType.IsArray ? $"{baseType}[]" : baseType;
    }

    /// <summary>
    /// 生成配置数据管理类
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="sheetInfo">表格信息</param>
    private void GenerateConfigDataManagerClass(StringBuilder codeBuilder, ExcelSheetInfo sheetInfo)
    {
        string className = sheetInfo.SheetName;
        string managerClassName = $"{className}Manager";
        
        codeBuilder.AppendLine("    /// <summary>");
        codeBuilder.AppendLine($"    /// {className} 配置数据管理器");
        codeBuilder.AppendLine("    /// 包含所有配置数据的静态访问类");
        codeBuilder.AppendLine("    /// </summary>");
        codeBuilder.AppendLine($"    public static class {managerClassName}");
        codeBuilder.AppendLine("    {");
        
        // 生成静态数据字典
        GenerateStaticDataDictionary(codeBuilder, sheetInfo);
        
        // 生成访问方法
        GenerateDataAccessMethods(codeBuilder, sheetInfo);
        
        codeBuilder.AppendLine("    }");
        codeBuilder.AppendLine();
    }

    /// <summary>
    /// 生成静态数据字典
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="sheetInfo">表格信息</param>
    private void GenerateStaticDataDictionary(StringBuilder codeBuilder, ExcelSheetInfo sheetInfo)
    {
        string className = sheetInfo.SheetName;
        
        // 查找ID字段以确定字典键的类型
        var idField = sheetInfo.Fields.FirstOrDefault(f => 
            f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) ||
            f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        
        string keyType = "int"; // 默认类型
        if (idField != null)
        {
            keyType = GetCSharpType(idField);
        }
        
        codeBuilder.AppendLine("        /// <summary>");
        codeBuilder.AppendLine("        /// 所有配置数据的静态字典");
        codeBuilder.AppendLine("        /// </summary>");
        codeBuilder.AppendLine($"        private static readonly Dictionary<{keyType}, {className}> _configs = new Dictionary<{keyType}, {className}>");
        codeBuilder.AppendLine("        {");
        
        // 生成每一行数据
        foreach (var dataRow in sheetInfo.DataRows)
        {
            GenerateConfigDataEntry(codeBuilder, sheetInfo, dataRow);
        }
        
        codeBuilder.AppendLine("        };");
        codeBuilder.AppendLine();
    }

    /// <summary>
    /// 生成单个配置数据条目
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="sheetInfo">表格信息</param>
    /// <param name="dataRow">数据行</param>
    private void GenerateConfigDataEntry(StringBuilder codeBuilder, ExcelSheetInfo sheetInfo, Dictionary<string, object> dataRow)
    {
        string className = sheetInfo.SheetName;
        
        // 获取ID值
        var idField = sheetInfo.Fields.FirstOrDefault(f => 
            f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) ||
            f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        
        if (idField == null || !dataRow.TryGetValue(idField.Name, out var idValue))
        {
            return; // 跳过没有ID的行
        }

        // 根据ID字段类型生成相应的键值
        string keyCode;
        string idType = GetCSharpType(idField);
        if (idType == "string")
        {
            keyCode = $"\"{EscapeString(idValue.ToString())}\"";
        }
        else if (idType == "int")
        {
            keyCode = Convert.ToInt32(idValue).ToString();
        }
        else
        {
            // 其他类型也按字符串处理
            keyCode = $"\"{EscapeString(idValue.ToString())}\"";
        }
        
        codeBuilder.AppendLine($"            {{ {keyCode}, new {className}");
        codeBuilder.AppendLine("            {");
        
        // 生成每个字段的赋值
        foreach (var field in sheetInfo.Fields)
        {
            if (dataRow.TryGetValue(field.Name, out var fieldValue))
            {
                string propertyName = GetPropertyName(field);
                string valueCode = GenerateValueCode(fieldValue, field);
                codeBuilder.AppendLine($"                {propertyName} = {valueCode},");
            }
        }
        
        codeBuilder.AppendLine("            }},");
    }

    /// <summary>
    /// 获取属性名（处理ID字段的特殊情况）
    /// </summary>
    /// <param name="field">字段信息</param>
    /// <returns>属性名</returns>
    private string GetPropertyName(FieldInfo field)
    {
        if (field.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) ||
            field.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
        {
            return field.Name + "Value";
        }
        return field.Name;
    }

    /// <summary>
    /// 生成字段值的C#代码
    /// </summary>
    /// <param name="value">字段值</param>
    /// <param name="field">字段信息</param>
    /// <returns>C#代码字符串</returns>
    private string GenerateValueCode(object value, FieldInfo field)
    {
        if (value == null)
        {
            return GetDefaultValueCode(field);
        }

        if (field.IsArray)
        {
            return GenerateArrayValueCode(value, field);
        }

        if (field.IsComplex)
        {
            return GenerateComplexValueCode(value, field);
        }

        return GenerateBasicValueCode(value, field.Type);
    }

    /// <summary>
    /// 生成基础类型值的代码
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="type">类型</param>
    /// <returns>C#代码</returns>
    private string GenerateBasicValueCode(object value, string type)
    {
        switch (type.ToLower())
        {
            case "int":
                return value.ToString();
            case "float":
                return $"{value}f";
            case "bool":
                return value.ToString().ToLower();
            case "string":
                return $"\"{EscapeString(value.ToString())}\"";
            default:
                return $"\"{EscapeString(value.ToString())}\"";
        }
    }

    /// <summary>
    /// 生成数组值的代码
    /// </summary>
    /// <param name="value">数组值</param>
    /// <param name="field">字段信息</param>
    /// <returns>C#代码</returns>
    private string GenerateArrayValueCode(object value, FieldInfo field)
    {
        string elementType = GetBasicCSharpType(field.Type.Replace("[]", ""));
        
        if (value is object[] array)
        {
            var elements = new List<string>();
            foreach (var element in array)
            {
                if (element is JObject jobj && field.Type.Contains("Data"))
                {
                    // 处理复杂类型数组
                    elements.Add(GenerateJObjectValueCode(jobj, elementType));
                }
                else
                {
                    elements.Add(GenerateBasicValueCode(element, field.Type.Replace("[]", "")));
                }
            }
            return $"new {elementType}[] {{ {string.Join(", ", elements)} }}";
        }
        
        return $"new {elementType}[0]";
    }

    /// <summary>
    /// 生成复杂对象值的代码
    /// </summary>
    /// <param name="value">复杂对象值</param>
    /// <param name="field">字段信息</param>
    /// <returns>C#代码</returns>
    private string GenerateComplexValueCode(object value, FieldInfo field)
    {
        // 使用字段的实际类型名，如果已经被分析过，否则使用默认命名
        string typeName = field.IsComplex && !string.IsNullOrEmpty(field.Type) && field.Type != "object" 
            ? field.Type 
            : $"{field.Name}Data";
        
        if (value is JObject jobj)
        {
            return GenerateJObjectValueCode(jobj, typeName);
        }
        
        if (value is Dictionary<string, object> dict)
        {
            var properties = new List<string>();
            foreach (var kvp in dict)
            {
                string propertyValue = GenerateBasicValueCode(kvp.Value, "string"); // 简化处理
                properties.Add($"{kvp.Key} = {propertyValue}");
            }
            return $"new {typeName} {{ {string.Join(", ", properties)} }}";
        }
        
        return $"new {typeName}()";
    }
    
    /// <summary>
    /// 生成JObject值的代码（递归处理嵌套对象）
    /// </summary>
    /// <param name="jobj">JSON对象</param>
    /// <param name="typeName">目标类型名</param>
    /// <returns>C#代码</returns>
    private string GenerateJObjectValueCode(JObject jobj, string typeName)
    {
        var properties = new List<string>();
        
        foreach (var property in jobj.Properties())
        {
            string propertyValue = GenerateJTokenValueCode(property.Value, property.Name);
            properties.Add($"{property.Name} = {propertyValue}");
        }
        
        return $"new {typeName} {{ {string.Join(", ", properties)} }}";
    }
    
    /// <summary>
    /// 生成JToken值的代码
    /// </summary>
    /// <param name="token">JSON Token</param>
    /// <param name="propertyName">属性名（用于生成嵌套类名）</param>
    /// <returns>C#代码</returns>
    private string GenerateJTokenValueCode(JToken token, string propertyName)
    {
        switch (token.Type)
        {
            case JTokenType.Integer:
                return token.Value<int>().ToString();
            case JTokenType.Float:
                return $"{token.Value<float>()}f";
            case JTokenType.String:
                return $"\"{EscapeString(token.Value<string>())}\"";
            case JTokenType.Boolean:
                return token.Value<bool>().ToString().ToLower();
            case JTokenType.Array:
                return GenerateJArrayValueCode(token as JArray, propertyName);
            case JTokenType.Object:
                string nestedTypeName = $"{propertyName}InfoData";
                return GenerateJObjectValueCode(token as JObject, nestedTypeName);
            default:
                return "null";
        }
    }
    
    /// <summary>
    /// 生成JArray值的代码
    /// </summary>
    /// <param name="jarray">JSON数组</param>
    /// <param name="propertyName">属性名</param>
    /// <returns>C#代码</returns>
    private string GenerateJArrayValueCode(JArray jarray, string propertyName)
    {
        if (jarray.Count == 0)
        {
            return "new object[0]";
        }
        
        var elements = new List<string>();
        var firstElement = jarray[0];
        
        string elementTypeName = null;
        switch (firstElement.Type)
        {
            case JTokenType.Integer:
                elementTypeName = "int";
                break;
            case JTokenType.Float:
                elementTypeName = "float";
                break;
            case JTokenType.String:
                elementTypeName = "string";
                break;
            case JTokenType.Boolean:
                elementTypeName = "bool";
                break;
            case JTokenType.Object:
                elementTypeName = $"{propertyName}ElementData";
                break;
            default:
                elementTypeName = "object";
                break;
        }
        
        foreach (var element in jarray)
        {
            elements.Add(GenerateJTokenValueCode(element, $"{propertyName}Element"));
        }
        
        return $"new {elementTypeName}[] {{ {string.Join(", ", elements)} }}";
    }

    /// <summary>
    /// 获取默认值代码
    /// </summary>
    /// <param name="field">字段信息</param>
    /// <returns>默认值的C#代码</returns>
    private string GetDefaultValueCode(FieldInfo field)
    {
        if (field.IsArray)
        {
            string elementType = GetBasicCSharpType(field.Type.Replace("[]", ""));
            return $"new {elementType}[0]";
        }

        if (field.IsComplex)
        {
            return $"new {field.Name}Data()";
        }

        switch (field.Type.ToLower())
        {
            case "int":
                return "0";
            case "float":
                return "0f";
            case "bool":
                return "false";
            case "string":
                return "\"\"";
            default:
                return "null";
        }
    }

    /// <summary>
    /// 生成数据访问方法
    /// </summary>
    /// <param name="codeBuilder">代码构建器</param>
    /// <param name="sheetInfo">表格信息</param>
    private void GenerateDataAccessMethods(StringBuilder codeBuilder, ExcelSheetInfo sheetInfo)
    {
        string className = sheetInfo.SheetName;
        
        // 查找ID字段以确定具体的类型
        var idField = sheetInfo.Fields.FirstOrDefault(f => 
            f.Name.Equals("ID", StringComparison.OrdinalIgnoreCase) ||
            f.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        
        string idType = "int"; // 默认类型
        if (idField != null)
        {
            idType = GetCSharpType(idField);
        }
        
        // 生成主要的泛型获取方法
        codeBuilder.AppendLine("        /// <summary>");
        codeBuilder.AppendLine("        /// 根据ID获取配置数据");
        codeBuilder.AppendLine("        /// </summary>");
        codeBuilder.AppendLine("        /// <param name=\"id\">配置ID</param>");
        codeBuilder.AppendLine($"        /// <returns>配置数据，如果不存在返回null</returns>");
        codeBuilder.AppendLine($"        public static {className} GetConfig<T>(T id)");
        codeBuilder.AppendLine("        {");
        codeBuilder.AppendLine("            // 尝试将泛型类型转换为字典键类型");
        codeBuilder.AppendLine($"            if (id is {idType} typedId)");
        codeBuilder.AppendLine("            {");
        codeBuilder.AppendLine("                return _configs.TryGetValue(typedId, out var config) ? config : null;");
        codeBuilder.AppendLine("            }");
        codeBuilder.AppendLine("            return null;");
        codeBuilder.AppendLine("        }");
        codeBuilder.AppendLine();

        // 生成具体类型的重载方法
        codeBuilder.AppendLine("        /// <summary>");
        codeBuilder.AppendLine($"        /// 根据{idType}类型ID获取配置数据");
        codeBuilder.AppendLine("        /// </summary>");
        codeBuilder.AppendLine($"        public static {className} GetConfig({idType} id)");
        codeBuilder.AppendLine("        {");
        codeBuilder.AppendLine("            return _configs.TryGetValue(id, out var config) ? config : null;");
        codeBuilder.AppendLine("        }");
        codeBuilder.AppendLine();

        // 如果ID不是int类型，也提供int类型的重载（为了兼容性）
        if (idType != "int")
        {
            codeBuilder.AppendLine("        /// <summary>");
            codeBuilder.AppendLine("        /// 根据int类型ID获取配置数据（兼容性方法）");
            codeBuilder.AppendLine("        /// </summary>");
            codeBuilder.AppendLine($"        public static {className} GetConfig(int id)");
            codeBuilder.AppendLine("        {");
            if (idType == "string")
            {
                codeBuilder.AppendLine("            return GetConfig(id.ToString());");
            }
            else
            {
                codeBuilder.AppendLine("            return GetConfig<int>(id);");
            }
            codeBuilder.AppendLine("        }");
            codeBuilder.AppendLine();
        }
        
        // 如果ID不是string类型，也提供string类型的重载（为了兼容性）
        if (idType != "string")
        {
            codeBuilder.AppendLine("        /// <summary>");
            codeBuilder.AppendLine("        /// 根据string类型ID获取配置数据（兼容性方法）");
            codeBuilder.AppendLine("        /// </summary>");
            codeBuilder.AppendLine($"        public static {className} GetConfig(string id)");
            codeBuilder.AppendLine("        {");
            if (idType == "int")
            {
                codeBuilder.AppendLine("            if (int.TryParse(id, out int intId))");
                codeBuilder.AppendLine("            {");
                codeBuilder.AppendLine("                return GetConfig(intId);");
                codeBuilder.AppendLine("            }");
                codeBuilder.AppendLine("            return null;");
            }
            else
            {
                codeBuilder.AppendLine("            return GetConfig<string>(id);");
            }
            codeBuilder.AppendLine("        }");
            codeBuilder.AppendLine();
        }

        // 获取所有配置的方法
        codeBuilder.AppendLine("        /// <summary>");
        codeBuilder.AppendLine("        /// 获取所有配置数据");
        codeBuilder.AppendLine("        /// </summary>");
        codeBuilder.AppendLine($"        /// <returns>所有配置数据的字典</returns>");
        codeBuilder.AppendLine($"        public static Dictionary<{idType}, {className}> GetAllConfigs()");
        codeBuilder.AppendLine("        {");
        codeBuilder.AppendLine($"            return new Dictionary<{idType}, {className}>(_configs);");
        codeBuilder.AppendLine("        }");
        codeBuilder.AppendLine();

        // 检查配置是否存在的方法
        codeBuilder.AppendLine("        /// <summary>");
        codeBuilder.AppendLine("        /// 检查指定ID的配置是否存在");
        codeBuilder.AppendLine("        /// </summary>");
        codeBuilder.AppendLine("        /// <param name=\"id\">配置ID</param>");
        codeBuilder.AppendLine("        /// <returns>如果存在返回true</returns>");
        codeBuilder.AppendLine($"        public static bool HasConfig({idType} id)");
        codeBuilder.AppendLine("        {");
        codeBuilder.AppendLine("            return _configs.ContainsKey(id);");
        codeBuilder.AppendLine("        }");
        codeBuilder.AppendLine();
        
        // 泛型版本
        codeBuilder.AppendLine("        /// <summary>");
        codeBuilder.AppendLine("        /// 检查指定ID的配置是否存在（泛型版本）");
        codeBuilder.AppendLine("        /// </summary>");
        codeBuilder.AppendLine("        public static bool HasConfig<T>(T id)");
        codeBuilder.AppendLine("        {");
        codeBuilder.AppendLine($"            if (id is {idType} typedId)");
        codeBuilder.AppendLine("            {");
        codeBuilder.AppendLine("                return _configs.ContainsKey(typedId);");
        codeBuilder.AppendLine("            }");
        codeBuilder.AppendLine("            return false;");
        codeBuilder.AppendLine("        }");
        codeBuilder.AppendLine();

        // 获取配置数量的方法
        codeBuilder.AppendLine("        /// <summary>");
        codeBuilder.AppendLine("        /// 获取配置数据总数");
        codeBuilder.AppendLine("        /// </summary>");
        codeBuilder.AppendLine("        /// <returns>配置数据总数</returns>");
        codeBuilder.AppendLine("        public static int GetConfigCount()");
        codeBuilder.AppendLine("        {");
        codeBuilder.AppendLine("            return _configs.Count;");
        codeBuilder.AppendLine("        }");
    }

    /// <summary>
    /// 获取C#类型字符串
    /// </summary>
    /// <param name="field">字段信息</param>
    /// <returns>C#类型字符串</returns>
    private string GetCSharpType(FieldInfo field)
    {
        if (field.IsArray)
        {
            string elementType = field.Type.Replace("[]", "");
            return $"{GetBasicCSharpType(elementType)}[]";
        }
        
        if (field.IsComplex)
        {
            // 使用已经分析和更新的复杂类型名称
            if (!string.IsNullOrEmpty(field.Type) && field.Type != "object")
            {
                return field.Type;
            }
            else
            {
                return $"{field.Name}Data"; // 复杂类型使用生成的辅助类（兜底）
            }
        }
        
        return GetBasicCSharpType(field.Type);
    }

    /// <summary>
    /// 获取基础C#类型
    /// </summary>
    /// <param name="type">类型字符串</param>
    /// <returns>C#类型字符串</returns>
    private string GetBasicCSharpType(string type)
    {
        switch (type.ToLower())
        {
            case "int":
                return "int";
            case "float":
                return "float";
            case "bool":
                return "bool";
            case "string":
                return "string";
            case "object":
                return "object";
            default:
                // 对于其他类型（如复杂类型名称），直接返回原类型名
                return type;
        }
    }

    /// <summary>
    /// 转义字符串中的特殊字符
    /// </summary>
    /// <param name="str">原始字符串</param>
    /// <returns>转义后的字符串</returns>
    private string EscapeString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
            
        return str.Replace("\\", "\\\\")
                  .Replace("\"", "\\\"")
                  .Replace("\n", "\\n")
                  .Replace("\r", "\\r")
                  .Replace("\t", "\\t");
    }

    /// <summary>
    /// 日志输出辅助方法
    /// </summary>
    private void LogMessage(string message, LogType logType = LogType.Log)
    {
        if (_logCallback != null)
        {
            _logCallback(message, logType);
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
}

#region 辅助数据结构

/// <summary>
/// 复杂类型信息
/// </summary>
public class ComplexTypeInfo
{
    public string Name;
    public Dictionary<string, PropertyTypeInfo> Properties = new Dictionary<string, PropertyTypeInfo>();
    public List<ComplexTypeInfo> NestedTypes = new List<ComplexTypeInfo>();
}

/// <summary>
/// 属性类型信息
/// </summary>
public class PropertyTypeInfo
{
    public string TypeName; // 基础类型名称，如 int, string, CustomClass
    public bool IsArray; // 是否为数组
    public ComplexTypeInfo NestedType; // 如果是复杂类型，指向嵌套类型信息
}

#endregion 