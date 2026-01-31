using UnityEngine;

/// <summary>
/// 玩家运行态实体（金币 / 声望 / 后续可扩展）
/// </summary>
public class PlayerEntity : MonoBehaviour
{
    public static PlayerEntity Instance { get; private set; }

    [Header("Currency")]
    [SerializeField] private int gold = 3000;
    [SerializeField] private int reputation = 0;

    public int Gold => gold;
    public int Reputation => reputation;

    private void Awake()
    {
        // 单例保护
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /* ================= 金币 ================= */

    public bool HasGold(int amount)
    {
        return gold >= amount;
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;

        gold -= amount;
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        gold += amount;
    }

    public int GetGold()
    {
        return gold;
    }

    /* ================= 声望 ================= */

    /// <summary>
    /// 增加或减少声望（负数表示降低）
    /// </summary>
    public void AddReputation(int amount)
    {
        reputation += amount;
    }

    /// <summary>
    /// 强制设置声望（读档 / Debug）
    /// </summary>
    public void SetReputation(int value)
    {
        reputation = value;
    }
}
