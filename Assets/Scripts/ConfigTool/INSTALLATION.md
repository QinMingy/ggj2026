# Unity配置表工具 - 安装指南

## 📋 系统要求

- **Unity版本**: 2019.4 LTS 或更高版本
- **操作系统**: Windows, macOS, Linux
- **.NET版本**: .NET Standard 2.0 或更高

## 🔧 依赖项安装

### 1. 安装 Newtonsoft.Json

这是Unity官方支持的JSON处理库：

#### 方法一：通过Package Manager（推荐）
1. 打开Unity
2. 进入 `Window` → `Package Manager`
3. 在左上角选择 `Unity Registry`
4. 搜索 `Newtonsoft Json`
5. 点击 `Install` 安装

#### 方法二：通过manifest.json
在 `Packages/manifest.json` 文件中添加：
```json
{
  "dependencies": {
    "com.unity.nuget.newtonsoft-json": "3.0.2"
  }
}
```

### 2. 安装 ExcelDataReader

这个库用于读取Excel文件：

#### 方法一：手动下载DLL文件（推荐）
1. 下载以下DLL文件并放入 `Assets/Plugins/` 目录：
   - `ExcelDataReader.dll`
   - `ExcelDataReader.DataSet.dll`
   - `System.Data.DataSetExtensions.dll`（如果需要）

你可以从以下地址获取这些文件：
- [ExcelDataReader GitHub Releases](https://github.com/ExcelDataReader/ExcelDataReader/releases)

#### 方法二：使用NuGet包管理器
如果你有NuGet for Unity插件：
1. 安装 `NuGet for Unity` 插件
2. 搜索并安装 `ExcelDataReader`
3. 搜索并安装 `ExcelDataReader.DataSet`

### 3. 验证安装

创建一个测试脚本来验证所有依赖项是否正确安装：

```csharp
using UnityEngine;
using Newtonsoft.Json;
using ExcelDataReader;
using System.Data;

public class DependencyTest : MonoBehaviour
{
    void Start()
    {
        // 测试 Newtonsoft.Json
        var testObj = new { name = "test", value = 123 };
        string json = JsonConvert.SerializeObject(testObj);
        Debug.Log($"JSON测试成功: {json}");
        
        // 测试 ExcelDataReader（基本API检查）
        Debug.Log($"ExcelDataReader版本检查通过");
        
        Debug.Log("所有依赖项安装成功！");
    }
}
```

## 🚨 常见问题与解决方案

### 问题1：`The type or namespace name 'ExcelDataReader' could not be found`

**解决方案：**
1. 确保已下载并放置ExcelDataReader相关DLL文件
2. 检查DLL文件是否在正确的文件夹中（`Assets/Plugins/`）
3. 重启Unity编辑器

### 问题2：`无法解析符号'CodePagesEncodingProvider'`

**解决方案：**
这个问题在最新版本中已修复。如果仍然遇到，请：
1. 更新到最新版本的工具
2. 确保使用.NET Standard 2.0或更高版本

### 问题3：Excel文件读取失败

**解决方案：**
1. 确保Excel文件没有被其他程序占用
2. 检查文件路径是否正确
3. 确保ExcelDataReader.DataSet.dll已正确安装

### 问题4：Newtonsoft.Json版本冲突

**解决方案：**
1. 使用Unity官方的Newtonsoft.Json包
2. 避免手动导入其他版本的Newtonsoft.Json
3. 清理项目中的旧版本DLL文件

## 📁 目录结构检查

安装完成后，你的项目结构应该类似：

```
Assets/
├── Plugins/                           # ExcelDataReader DLL文件
│   ├── ExcelDataReader.dll
│   ├── ExcelDataReader.DataSet.dll
│   └── ...
├── Scripts/
│   └── ConfigTool/                    # 配置工具文件
│       ├── ConfigManager.cs
│       ├── IConfigData.cs
│       ├── Editor/
│       │   ├── ConfigTableExporter.cs
│       │   ├── ExcelParser.cs
│       │   └── ...
│       └── ...
├── Resources/
│   └── ConfigData/                    # 生成的JSON文件
└── ...

Packages/
└── manifest.json                     # 包含Newtonsoft.Json依赖
```

## 🧪 安装验证清单

完成安装后，请检查以下项目：

- [ ] Unity Package Manager中可以看到Newtonsoft.Json包
- [ ] `Assets/Plugins/` 目录包含ExcelDataReader DLL文件
- [ ] 测试脚本可以成功编译和运行
- [ ] Unity控制台没有依赖项相关的错误
- [ ] 可以在菜单中看到 `工具/配置表工具` 选项

## 📞 技术支持

如果你在安装过程中遇到任何问题：

1. **检查Unity版本兼容性**
2. **清理并重新导入项目**：删除 `Library` 文件夹，重新打开项目
3. **检查.NET兼容性级别**：确保项目设置为.NET Standard 2.0
4. **重新安装依赖项**：先删除现有的依赖项，然后重新安装

## 🔄 更新指南

当工具更新时：

1. **备份现有配置**：保存你的Excel文件和生成的配置类
2. **更新工具文件**：替换ConfigTool文件夹中的文件
3. **检查依赖项**：确保所有依赖项仍然正确安装
4. **重新生成配置**：使用新版本重新导出配置文件

---

**安装完成后，请参阅 `README.md` 文件了解详细的使用说明。** 