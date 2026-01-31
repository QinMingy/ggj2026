// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2026-01-31 21:00:49

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// CustomerDialogue 配置数据类
    /// </summary>
    [Serializable]
    public class CustomerDialogue : ConfigDataBase
    {
        /// <summary>
        /// 回答
        /// </summary>
        [JsonProperty("responseText")]
        public string responseText { get; set; }

        /// <summary>
        /// 线索简介
        /// </summary>
        [JsonProperty("clueContent")]
        public string clueContent { get; set; }

        /// <summary>
        /// 信任度影响
        /// </summary>
        [JsonProperty("dangerLevel")]
        public int dangerLevel { get; set; }

        /// <summary>
        /// 是否有效线索
        /// </summary>
        [JsonProperty("isValidClue")]
        public int isValidClue { get; set; }

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
        /// CustomerDialogue 构造函数
        /// </summary>
        public CustomerDialogue()
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
            return $"CustomerDialogue[ID={ID}, responseText={responseText}, clueContent={clueContent}]";
        }
    }

    /// <summary>
    /// CustomerDialogue 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class CustomerDialogueManager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<int, CustomerDialogue> _configs = new Dictionary<int, CustomerDialogue>
        {
            { 10001, new CustomerDialogue
            {
                idValue = 10001,
                responseText = "某家来自北方铁剑门，此番南下是为了寻找一件重要的东西。",
                clueContent = "来自铁剑门，与金属有关",
                dangerLevel = 0,
                isValidClue = 1,
            }},
            { 10002, new CustomerDialogue
            {
                idValue = 10002,
                responseText = "某家是铁剑门的护法，专司门派安全",
                clueContent = "",
                dangerLevel = 0,
                isValidClue = 0,
            }},
            { 10003, new CustomerDialogue
            {
                idValue = 10003,
                responseText = "某家需要一副能够压制内伤的面具，最近与人交手受了些伤。",
                clueContent = "有内伤，近期与人交手",
                dangerLevel = -30,
                isValidClue = 1,
            }},
            { 10004, new CustomerDialogue
            {
                idValue = 10004,
                responseText = "（武者身材魁梧，腰间佩戴铁剑，手腕上有金属护腕，皮肤泛着金属光泽）",
                clueContent = "皮肤泛金属光泽，修炼金属性功法",
                dangerLevel = -10,
                isValidClue = 1,
            }},
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static CustomerDialogue GetConfig<T>(T id)
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
        public static CustomerDialogue GetConfig(int id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static CustomerDialogue GetConfig(string id)
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
        public static Dictionary<int, CustomerDialogue> GetAllConfigs()
        {
            return new Dictionary<int, CustomerDialogue>(_configs);
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
