// DialogueOptionButton.cs - 对话选项按钮组件
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MaskMerchantGame.CustomerSystem
{
    /// <summary>
    /// 对话选项按钮组件
    /// </summary>
    public class DialogueOptionButton : MonoBehaviour
    {
        [Header("UI组件")]
        public Button button;                   // 按钮组件
        public Image iconImage;                 // 图标
        public TextMeshProUGUI optionText;      // 选项文字
        public Image backgroundImage;           // 背景图片

        [Header("图标预设")]
        public Sprite chatIcon;                 // 对话图标
        public Sprite observeIcon;              // 观察图标
        public Sprite actionIcon;               // 行动图标

        private int _optionIndex;
        private string _optionContent;
        private Action<int, string> _onClickCallback;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            if (button != null)
                button.onClick.AddListener(OnButtonClick);
        }

        /// <summary>
        /// 初始化选项按钮
        /// </summary>
        public void Initialize(int index, string content, Sprite icon, Action<int, string> onClick)
        {
            _optionIndex = index;
            _optionContent = content;
            _onClickCallback = onClick;

            if (optionText != null)
                optionText.text = content;

            if (iconImage != null && icon != null)
            {
                iconImage.sprite = icon;
                iconImage.gameObject.SetActive(true);
            }
            else if (iconImage != null)
            {
                iconImage.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 设置选项类型图标
        /// </summary>
        public void SetOptionType(DialogueOptionType type)
        {
            if (iconImage == null) return;

            switch (type)
            {
                case DialogueOptionType.Chat:
                    iconImage.sprite = chatIcon;
                    break;
                case DialogueOptionType.Observe:
                    iconImage.sprite = observeIcon;
                    break;
                case DialogueOptionType.Action:
                    iconImage.sprite = actionIcon;
                    break;
            }

            iconImage.gameObject.SetActive(iconImage.sprite != null);
        }

        private void OnButtonClick()
        {
            _onClickCallback?.Invoke(_optionIndex, _optionContent);
        }
    }

    /// <summary>
    /// 对话选项类型
    /// </summary>
    public enum DialogueOptionType
    {
        Chat,       // 普通对话
        Observe,    // 观察物品
        Action      // 执行动作
    }
}
