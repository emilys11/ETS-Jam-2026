using UnityEngine;
using System;

public class DynoSoulsEvents : MonoBehaviour
{
    public static Action<int> OnGainCoins;
    public static Action<int> OnSpendCoins;
    public static Action OnResetCoins;

    public static void GainCoins(int value) 
    {
        OnGainCoins?.Invoke(value);
    }

    public static void SpendCoins(int value)
    {
        OnSpendCoins?.Invoke(value);
    }

    public static void ResetCoins()
    {
        OnResetCoins?.Invoke();
    }
}
