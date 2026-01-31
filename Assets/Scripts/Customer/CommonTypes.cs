// CommonTypes.cs - 共用类型定义
using System;
using UnityEngine;

namespace MaskMerchantGame.CustomerSystem
{
    /// <summary>
    /// 顾客目的类型枚举
    /// </summary>
    public enum CustomerPurposeType
    {
        None = 0,
        Combat = 6,         // 战斗
        Theft = 7,          // 行窃
        Trade = 8,          // 行商
        Assassination = 9,  // 暗杀
        Escape = 10         // 逃亡
    }

    /// <summary>
    /// 五行元素枚举
    /// </summary>
    public enum FiveElements
    {
        None = 0,
        Metal = 1,  // 金
        Wood = 2,   // 木
        Water = 3,  // 水
        Fire = 4,   // 火
        Earth = 5   // 土
    }

    /// <summary>
    /// 五行属性数值
    /// </summary>
    [Serializable]
    public class FiveElementsAttributes
    {
        public int metal;   // 金
        public int wood;    // 木
        public int water;   // 水
        public int fire;    // 火
        public int earth;   // 土

        public FiveElementsAttributes()
        {
            metal = 0;
            wood = 0;
            water = 0;
            fire = 0;
            earth = 0;
        }

        public FiveElementsAttributes(int metal, int wood, int water, int fire, int earth)
        {
            this.metal = metal;
            this.wood = wood;
            this.water = water;
            this.fire = fire;
            this.earth = earth;
        }

        /// <summary>
        /// 克隆属性
        /// </summary>
        public FiveElementsAttributes Clone()
        {
            return new FiveElementsAttributes(metal, wood, water, fire, earth);
        }

        /// <summary>
        /// 叠加属性
        /// </summary>
        public void Add(FiveElementsAttributes other)
        {
            if (other == null) return;
            metal += other.metal;
            wood += other.wood;
            water += other.water;
            fire += other.fire;
            earth += other.earth;
        }

        /// <summary>
        /// 获取指定元素的属性值
        /// </summary>
        public int GetValue(FiveElements element)
        {
            switch (element)
            {
                case FiveElements.Metal: return metal;
                case FiveElements.Wood: return wood;
                case FiveElements.Water: return water;
                case FiveElements.Fire: return fire;
                case FiveElements.Earth: return earth;
                default: return 0;
            }
        }

        /// <summary>
        /// 设置指定元素的属性值
        /// </summary>
        public void SetValue(FiveElements element, int value)
        {
            switch (element)
            {
                case FiveElements.Metal: metal = value; break;
                case FiveElements.Wood: wood = value; break;
                case FiveElements.Water: water = value; break;
                case FiveElements.Fire: fire = value; break;
                case FiveElements.Earth: earth = value; break;
            }
        }
    }

    /// <summary>
    /// 对话历史条目
    /// </summary>
    [Serializable]
    public class DialogueHistoryEntry
    {
        public bool isMerchant;         // 是否是商人说的话
        public string speakerName;      // 说话者名字
        public string content;          // 对话内容
        public int optionIndex;         // 对应的选项索引（-1表示非选项触发）
        public DateTime timestamp;      // 时间戳

        public DialogueHistoryEntry(bool isMerchant, string speakerName, string content, int optionIndex)
        {
            this.isMerchant = isMerchant;
            this.speakerName = speakerName;
            this.content = content;
            this.optionIndex = optionIndex;
            this.timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{speakerName}: {content}";
        }
    }

    /// <summary>
    /// 收集到的线索
    /// </summary>
    [Serializable]
    public class CollectedClue
    {
        public int sourceOptionIndex;   // 来源选项索引
        public string clueContent;      // 线索内容
        public DateTime collectedTime;  // 收集时间

        public CollectedClue(int sourceOptionIndex, string clueContent)
        {
            this.sourceOptionIndex = sourceOptionIndex;
            this.clueContent = clueContent;
            this.collectedTime = DateTime.Now;
        }
    }

    /// <summary>
    /// 顾客携带物品
    /// </summary>
    [Serializable]
    public class CarriedItem
    {
        public int itemId;              // 物品ID
        public string itemName;         // 物品名称
        public int quantity;            // 数量
        public int value;               // 价值

        public CarriedItem()
        {
            itemId = 0;
            itemName = string.Empty;
            quantity = 1;
            value = 0;
        }

        public CarriedItem(int itemId, string itemName, int quantity, int value)
        {
            this.itemId = itemId;
            this.itemName = itemName;
            this.quantity = quantity;
            this.value = value;
        }
    }
}
