// EventSystem.cs - 事件系统
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaskMerchantGame.CustomerSystem
{
    /// <summary>
    /// 死亡标签枚举
    /// </summary>
    public enum DeathTag
    {
        None = 0,
        WeaknessTriggered = 1,      // 弱点触发
        CombatDefeat = 2,           // 战斗失败
        Caught = 3,                 // 被抓获
        Betrayed = 4,               // 被背叛
        Accident = 5,               // 意外事故
        Poisoned = 6,               // 中毒
        Exhausted = 7               // 精疲力竭
    }

    /// <summary>
    /// 事件配置数据
    /// </summary>
    [Serializable]
    public class EventConfig
    {
        public int eventId;                         // 事件ID
        public string eventName;                    // 事件名称
        public string description;                  // 事件描述
        public CustomerPurposeType eventType;       // 事件类型（与顾客目的对应）
        
        [Header("检定配置")]
        public FiveElements checkElement;           // 检定的五行属性
        public int checkThreshold;                  // 检定阈值（属性值需要>=此值才能通过）
        
        [Header("死亡配置")]
        public DeathTag deathTag;                   // 触发死亡的标签
        public string deathDescription;             // 死亡描述
        
        [Header("奖励配置")]
        public int goldReward;                      // 金币奖励
        public int expReward;                       // 经验奖励
    }

    /// <summary>
    /// 战斗事件管理器
    /// </summary>
    public class BattleEventManager : MonoBehaviour
    {
        #region 单例

        private static BattleEventManager _instance;
        public static BattleEventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BattleEventManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("BattleEventManager");
                        _instance = go.AddComponent<BattleEventManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        #endregion

        [Header("事件配置")]
        [SerializeField] private EventConfigDatabase eventDatabase;

        private List<EventConfig> _eventPool = new List<EventConfig>();

        #region Unity生命周期

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            LoadEventPool();
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 加载事件池
        /// </summary>
        public void LoadEventPool()
        {
            _eventPool.Clear();
            if (eventDatabase != null)
            {
                _eventPool = eventDatabase.GetAllEvents();
            }
            else
            {
                // 使用默认事件
                _eventPool = EventExamples.CreateDefaultEvents();
            }
            Debug.Log($"[BattleEventManager] 加载了{_eventPool.Count}个事件");
        }

        /// <summary>
        /// 根据类型获取事件列表
        /// </summary>
        public List<EventConfig> GetEventsByType(CustomerPurposeType type)
        {
            List<EventConfig> result = new List<EventConfig>();
            foreach (var evt in _eventPool)
            {
                if (evt.eventType == type)
                {
                    result.Add(evt);
                }
            }
            return result;
        }

        /// <summary>
        /// 随机抽取一个与指定类型匹配的事件
        /// </summary>
        public EventConfig GetRandomEventByType(CustomerPurposeType purposeType)
        {
            List<EventConfig> matchingEvents = GetEventsByType(purposeType);

            if (matchingEvents.Count == 0)
            {
                Debug.LogWarning($"[BattleEventManager] 没有找到类型为{purposeType}的事件");
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, matchingEvents.Count);
            return matchingEvents[randomIndex];
        }

        /// <summary>
        /// 随机抽取任意事件
        /// </summary>
        public EventConfig GetRandomEvent()
        {
            if (_eventPool.Count == 0)
            {
                Debug.LogWarning("[BattleEventManager] 事件池为空");
                return null;
            }

            int randomIndex = UnityEngine.Random.Range(0, _eventPool.Count);
            return _eventPool[randomIndex];
        }

        /// <summary>
        /// 获取所有事件
        /// </summary>
        public List<EventConfig> GetAllEvents()
        {
            return new List<EventConfig>(_eventPool);
        }

        /// <summary>
        /// 根据ID获取事件
        /// </summary>
        public EventConfig GetEventById(int eventId)
        {
            foreach (var evt in _eventPool)
            {
                if (evt.eventId == eventId)
                    return evt;
            }
            return null;
        }

        #endregion
    }

    /// <summary>
    /// 事件配置数据库 - ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "EventConfigDatabase", menuName = "MaskMerchant/Event Config Database")]
    public class EventConfigDatabase : ScriptableObject
    {
        [SerializeField] private List<EventConfig> events = new List<EventConfig>();

        public List<EventConfig> GetAllEvents()
        {
            return new List<EventConfig>(events);
        }

        public EventConfig GetEvent(int eventId)
        {
            foreach (var evt in events)
            {
                if (evt.eventId == eventId)
                    return evt;
            }
            return null;
        }

        public List<EventConfig> GetEventsByType(CustomerPurposeType type)
        {
            List<EventConfig> result = new List<EventConfig>();
            foreach (var evt in events)
            {
                if (evt.eventType == type)
                    result.Add(evt);
            }
            return result;
        }

#if UNITY_EDITOR
        public void AddEvent(EventConfig evt)
        {
            events.Add(evt);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    /// <summary>
    /// 事件示例数据
    /// </summary>
    public static class EventExamples
    {
        public static List<EventConfig> CreateDefaultEvents()
        {
            return new List<EventConfig>
            {
                // 战斗类事件
                new EventConfig
                {
                    eventId = 3001,
                    eventName = "山贼伏击",
                    description = "顾客在山路上遭遇山贼伏击",
                    eventType = CustomerPurposeType.Combat,
                    checkElement = FiveElements.Metal,
                    checkThreshold = 50,
                    deathTag = DeathTag.CombatDefeat,
                    deathDescription = "顾客在与山贼的战斗中落败身亡",
                    goldReward = 100,
                    expReward = 50
                },
                new EventConfig
                {
                    eventId = 3002,
                    eventName = "武林高手挑战",
                    description = "一位武林高手前来挑战",
                    eventType = CustomerPurposeType.Combat,
                    checkElement = FiveElements.Fire,
                    checkThreshold = 60,
                    deathTag = DeathTag.CombatDefeat,
                    deathDescription = "顾客在比武中败北身亡",
                    goldReward = 150,
                    expReward = 80
                },
                // 行窃类事件
                new EventConfig
                {
                    eventId = 3003,
                    eventName = "夜探府邸",
                    description = "顾客潜入富商府邸行窃",
                    eventType = CustomerPurposeType.Theft,
                    checkElement = FiveElements.Water,
                    checkThreshold = 55,
                    deathTag = DeathTag.Caught,
                    deathDescription = "顾客被府邸护卫发现并击杀",
                    goldReward = 200,
                    expReward = 60
                },
                // 行商类事件
                new EventConfig
                {
                    eventId = 3004,
                    eventName = "商路遇险",
                    description = "顾客在商路上遭遇危险",
                    eventType = CustomerPurposeType.Trade,
                    checkElement = FiveElements.Earth,
                    checkThreshold = 40,
                    deathTag = DeathTag.Accident,
                    deathDescription = "顾客在商路上遭遇意外身亡",
                    goldReward = 80,
                    expReward = 40
                },
                // 暗杀类事件
                new EventConfig
                {
                    eventId = 3005,
                    eventName = "刺杀任务",
                    description = "顾客执行一项刺杀任务",
                    eventType = CustomerPurposeType.Assassination,
                    checkElement = FiveElements.Wood,
                    checkThreshold = 70,
                    deathTag = DeathTag.Betrayed,
                    deathDescription = "顾客在刺杀任务中被出卖身亡",
                    goldReward = 300,
                    expReward = 100
                },
                // 逃亡类事件
                new EventConfig
                {
                    eventId = 3006,
                    eventName = "官府追捕",
                    description = "顾客被官府追捕",
                    eventType = CustomerPurposeType.Escape,
                    checkElement = FiveElements.Water,
                    checkThreshold = 50,
                    deathTag = DeathTag.Caught,
                    deathDescription = "顾客被官府捕获处决",
                    goldReward = 120,
                    expReward = 70
                }
            };
        }
    }
}
