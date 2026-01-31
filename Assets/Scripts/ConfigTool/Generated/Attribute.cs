// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2026-01-31 21:12:08

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// Attribute 配置数据类
    /// </summary>
    [Serializable]
    public class Attribute : ConfigDataBase
    {
        /// <summary>
        /// 名字
        /// </summary>
        [JsonProperty("name")]
        public string name { get; set; }

        /// <summary>
        /// 克制
        /// </summary>
        [JsonProperty("restrain")]
        public int restrain { get; set; }

        /// <summary>
        /// 被克制
        /// </summary>
        [JsonProperty("restrained")]
        public int restrained { get; set; }

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
        /// Attribute 构造函数
        /// </summary>
        public Attribute()
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
            return $"Attribute[ID={ID}, name={name}, restrain={restrain}]";
        }
    }

    /// <summary>
    /// Attribute 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class AttributeManager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<int, Attribute> _configs = new Dictionary<int, Attribute>
        {
            { 1, new Attribute
            {
                idValue = 1,
                name = "金",
                restrain = 2,
                restrained = 5,
            }},
            { 2, new Attribute
            {
                idValue = 2,
                name = "木",
                restrain = 3,
                restrained = 1,
            }},
            { 3, new Attribute
            {
                idValue = 3,
                name = "水",
                restrain = 4,
                restrained = 2,
            }},
            { 4, new Attribute
            {
                idValue = 4,
                name = "火",
                restrain = 5,
                restrained = 3,
            }},
            { 5, new Attribute
            {
                idValue = 5,
                name = "土",
                restrain = 1,
                restrained = 4,
            }},
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static Attribute GetConfig<T>(T id)
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
        public static Attribute GetConfig(int id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static Attribute GetConfig(string id)
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
        public static Dictionary<int, Attribute> GetAllConfigs()
        {
            return new Dictionary<int, Attribute>(_configs);
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
