// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2026-01-31 12:18:32

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// item 配置数据类
    /// </summary>
    [Serializable]
    public class item : ConfigDataBase
    {
        /// <summary>
        /// 解锁关卡
        /// </summary>
        [JsonProperty("level")]
        public int level { get; set; }

        /// <summary>
        /// 对应预制体
        /// </summary>
        [JsonProperty("prefab")]
        public string prefab { get; set; }

        /// <summary>
        /// 重量
        /// </summary>
        [JsonProperty("weight")]
        public int weight { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        [JsonProperty("probability")]
        public int probability { get; set; }

        /// <summary>
        /// 进货价格
        /// </summary>
        [JsonProperty("import_price")]
        public int[] import_price { get; set; }

        /// <summary>
        /// 出售价格
        /// </summary>
        [JsonProperty("export_price")]
        public int[] export_price { get; set; }

        /// <summary>
        /// 标签（物品类型）
        /// </summary>
        [JsonProperty("tag")]
        public string[] tag { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [JsonProperty("icon")]
        public string icon { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [JsonProperty("remark")]
        public string remark { get; set; }

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
        /// item 构造函数
        /// </summary>
        public item()
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

            if (import_price == null)
            {
                Debug.LogWarning("配置数组字段 import_price 不能为null");
                return false;
            }

            if (export_price == null)
            {
                Debug.LogWarning("配置数组字段 export_price 不能为null");
                return false;
            }

            if (tag == null)
            {
                Debug.LogWarning("配置数组字段 tag 不能为null");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 返回配置信息的字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"item[ID={ID}, level={level}, prefab={prefab}]";
        }
    }

    /// <summary>
    /// item 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class itemManager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<int, item> _configs = new Dictionary<int, item>
        {
            { 10001, new item
            {
                idValue = 10001,
                level = 1001,
                prefab = "item10001",
                weight = 150,
                probability = 10,
                import_price = new int[] { 80, 100 },
                export_price = new int[] { 125, 160 },
                tag = new string[] { "\"furniture\"" },
                icon = "bingxiang.png",
                remark = "冰箱",
            }},
            { 10002, new item
            {
                idValue = 10002,
                level = 1001,
                prefab = "item10002",
                weight = 150,
                probability = 10,
                import_price = new int[] { 80, 100 },
                export_price = new int[] { 125, 160 },
                tag = new string[] { "\"furniture\"" },
                icon = "chuang.png",
                remark = "床",
            }},
            { 10003, new item
            {
                idValue = 10003,
                level = 1001,
                prefab = "item10003",
                weight = 50,
                probability = 10,
                import_price = new int[] { 20, 25 },
                export_price = new int[] { 30, 40 },
                tag = new string[] { "\"furniture\"" },
                icon = "dianfanbao.png",
                remark = "电饭煲",
            }},
            { 10004, new item
            {
                idValue = 10004,
                level = 1001,
                prefab = "item10004",
                weight = 25,
                probability = 10,
                import_price = new int[] { 20, 25 },
                export_price = new int[] { 30, 40 },
                tag = new string[] { "\"furniture\"" },
                icon = "dianfengshan.png",
                remark = "电风扇",
            }},
            { 10005, new item
            {
                idValue = 10005,
                level = 1001,
                prefab = "item10005",
                weight = 150,
                probability = 10,
                import_price = new int[] { 90, 120 },
                export_price = new int[] { 200, 250 },
                tag = new string[] { "\"furniture\"" },
                icon = "gangqing.png",
                remark = "钢琴",
            }},
            { 10006, new item
            {
                idValue = 10006,
                level = 1001,
                prefab = "item10006",
                weight = 25,
                probability = 10,
                import_price = new int[] { 20, 25 },
                export_price = new int[] { 30, 40 },
                tag = new string[] { "\"furniture\"" },
                icon = "huaping.png",
                remark = "花瓶",
            }},
            { 10007, new item
            {
                idValue = 10007,
                level = 1001,
                prefab = "item10007",
                weight = 35,
                probability = 10,
                import_price = new int[] { 40, 55 },
                export_price = new int[] { 80, 100 },
                tag = new string[] { "\"furniture\"" },
                icon = "jita.png",
                remark = "吉他",
            }},
            { 10008, new item
            {
                idValue = 10008,
                level = 1001,
                prefab = "item10008",
                weight = 30,
                probability = 10,
                import_price = new int[] { 20, 25 },
                export_price = new int[] { 30, 40 },
                tag = new string[] { "\"furniture\"" },
                icon = "lanqiu.png",
                remark = "篮球",
            }},
            { 10009, new item
            {
                idValue = 10009,
                level = 1001,
                prefab = "item10009",
                weight = 15,
                probability = 10,
                import_price = new int[] { 5, 10 },
                export_price = new int[] { 20, 30 },
                tag = new string[] { "\"furniture\"" },
                icon = "penzai.png",
                remark = "盆栽",
            }},
            { 10010, new item
            {
                idValue = 10010,
                level = 1001,
                prefab = "item10010",
                weight = 150,
                probability = 10,
                import_price = new int[] { 50, 70 },
                export_price = new int[] { 95, 130 },
                tag = new string[] { "\"furniture\"" },
                icon = "shafa.png",
                remark = "沙发",
            }},
            { 10011, new item
            {
                idValue = 10011,
                level = 1001,
                prefab = "item10011",
                weight = 40,
                probability = 10,
                import_price = new int[] { 20, 25 },
                export_price = new int[] { 30, 40 },
                tag = new string[] { "\"furniture\"" },
                icon = "shuitong.png",
                remark = "水桶",
            }},
            { 10012, new item
            {
                idValue = 10012,
                level = 1001,
                prefab = "item10012",
                weight = 15,
                probability = 10,
                import_price = new int[] { 20, 25 },
                export_price = new int[] { 30, 40 },
                tag = new string[] { "\"furniture\"" },
                icon = "taideng.png",
                remark = "台灯",
            }},
            { 10013, new item
            {
                idValue = 10013,
                level = 1001,
                prefab = "item10013",
                weight = 80,
                probability = 10,
                import_price = new int[] { 50, 70 },
                export_price = new int[] { 95, 130 },
                tag = new string[] { "\"furniture\"" },
                icon = "TV.png",
                remark = "电视",
            }},
            { 10014, new item
            {
                idValue = 10014,
                level = 1001,
                prefab = "item10014",
                weight = 50,
                probability = 10,
                import_price = new int[] { 20, 25 },
                export_price = new int[] { 30, 40 },
                tag = new string[] { "\"furniture\"" },
                icon = "weibolu.png",
                remark = "微波炉",
            }},
            { 10015, new item
            {
                idValue = 10015,
                level = 1001,
                prefab = "item10015",
                weight = 60,
                probability = 10,
                import_price = new int[] { 20, 25 },
                export_price = new int[] { 30, 40 },
                tag = new string[] { "\"furniture\"" },
                icon = "xinglixiang.png",
                remark = "行李箱",
            }},
            { 10016, new item
            {
                idValue = 10016,
                level = 1001,
                prefab = "item10016",
                weight = 150,
                probability = 10,
                import_price = new int[] { 80, 100 },
                export_price = new int[] { 125, 160 },
                tag = new string[] { "\"furniture\"" },
                icon = "yigui.png",
                remark = "衣柜",
            }},
            { 10017, new item
            {
                idValue = 10017,
                level = 1001,
                prefab = "item10017",
                weight = 60,
                probability = 10,
                import_price = new int[] { 20, 25 },
                export_price = new int[] { 30, 40 },
                tag = new string[] { "\"furniture\"" },
                icon = "zhixiang.png",
                remark = "纸箱",
            }},
            { 10018, new item
            {
                idValue = 10018,
                level = 1001,
                prefab = "item10018",
                weight = 30,
                probability = 10,
                import_price = new int[] { 20, 25 },
                export_price = new int[] { 30, 40 },
                tag = new string[] { "\"furniture\"" },
                icon = "zuqiu.png",
                remark = "足球",
            }},
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static item GetConfig<T>(T id)
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
        public static item GetConfig(int id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static item GetConfig(string id)
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
        public static Dictionary<int, item> GetAllConfigs()
        {
            return new Dictionary<int, item>(_configs);
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
