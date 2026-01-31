// CustomerDialogueUI.cs - 顾客对话UI界面
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MaskMerchantGame.CustomerSystem
{
    /// <summary>
    /// 顾客对话UI - 对应游戏主对话界面
    /// </summary>
    public class CustomerDialogueUI : MonoBehaviour
    {
        [Header("顶部信息栏")]
        public Image customerPortrait;              // 顾客头像
        public Image[] ratingStars;                 // 星级评分（危险度）
        public TextMeshProUGUI customerNameText;    // 顾客名字
        public TextMeshProUGUI occupationText;      // 职业

        [Header("左侧对话区")]
        public Transform dialogueContentRoot;       // 对话内容根节点
        public GameObject merchantBubblePrefab;     // 商人对话气泡预制体
        public GameObject customerBubblePrefab;     // 顾客对话气泡预制体
        public ScrollRect dialogueScrollRect;       // 对话滚动区域

        [Header("中间角色区")]
        public Image characterPortrait;             // 角色立绘
        public Slider trustSlider;                  // 信任度滑块
        public TextMeshProUGUI trustPercentText;    // 信任度百分比文字
        public Image trustFillGreen;                // 信任度绿色填充
        public Image trustFillRed;                  // 信任度红色填充

        [Header("右侧信息区 - 观察线索")]
        public Transform clueCheckboxRoot;          // 线索checkbox根节点
        public GameObject clueCheckboxPrefab;       // 线索checkbox预制体

        [Header("右侧信息区 - 弱点推测")]
        public TextMeshProUGUI weaknessDeductionTitle; // 弱点推测标题
        public ToggleGroup weaknessToggleGroup;     // 弱点单选组
        public Toggle[] weaknessToggles;            // 弱点单选按钮（金木水火土）
        public TextMeshProUGUI[] weaknessLabels;    // 弱点标签文字

        [Header("对话选项")]
        public Transform dialogueOptionsRoot;       // 对话选项根节点
        public Button[] quickOptionButtons;         // 快捷选项按钮

        [Header("底部选项")] 
        public Button OptionButton1;         // 按钮
        public Button OptionButton2;         // 按钮
        public Button OptionButton3;         // 按钮
        public Button OptionButton4;         // 按钮
        
        [Header("关闭按钮")]
        public Button closeButton;                  // 右上角关闭按钮

        [Header("配置")]
        public int maxDialogueBubbles = 20;         // 最大对话气泡数量
        public float scrollToBottomDelay = 0.1f;    // 滚动到底部延迟

        [Header("顾客管理器引用")]
        public CustomerManager customerManager;     // 顾客管理器引用

        // 当前顾客实例
        private CustomerInstance _currentCustomer;
        private List<GameObject> _dialogueBubbles = new List<GameObject>();
        private List<GameObject> _clueCheckboxes = new List<GameObject>();

        // 事件
        public event Action OnDialogueClosed;
        public event Action<int, string> OnDialogueOptionSelected;
        public event Action<FiveElements> OnWeaknessSelected;
        public event Action<string> OnItemObserved;

        #region Unity生命周期

        private void Awake()
        {
            // 绑定关闭按钮
            if (closeButton != null)
                closeButton.onClick.AddListener(CloseDialogue);

            // 绑定弱点选择
            for (int i = 0; i < weaknessToggles.Length; i++)
            {
                int index = i;
                if (weaknessToggles[i] != null)
                {
                    weaknessToggles[i].onValueChanged.AddListener((isOn) =>
                    {
                        if (isOn) OnWeaknessToggleChanged(index);
                    });
                }
            }
            
            // 绑定OptionButton3点击事件，切换dialogueOptionsRoot显隐
            if (OptionButton3 != null)
            {
                OptionButton3.onClick.AddListener(ToggleDialogueOptionsRoot);
            }
            
            // 模拟初始化一天
            if (customerManager == null)
            {
                customerManager = CustomerManager.Instance;
            }

            customerManager.InitializeDay(1);
            OpenCurrentCustomerDialogue();
            LoadDialogueOptionsFromConfig();
        }

        private void Update()
        {
            // ESC关闭
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseDialogue();
            }

            // 更新信任度显示
            UpdateTrustDisplay();
        }

        private void OnDestroy()
        {
            // 取消事件订阅
            if (_currentCustomer != null)
            {
                _currentCustomer.OnDialogueUpdated -= OnCustomerDialogueUpdated;
                _currentCustomer.OnWeaknessIdentified -= OnCustomerWeaknessIdentified;
            }
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 打开当前顾客对话（使用CustomerManager的当前索引）
        /// </summary>
        public void OpenCurrentCustomerDialogue()
        {
            if (customerManager == null)
            {
                customerManager = CustomerManager.Instance;
            }

            CustomerInstance customer = customerManager.GetCurrentCustomer();
            if (customer != null)
            {
                ShowDialogue(customer);
            }
        }

        /// <summary>
        /// 显示顾客对话界面
        /// </summary>
        public void ShowDialogue(CustomerInstance customer)
        {
            if (customer == null) return;

            _currentCustomer = customer;
            gameObject.SetActive(true);

            // 订阅事件
            _currentCustomer.OnDialogueUpdated += OnCustomerDialogueUpdated;
            _currentCustomer.OnWeaknessIdentified += OnCustomerWeaknessIdentified;

            // 初始化UI
            RefreshCustomerInfo();
            RefreshDialogueHistory();
            RefreshClues();
            RefreshWeaknessOptions();

            // 开始对话
            _currentCustomer.StartDialogue();
        }

        /// <summary>
        /// 关闭对话界面
        /// </summary>
        public void CloseDialogue()
        {
            if (_currentCustomer != null)
            {
                _currentCustomer.EndDialogue();
                _currentCustomer.OnDialogueUpdated -= OnCustomerDialogueUpdated;
                _currentCustomer.OnWeaknessIdentified -= OnCustomerWeaknessIdentified;
                _currentCustomer = null;
            }

            gameObject.SetActive(false);
            OnDialogueClosed?.Invoke();
        }

        /// <summary>
        /// 选择对话选项
        /// </summary>
        public void SelectDialogueOption(int optionIndex, string optionText)
        {
            if (_currentCustomer == null) return;

            // 添加商人对话气泡
            AddDialogueBubble($"商人: \"{optionText}\"", true);

            // 通知顾客实例处理选项（更新状态、收集线索等）
            // ProcessDialogueOption 会触发 OnDialogueUpdated 事件，由事件处理添加顾客回应气泡
            _currentCustomer.ProcessDialogueOption(optionIndex, optionText);

            // 触发事件
            OnDialogueOptionSelected?.Invoke(optionIndex, optionText);
        }

        /// <summary>
        /// 观察物品
        /// </summary>
        public void ObserveItem(string itemName)
        {
            if (_currentCustomer == null) return;

            _currentCustomer.ObserveItem(itemName);
            OnItemObserved?.Invoke(itemName);
        }

        /// <summary>
        /// 设置对话选项（从配置加载）
        /// </summary>
        public void SetDialogueOptions(List<string> options)
        {
            Debug.Log($"[CustomerDialogueUI] SetDialogueOptions: 选项数量={options.Count}, 按钮数量={quickOptionButtons?.Length ?? 0}");
            
            // 绑定按钮点击事件
            for (int i = 0; i < quickOptionButtons.Length; i++)
            {
                if (i < options.Count)
                {
                    quickOptionButtons[i].gameObject.SetActive(true);
                    int index = i;
                    string text = options[i];
                    quickOptionButtons[i].onClick.RemoveAllListeners();
                    quickOptionButtons[i].onClick.AddListener(() => OnOptionButtonClicked(index, text));
                }
                else
                {
                    quickOptionButtons[i].gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 从当前顾客配置加载对话选项
        /// </summary>
        public void LoadDialogueOptionsFromConfig()
        {
            if (_currentCustomer == null) return;
            
            // 从对话配置中获取选项数量
            int optionCount = _currentCustomer.GetDialogueOptionCount();
            Debug.Log($"[CustomerDialogueUI] optionCount: {optionCount}");
            if (optionCount > 0)
            {
                List<string> options = new List<string>();
                for (int i = 0; i < optionCount; i++)
                {
                    options.Add("歪 比 巴 卜");
                }
                SetDialogueOptions(options);
            }
        }
        
        /// <summary>
        /// 选项按钮点击处理
        /// </summary>
        private void OnOptionButtonClicked(int index, string optionText)
        {
            Debug.Log($"[CustomerDialogueUI] OnOptionButtonClicked: index={index}, text={optionText}");
            Debug.Log($"[CustomerDialogueUI] _currentCustomer is null: {_currentCustomer == null}");
            
            // 调用选择对话选项，会自动添加到对话历史并获取顾客回应
            SelectDialogueOption(index, optionText);
        }

        #endregion

        #region UI刷新方法

        /// <summary>
        /// 刷新顾客信息
        /// </summary>
        private void RefreshCustomerInfo()
        {
            if (_currentCustomer == null) return;

            // 名字和职业
            if (customerNameText != null)
                customerNameText.text = $"Name: {_currentCustomer.GetCustomerName()}";

            if (occupationText != null)
                occupationText.text = $"Occupation: {_currentCustomer.GetCustomerType()}";

            // 星级评分（危险度）
            if (ratingStars != null)
            {
                int dangerLevel = _currentCustomer.GetDangerLevel();
                for (int i = 0; i < ratingStars.Length; i++)
                {
                    if (ratingStars[i] != null)
                    {
                        ratingStars[i].enabled = i < dangerLevel;
                    }
                }
            }

            // 加载立绘
            LoadCustomerPortrait(_currentCustomer.GetCharacterPrefabPath());
        }

        /// <summary>
        /// 刷新对话历史
        /// </summary>
        private void RefreshDialogueHistory()
        {
            // 清除旧气泡
            ClearDialogueBubbles();

            if (_currentCustomer == null) return;

            // 添加历史对话
            List<string> history = _currentCustomer.GetDialogueHistory();
            foreach (string dialogue in history)
            {
                bool isMerchant = dialogue.Contains("商人:") || dialogue.Contains("Merchant:");
                AddDialogueBubble(dialogue, isMerchant);
            }

            // 滚动到底部
            ScrollToBottom();
        }

        /// <summary>
        /// 刷新线索列表 - 显示通过对话收集到的有效线索
        /// </summary>
        private void RefreshClues()
        {
            // 清除旧checkbox
            ClearClueCheckboxes();

            if (_currentCustomer == null) return;

            // 获取收集到的有效线索（来自对话选项的validClueIndex）
            List<CollectedClue> collectedClues = _currentCustomer.GetCollectedClueObjects();

            // 显示已收集的有效线索
            foreach (var clue in collectedClues)
            {
                AddClueCheckbox(clue.clueContent, true);
            }

            // 如果没有收集到任何线索，显示提示
            if (collectedClues.Count == 0)
            {
                AddClueCheckbox("（暂无线索，请通过对话获取）", false);
            }
        }

        /// <summary>
        /// 刷新弱点选项
        /// </summary>
        private void RefreshWeaknessOptions()
        {
            if (_currentCustomer == null) return;

            // 重置所有toggle
            foreach (var toggle in weaknessToggles)
            {
                if (toggle != null)
                {
                    toggle.isOn = false;
                    toggle.interactable = !_currentCustomer.IsWeaknessIdentified();
                }
            }

            // 如果已识别弱点，选中对应选项
            if (_currentCustomer.IsWeaknessIdentified())
            {
                FiveElements identified = _currentCustomer.GetIdentifiedWeakness();
                int index = (int)identified - 1; // Metal=1, 所以-1
                if (index >= 0 && index < weaknessToggles.Length && weaknessToggles[index] != null)
                {
                    weaknessToggles[index].isOn = true;
                }
            }
        }

        /// <summary>
        /// 更新信任度显示
        /// </summary>
        private void UpdateTrustDisplay()
        {
            if (_currentCustomer == null) return;

            float trust = _currentCustomer.GetTrustValue();

            if (trustSlider != null)
                trustSlider.value = trust / 100f;

            if (trustPercentText != null)
                trustPercentText.text = $"TRUST: {Mathf.RoundToInt(trust)}%";

            // 更新颜色填充
            if (trustFillGreen != null)
                trustFillGreen.fillAmount = trust / 100f;

            if (trustFillRed != null)
                trustFillRed.fillAmount = _currentCustomer.GetSuspicionValue() / 100f;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 添加对话气泡
        /// </summary>
        private void AddDialogueBubble(string text, bool isMerchant)
        {
            Debug.Log($"[CustomerDialogueUI] AddDialogueBubble: {text}, isMerchant: {isMerchant}");
            
            if (dialogueContentRoot == null)
            {
                Debug.LogError("[CustomerDialogueUI] dialogueContentRoot 为空！");
                return;
            }

            GameObject prefab = isMerchant ? merchantBubblePrefab : customerBubblePrefab;
            if (prefab == null)
            {
                Debug.LogError($"[CustomerDialogueUI] {(isMerchant ? "merchantBubblePrefab" : "customerBubblePrefab")} 为空！");
                return;
            }

            GameObject bubble = Instantiate(prefab, dialogueContentRoot);
            Debug.Log($"[CustomerDialogueUI] 创建气泡成功: {bubble.name}");
            TextMeshProUGUI bubbleText = bubble.GetComponentInChildren<TextMeshProUGUI>();
            if (bubbleText != null)
                bubbleText.text = text;

            _dialogueBubbles.Add(bubble);

            // 限制气泡数量
            while (_dialogueBubbles.Count > maxDialogueBubbles)
            {
                Destroy(_dialogueBubbles[0]);
                _dialogueBubbles.RemoveAt(0);
            }

            // 滚动到底部
            Invoke(nameof(ScrollToBottom), scrollToBottomDelay);
        }

        /// <summary>
        /// 清除对话气泡
        /// </summary>
        private void ClearDialogueBubbles()
        {
            foreach (var bubble in _dialogueBubbles)
            {
                if (bubble != null)
                    Destroy(bubble);
            }
            _dialogueBubbles.Clear();
        }

        /// <summary>
        /// 添加线索checkbox
        /// </summary>
        private void AddClueCheckbox(string clueText, bool isChecked)
        {
            if (clueCheckboxRoot == null || clueCheckboxPrefab == null) return;

            GameObject checkbox = Instantiate(clueCheckboxPrefab, clueCheckboxRoot);

            Toggle toggle = checkbox.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = isChecked;
                toggle.interactable = false; // 只读显示
            }

            TextMeshProUGUI label = checkbox.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = clueText;

            _clueCheckboxes.Add(checkbox);
        }

        /// <summary>
        /// 清除线索checkbox
        /// </summary>
        private void ClearClueCheckboxes()
        {
            foreach (var checkbox in _clueCheckboxes)
            {
                if (checkbox != null)
                    Destroy(checkbox);
            }
            _clueCheckboxes.Clear();
        }

        /// <summary>
        /// 滚动到底部
        /// </summary>
        private void ScrollToBottom()
        {
            if (dialogueScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                dialogueScrollRect.verticalNormalizedPosition = 0f;
            }
        }

        /// <summary>
        /// 加载顾客立绘
        /// </summary>
        private void LoadCustomerPortrait(string prefabPath)
        {
            if (string.IsNullOrEmpty(prefabPath)) return;

            // 从Resources加载立绘
            Sprite portrait = Resources.Load<Sprite>(prefabPath);
            if (portrait != null && characterPortrait != null)
            {
                characterPortrait.sprite = portrait;
            }
        }
        
        /// <summary>
        /// 弱点选择变更
        /// </summary>
        private void OnWeaknessToggleChanged(int index)
        {
            if (_currentCustomer == null || _currentCustomer.IsWeaknessIdentified())
                return;

            FiveElements selectedElement = (FiveElements)(index + 1); // Metal=1
            _currentCustomer.IdentifyWeakness(selectedElement);
            OnWeaknessSelected?.Invoke(selectedElement);

            // 锁定选择
            foreach (var toggle in weaknessToggles)
            {
                if (toggle != null)
                    toggle.interactable = false;
            }
        }

        /// <summary>
        /// 切换对话选项根节点的显隐
        /// </summary>
        private void ToggleDialogueOptionsRoot()
        {
            if (dialogueOptionsRoot != null)
            {
                dialogueOptionsRoot.gameObject.SetActive(!dialogueOptionsRoot.gameObject.activeSelf);
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 顾客对话更新事件处理
        /// </summary>
        private void OnCustomerDialogueUpdated(CustomerInstance customer, string response)
        {
            // 添加顾客回应气泡
            AddDialogueBubble($"{customer.GetCustomerName()}: \"{response}\"", false);

            // 刷新线索
            RefreshClues();
        }

        /// <summary>
        /// 弱点识别事件处理
        /// </summary>
        private void OnCustomerWeaknessIdentified(CustomerInstance customer, FiveElements weakness)
        {
            RefreshWeaknessOptions();
        }

        #endregion
    }
}
