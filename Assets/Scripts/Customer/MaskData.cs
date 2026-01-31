// MaskData.cs - 面具数据类
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaskMerchantGame.CustomerSystem
{
    /// <summary>
    /// 面具配置数据
    /// </summary>
    [Serializable]
    public class MaskConfig
    {
        public int maskId;                          // 面具ID
        public string maskName;                     // 面具名称
        public string description;                  // 面具描述
        public string iconPath;                     // 图标路径
        
        [Header("属性加成")]
        public FiveElementsAttributes attributeBonus;  // 五行属性加成（可叠加到检定属性）
        
        [Header("特殊效果")]
        public string[] specialEffects;             // 特殊效果描述
    }

    /// <summary>
    /// 面具接口 - 用于获取面具属性并应用到顾客
    /// </summary>
    public interface IMaskProvider
    {
        /// <summary>
        /// 获取面具配置
        /// </summary>
        MaskConfig GetMaskConfig(int maskId);

        /// <summary>
        /// 获取所有可用面具
        /// </summary>
        List<MaskConfig> GetAllMasks();
    }

    /// <summary>
    /// 面具数据库 - ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "MaskDatabase", menuName = "MaskMerchant/Mask Database")]
    public class MaskDatabase : ScriptableObject, IMaskProvider
    {
        [SerializeField] private List<MaskConfig> masks = new List<MaskConfig>();

        public MaskConfig GetMaskConfig(int maskId)
        {
            foreach (var mask in masks)
            {
                if (mask.maskId == maskId)
                    return mask;
            }
            return null;
        }

        public List<MaskConfig> GetAllMasks()
        {
            return new List<MaskConfig>(masks);
        }

#if UNITY_EDITOR
        public void AddMask(MaskConfig mask)
        {
            masks.Add(mask);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    /// <summary>
    /// 面具示例数据
    /// </summary>
    public static class MaskExamples
    {
        /// <summary>
        /// 创建火焰面具示例
        /// </summary>
        public static MaskConfig CreateFireMask()
        {
            return new MaskConfig
            {
                maskId = 2001,
                maskName = "烈焰鬼面",
                description = "蕴含火焰之力的面具，能增强火属性。",
                iconPath = "Masks/FireMask",
                attributeBonus = new FiveElementsAttributes(0, -10, 0, 20, 0),
                specialEffects = new string[] { "火属性+20", "木属性-10" }
            };
        }

        /// <summary>
        /// 创建金属面具示例
        /// </summary>
        public static MaskConfig CreateMetalMask()
        {
            return new MaskConfig
            {
                maskId = 2002,
                maskName = "铁面判官",
                description = "坚硬如铁的面具，增强金属性。",
                iconPath = "Masks/MetalMask",
                attributeBonus = new FiveElementsAttributes(20, 0, 0, -10, 0),
                specialEffects = new string[] { "金属性+20", "火属性-10" }
            };
        }

        /// <summary>
        /// 创建水属性面具示例
        /// </summary>
        public static MaskConfig CreateWaterMask()
        {
            return new MaskConfig
            {
                maskId = 2003,
                maskName = "寒冰面具",
                description = "冰冷刺骨的面具，增强水属性。",
                iconPath = "Masks/WaterMask",
                attributeBonus = new FiveElementsAttributes(-10, 0, 20, 0, 0),
                specialEffects = new string[] { "水属性+20", "金属性-10" }
            };
        }
    }
}
