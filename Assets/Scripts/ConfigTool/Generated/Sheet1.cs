// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2025-07-26 15:30:35

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// PropertiesData 复杂数据类型
    /// </summary>
    [Serializable]
    public class PropertiesData
    {
        /// <summary>
        /// attack
        /// </summary>
        [JsonProperty("attack")]
        public int attack { get; set; }

        /// <summary>
        /// durability
        /// </summary>
        [JsonProperty("durability")]
        public int durability { get; set; }

    }

    /// <summary>
    /// infoInfoData 复杂数据类型
    /// </summary>
    [Serializable]
    public class infoInfoData
    {
        /// <summary>
        /// age
        /// </summary>
        [JsonProperty("age")]
        public int age { get; set; }

        /// <summary>
        /// gender
        /// </summary>
        [JsonProperty("gender")]
        public int gender { get; set; }

        /// <summary>
        /// skills
        /// </summary>
        [JsonProperty("skills")]
        public string[] skills { get; set; }

    }

    /// <summary>
    /// HumanData 复杂数据类型
    /// </summary>
    [Serializable]
    public class HumanData
    {
        /// <summary>
        /// name
        /// </summary>
        [JsonProperty("name")]
        public string name { get; set; }

        /// <summary>
        /// info
        /// </summary>
        [JsonProperty("info")]
        public infoInfoData info { get; set; }

    }

    /// <summary>
    /// Sheet1 配置数据类
    /// </summary>
    [Serializable]
    public class Sheet1 : ConfigDataBase
    {
        /// <summary>
        /// 物品名称
        /// </summary>
        [JsonProperty("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 物品价格
        /// </summary>
        [JsonProperty("Price")]
        public int Price { get; set; }

        /// <summary>
        /// 标签列表
        /// </summary>
        [JsonProperty("Tags")]
        public string[] Tags { get; set; }

        /// <summary>
        /// 额外属性
        /// </summary>
        [JsonProperty("Properties")]
        public PropertiesData Properties { get; set; }

        /// <summary>
        /// Human
        /// </summary>
        [JsonProperty("Human")]
        public infoInfoData Human { get; set; }

        /// <summary>
        /// IntArray
        /// </summary>
        [JsonProperty("IntArray")]
        public int[] IntArray { get; set; }

        /// <summary>
        /// floatArray
        /// </summary>
        [JsonProperty("floatArray")]
        public float[] floatArray { get; set; }

        /// <summary>
        /// 物品ID
        /// </summary>
        [JsonProperty("ID")]
        public int IDValue { get; set; }

        /// <summary>
        /// 配置项唯一ID
        /// </summary>
        public override int ID => IDValue;

        /// <summary>
        /// Sheet1 构造函数
        /// </summary>
        public Sheet1()
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

            if (string.IsNullOrWhiteSpace(Name))
            {
                Debug.LogWarning("配置字段 Name 不能为空");
                return false;
            }

            if (Tags == null)
            {
                Debug.LogWarning("配置数组字段 Tags 不能为null");
                return false;
            }

            if (IntArray == null)
            {
                Debug.LogWarning("配置数组字段 IntArray 不能为null");
                return false;
            }

            if (floatArray == null)
            {
                Debug.LogWarning("配置数组字段 floatArray 不能为null");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 返回配置信息的字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"Sheet1[ID={ID}, Name={Name}, Price={Price}]";
        }
    }

    /// <summary>
    /// Sheet1 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class Sheet1Manager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<int, Sheet1> _configs = new Dictionary<int, Sheet1>
        {
            { 1001, new Sheet1
            {
                IDValue = 1001,
                Name = "火焰剑",
                Price = 500,
                Tags = new string[] { "测试", "打开状态" },
                Properties = new PropertiesData(),
                Human = new infoInfoData(),
                IntArray = new int[] { 1, 2, 3 },
                floatArray = new float[] { 1.5f, 2.2f },
            }},
            { 1002, new Sheet1
            {
                IDValue = 1002,
                Name = "治疗药水",
                Price = 50,
                Tags = new string[] { "消耗品", "治疗", "上下两种都可以", "但是建议这种" },
                Properties = new PropertiesData(),
                Human = new infoInfoData(),
                IntArray = new int[] { 1, 2, 3 },
                floatArray = new float[] { 1.5f, 2.2f },
            }},
            { 1003, new Sheet1
            {
                IDValue = 1003,
                Name = "防护盾",
                Price = 300,
                Tags = new string[] { "防具", "123123" },
                Properties = new PropertiesData(),
                Human = new infoInfoData(),
                IntArray = new int[] { 1, 2, 3 },
                floatArray = new float[] { 1.5f, 2.2f },
            }},
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static Sheet1 GetConfig<T>(T id)
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
        public static Sheet1 GetConfig(int id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static Sheet1 GetConfig(string id)
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
        public static Dictionary<int, Sheet1> GetAllConfigs()
        {
            return new Dictionary<int, Sheet1>(_configs);
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
