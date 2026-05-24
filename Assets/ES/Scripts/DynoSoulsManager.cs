using UnityEngine;

public class DynoSoulsManager : MonoBehaviour
{
    public static int startAmount = 300;
    public static int passiveAmount = 10;
    public static int currentAmount;
    public static int totalAmount;

    void OnEnable()
    {
        DynoSoulsEvents.OnGainCoins += AddCoins;
        DynoSoulsEvents.OnSpendCoins += TryRemoveCoins;
        DynoSoulsEvents.OnResetCoins += ResetCoins;

        currentAmount = startAmount;

        InvokeRepeating("GeneratePassiveCoins", 1.0f, 1.0f);
    }

    void OnDisable()
    {
        DynoSoulsEvents.OnGainCoins -= AddCoins;
        DynoSoulsEvents.OnSpendCoins -= TryRemoveCoins;
        DynoSoulsEvents.OnResetCoins -= ResetCoins;
    }

    public void ResetCoins()
    {
        currentAmount = startAmount;
        DynoSoulsEvents.UpdateUI();
    }

    public void AddCoins(int addAmount)
    {
        currentAmount += addAmount;
        totalAmount += addAmount;
        DynoSoulsEvents.UpdateUI();
    }

    public void RemoveCoins(int remAmount)
    {
        currentAmount -= remAmount;
        DynoSoulsEvents.UpdateUI();
    }

    public void TryRemoveCoins(int remAmount)
    {
        if (currentAmount < remAmount) return;
        RemoveCoins(remAmount);
    }

    void GeneratePassiveCoins()
    {
        currentAmount += passiveAmount;
        DynoSoulsEvents.UpdateUI();
    }

}
