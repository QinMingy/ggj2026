using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MaterialIconDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private MaterialItem parentItem;
    private GameObject dragObject;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Image iconImage;

    private void Awake()
    {
        parentItem = GetComponentInParent<MaterialItem>();
        iconImage = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (parentItem == null || parentItem.GetQuantity() <= 0) return;

        dragObject = new GameObject("DragIcon");
        dragObject.transform.SetParent(canvas.transform, false);
        dragObject.transform.SetAsLastSibling();

        Image dragImage = dragObject.AddComponent<Image>();
        dragImage.sprite = iconImage.sprite;
        dragImage.raycastTarget = false;
        dragImage.SetNativeSize();

        RectTransform rectTransform = dragObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(60, 60);

        canvasGroup = dragObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        dragObject.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragObject != null)
        {
            dragObject.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragObject != null)
        {
            Destroy(dragObject);
        }

        if (eventData.pointerEnter != null && parentItem != null)
        {
            MaskDisplay maskDisplay = eventData.pointerEnter.GetComponentInParent<MaskDisplay>();
            if (maskDisplay != null && parentItem.GetQuantity() > 0)
            {
                parentItem.UseMaterial();
            }
        }
    }
}
