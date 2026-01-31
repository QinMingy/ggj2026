using System;

/// <summary>
/// 配置数据基础接口（泛型版本）
/// 所有从Excel生成的配置数据类都应该实现此接口
/// </summary>
/// <typeparam name="TId">ID的类型</typeparam>
public interface IConfigData<TId>
{
    /// <summary>
    /// 配置项的唯一ID
    /// </summary>
    TId ID { get; }
    
    /// <summary>
    /// 验证配置数据的有效性
    /// </summary>
    /// <returns>如果数据有效返回true，否则返回false</returns>
    bool IsValid();
}

/// <summary>
/// 配置数据基础接口（向后兼容）
/// </summary>
public interface IConfigData : IConfigData<int>
{
}

/// <summary>
/// 配置数据基类（泛型版本），提供默认实现
/// </summary>
[Serializable]
public abstract class ConfigDataBase<TId> : IConfigData<TId>
{
    public abstract TId ID { get; }
    
    /// <summary>
    /// 默认验证实现，子类可以重写此方法添加自定义验证逻辑
    /// </summary>
    /// <returns></returns>
    public virtual bool IsValid()
    {
        if (ID == null) return false;
        
        // 对于不同类型的ID有不同的验证逻辑
        if (ID is int intId)
        {
            return intId > 0;
        }
        if (ID is string stringId)
        {
            return !string.IsNullOrWhiteSpace(stringId);
        }
        
        return true; // 其他类型默认为有效
    }
}

/// <summary>
/// 配置数据基类（向后兼容）
/// </summary>
[Serializable]
public abstract class ConfigDataBase : ConfigDataBase<int>, IConfigData
{
} 