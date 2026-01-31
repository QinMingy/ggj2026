using UnityEngine;

public class UIPage : MonoBehaviour
{
    public string pageName;

    public virtual void OnPageOpen()
    {
    }

    public virtual void OnPageClose()
    {
    }

    public void Show()
    {
        gameObject.SetActive(true);
        OnPageOpen();
    }

    public void Hide()
    {
        OnPageClose();
        gameObject.SetActive(false);
    }
}
