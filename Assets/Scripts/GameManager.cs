using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameManager");
                instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    [Header("UI Settings")]
    public Transform uiRoot;
    
    [Header("Startup Settings")]
    public string initialPageName = "StartMenuUI";
    public bool autoShowInitialPage = true;

    private Dictionary<string, GameObject> loadedPages = new Dictionary<string, GameObject>();
    private Stack<string> pageHistory = new Stack<string>();
    private string currentPageName;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        EnsurePlayerEntity();
    }

    private void EnsurePlayerEntity()
    {
        if (PlayerEntity.Instance == null)
        {
            GameObject playerObj = new GameObject("PlayerEntity");
            playerObj.AddComponent<PlayerEntity>();
            Debug.Log("GameManager: 自动创建PlayerEntity");
        }
    }

    private void Start()
    {
        if (autoShowInitialPage && !string.IsNullOrEmpty(initialPageName))
        {
            ShowUI(initialPageName, false);
        }
    }

    public void ShowUI(string pageName, bool addToHistory = true)
    {
        if (string.IsNullOrEmpty(pageName))
        {
            Debug.LogError("GameManager: 页面名称为空！");
            return;
        }

        if (currentPageName == pageName)
        {
            Debug.LogWarning($"GameManager: 页面 {pageName} 已经在显示中");
            return;
        }

        string previousPageName = currentPageName;

        if (addToHistory && !string.IsNullOrEmpty(previousPageName))
        {
            pageHistory.Push(previousPageName);
            Debug.Log($"GameManager: 将 {previousPageName} 加入历史栈，栈深度: {pageHistory.Count}");
        }

        HideCurrentPage();

        GameObject pageObj = GetOrLoadPage(pageName);
        if (pageObj == null)
        {
            Debug.LogError($"GameManager: 无法加载页面 {pageName}");
            return;
        }

        UIPage page = pageObj.GetComponent<UIPage>();
        if (page != null)
        {
            page.Show();
        }
        else
        {
            pageObj.SetActive(true);
        }

        currentPageName = pageName;
        Debug.Log($"GameManager: 显示页面 {pageName}，当前栈深度: {pageHistory.Count}");
    }

    public void HideUI(string pageName)
    {
        if (loadedPages.TryGetValue(pageName, out GameObject pageObj))
        {
            UIPage page = pageObj.GetComponent<UIPage>();
            if (page != null)
            {
                page.Hide();
            }
            else
            {
                pageObj.SetActive(false);
            }

            if (currentPageName == pageName)
            {
                currentPageName = null;
            }

            Debug.Log($"GameManager: 隐藏页面 {pageName}");
        }
    }

    public void HideCurrentPage()
    {
        if (!string.IsNullOrEmpty(currentPageName))
        {
            HideUI(currentPageName);
        }
    }

    public void GoBack()
    {
        Debug.Log($"GameManager: GoBack调用，当前页面: {currentPageName}，栈深度: {pageHistory.Count}");
        
        if (pageHistory.Count > 0)
        {
            string previousPage = pageHistory.Pop();
            Debug.Log($"GameManager: 从栈中弹出 {previousPage}，剩余栈深度: {pageHistory.Count}");
            ShowUI(previousPage, false);
        }
        else
        {
            Debug.LogWarning($"GameManager: 没有可返回的页面，当前页面: {currentPageName}");
        }
    }

    private GameObject GetOrLoadPage(string pageName)
    {
        if (loadedPages.TryGetValue(pageName, out GameObject existingPage))
        {
            return existingPage;
        }

        GameObject prefab = ResourceManager.Instance.LoadPrefab(pageName);
        if (prefab == null)
        {
            Debug.LogError($"GameManager: 无法从Resources/Prefab/{pageName}加载预制体");
            return null;
        }

        UIPage prefabPage = prefab.GetComponent<UIPage>();
        if (prefabPage == null)
        {
            Debug.LogError($"GameManager: 预制体 {pageName} 没有UIPage组件");
            return null;
        }

        Transform parent = uiRoot != null ? uiRoot : transform;
        GameObject pageObj = Instantiate(prefab, parent);
        pageObj.name = pageName;

        UIPage page = pageObj.GetComponent<UIPage>();
        if (page != null)
        {
            page.pageName = pageName;
        }

        pageObj.SetActive(false);
        loadedPages[pageName] = pageObj;

        Debug.Log($"GameManager: 加载页面 {pageName}");
        return pageObj;
    }

    public GameObject GetLoadedPage(string pageName)
    {
        loadedPages.TryGetValue(pageName, out GameObject page);
        return page;
    }

    public T GetLoadedPage<T>(string pageName) where T : UIPage
    {
        GameObject pageObj = GetLoadedPage(pageName);
        return pageObj != null ? pageObj.GetComponent<T>() : null;
    }

    public void UnloadPage(string pageName)
    {
        if (loadedPages.TryGetValue(pageName, out GameObject pageObj))
        {
            if (currentPageName == pageName)
            {
                currentPageName = null;
            }

            loadedPages.Remove(pageName);
            Destroy(pageObj);
            Debug.Log($"GameManager: 卸载页面 {pageName}");
        }
    }

    public void UnloadAllPages()
    {
        foreach (var kvp in loadedPages)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        loadedPages.Clear();
        pageHistory.Clear();
        currentPageName = null;
        Debug.Log("GameManager: 卸载所有页面");
    }

    public bool IsPageLoaded(string pageName)
    {
        return loadedPages.ContainsKey(pageName);
    }

    public string GetCurrentPageName()
    {
        return currentPageName;
    }
}
