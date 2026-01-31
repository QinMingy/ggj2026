// 此文件由ConfigTableExporter自动生成，请勿手动修改！
// 生成时间：2026-01-31 12:18:32

using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ConfigData
{
    /// <summary>
    /// misc 配置数据类
    /// </summary>
    [Serializable]
    public class misc : ConfigDataBase
    {
        /// <summary>
        /// key
        /// </summary>
        [JsonProperty("key")]
        public int key { get; set; }

        /// <summary>
        /// 物品价格
        /// </summary>
        [JsonProperty("Price")]
        public int Price { get; set; }

        /// <summary>
        /// 配置项唯一ID（默认实现）
        /// 警告：Excel表格中没有找到ID字段，请添加ID列
        /// </summary>
        public override int ID => 0; // 请确保Excel表格中有ID字段

        /// <summary>
        /// misc 构造函数
        /// </summary>
        public misc()
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
            return $"misc[ID={ID}, key={key}, Price={Price}]";
        }
    }

    /// <summary>
    /// misc 配置数据管理器
    /// 包含所有配置数据的静态访问类
    /// </summary>
    public static class miscManager
    {
        /// <summary>
        /// 所有配置数据的静态字典
        /// </summary>
        private static readonly Dictionary<int, misc> _configs = new Dictionary<int, misc>
        {
        };

        /// <summary>
        /// 根据ID获取配置数据
        /// </summary>
        /// <param name="id">配置ID</param>
        /// <returns>配置数据，如果不存在返回null</returns>
        public static misc GetConfig<T>(T id)
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
        public static misc GetConfig(int id)
        {
            return _configs.TryGetValue(id, out var config) ? config : null;
        }

        /// <summary>
        /// 根据string类型ID获取配置数据（兼容性方法）
        /// </summary>
        public static misc GetConfig(string id)
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
        public static Dictionary<int, misc> GetAllConfigs()
        {
            return new Dictionary<int, misc>(_configs);
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
