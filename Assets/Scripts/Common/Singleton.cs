/// <summary>
/// 非MonoBehaviour单例基类
/// 使用方法：public class MyClass : Singleton<MyClass> { ... }
/// </summary>
public abstract class Singleton<T> where T : new()
{
    private static readonly object _lock = new object();
    private static T _instance;

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }

    protected Singleton()
    {
        // 防止外部实例化
        if (_instance != null)
        {
            throw new System.Exception("Singleton instance already exists!");
        }
    }
}