// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2026-01-31 14:57:12

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// dayLevel 配置数据类
    /// </summary>
    [Serializable]
    public class dayLevel : ConfigDataBase
    {
        /// <summary>
        /// 产生顾客数
        /// </summary>
        [JsonProperty("customerCount")]
        public float customerCount { get; set; }

        /// <summary>
        /// 目标钱数
        /// </summary>
        [JsonProperty("targetMoney")]
        public string targetMoney { get; set; }

        /// <summary>
        /// 杂项id
        /// </summary>
        [JsonProperty("ID")]
        public int IDValue { get; set; }

        /// <summary>
        /// 配置项唯一ID
        /// </summary>
        public override int ID => IDValue;

        /// <summary>
        /// dayLevel 构造函数
        /// </summary>
        public dayLevel()
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
            return $"dayLevel[ID={ID}, customerCount={customerCount}, targetMoney={targetMoney}]";
        }
    }

    /// <summary>
    /// dayLevel 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class dayLevelManager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<int, dayLevel> _configs = new Dictionary<int, dayLevel>
        {
            { 1, new dayLevel
            {
                IDValue = 1,
                customerCount = 5f,
                targetMoney = "100",
            }},
            { 2, new dayLevel
            {
                IDValue = 2,
                customerCount = 5f,
                targetMoney = "200",
            }},
            { 3, new dayLevel
            {
                IDValue = 3,
                customerCount = 5f,
                targetMoney = "300",
            }},
            { 4, new dayLevel
            {
                IDValue = 4,
                customerCount = 6f,
                targetMoney = "400",
            }},
            { 5, new dayLevel
            {
                IDValue = 5,
                customerCount = 6f,
                targetMoney = "500",
            }},
            { 6, new dayLevel
            {
                IDValue = 6,
                customerCount = 6f,
                targetMoney = "600",
            }},
            { 7, new dayLevel
            {
                IDValue = 7,
                customerCount = 8f,
                targetMoney = "700",
            }},
            { 8, new dayLevel
            {
                IDValue = 8,
                customerCount = 8f,
                targetMoney = "800",
            }},
            { 9, new dayLevel
            {
                IDValue = 9,
                customerCount = 8f,
                targetMoney = "900",
            }},
            { 10, new dayLevel
            {
                IDValue = 10,
                customerCount = 10f,
                targetMoney = "1000",
            }},
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static dayLevel GetConfig<T>(T id)
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
        public static dayLevel GetConfig(int id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static dayLevel GetConfig(string id)
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
        public static Dictionary<int, dayLevel> GetAllConfigs()
        {
            return new Dictionary<int, dayLevel>(_configs);
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
