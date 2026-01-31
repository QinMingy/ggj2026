// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2026-01-31 17:01:16

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// Item 配置数据类
    /// </summary>
    [Serializable]
    public class Item : ConfigDataBase
    {
        /// <summary>
        /// 名字
        /// </summary>
        [JsonProperty("name")]
        public string name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty("desc")]
        public string desc { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [JsonProperty("price")]
        public int price { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [JsonProperty("icon")]
        public string icon { get; set; }

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
        /// Item 构造函数
        /// </summary>
        public Item()
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
            return $"Item[ID={ID}, name={name}, desc={desc}]";
        }
    }

    /// <summary>
    /// Item 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class ItemManager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<int, Item> _configs = new Dictionary<int, Item>
        {
            { 10001, new Item
            {
                idValue = 10001,
                name = "朱砂",
                desc = "笔触刚猛，直线向上冲刺",
                price = 100,
                icon = "Icon_1",
            }},
            { 10002, new Item
            {
                idValue = 10002,
                name = "金粉",
                desc = "笔触锐利，向四周发散。",
                price = 100,
                icon = "Icon_1",
            }},
            { 10003, new Item
            {
                idValue = 10003,
                name = "墨汁",
                desc = "笔触柔顺，向下流淌，且带有惯性（类似惯性漂移）。",
                price = 150,
                icon = "Icon_1",
            }},
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static Item GetConfig<T>(T id)
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
        public static Item GetConfig(int id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static Item GetConfig(string id)
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
        public static Dictionary<int, Item> GetAllConfigs()
        {
            return new Dictionary<int, Item>(_configs);
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
