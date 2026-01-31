using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;

/// <summary>
/// 配置数据管理器 - 单例模式
/// 负责加载、缓存和提供配置数据的访问接口
/// </summary>
public class ConfigManager : Singleton<ConfigManager>
{
    #region 配置路径常量
    
    /// <summary>
    /// 配置数据文件存放路径（相对于Resources文件夹）
    /// </summary>
    private const string CONFIG_DATA_PATH = "ConfigData";
    
    /// <summary>
    /// 配置数据命名空间
    /// </summary>
    private const string CONFIG_NAMESPACE = "ConfigData";
    
    #endregion

    #region 私有字段
    
    /// <summary>
    /// 配置数据缓存字典 - 按类型存储配置数据列表
    /// </summary>
    private Dictionary<Type, Dictionary<int, IConfigData>> _configCache = 
        new Dictionary<Type, Dictionary<int, IConfigData>>();
    
    /// <summary>
    /// 已加载的配置类型集合
    /// </summary>
    private HashSet<Type> _loadedTypes = new HashSet<Type>();
    
    /// <summary>
    /// 是否已初始化
    /// </summary>
    private bool _isInitialized = false;
    
    #endregion

    #region 公共接口

    /// <summary>
    /// 初始化配置管理器，加载所有配置数据
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized)
        {
            Debug.LogWarning("ConfigManager已经初始化过了");
            return;
        }

        Debug.Log("开始初始化ConfigManager...");
        
        try
        {
            LoadAllConfigs();
            _isInitialized = true;
            Debug.Log($"ConfigManager初始化完成，共加载{_loadedTypes.Count}种配置类型");
        }
        catch (Exception e)
        {
            Debug.LogError($"ConfigManager初始化失败：{e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 获取指定类型的单个配置数据
    /// </summary>
    /// <typeparam name="T">配置数据类型</typeparam>
    /// <param name="id">配置ID</param>
    /// <returns>配置数据实例，如果不存在返回null</returns>
    public T GetConfig<T>(int id) where T : class, IConfigData
    {
        Type configType = typeof(T);
        
        // 如果该类型尚未加载，尝试加载
        if (!_loadedTypes.Contains(configType))
        {
            LoadConfigType<T>();
        }
        
        // 从缓存中获取数据
        if (_configCache.TryGetValue(configType, out var configDict))
        {
            if (configDict.TryGetValue(id, out var configData))
            {
                return configData as T;
            }
        }
        
        Debug.LogWarning($"未找到配置数据：类型={configType.Name}, ID={id}");
        return null;
    }

    /// <summary>
    /// 获取指定类型的所有配置数据
    /// </summary>
    /// <typeparam name="T">配置数据类型</typeparam>
    /// <returns>配置数据字典，key为ID</returns>
    public Dictionary<int, T> GetAllConfigs<T>() where T : class, IConfigData
    {
        Type configType = typeof(T);
        
        // 如果该类型尚未加载，尝试加载
        if (!_loadedTypes.Contains(configType))
        {
            LoadConfigType<T>();
        }
        
        var result = new Dictionary<int, T>();
        
        if (_configCache.TryGetValue(configType, out var configDict))
        {
            foreach (var kvp in configDict)
            {
                if (kvp.Value is T typedConfig)
                {
                    result[kvp.Key] = typedConfig;
                }
            }
        }
        
        return result;
    }

    /// <summary>
    /// 检查是否存在指定的配置数据
    /// </summary>
    /// <typeparam name="T">配置数据类型</typeparam>
    /// <param name="id">配置ID</param>
    /// <returns>如果存在返回true</returns>
    public bool HasConfig<T>(int id) where T : class, IConfigData
    {
        Type configType = typeof(T);
        
        if (!_loadedTypes.Contains(configType))
        {
            LoadConfigType<T>();
        }
        
        return _configCache.TryGetValue(configType, out var configDict) && 
               configDict.ContainsKey(id);
    }

    /// <summary>
    /// 重新加载所有配置数据
    /// </summary>
    public void ReloadAllConfigs()
    {
        Debug.Log("重新加载所有配置数据...");
        
        _configCache.Clear();
        _loadedTypes.Clear();
        _isInitialized = false;
        
        Initialize();
    }

    /// <summary>
    /// 获取配置统计信息
    /// </summary>
    /// <returns>配置统计信息字符串</returns>
    public string GetConfigStats()
    {
        var stats = "=== 配置数据统计 ===\n";
        
        foreach (var kvp in _configCache)
        {
            var typeName = kvp.Key.Name;
            var count = kvp.Value.Count;
            stats += $"{typeName}: {count}条数据\n";
        }
        
        return stats;
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 加载所有配置数据
    /// </summary>
    private void LoadAllConfigs()
    {
        // 获取所有实现了IConfigData接口的类型
        var configTypes = GetAllConfigDataTypes();
        
        foreach (var configType in configTypes)
        {
            try
            {
                LoadConfigTypeByType(configType);
            }
            catch (Exception e)
            {
                Debug.LogError($"加载配置类型 {configType.Name} 失败：{e.Message}");
            }
        }
    }

    /// <summary>
    /// 加载指定类型的配置数据
    /// </summary>
    /// <typeparam name="T">配置数据类型</typeparam>
    private void LoadConfigType<T>() where T : class, IConfigData
    {
        LoadConfigTypeByType(typeof(T));
    }

    /// <summary>
    /// 根据类型加载配置数据
    /// </summary>
    /// <param name="configType">配置数据类型</param>
    private void LoadConfigTypeByType(Type configType)
    {
        if (_loadedTypes.Contains(configType))
        {
            return; // 已经加载过了
        }

        string fileName = configType.Name;
        string resourcePath = $"{CONFIG_DATA_PATH}/{fileName}";
        
        // 从Resources文件夹加载配置数据
        TextAsset jsonAsset = Resources.Load<TextAsset>(resourcePath);
        
        if (jsonAsset == null)
        {
            Debug.LogWarning($"未找到配置文件：{resourcePath}");
            return;
        }

        try
        {
            // 解析JSON数据
            var configList = ParseConfigData(jsonAsset.text, configType);
            
            // 添加到缓存
            var configDict = new Dictionary<int, IConfigData>();
            
            foreach (var config in configList)
            {
                if (config.IsValid())
                {
                    configDict[config.ID] = config;
                }
                else
                {
                    Debug.LogWarning($"无效的配置数据：{configType.Name}, ID={config.ID}");
                }
            }
            
            _configCache[configType] = configDict;
            _loadedTypes.Add(configType);
            
            Debug.Log($"成功加载配置：{configType.Name}，共{configDict.Count}条数据");
        }
        catch (Exception e)
        {
            Debug.LogError($"解析配置文件失败：{resourcePath}, 错误：{e.Message}");
        }
    }

    /// <summary>
    /// 解析配置数据JSON
    /// </summary>
    /// <param name="jsonContent">JSON内容</param>
    /// <param name="configType">配置类型</param>
    /// <returns>配置数据列表</returns>
    private List<IConfigData> ParseConfigData(string jsonContent, Type configType)
    {
        var result = new List<IConfigData>();
        
        try
        {
            // 创建包装类来解析JSON数组
            var wrapperType = typeof(JsonWrapper<>).MakeGenericType(configType);
            var wrapper = JsonUtility.FromJson(jsonContent, wrapperType);
            
            // 获取数据列表
            var itemsField = wrapperType.GetField("items");
            var items = itemsField.GetValue(wrapper) as Array;
            
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item is IConfigData configData)
                    {
                        result.Add(configData);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON解析失败：{e.Message}");
        }
        
        return result;
    }

    /// <summary>
    /// 获取所有配置数据类型
    /// </summary>
    /// <returns>配置数据类型列表</returns>
    private List<Type> GetAllConfigDataTypes()
    {
        var configTypes = new List<Type>();
        
        // 获取当前程序集中所有实现IConfigData接口的类型
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        
        foreach (var type in types)
        {
            if (typeof(IConfigData).IsAssignableFrom(type) && 
                !type.IsInterface && 
                !type.IsAbstract &&
                type.Namespace == CONFIG_NAMESPACE)
            {
                configTypes.Add(type);
            }
        }
        
        return configTypes;
    }

    #endregion

    #region 辅助类

    /// <summary>
    /// JSON数组包装器，用于Unity的JsonUtility解析
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    [Serializable]
    private class JsonWrapper<T>
    {
        public T[] items;
    }

    #endregion
} 