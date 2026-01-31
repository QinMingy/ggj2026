using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuUI : UIPage
{
    public Button startButton;
    public string nextPageName = "MaskCrafterUI";
    public string gameSceneName = "GameScene";

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }

    public override void OnPageOpen()
    {
        base.OnPageOpen();
        Debug.Log("开始菜单已打开");
    }

    public override void OnPageClose()
    {
        base.OnPageClose();
        Debug.Log("开始菜单已关闭");
    }

    private void OnStartButtonClicked()
    {
        Debug.Log("开始游戏");
        if (!string.IsNullOrEmpty(nextPageName))
        {
            GameManager.Instance.ShowUI(nextPageName);
        }
        else
        {
            Hide();
        }
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }
    }
}
