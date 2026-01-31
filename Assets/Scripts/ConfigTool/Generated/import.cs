// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2026-01-31 12:18:32

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// import 配置数据类
    /// </summary>
    [Serializable]
    public class import : ConfigDataBase
    {
        /// <summary>
        /// 地图
        /// </summary>
        [JsonProperty("map")]
        public int map { get; set; }

        /// <summary>
        /// 解锁关卡
        /// </summary>
        [JsonProperty("unlock_level")]
        public int unlock_level { get; set; }

        /// <summary>
        /// 对应预制体
        /// </summary>
        [JsonProperty("prefab")]
        public string prefab { get; set; }

        /// <summary>
        /// id
        /// </summary>
        [JsonProperty("id")]
        public int idValue { get; set; }

        /// <summary>
        /// 配置项唯一ID
        /// </summary>
        public override int ID => idValue;

        /// <summary>
        /// import 构造函数
        /// </summary>
        public import()
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
            return $"import[ID={ID}, map={map}, unlock_level={unlock_level}]";
        }
    }

    /// <summary>
    /// import 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class importManager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<int, import> _configs = new Dictionary<int, import>
        {
            { 10001, new import
            {
                idValue = 10001,
                map = 1,
                unlock_level = 10001,
                prefab = "import10001",
            }},
            { 10002, new import
            {
                idValue = 10002,
                map = 1,
                unlock_level = 10001,
                prefab = "import10002",
            }},
            { 10003, new import
            {
                idValue = 10003,
                map = 1,
                unlock_level = 10001,
                prefab = "import10003",
            }},
            { 10004, new import
            {
                idValue = 10004,
                map = 1,
                unlock_level = 10001,
                prefab = "import10004",
            }},
            { 10005, new import
            {
                idValue = 10005,
                map = 1,
                unlock_level = 10001,
                prefab = "import10005",
            }},
            { 10006, new import
            {
                idValue = 10006,
                map = 1,
                unlock_level = 10001,
                prefab = "import10006",
            }},
            { 10007, new import
            {
                idValue = 10007,
                map = 1,
                unlock_level = 10001,
                prefab = "import10007",
            }},
            { 10008, new import
            {
                idValue = 10008,
                map = 1,
                unlock_level = 10001,
                prefab = "import10008",
            }},
            { 10009, new import
            {
                idValue = 10009,
                map = 1,
                unlock_level = 10001,
                prefab = "import10009",
            }},
            { 10010, new import
            {
                idValue = 10010,
                map = 1,
                unlock_level = 10001,
                prefab = "import10010",
            }},
            { 10011, new import
            {
                idValue = 10011,
                map = 1,
                unlock_level = 10001,
                prefab = "import10011",
            }},
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static import GetConfig<T>(T id)
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
        public static import GetConfig(int id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static import GetConfig(string id)
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
        public static Dictionary<int, import> GetAllConfigs()
        {
            return new Dictionary<int, import>(_configs);
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
