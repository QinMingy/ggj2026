# Unity配置表工具使用说明

## 功能概述

这是一个完整的Unity配置表管理系统，支持从Excel文件自动生成C#配置数据类和JSON数据文件，并提供运行时加载和访问接口。

## 核心特性

- 📊 **Excel解析**：支持.xlsx和.xls格式的Excel文件
- 🔧 **自动代码生成**：根据Excel结构自动生成C#配置数据类
- 📝 **多种数据类型**：支持int、string、数组、复杂JSON对象
- 🎯 **类型安全**：生成的代码具有完整的类型安全保证
- 🚀 **运行时高效**：使用单例模式管理，支持缓存和按需加载
- 🔍 **数据验证**：内置数据有效性验证机制
- 📂 **智能文件访问**：支持Excel文件被打开时的导表操作

## Excel表格格式规范

### 表格结构
```
第1行：字段描述（中文说明）
第2行：字段名称（C#属性名）
第3行：数据类型（int、string、int[]、object等）
第4行+：实际配置数据
```

### 示例表格

| 物品ID | 物品名称 | 物品价格 | 标签列表 | 额外属性 | 人物信息 |
|--------|----------|----------|----------|----------|----------|
| ID | Name | Price | Tags | Properties | Human |
| int | string | int | string[] | object | object |
| 1001 | 火焰剑 | 500 | 武器,火属性 | {"attack":50,"durability":100} | {"name":"张三","info":{"age":25,"gender":1}} |
| 1002 | 治疗药水 | 50 | 消耗品,治疗 | {"heal":25,"cooldown":3} | {"name":"李四","info":{"age":30,"gender":0}} |

### 支持的数据类型

#### 基础类型
- `int`：整数
- `string`：字符串
- `float`：浮点数
- `bool`：布尔值

#### 数组类型
- `int[]`：整数数组，格式：`1,2,3`（也支持旧格式：`{1,2,3}`）
- `string[]`：字符串数组，格式：`item1,item2,item3`（也支持旧格式：`{item1,item2,item3}`）

#### 复杂类型
- `object`：JSON对象，格式：`{"key1":"value1","key2":123}`
- 支持嵌套对象和数组的递归类型生成
- 例如：`{"name":"John", "info":{"age":25, "skills":["C#","Unity"]}}`
  - 自动生成 `HumanData` 类
  - 自动生成 `InfoInfoData` 嵌套类
  - 生成强类型对象而非JSON字符串

#### 生成的代码示例
对于上述Human字段，会自动生成：
```csharp
public class HumanData
{
    [JsonProperty("name")]
    public string name { get; set; }
    
    [JsonProperty("info")]
    public InfoInfoData info { get; set; }
}

public class InfoInfoData
{
    [JsonProperty("age")]
    public int age { get; set; }
    
    [JsonProperty("skills")]
    public string[] skills { get; set; }
}
```

实例化代码：
```csharp
Human = new HumanData 
{ 
    name = "John", 
    info = new InfoInfoData 
    { 
        age = 25, 
        skills = new string[] { "C#", "Unity" } 
    } 
}
```

## 使用流程

### 1. 准备Excel文件
按照上述格式创建Excel配置表，确保：
- 第一行是字段描述
- 第二行是有效的C#标识符
- 第三行是支持的数据类型
- 数据从第四行开始

### 2. 使用Editor工具导出

#### 方法一：使用导出器窗口
1. 在Unity中打开：`工具 → 配置表工具 → Excel配置导出器`
2. 点击"浏览"选择Excel文件
3. 点击"解析Excel文件"预览表格结构
4. 点击"生成配置代码和数据"完成导出

#### 方法二：一键导出
1. 在Unity中选择：`工具 → 配置表工具 → 一键导出所有配置`
2. 选择Excel文件后自动完成所有步骤

### 3. 文件占用处理

当Excel文件被打开时，导表工具会自动尝试多种方式读取文件：

#### 🔄 自动处理机制
1. **共享读取模式**：首先尝试与Excel共享文件访问
2. **临时文件复制**：如果共享失败，自动复制到临时文件
3. **只读模式重试**：最后尝试只读模式访问

#### 💡 最佳实践
- **推荐**：可以保持Excel打开，导表工具会自动处理
- **备选**：如遇问题，关闭Excel后重试
- **避免**：避免在导表过程中修改Excel文件

#### 📋 状态提示
- ✅ **文件状态正常**：可以直接导表
- ⚠️ **文件被占用**：工具会自动尝试多种读取方式
- ❌ **读取失败**：建议关闭Excel或另存为新文件

### 4. 生成的文件

工具会在以下位置生成文件：

```
Assets/Scripts/ConfigTool/Generated/  # C#配置类文件
├── ItemConfig.cs                    # 物品配置类
├── PlayerConfig.cs                  # 玩家配置类
└── ...

Assets/Resources/ConfigData/         # JSON数据文件
├── ItemConfig.json                  # 物品配置数据
├── PlayerConfig.json                # 玩家配置数据
└── ...
```

### 5. 运行时使用

#### 初始化配置管理器
```csharp
// 方法1：使用ConfigInitializer组件（推荐）
// 在场景中添加ConfigInitializer组件，它会在Start时自动初始化

// 方法2：手动初始化
ConfigManager.Instance.Initialize();
```

#### 访问配置数据
```csharp
// 获取单个配置
var itemConfig = ConfigManager.Instance.GetConfig<ItemConfig>(1001);
if (itemConfig != null)
{
    Debug.Log($"物品名称：{itemConfig.Name}，价格：{itemConfig.Price}");
}

// 获取所有配置
var allItems = ConfigManager.Instance.GetAllConfigs<ItemConfig>();
foreach (var item in allItems.Values)
{
    Debug.Log($"物品：{item.Name}");
}

// 检查配置是否存在
bool hasItem = ConfigManager.Instance.HasConfig<ItemConfig>(1001);
```

#### 生成的配置类示例
```csharp
namespace ConfigData
{
    [Serializable]
    public class ItemConfig : ConfigDataBase
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        [JsonProperty("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 物品价格
        /// </summary>
        [JsonProperty("Price")]
        public int Price { get; set; }

        /// <summary>
        /// 标签列表
        /// </summary>
        [JsonProperty("Tags")]
        public string[] Tags { get; set; }

        /// <summary>
        /// 额外属性
        /// </summary>
        [JsonProperty("Properties")]
        public PropertiesData Properties { get; set; }

        public override int ID => ID;
        
        public override bool IsValid()
        {
            return base.IsValid() && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
```

## 高级功能

### 数据验证
每个生成的配置类都包含`IsValid()`方法，用于验证数据有效性：

```csharp
if (!itemConfig.IsValid())
{
    Debug.LogWarning("配置数据无效");
}
```

### 复杂类型处理
对于JSON格式的复杂数据，工具会自动生成对应的数据类：

```csharp
// Excel中的JSON：{"attack":50,"durability":100}
// 生成的类：
public class PropertiesData
{
    [JsonProperty("attack")]
    public int attack { get; set; }
    
    [JsonProperty("durability")]
    public int durability { get; set; }
}
```

### 配置重载
支持运行时重新加载配置数据：

```csharp
ConfigManager.Instance.ReloadAllConfigs();
```

## 最佳实践

### 1. Excel表格设计
- 使用清晰的字段描述
- 字段名使用PascalCase命名规范
- 确保ID字段唯一性
- 复杂JSON数据保持结构一致

### 2. 代码组织
- 将生成的配置类放在单独的命名空间中
- 不要手动修改生成的代码
- 使用继承和接口扩展配置功能

### 3. 性能优化
- 在游戏启动时一次性加载所有配置
- 避免频繁的配置查询
- 考虑缓存常用的配置数据

### 4. 版本管理
- 生成的代码文件加入版本控制
- JSON数据文件也应纳入版本管理
- 保持Excel源文件的版本同步

## 故障排除

### 常见问题

1. **Excel解析失败**
   - 检查Excel文件格式是否正确
   - 确保前三行的格式符合规范
   - 验证字段名是否为有效的C#标识符

2. **代码生成错误**
   - 检查字段类型是否支持
   - 确保复杂JSON格式正确
   - 查看Unity控制台的详细错误信息

3. **运行时加载失败**
   - 确认JSON文件在Resources/ConfigData目录下
   - 检查生成的类是否在ConfigData命名空间中
   - 验证JSON数据格式是否正确

### 调试技巧

```csharp
// 输出配置统计信息
Debug.Log(ConfigManager.Instance.GetConfigStats());

// 验证配置数据
var validation = ConfigManager.Instance.ValidateConfigs();
if (!validation.IsValid)
{
    foreach (var error in validation.Errors)
    {
        Debug.LogError(error);
    }
}
```

## 扩展开发

### 自定义类型支持
在`ExcelParser.cs`中的`ParseFieldType`方法中添加新的类型支持。

### 自定义验证规则
在生成的配置类中重写`IsValid()`方法添加业务逻辑验证。

### 性能监控
```csharp
// 添加性能监控
System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
ConfigManager.Instance.Initialize();
sw.Stop();
Debug.Log($"配置加载耗时：{sw.ElapsedMilliseconds}ms");
```

## 技术架构

```
Excel文件 → ExcelParser → ExcelSheetInfo → ConfigCodeGenerator → C#类文件
                                        → ConfigDataGenerator → JSON数据文件
                                                              ↓
运行时：ConfigManager ← JSON数据加载 ← Resources.Load ← JSON文件
```

## 依赖项

- Unity 2019.4+
- Newtonsoft.Json（Unity Package Manager）
- ExcelDataReader（需要手动安装NuGet包）

---

**注意：这是一个自动生成的工具，请勿手动修改生成的配置类文件。如需自定义功能，请通过继承或扩展的方式实现。** 