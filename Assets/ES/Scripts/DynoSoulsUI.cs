using UnityEngine;
using TMPro;


public class DynoSoulsUI : MonoBehaviour
{
    [Header("Text settings")]
    public TextMeshProUGUI amountText;

    

    void OnEnable()
    {
        DynoSoulsEvents.OnUpdateUI += UpdateAmount;
    }

    void OnDisable()
    {
        DynoSoulsEvents.OnUpdateUI += UpdateAmount;
    }

    // AMOUNT TEXT RELATED -------------------------------------------------------------------------------------------


    public void UpdateAmount()
    {
        Debug.Log("UpdateAmount called");
        amountText.text = DynoSoulsManager.currentAmount.ToString();

    }


}
