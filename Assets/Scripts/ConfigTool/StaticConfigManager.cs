using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 静态配置管理器
/// 用于管理直接写入C#代码中的配置数据
/// 提供统一的配置访问接口
/// </summary>
public static class StaticConfigManager
{
    #region 私有字段

    /// <summary>
    /// 配置统计信息缓存
    /// </summary>
    private static string _cachedStats = null;

    #endregion

    #region 公共接口

    /// <summary>
    /// 获取配置统计信息
    /// </summary>
    /// <returns>配置统计信息字符串</returns>
    public static string GetConfigStats()
    {
        if (_cachedStats == null)
        {
            GenerateConfigStats();
        }
        return _cachedStats;
    }

    /// <summary>
    /// 刷新配置统计信息
    /// </summary>
    public static void RefreshStats()
    {
        _cachedStats = null;
        GenerateConfigStats();
    }

    /// <summary>
    /// 验证所有配置数据
    /// </summary>
    /// <returns>验证结果</returns>
    public static ConfigValidationResult ValidateAllConfigs()
    {
        var result = new ConfigValidationResult();
        
        try
        {
            // 这里可以通过反射找到所有的配置管理器类并验证
            // 当前提供基础的验证框架
            result.IsValid = true;
            result.Message = "配置验证功能需要根据具体的配置类进行实现";
        }
        catch (Exception e)
        {
            result.IsValid = false;
            result.Message = $"配置验证时发生错误：{e.Message}";
        }
        
        return result;
    }

    /// <summary>
    /// 初始化所有配置（实际上静态配置无需初始化）
    /// </summary>
    public static void Initialize()
    {
        Debug.Log("静态配置管理器：配置数据已在编译时加载，无需初始化");
        Debug.Log(GetConfigStats());
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 生成配置统计信息
    /// </summary>
    private static void GenerateConfigStats()
    {
        var stats = "=== 静态配置数据统计 ===\n";
        stats += "配置数据已编译到代码中，访问效率最高\n";
        stats += "无需运行时文件读取，启动速度更快\n";
        
        // 这里可以通过反射统计具体的配置类数量
        // 当前提供基础信息
        stats += "提示：具体配置统计信息需要在生成的管理器类中实现\n";
        
        _cachedStats = stats;
    }

    #endregion
}

#region 辅助类

/// <summary>
/// 配置验证结果
/// </summary>
public class ConfigValidationResult
{
    public bool IsValid { get; set; } = true;
    public string Message { get; set; } = "";
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
}

#endregion 