using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 配置使用示例
/// 展示如何在实际项目中使用生成的配置数据
/// </summary>
public class ConfigUsageExample : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool runTestOnStart = false;
    
    void Start()
    {
        if (runTestOnStart)
        {
            // 确保配置管理器已初始化
            if (!IsConfigManagerInitialized())
            {
                ConfigManager.Instance.Initialize();
            }
            
            RunConfigUsageExamples();
        }
    }

    /// <summary>
    /// 运行配置使用示例
    /// </summary>
    [ContextMenu("运行配置使用示例")]
    public void RunConfigUsageExamples()
    {
        Debug.Log("=== 配置使用示例开始 ===");
        
        // 示例1：基础配置访问
        ExampleBasicConfigAccess();
        
        // 示例2：批量配置处理
        ExampleBatchConfigProcessing();
        
        // 示例3：配置数据验证
        ExampleConfigValidation();
        
        // 示例4：配置不存在的处理
        ExampleHandleMissingConfig();
        
        Debug.Log("=== 配置使用示例结束 ===");
    }

    /// <summary>
    /// 示例1：基础配置访问
    /// </summary>
    private void ExampleBasicConfigAccess()
    {
        Debug.Log("--- 示例1：基础配置访问 ---");
        
        // 注意：这里使用的是假设的ItemConfig类
        // 实际使用时，请替换为你生成的配置类
        
        /*
        // 获取单个配置
        var itemConfig = ConfigManager.Instance.GetConfig<ItemConfig>(1001);
        if (itemConfig != null)
        {
            Debug.Log($"找到物品：{itemConfig.Name}，价格：{itemConfig.Price}");
            
            // 访问数组属性
            if (itemConfig.Tags != null && itemConfig.Tags.Length > 0)
            {
                Debug.Log($"标签：{string.Join(", ", itemConfig.Tags)}");
            }
            
            // 访问复杂对象属性（如果存在）
            if (itemConfig.Properties != null)
            {
                Debug.Log($"额外属性：{JsonUtility.ToJson(itemConfig.Properties)}");
            }
        }
        else
        {
            Debug.LogWarning("未找到ID为1001的物品配置");
        }
        */
        
        Debug.Log("基础配置访问示例（请在生成配置类后取消注释上述代码）");
    }

    /// <summary>
    /// 示例2：批量配置处理
    /// </summary>
    private void ExampleBatchConfigProcessing()
    {
        Debug.Log("--- 示例2：批量配置处理 ---");
        
        /*
        // 获取所有配置
        var allItems = ConfigManager.Instance.GetAllConfigs<ItemConfig>();
        
        Debug.Log($"总共有{allItems.Count}个物品配置");
        
        // 筛选特定类型的物品
        var weapons = new List<ItemConfig>();
        foreach (var item in allItems.Values)
        {
            if (item.Tags != null && System.Array.Exists(item.Tags, tag => tag.Contains("武器")))
            {
                weapons.Add(item);
            }
        }
        
        Debug.Log($"找到{weapons.Count}件武器");
        
        // 按价格排序
        weapons.Sort((a, b) => a.Price.CompareTo(b.Price));
        
        foreach (var weapon in weapons)
        {
            Debug.Log($"武器：{weapon.Name}，价格：{weapon.Price}");
        }
        */
        
        Debug.Log("批量配置处理示例（请在生成配置类后取消注释上述代码）");
    }

    /// <summary>
    /// 示例3：配置数据验证
    /// </summary>
    private void ExampleConfigValidation()
    {
        Debug.Log("--- 示例3：配置数据验证 ---");
        
        /*
        var allItems = ConfigManager.Instance.GetAllConfigs<ItemConfig>();
        
        int validCount = 0;
        int invalidCount = 0;
        
        foreach (var item in allItems.Values)
        {
            if (item.IsValid())
            {
                validCount++;
            }
            else
            {
                invalidCount++;
                Debug.LogWarning($"无效的配置数据：{item}");
            }
        }
        
        Debug.Log($"配置验证完成：有效{validCount}个，无效{invalidCount}个");
        */
        
        Debug.Log("配置数据验证示例（请在生成配置类后取消注释上述代码）");
    }

    /// <summary>
    /// 示例4：配置不存在的处理
    /// </summary>
    private void ExampleHandleMissingConfig()
    {
        Debug.Log("--- 示例4：配置不存在的处理 ---");
        
        /*
        int nonExistentId = 9999;
        
        // 方法1：先检查是否存在
        if (ConfigManager.Instance.HasConfig<ItemConfig>(nonExistentId))
        {
            var config = ConfigManager.Instance.GetConfig<ItemConfig>(nonExistentId);
            Debug.Log($"找到配置：{config.Name}");
        }
        else
        {
            Debug.LogWarning($"配置ID {nonExistentId} 不存在");
        }
        
        // 方法2：直接获取并检查null
        var config2 = ConfigManager.Instance.GetConfig<ItemConfig>(nonExistentId);
        if (config2 != null)
        {
            Debug.Log($"找到配置：{config2.Name}");
        }
        else
        {
            Debug.LogWarning($"配置ID {nonExistentId} 不存在");
            
            // 可以提供默认值或降级处理
            Debug.Log("使用默认配置或跳过该逻辑");
        }
        */
        
        Debug.Log("配置不存在处理示例（请在生成配置类后取消注释上述代码）");
    }

    /// <summary>
    /// 检查配置管理器是否已初始化
    /// </summary>
    /// <returns></returns>
    private bool IsConfigManagerInitialized()
    {
        try
        {
            // 尝试获取配置统计信息，如果出错说明未初始化
            var stats = ConfigManager.Instance.GetConfigStats();
            return !string.IsNullOrEmpty(stats);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 实战应用示例：物品系统
    /// </summary>
    [ContextMenu("物品系统应用示例")]
    public void ItemSystemExample()
    {
        Debug.Log("=== 物品系统应用示例 ===");
        
        /*
        // 假设在游戏中获得了一个物品
        int obtainedItemId = 1001;
        
        var itemConfig = ConfigManager.Instance.GetConfig<ItemConfig>(obtainedItemId);
        if (itemConfig != null)
        {
            // 创建物品实例
            var gameItem = CreateGameItem(itemConfig);
            
            // 添加到背包
            AddToInventory(gameItem);
            
            // 显示获得物品的UI提示
            ShowItemObtainedUI(itemConfig);
        }
        */
        
        Debug.Log("请在生成配置类后实现具体的物品系统逻辑");
    }

    /*
    // 这些方法在实际项目中需要具体实现
    private GameObject CreateGameItem(ItemConfig config)
    {
        // 根据配置创建游戏物品对象
        var gameObject = new GameObject(config.Name);
        var item = gameObject.AddComponent<Item>();
        item.Initialize(config);
        return gameObject;
    }
    
    private void AddToInventory(GameObject item)
    {
        // 添加到背包系统
        Debug.Log($"物品 {item.name} 已添加到背包");
    }
    
    private void ShowItemObtainedUI(ItemConfig config)
    {
        // 显示获得物品的UI
        Debug.Log($"获得物品：{config.Name}");
    }
    */

    /// <summary>
    /// 性能测试示例
    /// </summary>
    [ContextMenu("配置访问性能测试")]
    public void PerformanceTest()
    {
        Debug.Log("=== 配置访问性能测试 ===");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // 模拟频繁的配置访问
        int testCount = 10000;
        
        /*
        for (int i = 0; i < testCount; i++)
        {
            var config = ConfigManager.Instance.GetConfig<ItemConfig>(1001);
            // 模拟使用配置
            if (config != null)
            {
                var name = config.Name;
            }
        }
        */
        
        stopwatch.Stop();
        
        Debug.Log($"访问{testCount}次配置耗时：{stopwatch.ElapsedMilliseconds}ms");
        Debug.Log($"平均每次访问耗时：{(float)stopwatch.ElapsedMilliseconds / testCount:F4}ms");
    }
} 