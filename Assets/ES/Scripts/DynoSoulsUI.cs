using UnityEngine;
using TMPro;


public class DynoSoulsUI : MonoBehaviour
{
    [Header("Text settings")]
    public TextMeshProUGUI amountText;
    
    void Start()
    {
        amountText.text = DynoSoulsManager.startAmount.ToString();
    }


    void OnEnable()
    {
        DynoSoulsEvents.OnUpdateUI += UpdateAmount;
        amountText.text = DynoSoulsManager.currentAmount.ToString();
    }

    void OnDisable()
    {
        DynoSoulsEvents.OnUpdateUI += UpdateAmount;
    }

    // AMOUNT TEXT RELATED -------------------------------------------------------------------------------------------

    public void UpdateAmount()
    {
        amountText.text = DynoSoulsManager.currentAmount.ToString();

    }


}
