using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class StartMenuUIBuilder : EditorWindow
{
    [MenuItem("Tools/Build Start Menu UI")]
    public static void BuildStartMenu()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        GameObject startMenuPanel = new GameObject("StartMenuPanel");
        startMenuPanel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = startMenuPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelBg = startMenuPanel.AddComponent<Image>();
        panelBg.color = new Color(0.05f, 0.05f, 0.1f, 1f);

        GameObject bgImage = new GameObject("Background");
        bgImage.transform.SetParent(startMenuPanel.transform, false);
        RectTransform bgRect = bgImage.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bg = bgImage.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.15f, 0.25f, 1f);

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(startMenuPanel.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(800, 150);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "面具制造器";
        titleText.fontSize = 80;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(1f, 0.9f, 0.5f, 1f);
        
        Outline titleOutline = titleObj.AddComponent<Outline>();
        titleOutline.effectColor = new Color(0.2f, 0.1f, 0f, 1f);
        titleOutline.effectDistance = new Vector2(3, -3);

        GameObject subtitleObj = new GameObject("Subtitle");
        subtitleObj.transform.SetParent(startMenuPanel.transform, false);
        RectTransform subtitleRect = subtitleObj.AddComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0.5f, 0.6f);
        subtitleRect.anchorMax = new Vector2(0.5f, 0.6f);
        subtitleRect.sizeDelta = new Vector2(600, 60);
        TextMeshProUGUI subtitleText = subtitleObj.AddComponent<TextMeshProUGUI>();
        subtitleText.text = "Mask Crafter";
        subtitleText.fontSize = 36;
        subtitleText.alignment = TextAlignmentOptions.Center;
        subtitleText.color = new Color(0.7f, 0.8f, 1f, 0.8f);

        GameObject startButton = CreateStartButton(startMenuPanel.transform);

        GameObject versionObj = new GameObject("Version");
        versionObj.transform.SetParent(startMenuPanel.transform, false);
        RectTransform versionRect = versionObj.AddComponent<RectTransform>();
        versionRect.anchorMin = new Vector2(1, 0);
        versionRect.anchorMax = new Vector2(1, 0);
        versionRect.anchoredPosition = new Vector2(-20, 20);
        versionRect.sizeDelta = new Vector2(200, 40);
        TextMeshProUGUI versionText = versionObj.AddComponent<TextMeshProUGUI>();
        versionText.text = "v1.0.0";
        versionText.fontSize = 18;
        versionText.alignment = TextAlignmentOptions.BottomRight;
        versionText.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);

        StartMenuUI menuUI = startMenuPanel.AddComponent<StartMenuUI>();
        menuUI.startButton = startButton.GetComponent<Button>();

        EditorUtility.SetDirty(startMenuPanel);
        Selection.activeGameObject = startMenuPanel;

        Debug.Log("开始菜单UI已成功创建在场景中！");
    }

    private static GameObject CreateStartButton(Transform parent)
    {
        GameObject buttonObj = new GameObject("StartButton");
        buttonObj.transform.SetParent(parent, false);
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.35f);
        rect.anchorMax = new Vector2(0.5f, 0.35f);
        rect.sizeDelta = new Vector2(300, 80);

        Image bg = buttonObj.AddComponent<Image>();
        bg.color = new Color(0.3f, 0.6f, 0.9f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.3f, 0.6f, 0.9f, 1f);
        colors.highlightedColor = new Color(0.4f, 0.7f, 1f, 1f);
        colors.pressedColor = new Color(0.2f, 0.5f, 0.8f, 1f);
        colors.selectedColor = new Color(0.3f, 0.6f, 0.9f, 1f);
        button.colors = colors;

        Shadow shadow = buttonObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(4, -4);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "开始游戏";
        text.fontSize = 36;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;

        return buttonObj;
    }
}
