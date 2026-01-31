// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2026-01-31 21:00:49

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// Customer 配置数据类
    /// </summary>
    [Serializable]
    public class Customer : ConfigDataBase
    {
        /// <summary>
        /// 名字
        /// </summary>
        [JsonProperty("customerName")]
        public string customerName { get; set; }

        /// <summary>
        /// 职业
        /// </summary>
        [JsonProperty("customerType")]
        public string customerType { get; set; }

        /// <summary>
        /// 危险等级
        /// </summary>
        [JsonProperty("dangerLevel")]
        public int dangerLevel { get; set; }

        /// <summary>
        /// 弱点
        /// </summary>
        [JsonProperty("trueWeakness")]
        public int trueWeakness { get; set; }

        /// <summary>
        /// 自我介绍描述
        /// </summary>
        [JsonProperty("introDialogue")]
        public string introDialogue { get; set; }

        /// <summary>
        /// 立绘
        /// </summary>
        [JsonProperty("characterPrefabPath")]
        public string characterPrefabPath { get; set; }

        /// <summary>
        /// 对话回答
        /// </summary>
        [JsonProperty("dialogueOptions")]
        public int[] dialogueOptions { get; set; }

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
        /// Customer 构造函数
        /// </summary>
        public Customer()
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

            if (string.IsNullOrWhiteSpace(customerName))
            {
                Debug.LogWarning("配置字段 customerName 不能为空");
                return false;
            }

            if (dialogueOptions == null)
            {
                Debug.LogWarning("配置数组字段 dialogueOptions 不能为null");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 返回配置信息的字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"Customer[ID={ID}, customerName={customerName}, customerType={customerType}]";
        }
    }

    /// <summary>
    /// Customer 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class CustomerManager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<int, Customer> _configs = new Dictionary<int, Customer>
        {
            { 10001, new Customer
            {
                idValue = 10001,
                customerName = "李无常",
                customerType = "武者",
                dangerLevel = 3,
                trueWeakness = 4,
                introDialogue = "在下李无常，走南闯北多年，今日有缘来到贵店。听闻贵店有些特殊的面具，不知可否一观？",
                characterPrefabPath = "Characters/MysteriousMerchant",
                dialogueOptions = new int[] { 10001, 10002, 10003, 10004 },
            }},
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static Customer GetConfig<T>(T id)
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
        public static Customer GetConfig(int id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static Customer GetConfig(string id)
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
        public static Dictionary<int, Customer> GetAllConfigs()
        {
            return new Dictionary<int, Customer>(_configs);
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
