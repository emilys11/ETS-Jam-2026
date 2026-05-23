using UnityEngine;

public class DynoSoulsManager : MonoBehaviour
{
    public static int startAmount = 10;
    public static int currentAmount;

    void OnEnable()
    {
        DynoSoulsEvents.OnGainCoins += AddCoins;
        DynoSoulsEvents.OnSpendCoins += RemoveCoins;
        DynoSoulsEvents.OnResetCoins += ResetCoins;

        currentAmount = startAmount;
    }

    void OnDisable()
    {
        DynoSoulsEvents.OnGainCoins -= AddCoins;
        DynoSoulsEvents.OnSpendCoins -= RemoveCoins;
        DynoSoulsEvents.OnResetCoins -= ResetCoins;
    }


    public void ResetCoins()
    {
        currentAmount = startAmount;
    }

    public void AddCoins(int addAmount)
    {
        currentAmount += addAmount;
    }

    public void RemoveCoins(int remAmount)
    {
        currentAmount -= remAmount;
    }

}
