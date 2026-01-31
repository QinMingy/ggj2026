// DialogueBubble.cs - 对话气泡组件
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MaskMerchantGame.CustomerSystem
{
    /// <summary>
    /// 对话气泡组件 - 用于显示单条对话
    /// </summary>
    public class DialogueBubble : MonoBehaviour
    {
        [Header("UI组件")]
        public Image bubbleBackground;          // 气泡背景
        public TextMeshProUGUI speakerNameText; // 说话者名字
        public TextMeshProUGUI dialogueText;    // 对话内容
        public Image speakerIcon;               // 说话者图标（可选）

        [Header("样式配置")]
        public Color merchantBubbleColor = new Color(0.9f, 0.9f, 0.85f);  // 商人气泡颜色
        public Color customerBubbleColor = new Color(0.85f, 0.85f, 0.8f); // 顾客气泡颜色
        public Color highlightColor = Color.red; // 高亮关键词颜色

        /// <summary>
        /// 初始化气泡
        /// </summary>
        public void Initialize(string speaker, string content, bool isMerchant)
        {
            if (speakerNameText != null)
                speakerNameText.text = speaker;

            if (dialogueText != null)
                dialogueText.text = HighlightKeywords(content);

            if (bubbleBackground != null)
                bubbleBackground.color = isMerchant ? merchantBubbleColor : customerBubbleColor;
        }

        /// <summary>
        /// 高亮关键词（如五行属性）
        /// </summary>
        private string HighlightKeywords(string text)
        {
            string[] keywords = { "金", "木", "水", "火", "土", "true qi", "Metal", "Wood", "Water", "Fire", "Earth" };
            string colorHex = ColorUtility.ToHtmlStringRGB(highlightColor);

            foreach (string keyword in keywords)
            {
                text = text.Replace(keyword, $"<color=#{colorHex}>{keyword}</color>");
            }

            return text;
        }
    }
}
