using UnityEngine;

/// <summary>
/// MonoBehaviour单例基类
/// 使用方法：public class MyClass : MonoSingleton<MyClass> { ... }
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    
                    // 设置为持久化对象
                    // DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (this != _instance)
            {
                Destroy(gameObject);
            }
        }
    }
}