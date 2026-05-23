using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private float babyBoomDuration = 30f;


    public void TriggerBabyBoom()
    {
        Dinosaur[] allDinos = FindObjectsByType<Dinosaur>(FindObjectsInactive.Exclude);
        foreach (Dinosaur dino in allDinos)
            dino.StartBabyBoom(babyBoomDuration);

        Debug.Log($"Baby Boom. {allDinos.Length} dinos affected");
    }

    
    [ContextMenu("Test Baby Boom")]
    private void TestBabyBoom() => TriggerBabyBoom();
}