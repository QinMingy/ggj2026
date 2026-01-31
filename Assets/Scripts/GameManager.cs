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
    public List<UIPageConfig> uiPageConfigs = new List<UIPageConfig>();

    private Dictionary<string, GameObject> loadedPages = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> uiPrefabMap = new Dictionary<string, GameObject>();
    private Stack<string> pageHistory = new Stack<string>();
    private string currentPageName;

    [System.Serializable]
    public class UIPageConfig
    {
        public string pageName;
        public GameObject prefab;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUIPrefabMap();
    }

    private void InitializeUIPrefabMap()
    {
        uiPrefabMap.Clear();
        foreach (var config in uiPageConfigs)
        {
            if (!string.IsNullOrEmpty(config.pageName) && config.prefab != null)
            {
                uiPrefabMap[config.pageName] = config.prefab;
            }
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

        if (addToHistory && !string.IsNullOrEmpty(currentPageName))
        {
            pageHistory.Push(currentPageName);
        }

        currentPageName = pageName;
        Debug.Log($"GameManager: 显示页面 {pageName}");
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
        if (pageHistory.Count > 0)
        {
            string previousPage = pageHistory.Pop();
            ShowUI(previousPage, false);
        }
        else
        {
            Debug.LogWarning("GameManager: 没有可返回的页面");
        }
    }

    private GameObject GetOrLoadPage(string pageName)
    {
        if (loadedPages.TryGetValue(pageName, out GameObject existingPage))
        {
            return existingPage;
        }

        if (!uiPrefabMap.TryGetValue(pageName, out GameObject prefab))
        {
            Debug.LogError($"GameManager: 未找到页面 {pageName} 的预制体配置");
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
