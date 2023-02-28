using System;
using UnityEngine;

public static class Wallet
{
    public static event Action<int> OnChanged = null;

    public static int Money
    {
        get => PlayerPrefs.GetInt("WalletMoney", 1000);

        private set
        {
            PlayerPrefs.SetInt("WalletMoney", value);

            OnChanged?.Invoke(value);
        }
    }

    public static void AddMoney(int money)
    {
        if (money > 0)
            Money += money;
    }

    public static bool TryPurchase(int money)
    {
        if (Money >= money)
        {
            Money -= money;

            return true;
        }

        return false;
    }
}
