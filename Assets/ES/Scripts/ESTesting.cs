using UnityEngine;

public class ESTesting : MonoBehaviour
{
    public GameObject dino;

    public void KillDino()
    {
        DynoSoulsEvents.DinoKill(dino.transform.position);
        DynoSoulsEvents.SpendCoins(100);
        dino.SetActive(false);
    }

    public void GetMoney()
    {
        DynoSoulsEvents.GainCoins(12);
        dino.SetActive(true);
    }
}
