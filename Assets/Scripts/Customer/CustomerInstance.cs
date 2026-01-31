// CustomerInstance.cs - 顾客实例类
using System;
using System.Collections.Generic;
using UnityEngine;
using ConfigData;

namespace MaskMerchantGame.CustomerSystem
{
    /// <summary>
    /// 顾客实例 - 运行时顾客数据
    /// 每个顾客实例包含配置引用、对话历史、收集的线索、弱点推测等
    /// </summary>
    [Serializable]
    public class CustomerInstance
    {
        #region 私有字段

        private Customer _configData;                                // 配置表数据（ConfigData.Customer）
        private CustomerDialogue[] _dialogueConfigs;                 // 对话配置数据（ConfigData.CustomerDialogue）
        private List<DialogueHistoryEntry> _dialogueHistory;         // 对话历史
        private List<CollectedClue> _collectedClues;                 // 收集到的有效线索
        private HashSet<int> _usedOptionIndices;                     // 已使用的选项索引
        
        private float _currentTrust;                                 // 当前信任度
        private float _currentSuspicion;                             // 当前怀疑度
        private FiveElements _identifiedWeakness;                    // 玩家推测的弱点
        private bool _isWeaknessIdentified;                          // 是否已推测弱点
        private bool _isDialogueActive;                              // 对话是否激活

        // 面具相关
        private MaskConfig _equippedMask;                            // 装备的面具

        // 死亡状态
        private bool _isDead;                                        // 是否死亡
        private DeathTag _deathTag;                                  // 死亡标签
        private string _deathReason;                                 // 死亡原因

        #endregion

        #region 事件

        /// <summary>
        /// 对话更新事件
        /// </summary>
        public event Action<CustomerInstance, string> OnDialogueUpdated;

        /// <summary>
        /// 弱点识别事件
        /// </summary>
        public event Action<CustomerInstance, FiveElements> OnWeaknessIdentified;

        /// <summary>
        /// 线索收集事件
        /// </summary>
        public event Action<CustomerInstance, CollectedClue> OnClueCollected;

        /// <summary>
        /// 信任度变化事件
        /// </summary>
        public event Action<CustomerInstance, float> OnTrustChanged;

        /// <summary>
        /// 顾客死亡事件
        /// </summary>
        public event Action<CustomerInstance, DeathTag, string> OnCustomerDeath;

        /// <summary>
        /// 面具装备变化事件
        /// </summary>
        public event Action<CustomerInstance, MaskConfig> OnMaskEquipped;

        #endregion

        #region 构造函数

        /// <summary>
        /// 通过配置ID创建顾客实例（使用ConfigData配置表）
        /// </summary>
        /// <param name="customerId">顾客配置ID</param>
        public CustomerInstance(int customerId)
        {
            // 从配置表读取顾客数据
            _configData = ConfigData.CustomerManager.GetConfig(customerId);
            if (_configData == null)
            {
                Debug.LogError($"[CustomerInstance] 未找到顾客配置: ID={customerId}");
                return;
            }

            // 加载对话配置
            LoadDialogueConfigs();

            // 初始化基础数据
            InitializeBaseData();
        }

        /// <summary>
        /// 通过ConfigData.Customer创建顾客实例
        /// </summary>
        /// <param name="configData">配置数据</param>
        public CustomerInstance(Customer configData)
        {
            _configData = configData;
            if (_configData == null)
            {
                Debug.LogError("[CustomerInstance] 配置数据为空");
                return;
            }

            // 加载对话配置
            LoadDialogueConfigs();

            // 初始化基础数据
            InitializeBaseData();
        }

        /// <summary>
        /// 加载对话配置（从CustomerDialogue配置表）
        /// </summary>
        private void LoadDialogueConfigs()
        {
            if (_configData == null || _configData.dialogueOptions == null)
            {
                _dialogueConfigs = new CustomerDialogue[0];
                return;
            }

            int[] dialogueIds = _configData.dialogueOptions;
            _dialogueConfigs = new CustomerDialogue[dialogueIds.Length];

            for (int i = 0; i < dialogueIds.Length; i++)
            {
                int dialogueId = dialogueIds[i];
                if (dialogueId > 0)
                {
                    _dialogueConfigs[i] = CustomerDialogueManager.GetConfig(dialogueId);
                }
                else
                {
                    _dialogueConfigs[i] = null;
                }
            }
        }

        /// <summary>
        /// 初始化基础数据
        /// </summary>
        private void InitializeBaseData()
        {
            _dialogueHistory = new List<DialogueHistoryEntry>();
            _collectedClues = new List<CollectedClue>();
            _usedOptionIndices = new HashSet<int>();
            
            _currentTrust = 50f; // 默认初始信任度
            _currentSuspicion = 0f;
            _identifiedWeakness = FiveElements.None;
            _isWeaknessIdentified = false;
            _isDialogueActive = false;

            _equippedMask = null;

            // 死亡状态初始化
            _isDead = false;
            _deathTag = DeathTag.None;
            _deathReason = string.Empty;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 获取配置表数据（ConfigData.Customer）
        /// </summary>
        public Customer GetConfigData()
        {
            return _configData;
        }

        /// <summary>
        /// 获取顾客名字
        /// </summary>
        public string GetCustomerName()
        {
            return _configData?.customerName ?? "未知顾客";
        }

        /// <summary>
        /// 获取顾客类型/职业
        /// </summary>
        public string GetCustomerType()
        {
            return _configData?.customerType ?? "未知";
        }

        /// <summary>
        /// 获取危险等级
        /// </summary>
        public int GetDangerLevel()
        {
            return _configData?.dangerLevel ?? 1;
        }

        /// <summary>
        /// 获取立绘路径
        /// </summary>
        public string GetCharacterPrefabPath()
        {
            return _configData?.characterPrefabPath ?? string.Empty;
        }

        /// <summary>
        /// 获取真实弱点
        /// </summary>
        public FiveElements GetTrueWeakness()
        {
            return _configData != null ? (FiveElements)_configData.trueWeakness : FiveElements.None;
        }

        /// <summary>
        /// 开始对话
        /// </summary>
        public void StartDialogue()
        {
            _isDialogueActive = true;

            // 如果对话历史为空，添加自我介绍
            if (_dialogueHistory.Count == 0)
            {
                AddIntroDialogue();
            }
        }

        /// <summary>
        /// 结束对话
        /// </summary>
        public void EndDialogue()
        {
            _isDialogueActive = false;
        }

        /// <summary>
        /// 添加自我介绍对话（第一条固定内容）
        /// </summary>
        private void AddIntroDialogue()
        {
            if (_configData == null) return;

            string customerName = GetCustomerName();
            string introContent = _configData.introDialogue;

            if (!string.IsNullOrEmpty(introContent))
            {
                var introEntry = new DialogueHistoryEntry(false, customerName, introContent, -1);
                _dialogueHistory.Add(introEntry);
            }
        }

        /// <summary>
        /// 处理对话选项
        /// </summary>
        /// <param name="optionIndex">选项索引</param>
        /// <param name="optionText">选项文本</param>
        public void ProcessDialogueOption(int optionIndex, string optionText)
        {
            if (!_isDialogueActive) return;

            // 记录已使用的选项
            _usedOptionIndices.Add(optionIndex);

            // 添加商人对话到历史
            var merchantEntry = new DialogueHistoryEntry(true, "商人", optionText, optionIndex);
            _dialogueHistory.Add(merchantEntry);

            string customerName = GetCustomerName();
            string responseText = string.Empty;
            string clueContent = string.Empty;
            bool isValidClue = false;
            int trustChange = 0;

            if (_dialogueConfigs == null || optionIndex >= _dialogueConfigs.Length || _dialogueConfigs[optionIndex] == null)
            {
                Debug.LogWarning($"[CustomerInstance] 未找到对话选项配置: index={optionIndex}");
                return;
            }

            CustomerDialogue dialogueConfig = _dialogueConfigs[optionIndex];
            responseText = dialogueConfig.responseText;
            clueContent = dialogueConfig.clueContent;
            isValidClue = dialogueConfig.isValidClue > 0;
            trustChange = dialogueConfig.dangerLevel; // dangerLevel在对话配置中表示信任度影响

            // 添加顾客回应到历史
            if (!string.IsNullOrEmpty(responseText))
            {
                var customerEntry = new DialogueHistoryEntry(false, customerName, responseText, optionIndex);
                _dialogueHistory.Add(customerEntry);
            }

            // 检查是否有有效线索
            if (isValidClue && !string.IsNullOrEmpty(clueContent))
            {
                CollectClue(optionIndex, clueContent);
            }

            // 更新信任度
            if (trustChange != 0)
            {
                ChangeTrust(trustChange);
            }

            // 触发对话更新事件
            OnDialogueUpdated?.Invoke(this, responseText);
        }

        /// <summary>
        /// 获取选项对应的回应文本
        /// </summary>
        /// <param name="optionIndex">选项索引</param>
        /// <returns>回应文本</returns>
        public string GetResponseForOption(int optionIndex)
        {
            Debug.Log($"[CustomerInstance] GetResponseForOption: index={optionIndex}, _dialogueConfigs is null: {_dialogueConfigs == null}, length: {_dialogueConfigs?.Length ?? 0}");
            
            if (_dialogueConfigs != null && optionIndex < _dialogueConfigs.Length)
            {
                if (_dialogueConfigs[optionIndex] != null)
                {
                    Debug.Log($"[CustomerInstance] responseText: {_dialogueConfigs[optionIndex].responseText}");
                    return _dialogueConfigs[optionIndex].responseText;
                }
                else
                {
                    Debug.LogWarning($"[CustomerInstance] _dialogueConfigs[{optionIndex}] 为 null");
                }
            }
            
            Debug.LogWarning($"[CustomerInstance] 未找到对话配置: index={optionIndex}");
            return string.Empty;
        }

        /// <summary>
        /// 获取对话选项数量
        /// </summary>
        public int GetDialogueOptionCount()
        {
            return _dialogueConfigs?.Length ?? 0;
        }

        /// <summary>
        /// 获取对话配置（ConfigData.CustomerDialogue）
        /// </summary>
        public CustomerDialogue GetDialogueConfig(int optionIndex)
        {
            if (_dialogueConfigs != null && optionIndex < _dialogueConfigs.Length)
            {
                return _dialogueConfigs[optionIndex];
            }
            return null;
        }

        /// <summary>
        /// 收集线索
        /// </summary>
        /// <param name="sourceOptionIndex">来源选项索引</param>
        /// <param name="clueContent">线索内容</param>
        private void CollectClue(int sourceOptionIndex, string clueContent)
        {
            // 检查是否已收集过
            foreach (var clue in _collectedClues)
            {
                if (clue.sourceOptionIndex == sourceOptionIndex)
                    return;
            }

            var newClue = new CollectedClue(sourceOptionIndex, clueContent);
            _collectedClues.Add(newClue);

            OnClueCollected?.Invoke(this, newClue);
        }

        /// <summary>
        /// 观察物品
        /// </summary>
        /// <param name="itemName">物品名称</param>
        public void ObserveItem(string itemName)
        {
            // 可以扩展为观察特定物品获得线索
            Debug.Log($"观察物品: {itemName}");
        }

        /// <summary>
        /// 获取对话历史
        /// </summary>
        /// <returns>对话历史字符串列表</returns>
        public List<string> GetDialogueHistory()
        {
            var result = new List<string>();
            foreach (var entry in _dialogueHistory)
            {
                result.Add(entry.ToString());
            }
            return result;
        }

        /// <summary>
        /// 获取对话历史条目
        /// </summary>
        /// <returns>对话历史条目列表</returns>
        public List<DialogueHistoryEntry> GetDialogueHistoryEntries()
        {
            return new List<DialogueHistoryEntry>(_dialogueHistory);
        }

        /// <summary>
        /// 获取收集到的线索
        /// </summary>
        /// <returns>线索字符串列表</returns>
        public List<string> GetCollectedClues()
        {
            var result = new List<string>();
            foreach (var clue in _collectedClues)
            {
                result.Add(clue.clueContent);
            }
            return result;
        }

        /// <summary>
        /// 获取收集到的线索对象列表
        /// </summary>
        /// <returns>线索对象列表</returns>
        public List<CollectedClue> GetCollectedClueObjects()
        {
            return new List<CollectedClue>(_collectedClues);
        }

        /// <summary>
        /// 检查选项是否已使用
        /// </summary>
        /// <param name="optionIndex">选项索引</param>
        /// <returns>是否已使用</returns>
        public bool IsOptionUsed(int optionIndex)
        {
            return _usedOptionIndices.Contains(optionIndex);
        }

        #endregion

        #region 信任度相关

        /// <summary>
        /// 获取当前信任度
        /// </summary>
        public float GetTrustValue()
        {
            return _currentTrust;
        }

        /// <summary>
        /// 获取当前怀疑度
        /// </summary>
        public float GetSuspicionValue()
        {
            return _currentSuspicion;
        }

        /// <summary>
        /// 改变信任度
        /// </summary>
        /// <param name="delta">变化量</param>
        public void ChangeTrust(float delta)
        {
            _currentTrust = Mathf.Clamp(_currentTrust + delta, 0f, 100f);
            OnTrustChanged?.Invoke(this, _currentTrust);
        }

        /// <summary>
        /// 改变怀疑度
        /// </summary>
        /// <param name="delta">变化量</param>
        public void ChangeSuspicion(float delta)
        {
            _currentSuspicion = Mathf.Clamp(_currentSuspicion + delta, 0f, 100f);
        }

        #endregion

        #region 弱点推测相关

        /// <summary>
        /// 识别/推测弱点
        /// </summary>
        /// <param name="weakness">推测的弱点</param>
        public void IdentifyWeakness(FiveElements weakness)
        {
            if (_isWeaknessIdentified) return;

            _identifiedWeakness = weakness;
            _isWeaknessIdentified = true;

            OnWeaknessIdentified?.Invoke(this, weakness);
        }

        /// <summary>
        /// 获取玩家推测的弱点
        /// </summary>
        public FiveElements GetIdentifiedWeakness()
        {
            return _identifiedWeakness;
        }

        /// <summary>
        /// 检查弱点是否已被识别
        /// </summary>
        public bool IsWeaknessIdentified()
        {
            return _isWeaknessIdentified;
        }

        /// <summary>
        /// 检查推测是否正确
        /// </summary>
        /// <returns>推测是否正确</returns>
        public bool IsWeaknessCorrect()
        {
            FiveElements trueWeakness = GetTrueWeakness();
            return _isWeaknessIdentified && _identifiedWeakness == trueWeakness;
        }

        /// <summary>
        /// 重置弱点推测（允许重新选择）
        /// </summary>
        public void ResetWeaknessIdentification()
        {
            _identifiedWeakness = FiveElements.None;
            _isWeaknessIdentified = false;
        }

        #endregion

        #region 面具属性相关

        /// <summary>
        /// 获取面具属性加成
        /// </summary>
        public FiveElementsAttributes GetMaskAttributes()
        {
            if (_equippedMask != null && _equippedMask.attributeBonus != null)
            {
                return _equippedMask.attributeBonus.Clone();
            }
            return new FiveElementsAttributes();
        }

        #endregion

        #region 面具接口

        /// <summary>
        /// 装备面具
        /// </summary>
        /// <param name="mask">面具配置</param>
        public void EquipMask(MaskConfig mask)
        {
            _equippedMask = mask;
            OnMaskEquipped?.Invoke(this, mask);
            Debug.Log($"[顾客{GetCustomerName()}] 装备面具: {mask?.maskName ?? "无"}");
        }

        /// <summary>
        /// 通过面具ID装备面具
        /// </summary>
        /// <param name="maskId">面具ID</param>
        /// <param name="maskProvider">面具提供者接口</param>
        public void EquipMask(int maskId, IMaskProvider maskProvider)
        {
            if (maskProvider == null)
            {
                Debug.LogWarning("[CustomerInstance] 面具提供者为空");
                return;
            }

            MaskConfig mask = maskProvider.GetMaskConfig(maskId);
            if (mask != null)
            {
                EquipMask(mask);
            }
            else
            {
                Debug.LogWarning($"[CustomerInstance] 未找到面具: ID={maskId}");
            }
        }

        /// <summary>
        /// 卸下面具
        /// </summary>
        public void UnequipMask()
        {
            _equippedMask = null;
            OnMaskEquipped?.Invoke(this, null);
            Debug.Log($"[顾客{GetCustomerName()}] 卸下面具");
        }

        /// <summary>
        /// 获取当前装备的面具
        /// </summary>
        public MaskConfig GetEquippedMask()
        {
            return _equippedMask;
        }

        /// <summary>
        /// 检查是否装备了面具
        /// </summary>
        public bool HasMaskEquipped()
        {
            return _equippedMask != null;
        }

        #endregion

        #region 死亡状态相关

        /// <summary>
        /// 设置顾客死亡
        /// </summary>
        /// <param name="tag">死亡标签</param>
        /// <param name="reason">死亡原因</param>
        public void SetDead(DeathTag tag, string reason)
        {
            if (_isDead) return; // 已经死亡

            _isDead = true;
            _deathTag = tag;
            _deathReason = reason;

            OnCustomerDeath?.Invoke(this, tag, reason);
            Debug.Log($"[顾客{GetCustomerName()}] 死亡 - 标签:{tag}, 原因:{reason}");
        }

        /// <summary>
        /// 检查顾客是否死亡
        /// </summary>
        public bool IsDead()
        {
            return _isDead;
        }

        /// <summary>
        /// 获取死亡标签
        /// </summary>
        public DeathTag GetDeathTag()
        {
            return _deathTag;
        }

        /// <summary>
        /// 获取死亡原因
        /// </summary>
        public string GetDeathReason()
        {
            return _deathReason;
        }

        #endregion
    }
}
