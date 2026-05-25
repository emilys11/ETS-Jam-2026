using UnityEngine;
using TMPro;


public class DynoSoulsUI : MonoBehaviour
{
    [Header("Text settings")]
    public int numOfDigitsToShow = 8;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI scoreText;
    
    void Start()
    {
        amountText.text = DynoSoulsManager.startAmount.ToString();
        scoreText.text = "0";
    }


    void OnEnable()
    {
        DynoSoulsEvents.OnUpdateUI += UpdateAmount;
        amountText.text = DynoSoulsManager.currentAmount.ToString();

        DynoSoulsEvents.OnUpdateUI += UpdateAmount;
        scoreText.text = "0";
    }

    void OnDisable()
    {
        DynoSoulsEvents.OnUpdateUI -= UpdateAmount; //was +=
        DynoSoulsEvents.OnUpdateUI -= UpdateAmount;
    }

    // AMOUNT TEXT RELATED -------------------------------------------------------------------------------------------

    public void UpdateAmount()
    {
        amountText.text = DynoSoulsManager.currentAmount.ToString();
        scoreText.text = PrependDigitsToString(DynoSoulsManager.totalAmount.ToString());
    }

    string PrependDigitsToString(string text)
    {
        string zeros = new string('0', numOfDigitsToShow - text.Length);
        return text.Insert(0, zeros);
    }
}
