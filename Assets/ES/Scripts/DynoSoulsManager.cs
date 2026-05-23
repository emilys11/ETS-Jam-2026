using UnityEngine;

public class DynoSoulsManager : MonoBehaviour
{
    public static int startAmount = 500;
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
        DynoSoulsEvents.UpdateUI();
    }

    public void AddCoins(int addAmount)
    {
        Debug.Log("RemoveCoins called: "+addAmount);
        currentAmount += addAmount;
        DynoSoulsEvents.UpdateUI();
    }

    public void RemoveCoins(int remAmount)
    {
        Debug.Log("RemoveCoins called: "+remAmount);
        currentAmount -= remAmount;
        DynoSoulsEvents.UpdateUI();
    }

}
