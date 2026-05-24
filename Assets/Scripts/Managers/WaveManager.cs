using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private float babyBoomDuration = 30f;
    [SerializeField] private float migrationDelay    = 15f; //after mega dino spawn


    private float _nextBabyBoomTime;

    public void ScheduleMegaMigration(MegaDinosaur mega)
    {
        StartCoroutine(MigrationDelayRoutine(mega));
    }
    private IEnumerator MigrationDelayRoutine(MegaDinosaur mega)
    {
        yield return new WaitForSeconds(migrationDelay);
        if(mega != null && !mega.IsDead)
            mega.InitiateMigration();
    }


    public void TriggerAllMegaMigrations()
    {
        MegaDinosaur[] megas = FindObjectsByType<MegaDinosaur>(FindObjectsInactive.Exclude);
        foreach(MegaDinosaur mega in megas)
            mega.InitiateMigration();
    }

    public void TriggerBabyBoom()
    {
        Dinosaur[] allDinos = FindObjectsByType<Dinosaur>(FindObjectsInactive.Exclude);
        foreach (Dinosaur dino in allDinos)
            dino.StartBabyBoom(babyBoomDuration);

        Debug.Log($"Baby Boom. {allDinos.Length} dinos affected");
    }

    private void Start()
    {
        migrationDelay = GameManager.Instance.MigrationDelay;
        _nextBabyBoomTime = GameManager.Instance.BabyBoomTriggerTime; //for the first wave events
    }
    
    private void Update()
    {
        if(GameManager.Instance.GetgameTime >= _nextBabyBoomTime)
        {
            TriggerBabyBoom();
            float randomBreak = Random.Range(GameManager.Instance.BreakTimeMin, GameManager.Instance.BreakTimeMax);

            _nextBabyBoomTime = GameManager.Instance.GetgameTime + randomBreak;
            Debug.Log($"Next Baby Boom in {randomBreak:F1}s");
        }
    }
    
    [ContextMenu("Test Baby Boom")]
    private void TestBabyBoom() => TriggerBabyBoom();
    [ContextMenu("Test Migration")]
    private void TestMigration() => TriggerAllMegaMigrations();
}