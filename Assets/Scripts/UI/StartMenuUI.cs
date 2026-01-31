using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuUI : MonoBehaviour
{
    public Button startButton;
    public string gameSceneName = "GameScene";

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }

    private void OnStartButtonClicked()
    {
        Debug.Log("开始游戏");
        gameObject.SetActive(false);
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
