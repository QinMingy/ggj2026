// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2025-08-13 00:19:25

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// BeginItemGroup 配置数据类
    /// </summary>
    [Serializable]
    public class BeginItemGroup : ConfigDataBase
    {
        /// <summary>
        /// 关卡
        /// </summary>
        [JsonProperty("level")]
        public int level { get; set; }

        /// <summary>
        /// 对应预制体
        /// </summary>
        [JsonProperty("prefab")]
        public string prefab { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        [JsonProperty("weight")]
        public int weight { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [JsonProperty("misc")]
        public string misc { get; set; }

        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("ID")]
        public int IDValue { get; set; }

        /// <summary>
        /// 配置项唯一ID
        /// </summary>
        public override int ID => IDValue;

        /// <summary>
        /// BeginItemGroup 构造函数
        /// </summary>
        public BeginItemGroup()
        {
            // 可以在这里设置默认值
        }

        /// <summary>
        /// 验证配置数据的有效性
        /// </summary>
        /// <returns>如果数据有效返回true</returns>
        public override bool IsValid()
        {
            if (!base.IsValid())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 返回配置信息的字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"BeginItemGroup[ID={ID}, level={level}, prefab={prefab}]";
        }
    }

    /// <summary>
    /// BeginItemGroup 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class BeginItemGroupManager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<int, BeginItemGroup> _configs = new Dictionary<int, BeginItemGroup>
        {
            { 1001, new BeginItemGroup
            {
                IDValue = 1001,
                level = 10001,
                prefab = "initial_1001",
                weight = 10,
                misc = "冰箱、沙发、床",
            }},
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static BeginItemGroup GetConfig<T>(T id)
        {
            // 尝试将泛型类型转换为字典键类型
            if (id is int typedId)
            {
                return _configs.TryGetValue(typedId, out var config) ? config : null;
            }
            return null;
        }

        /// <summary>
        /// 根据int类型ID获取配置数据
        /// </summary>
        public static BeginItemGroup GetConfig(int id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static BeginItemGroup GetConfig(string id)
        {
            if (int.TryParse(id, out int intId))
            {
                return GetConfig(intId);
            }
            return null;
        }

        /// <summary>
        /// 获取所有配置数据
        /// </summary>
        /// <returns>所有配置数据的字典</returns>
        public static Dictionary<int, BeginItemGroup> GetAllConfigs()
        {
            return new Dictionary<int, BeginItemGroup>(_configs);
        }

        /// <summary>
        /// 检查指定ID的配置是否存在
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>如果存在返回true</returns>
        public static bool HasConfig(int id)
        {
            return _configs.ContainsKey(id);
        }

        /// <summary>
        /// 检查指定ID的配置是否存在（泛型版本）
        /// </summary>
        public static bool HasConfig<T>(T id)
        {
            if (id is int typedId)
            {
                return _configs.ContainsKey(typedId);
            }
            return false;
        }

        /// <summary>
        /// 获取配置数据总数
        /// </summary>
        /// <returns>配置数据总数</returns>
        public static int GetConfigCount()
        {
            return _configs.Count;
        }
    }

}
