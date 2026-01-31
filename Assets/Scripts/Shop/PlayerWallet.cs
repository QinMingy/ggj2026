using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public static PlayerWallet Instance;

    public int Gold { get; private set; } = 3000;

    private void Awake()
    {
        Instance = this;
    }

    public bool Spend(int amount)
    {
        if (Gold < amount) return false;
        Gold -= amount;
        return true;
    }

    public void Add(int amount)
    {
        Gold += amount;
    }
}

