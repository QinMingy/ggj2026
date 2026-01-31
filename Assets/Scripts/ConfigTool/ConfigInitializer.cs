using UnityEngine;

/// <summary>
/// 配置初始化器
/// 在游戏启动时自动初始化配置管理器
/// </summary>
public class ConfigInitializer : MonoBehaviour
{
    [Header("配置设置")]
    [SerializeField] private bool autoInitializeOnStart = true;
    [SerializeField] private bool showDebugInfo = true;
    
    #region Unity生命周期

    void Start()
    {
        if (autoInitializeOnStart)
        {
            InitializeConfigs();
        }
    }

    #endregion

    #region 公共方法

    /// <summary>
    /// 初始化所有配置数据
    /// </summary>
    [ContextMenu("初始化配置数据")]
    public void InitializeConfigs()
    {
        try
        {
            Debug.Log("开始初始化配置数据...");
            
            // 初始化配置管理器
            ConfigManager.Instance.Initialize();
            
            if (showDebugInfo)
            {
                // 输出配置统计信息
                Debug.Log(ConfigManager.Instance.GetConfigStats());
            }
            
            Debug.Log("配置数据初始化完成！");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"配置数据初始化失败：{e.Message}");
        }
    }

    /// <summary>
    /// 重新加载所有配置数据
    /// </summary>
    [ContextMenu("重新加载配置数据")]
    public void ReloadConfigs()
    {
        try
        {
            Debug.Log("重新加载配置数据...");
            ConfigManager.Instance.ReloadAllConfigs();
            
            if (showDebugInfo)
            {
                Debug.Log(ConfigManager.Instance.GetConfigStats());
            }
            
            Debug.Log("配置数据重新加载完成！");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"配置数据重新加载失败：{e.Message}");
        }
    }

    /// <summary>
    /// 测试配置数据访问
    /// </summary>
    [ContextMenu("测试配置数据访问")]
    public void TestConfigAccess()
    {
        Debug.Log("=== 配置数据访问测试 ===");
        
        // 这里可以添加具体的配置测试代码
        // 例如：
        // var itemConfig = ConfigManager.Instance.GetConfig<ItemConfig>(1001);
        // if (itemConfig != null)
        // {
        //     Debug.Log($"找到物品配置：{itemConfig}");
        // }
        
        Debug.Log("测试完成");
    }

    #endregion

    #region 编辑器方法

#if UNITY_EDITOR
    /// <summary>
    /// 在Inspector中显示配置统计信息
    /// </summary>
    void OnValidate()
    {
        // 这个方法在Inspector值改变时调用
        // 可以在这里添加验证逻辑
    }
#endif

    #endregion
} 