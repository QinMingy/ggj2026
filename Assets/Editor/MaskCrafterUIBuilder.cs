using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class MaskCrafterUIBuilder : EditorWindow
{
    [MenuItem("Tools/Build MaskCrafter UI")]
    public static void BuildUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        GameObject maskCrafterPanel = new GameObject("MaskCrafterPanel");
        maskCrafterPanel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = maskCrafterPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelBg = maskCrafterPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.15f, 1f);

        GameObject centerArea = CreateCenterArea(maskCrafterPanel.transform);
        GameObject rightArea = CreateRightArea(maskCrafterPanel.transform);
        GameObject bottomArea = CreateBottomArea(maskCrafterPanel.transform);
        GameObject leftArea = CreateLeftArea(maskCrafterPanel.transform);
        GameObject createButton = CreateCreateMaskButton(maskCrafterPanel.transform);

        GameObject maskInventoryObj = new GameObject("MaskInventory");
        maskInventoryObj.transform.SetParent(maskCrafterPanel.transform, false);
        MaskInventory maskInventory = maskInventoryObj.AddComponent<MaskInventory>();

        MaskCrafterUI mainUI = maskCrafterPanel.AddComponent<MaskCrafterUI>();
        mainUI.maskDisplay = centerArea.GetComponent<MaskDisplay>();
        mainUI.inventoryPanel = rightArea.GetComponentInChildren<InventoryPanel>();
        mainUI.previewPanel = bottomArea.GetComponent<PreviewPanel>();
        mainUI.maskInventory = maskInventory;
        mainUI.maskInventoryPanel = leftArea.GetComponent<MaskInventoryPanel>();
        mainUI.createMaskButton = createButton.GetComponent<Button>();

        EditorUtility.SetDirty(maskCrafterPanel);
        Selection.activeGameObject = maskCrafterPanel;
        
        Debug.Log("MaskCrafter UI 已成功创建在场景中！");
    }

    private static GameObject CreateCenterArea(Transform parent)
    {
        GameObject centerArea = new GameObject("CenterArea");
        centerArea.transform.SetParent(parent, false);
        RectTransform rect = centerArea.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, 0.2f);
        rect.anchorMax = new Vector2(0.6f, 0.95f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        GameObject maskObj = new GameObject("MaskImage");
        maskObj.transform.SetParent(centerArea.transform, false);
        RectTransform maskRect = maskObj.AddComponent<RectTransform>();
        maskRect.anchorMin = new Vector2(0.5f, 0.5f);
        maskRect.anchorMax = new Vector2(0.5f, 0.5f);
        maskRect.sizeDelta = new Vector2(300, 300);
        Image maskImage = maskObj.AddComponent<Image>();
        maskImage.color = new Color(0.8f, 0.8f, 0.9f, 1f);

        MaskDisplay maskDisplay = centerArea.AddComponent<MaskDisplay>();
        maskDisplay.maskImage = maskImage;

        GameObject elementsParent = new GameObject("ElementModules");
        elementsParent.transform.SetParent(centerArea.transform, false);
        RectTransform elementsRect = elementsParent.AddComponent<RectTransform>();
        elementsRect.anchorMin = Vector2.zero;
        elementsRect.anchorMax = Vector2.one;
        elementsRect.offsetMin = Vector2.zero;
        elementsRect.offsetMax = Vector2.zero;

        maskDisplay.fireValueText = CreateElementModule(elementsParent.transform, "FireModule", "火", new Vector2(0.75f, 0.75f), new Color(1f, 0.3f, 0.2f));
        maskDisplay.waterValueText = CreateElementModule(elementsParent.transform, "WaterModule", "水", new Vector2(0.75f, 0.25f), new Color(0.2f, 0.5f, 1f));
        maskDisplay.windValueText = CreateElementModule(elementsParent.transform, "WindModule", "风", new Vector2(0.25f, 0.75f), new Color(0.6f, 1f, 0.6f));
        maskDisplay.thunderValueText = CreateElementModule(elementsParent.transform, "ThunderModule", "雷", new Vector2(0.25f, 0.25f), new Color(0.9f, 0.8f, 0.2f));
        maskDisplay.earthValueText = CreateElementModule(elementsParent.transform, "EarthModule", "土", new Vector2(0.5f, 0.85f), new Color(0.7f, 0.5f, 0.3f));

        return centerArea;
    }

    private static TextMeshProUGUI CreateElementModule(Transform parent, string name, string elementName, Vector2 anchorPos, Color color)
    {
        GameObject module = new GameObject(name);
        module.transform.SetParent(parent, false);
        RectTransform rect = module.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.sizeDelta = new Vector2(100, 80);
        
        Image bg = module.AddComponent<Image>();
        bg.color = color;

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(module.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = elementName;
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 18;
        label.color = Color.white;

        GameObject valueObj = new GameObject("Value");
        valueObj.transform.SetParent(module.transform, false);
        RectTransform valueRect = valueObj.AddComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(0, 0);
        valueRect.anchorMax = new Vector2(1, 0.5f);
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = Vector2.zero;
        TextMeshProUGUI value = valueObj.AddComponent<TextMeshProUGUI>();
        value.text = "0";
        value.alignment = TextAlignmentOptions.Center;
        value.fontSize = 24;
        value.fontStyle = FontStyles.Bold;
        value.color = Color.white;

        return value;
    }

    private static GameObject CreateRightArea(Transform parent)
    {
        GameObject rightArea = new GameObject("RightArea");
        rightArea.transform.SetParent(parent, false);
        RectTransform rect = rightArea.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.62f, 0.05f);
        rect.anchorMax = new Vector2(0.98f, 0.95f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image bg = rightArea.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.2f, 1f);

        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(rightArea.transform, false);
        RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
        scrollRect.anchorMin = Vector2.zero;
        scrollRect.anchorMax = Vector2.one;
        scrollRect.offsetMin = new Vector2(10, 10);
        scrollRect.offsetMax = new Vector2(-10, -10);

        ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
        scroll.vertical = true;
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewport.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 1f);
        viewport.AddComponent<Mask>().showMaskGraphic = true;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 1000);
        
        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 5;
        layout.padding = new RectOffset(10, 10, 10, 10);

        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewportRect;
        scroll.content = contentRect;

        GameObject categoryTemplate = CreateCategoryTemplate(content.transform);
        GameObject materialItemTemplate = CreateMaterialItemTemplate(content.transform);

        InventoryPanel inventory = scrollView.AddComponent<InventoryPanel>();
        inventory.contentParent = content.transform;

        return rightArea;
    }

    private static GameObject CreateCategoryTemplate(Transform parent)
    {
        GameObject category = new GameObject("CategoryHeader_Template");
        category.transform.SetParent(parent, false);
        RectTransform rect = category.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 40);
        
        Image bg = category.AddComponent<Image>();
        bg.color = new Color(0.3f, 0.3f, 0.4f, 1f);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(category.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 0);
        textRect.offsetMax = new Vector2(-10, 0);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 18;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.MidlineLeft;
        text.color = Color.yellow;

        category.SetActive(false);
        return category;
    }

    private static GameObject CreateMaterialItemTemplate(Transform parent)
    {
        GameObject item = new GameObject("MaterialItem_Template");
        item.transform.SetParent(parent, false);
        RectTransform rect = item.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 70);
        
        Image bg = item.AddComponent<Image>();
        bg.color = new Color(0.25f, 0.25f, 0.3f, 1f);
        bg.raycastTarget = true;

        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(item.transform, false);
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.anchoredPosition = new Vector2(35, 0);
        iconRect.sizeDelta = new Vector2(50, 50);
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = Color.white;
        iconImage.raycastTarget = true;
        icon.AddComponent<MaterialIconDragHandler>();

        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(item.transform, false);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.5f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.offsetMin = new Vector2(90, 0);
        nameRect.offsetMax = new Vector2(-10, -5);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = 14;
        nameText.alignment = TextAlignmentOptions.MidlineLeft;
        nameText.color = Color.white;
        nameText.raycastTarget = false;

        GameObject attrObj = new GameObject("Attribute");
        attrObj.transform.SetParent(item.transform, false);
        RectTransform attrRect = attrObj.AddComponent<RectTransform>();
        attrRect.anchorMin = new Vector2(0, 0);
        attrRect.anchorMax = new Vector2(1, 0.5f);
        attrRect.offsetMin = new Vector2(90, 5);
        attrRect.offsetMax = new Vector2(-10, 0);
        TextMeshProUGUI attrText = attrObj.AddComponent<TextMeshProUGUI>();
        attrText.fontSize = 12;
        attrText.alignment = TextAlignmentOptions.MidlineLeft;
        attrText.color = new Color(0.7f, 1f, 0.7f);
        attrText.raycastTarget = false;

        GameObject qtyObj = new GameObject("Quantity");
        qtyObj.transform.SetParent(item.transform, false);
        RectTransform qtyRect = qtyObj.AddComponent<RectTransform>();
        qtyRect.anchorMin = new Vector2(1, 1);
        qtyRect.anchorMax = new Vector2(1, 1);
        qtyRect.anchoredPosition = new Vector2(-20, -10);
        qtyRect.sizeDelta = new Vector2(40, 25);
        TextMeshProUGUI qtyText = qtyObj.AddComponent<TextMeshProUGUI>();
        qtyText.fontSize = 12;
        qtyText.alignment = TextAlignmentOptions.Center;
        qtyText.color = Color.cyan;
        qtyText.raycastTarget = false;

        MaterialItem materialItem = item.AddComponent<MaterialItem>();
        materialItem.icon = iconImage;
        materialItem.nameText = nameText;
        materialItem.quantityText = qtyText;
        materialItem.attributeText = attrText;

        item.SetActive(false);
        return item;
    }

    private static GameObject CreateBottomArea(Transform parent)
    {
        GameObject bottomArea = new GameObject("BottomArea");
        bottomArea.transform.SetParent(parent, false);
        RectTransform rect = bottomArea.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, 0.02f);
        rect.anchorMax = new Vector2(0.6f, 0.18f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        GameObject panel = new GameObject("PreviewPanel");
        panel.transform.SetParent(bottomArea.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelBg = panel.AddComponent<Image>();
        panelBg.color = new Color(0.2f, 0.2f, 0.3f, 1f);

        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(panel.transform, false);
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0, 0.5f);
        iconRect.anchorMax = new Vector2(0, 0.5f);
        iconRect.anchoredPosition = new Vector2(80, 0);
        iconRect.sizeDelta = new Vector2(80, 80);
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = Color.white;

        GameObject nameObj = new GameObject("MaterialName");
        nameObj.transform.SetParent(panel.transform, false);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.6f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.offsetMin = new Vector2(170, 0);
        nameRect.offsetMax = new Vector2(-20, -10);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = 20;
        nameText.fontStyle = FontStyles.Bold;
        nameText.alignment = TextAlignmentOptions.MidlineLeft;
        nameText.color = Color.white;

        GameObject attrObj = new GameObject("AttributeInfo");
        attrObj.transform.SetParent(panel.transform, false);
        RectTransform attrRect = attrObj.AddComponent<RectTransform>();
        attrRect.anchorMin = new Vector2(0, 0);
        attrRect.anchorMax = new Vector2(1, 0.6f);
        attrRect.offsetMin = new Vector2(170, 10);
        attrRect.offsetMax = new Vector2(-20, 0);
        TextMeshProUGUI attrText = attrObj.AddComponent<TextMeshProUGUI>();
        attrText.fontSize = 16;
        attrText.alignment = TextAlignmentOptions.TopLeft;
        attrText.color = new Color(0.8f, 1f, 0.8f);

        PreviewPanel previewPanel = bottomArea.AddComponent<PreviewPanel>();
        previewPanel.panel = panel;
        previewPanel.materialIcon = iconImage;
        previewPanel.materialNameText = nameText;
        previewPanel.attributeInfoText = attrText;

        return bottomArea;
    }

    private static GameObject CreateLeftArea(Transform parent)
    {
        GameObject leftArea = new GameObject("LeftArea_MaskInventory");
        leftArea.transform.SetParent(parent, false);
        RectTransform rect = leftArea.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.02f, 0.2f);
        rect.anchorMax = new Vector2(0.08f, 0.95f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image bg = leftArea.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.2f, 1f);

        GameObject title = new GameObject("Title");
        title.transform.SetParent(leftArea.transform, false);
        RectTransform titleRect = title.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.anchoredPosition = new Vector2(0, -20);
        titleRect.sizeDelta = new Vector2(0, 40);
        TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "面具背包";
        titleText.fontSize = 16;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.yellow;

        GameObject countObj = new GameObject("Count");
        countObj.transform.SetParent(leftArea.transform, false);
        RectTransform countRect = countObj.AddComponent<RectTransform>();
        countRect.anchorMin = new Vector2(0, 1);
        countRect.anchorMax = new Vector2(1, 1);
        countRect.anchoredPosition = new Vector2(0, -50);
        countRect.sizeDelta = new Vector2(0, 30);
        TextMeshProUGUI countText = countObj.AddComponent<TextMeshProUGUI>();
        countText.text = "已创建面具: 0";
        countText.fontSize = 12;
        countText.alignment = TextAlignmentOptions.Center;
        countText.color = Color.white;

        GameObject scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(leftArea.transform, false);
        RectTransform scrollRect = scrollView.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(1, 1);
        scrollRect.offsetMin = new Vector2(5, 5);
        scrollRect.offsetMax = new Vector2(-5, -90);

        ScrollRect scroll = scrollView.AddComponent<ScrollRect>();
        scroll.vertical = true;
        scroll.horizontal = false;
        scroll.movementType = ScrollRect.MovementType.Clamped;

        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewport.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 1f);
        viewport.AddComponent<Mask>().showMaskGraphic = true;

        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 500);

        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 5;
        layout.padding = new RectOffset(5, 5, 5, 5);

        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewportRect;
        scroll.content = contentRect;

        GameObject maskItemTemplate = CreateMaskInventoryItemTemplate(content.transform);

        MaskInventoryPanel inventoryPanel = leftArea.AddComponent<MaskInventoryPanel>();
        inventoryPanel.contentParent = content.transform;
        inventoryPanel.countText = countText;

        return leftArea;
    }

    private static GameObject CreateMaskInventoryItemTemplate(Transform parent)
    {
        GameObject item = new GameObject("MaskInventoryItem_Template");
        item.transform.SetParent(parent, false);
        RectTransform rect = item.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 120);

        Image bg = item.AddComponent<Image>();
        bg.color = new Color(0.3f, 0.3f, 0.4f, 1f);

        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(item.transform, false);
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(0, 15);
        iconRect.sizeDelta = new Vector2(50, 50);
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = new Color(0.8f, 0.8f, 0.9f, 1f);

        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(item.transform, false);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0);
        nameRect.anchorMax = new Vector2(1, 0);
        nameRect.anchoredPosition = new Vector2(0, 35);
        nameRect.sizeDelta = new Vector2(-10, 20);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.fontSize = 12;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = Color.white;
        nameText.raycastTarget = false;

        GameObject attrObj = new GameObject("Attributes");
        attrObj.transform.SetParent(item.transform, false);
        RectTransform attrRect = attrObj.AddComponent<RectTransform>();
        attrRect.anchorMin = new Vector2(0, 0);
        attrRect.anchorMax = new Vector2(1, 0);
        attrRect.anchoredPosition = new Vector2(0, 15);
        attrRect.sizeDelta = new Vector2(-10, 30);
        TextMeshProUGUI attrText = attrObj.AddComponent<TextMeshProUGUI>();
        attrText.fontSize = 9;
        attrText.alignment = TextAlignmentOptions.Center;
        attrText.color = new Color(0.7f, 1f, 0.7f);
        attrText.raycastTarget = false;

        GameObject timeObj = new GameObject("CreateTime");
        timeObj.transform.SetParent(item.transform, false);
        RectTransform timeRect = timeObj.AddComponent<RectTransform>();
        timeRect.anchorMin = new Vector2(0, 0);
        timeRect.anchorMax = new Vector2(1, 0);
        timeRect.anchoredPosition = new Vector2(0, 5);
        timeRect.sizeDelta = new Vector2(-10, 15);
        TextMeshProUGUI timeText = timeObj.AddComponent<TextMeshProUGUI>();
        timeText.fontSize = 8;
        timeText.alignment = TextAlignmentOptions.Center;
        timeText.color = new Color(0.7f, 0.7f, 0.7f);
        timeText.raycastTarget = false;

        MaskInventoryItem maskItem = item.AddComponent<MaskInventoryItem>();
        maskItem.maskIcon = iconImage;
        maskItem.maskNameText = nameText;
        maskItem.attributesText = attrText;
        maskItem.createTimeText = timeText;

        item.SetActive(false);
        return item;
    }

    private static GameObject CreateCreateMaskButton(Transform parent)
    {
        GameObject buttonObj = new GameObject("CreateMaskButton");
        buttonObj.transform.SetParent(parent, false);
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.35f, 0.02f);
        rect.anchorMax = new Vector2(0.45f, 0.08f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image bg = buttonObj.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.7f, 0.3f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.7f, 0.3f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.8f, 0.4f, 1f);
        colors.pressedColor = new Color(0.15f, 0.6f, 0.25f, 1f);
        button.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "创建面具";
        text.fontSize = 24;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.raycastTarget = false;

        return buttonObj;
    }
}
