using UnityEngine;
using TMPro;

public class DynoSoulsUI : MonoBehaviour
{
    public TextMeshProUGUI amountText;
    public Sprite dynoSoulSprite;

    void OnEnable()
    {
        DynoSoulsEvents.OnGainCoins += UpdateAmount;
        DynoSoulsEvents.OnSpendCoins += UpdateAmount;
        DynoSoulsEvents.OnResetCoins += ResetAmount;
    }

    void OnDisable()
    {
        DynoSoulsEvents.OnGainCoins -= UpdateAmount;
        DynoSoulsEvents.OnSpendCoins -= UpdateAmount;
        DynoSoulsEvents.OnResetCoins -= ResetAmount;
    }

    public void ResetAmount()
    {
        amountText.text = DynoSoulsManager.startAmount.ToString();
    }

    public void UpdateAmount(int extraAmount)
    {
        amountText.text = DynoSoulsManager.currentAmount.ToString() + extraAmount.ToString();
    }
}
