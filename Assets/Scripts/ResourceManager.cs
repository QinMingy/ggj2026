using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<string, Object> _cachedResources = new Dictionary<string, Object>();

    public Sprite LoadSprite(string resourceName)
    {
        string key = $"Sprite_{resourceName}";
        
        if (_cachedResources.TryGetValue(key, out Object cachedObj))
        {
            return cachedObj as Sprite;
        }

        string path = $"Sprite/{resourceName}";
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite != null)
        {
            _cachedResources[key] = sprite;
        }
        else
        {
            Debug.LogWarning($"Failed to load sprite: {path}");
        }

        return sprite;
    }

    public GameObject LoadPrefab(string resourceName)
    {
        string key = $"Prefab_{resourceName}";
        
        if (_cachedResources.TryGetValue(key, out Object cachedObj))
        {
            return cachedObj as GameObject;
        }

        string path = $"Prefab/{resourceName}";
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab != null)
        {
            _cachedResources[key] = prefab;
        }
        else
        {
            Debug.LogWarning($"Failed to load prefab: {path}");
        }

        return prefab;
    }

    public T Load<T>(string resourceName) where T : Object
    {
        string key = $"{typeof(T).Name}_{resourceName}";
        
        if (_cachedResources.TryGetValue(key, out Object cachedObj))
        {
            return cachedObj as T;
        }

        T resource = Resources.Load<T>(resourceName);
        if (resource != null)
        {
            _cachedResources[key] = resource;
        }
        else
        {
            Debug.LogWarning($"Failed to load resource of type {typeof(T).Name}: {resourceName}");
        }

        return resource;
    }

    public void ClearCache()
    {
        _cachedResources.Clear();
        Debug.Log("ResourceManager cache cleared");
    }

    public void UnloadResource(string resourceName, System.Type type)
    {
        string key = $"{type.Name}_{resourceName}";
        if (_cachedResources.ContainsKey(key))
        {
            _cachedResources.Remove(key);
        }
    }
}
