// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2025-08-18 22:53:51

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// CommonConfig 配置数据类
    /// </summary>
    [Serializable]
    public class CommonConfig : ConfigDataBase<string>
    {
        /// <summary>
        /// 值
        /// </summary>
        [JsonProperty("Value")]
        public float Value { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [JsonProperty("Desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 杂项id
        /// </summary>
        [JsonProperty("ID")]
        public string IDValue { get; set; }

        /// <summary>
        /// 配置项唯一ID
        /// </summary>
        public override string ID => IDValue;

        /// <summary>
        /// CommonConfig 构造函数
        /// </summary>
        public CommonConfig()
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
            return $"CommonConfig[ID={ID}, Value={Value}, Desc={Desc}]";
        }
    }

    /// <summary>
    /// CommonConfig 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class CommonConfigManager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<string, CommonConfig> _configs = new Dictionary<string, CommonConfig>
        {
            { "jetpack_max", new CommonConfig
            {
                IDValue = "jetpack_max",
                Value = 1000f,
                Desc = "喷气背包初始容量：单位喷气量",
            }},
            { "jetpack_consume", new CommonConfig
            {
                IDValue = "jetpack_consume",
                Value = 0f,
                Desc = "喷气消耗速度：单位喷气量每秒",
            }},
            { "jetpack_recover", new CommonConfig
            {
                IDValue = "jetpack_recover",
                Value = 200f,
                Desc = "喷气恢复速度：单位喷气量每秒",
            }},
            { "jetpack_recover_cd", new CommonConfig
            {
                IDValue = "jetpack_recover_cd",
                Value = 0.2f,
                Desc = "停止喷气多久后开始恢复气体，单位：秒",
            }},
            { "power", new CommonConfig
            {
                IDValue = "power",
                Value = 1500f,
                Desc = "初始力量，以单位重力为单位",
            }},
            { "character_die_time", new CommonConfig
            {
                IDValue = "character_die_time",
                Value = 3f,
                Desc = "角色离开屏幕多久后死亡，单位：秒",
            }},
            { "item_fall_time", new CommonConfig
            {
                IDValue = "item_fall_time",
                Value = 0.3f,
                Desc = "商品掉落倒计时：商品触碰地面后多久玩家失去控制权，单位：秒",
            }},
            { "prepare_time", new CommonConfig
            {
                IDValue = "prepare_time",
                Value = 10f,
                Desc = "准备阶段时间",
            }},
            { "initial_coin", new CommonConfig
            {
                IDValue = "initial_coin",
                Value = 100f,
                Desc = "初始金币",
            }},
            { "import_fist_time", new CommonConfig
            {
                IDValue = "import_fist_time",
                Value = 0f,
                Desc = "进货区：第一辆车进场的时间，单位：秒",
            }},
            { "import_duration_time", new CommonConfig
            {
                IDValue = "import_duration_time",
                Value = 10f,
                Desc = "进货区：车辆停下来的时间，停稳后开始记，单位：秒",
            }},
            { "import_cd", new CommonConfig
            {
                IDValue = "import_cd",
                Value = 5f,
                Desc = "进货区：车辆离开屏幕后下一辆车多久刷出来，单位：秒",
            }},
            { "export_fist_time", new CommonConfig
            {
                IDValue = "export_fist_time",
                Value = 10f,
                Desc = "出货区：第一辆车进场的时间，单位：秒",
            }},
            { "export_duration_time", new CommonConfig
            {
                IDValue = "export_duration_time",
                Value = 10f,
                Desc = "出货区：车辆停下来的时间，停稳后开始记，单位：秒",
            }},
            { "export_cd", new CommonConfig
            {
                IDValue = "export_cd",
                Value = 4f,
                Desc = "出货区：车辆离开屏幕后下一辆车多久刷出来，单位：秒",
            }},
            { "item_disappear_time", new CommonConfig
            {
                IDValue = "item_disappear_time",
                Value = 0.3f,
                Desc = "物品出货失败判定时间，单位：秒",
            }},
            { "game_time", new CommonConfig
            {
                IDValue = "game_time",
                Value = 1800f,
                Desc = "一局游戏的时间",
            }},
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static CommonConfig GetConfig<T>(T id)
        {
            // 尝试将泛型类型转换为字典键类型
            if (id is string typedId)
            {
                return _configs.TryGetValue(typedId, out var config) ? config : null;
            }
            return null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据
        /// </summary>
        public static CommonConfig GetConfig(string id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据int类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static CommonConfig GetConfig(int id)
        {
            return GetConfig(id.ToString());
        }

        /// <summary>
        /// 获取所有配置数据
        /// </summary>
        /// <returns>所有配置数据的字典</returns>
        public static Dictionary<string, CommonConfig> GetAllConfigs()
        {
            return new Dictionary<string, CommonConfig>(_configs);
        }

        /// <summary>
        /// 检查指定ID的配置是否存在
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>如果存在返回true</returns>
        public static bool HasConfig(string id)
        {
            return _configs.ContainsKey(id);
        }

        /// <summary>
        /// 检查指定ID的配置是否存在（泛型版本）
        /// </summary>
        public static bool HasConfig<T>(T id)
        {
            if (id is string typedId)
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
