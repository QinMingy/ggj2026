using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskCrafterUI : UIPage
{
    public MaskDisplay maskDisplay;
    public InventoryPanel inventoryPanel;
    public PreviewPanel previewPanel;
    public MaskInventory maskInventory;
    public MaskInventoryPanel maskInventoryPanel;
    public Button createMaskButton;
    public Button shopBtn;

    private void Start()
    {
        if (createMaskButton != null)
        {
            createMaskButton.onClick.AddListener(OnCreateMaskClicked);
        }

        if (maskInventory != null)
        {
            maskInventory.OnInventoryChanged += UpdateMaskInventoryDisplay;
        }

        if (shopBtn != null)
        {
            shopBtn.onClick.AddListener(OnShopButtonClicked);
        }

        MaterialInventoryManager.Instance.OnInventoryChanged += RefreshInventory;
    }

    public override void OnPageOpen()
    {
        base.OnPageOpen();
        RefreshInventory();
        Debug.Log("面具制造器页面已打开");
    }

    private void RefreshInventory()
    {
        List<MaterialData> materials = MaterialInventoryManager.Instance.GetMaterialDataList();
        Initialize(materials);
    }

    public override void OnPageClose()
    {
        base.OnPageClose();
        Debug.Log("面具制造器页面已关闭");
    }

    private void OnShopButtonClicked()
    {
        GameManager.Instance.ShowUI("ShopUI");
    }

    public void Initialize(List<MaterialData> materials)
    {
        inventoryPanel.Initialize(materials, OnMaterialSelected, OnMaterialUsed);
        maskDisplay.ResetAttributes();
    }

    private void OnMaterialSelected(MaterialData material)
    {
        previewPanel.ShowPreview(material);
    }

    private void OnMaterialUsed(MaterialData material)
    {
        maskDisplay.ApplyMaterial(material);
    }

    private void OnCreateMaskClicked()
    {
        MaskAttributeData currentAttributes = maskDisplay.GetAttributes();
        
        int totalValue = currentAttributes.fireValue + currentAttributes.waterValue + 
                        currentAttributes.windValue + currentAttributes.thunderValue + 
                        currentAttributes.earthValue;

        if (totalValue == 0)
        {
            Debug.LogWarning("当前面具没有任何属性，无法创建！请先添加材料。");
            return;
        }

        MaskData newMask = maskInventory.CreateMask(currentAttributes);
        maskDisplay.ResetAttributes();
        
        Debug.Log($"成功创建面具: {newMask.maskName}");
    }

    private void UpdateMaskInventoryDisplay()
    {
        if (maskInventoryPanel != null)
        {
            maskInventoryPanel.UpdateInventory(maskInventory.GetAllMasks());
        }
    }

    private void OnDestroy()
    {
        if (createMaskButton != null)
        {
            createMaskButton.onClick.RemoveListener(OnCreateMaskClicked);
        }

        if (maskInventory != null)
        {
            maskInventory.OnInventoryChanged -= UpdateMaskInventoryDisplay;
        }

        if (MaterialInventoryManager.Instance != null)
        {
            MaterialInventoryManager.Instance.OnInventoryChanged -= RefreshInventory;
        }
    }
}
