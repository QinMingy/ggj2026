using UnityEngine;
using ConfigData;

public class TaskSystem : Singleton<TaskSystem>
{
    private int _currentDay = 1;

    public int CurrentDay => _currentDay;

    public void Initialize(int startDay = 1)
    {
        _currentDay = startDay;
        Debug.Log($"TaskSystem initialized at day {_currentDay}");
    }

    public bool CheckMoneyGoal(int currentMoney)
    {
        var dayConfig = ConfigManager.Instance.GetConfig<dayLevel>(_currentDay);
        if (dayConfig == null)
        {
            Debug.LogWarning($"No day level config found for day {_currentDay}");
            return false;
        }

        if (int.TryParse(dayConfig.targetMoney, out int targetMoney))
        {
            return currentMoney >= targetMoney;
        }

        Debug.LogError($"Failed to parse target money: {dayConfig.targetMoney}");
        return false;
    }

    public int GetTargetMoney()
    {
        var dayConfig = ConfigManager.Instance.GetConfig<dayLevel>(_currentDay);
        if (dayConfig == null)
        {
            Debug.LogWarning($"No day level config found for day {_currentDay}");
            return 0;
        }

        if (int.TryParse(dayConfig.targetMoney, out int targetMoney))
        {
            return targetMoney;
        }

        Debug.LogError($"Failed to parse target money: {dayConfig.targetMoney}");
        return 0;
    }

    public float GetCustomerCount()
    {
        var dayConfig = ConfigManager.Instance.GetConfig<dayLevel>(_currentDay);
        if (dayConfig == null)
        {
            Debug.LogWarning($"No day level config found for day {_currentDay}");
            return 0;
        }

        return dayConfig.customerCount;
    }

    public dayLevel GetCurrentDayConfig()
    {
        return ConfigManager.Instance.GetConfig<dayLevel>(_currentDay);
    }

    public void Tick()
    {
        _currentDay++;
        Debug.Log($"Advanced to day {_currentDay}");

        var nextDayConfig = ConfigManager.Instance.GetConfig<dayLevel>(_currentDay);
        if (nextDayConfig == null)
        {
            Debug.LogWarning($"No configuration found for day {_currentDay}. Game may have reached max days.");
        }
    }

    public bool HasNextDay()
    {
        return ConfigManager.Instance.HasConfig<dayLevel>(_currentDay + 1);
    }

    public int GetMaxDay()
    {
        var allConfigs = ConfigManager.Instance.GetAllConfigs<dayLevel>();
        int maxDay = 0;
        foreach (var config in allConfigs.Values)
        {
            if (config.ID > maxDay)
            {
                maxDay = config.ID;
            }
        }
        return maxDay;
    }
}
