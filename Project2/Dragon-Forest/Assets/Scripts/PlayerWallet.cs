using UnityEngine;
using System;

public class PlayerWallet : MonoBehaviour
{
    [Header("Money Settings")]
    public int currentMoney = 0;

    public event Action<int> OnMoneyChanged;

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        
        // Notify the UI
        OnMoneyChanged?.Invoke(currentMoney);
    }
}