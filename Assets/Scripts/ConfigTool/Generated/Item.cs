// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2026-01-31 21:12:08

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
        /// 属性
        /// </summary>
        [JsonProperty("attribute")]
        public int[] attribute { get; set; }

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

            if (attribute == null)
            {
                Debug.LogWarning("配置数组字段 attribute 不能为null");
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
                name = "玄铁屑",
                desc = "玄铁炼熔后的细屑，凝天地金气，为面具锻铸灵骨，坚而不脆",
                price = 100,
                attribute = new int[] { 0, 0, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 10002, new Item
            {
                idValue = 10002,
                name = "铜铃片",
                desc = "古寺铜铃磨下的薄片，藏清越金声，能为面具聚魂，驱散阴翳",
                price = 150,
                attribute = new int[] { 0, 0, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 10003, new Item
            {
                idValue = 10003,
                name = "紫金砂",
                desc = "紫金矿脉中结出的细砂，金气醇厚，铸入面具可令灵核凝实，百邪不侵",
                price = 200,
                attribute = new int[] { 0, 0, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 10004, new Item
            {
                idValue = 10004,
                name = "金纹片",
                desc = "古器上剥落的鎏金纹路，含岁月金韵，为面具饰骨，增其镇煞之力",
                price = 250,
                attribute = new int[] { 0, 0, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 20001, new Item
            {
                idValue = 20001,
                name = "青竹篾",
                desc = "春山嫩竹剖成的细篾，蕴草木生机，为面具塑肌理，柔而有韧",
                price = 100,
                attribute = new int[] { 0, 1, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 20002, new Item
            {
                idValue = 20002,
                name = "柏叶芯",
                desc = "古柏新抽的叶芯，藏山林清气，温养面具灵脉，令其气息绵长",
                price = 150,
                attribute = new int[] { 0, 1, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 20003, new Item
            {
                idValue = 20003,
                name = "灵桃木枝",
                desc = "东方灵桃枝截成的细段，木气精纯，铸入面具可聚天地生气，滋养灵韵",
                price = 200,
                attribute = new int[] { 0, 1, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 20004, new Item
            {
                idValue = 20004,
                name = "沉香木粉",
                desc = "沉香木磨成的细粉，凝草木精华，为面具熏脉，令其灵息醇厚不散",
                price = 250,
                attribute = new int[] { 0, 1, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 30001, new Item
            {
                idValue = 30001,
                name = "清露珠",
                desc = "晨朝荷叶上的清露凝珠，蕴天地水气，涤面具尘秽，润其灵韵",
                price = 100,
                attribute = new int[] { 0, 0, 1, 0, 0 },
                icon = "Icon_1",
            }},
            { 30002, new Item
            {
                idValue = 30002,
                name = "溪泉石髓",
                desc = "溪泉底石中渗出的清髓，藏柔润水意，为面具凝魂，令其灵息澄澈",
                price = 150,
                attribute = new int[] { 0, 0, 1, 0, 0 },
                icon = "Icon_1",
            }},
            { 30003, new Item
            {
                idValue = 30003,
                name = "瑶池甘露",
                desc = "仙池所凝的甘露，水气清冽醇厚，涤面具浊质，蕴养灵核，令其清透无垢",
                price = 200,
                attribute = new int[] { 0, 0, 1, 0, 0 },
                icon = "Icon_1",
            }},
            { 30004, new Item
            {
                idValue = 30004,
                name = "玄冰玉屑",
                desc = "极北玄冰玉磨成的细屑，水气化冰，凝而不散，铸入面具可令灵韵坚凝",
                price = 250,
                attribute = new int[] { 0, 0, 1, 0, 0 },
                icon = "Icon_1",
            }},
            { 40001, new Item
            {
                idValue = 40001,
                name = "丹枫薪",
                desc = "秋山丹枫枝干制成的薪柴，燃之有温火，炼面具灵核，驱散阴寒",
                price = 100,
                attribute = new int[] { 0, 0, 0, 1, 0 },
                icon = "Icon_1",
            }},
            { 40002, new Item
            {
                idValue = 40002,
                name = "艾草绒",
                desc = "陈年艾草捣成的绒絮，燃之出纯阳火气，为面具驱秽，凝其灵焰",
                price = 150,
                attribute = new int[] { 0, 0, 0, 1, 0 },
                icon = "Icon_1",
            }},
            { 40003, new Item
            {
                idValue = 40003,
                name = "凤凰羽灰",
                desc = "凤凰羽翼燃后所凝的轻灰，含纯阳真火，炼面具灵骨，焚尽一切阴邪",
                price = 200,
                attribute = new int[] { 0, 0, 0, 1, 0 },
                icon = "Icon_1",
            }},
            { 40004, new Item
            {
                idValue = 40004,
                name = "朱砂焰屑",
                desc = "朱砂混真火炼出的焰屑，火气相融，铸入面具可令灵焰炽烈，镇煞破障",
                price = 250,
                attribute = new int[] { 0, 0, 0, 1, 0 },
                icon = "Icon_1",
            }},
            { 50001, new Item
            {
                idValue = 50001,
                name = "黄黏土",
                desc = "中原厚土所出的黄黏土，蕴坤地之气，为面具筑基，固其灵根",
                price = 100,
                attribute = new int[] { 0, 0, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 50002, new Item
            {
                idValue = 50002,
                name = "麦饭石粉",
                desc = "古山麦饭石磨成的细粉，藏土之精，承托面具灵韵，令其根基稳固",
                price = 150,
                attribute = new int[] { 0, 0, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 50003, new Item
            {
                idValue = 50003,
                name = "昆仑玉土",
                desc = "昆仑山下结出的玉质净土，土气醇厚凝实，为面具铸基，固魂守灵，百扰不侵",
                price = 200,
                attribute = new int[] { 0, 0, 0, 0, 0 },
                icon = "Icon_1",
            }},
            { 50004, new Item
            {
                idValue = 50004,
                name = "息壤屑",
                desc = "上古息壤所凝的细屑，藏生生不息的土气，承托面具灵核，令其灵韵绵长不散",
                price = 250,
                attribute = new int[] { 0, 0, 0, 0, 0 },
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
