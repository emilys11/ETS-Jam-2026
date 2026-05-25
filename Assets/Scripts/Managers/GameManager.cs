using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    //Properties
    private float gameTime;
    public float GetgameTime { get { return gameTime; } }

    private float incrementTimer=0f;

    private int dinosAlive;
    public void SetDinosAlive(int da) {  dinosAlive = da; }
    public int GetDinosAlive() { return dinosAlive; }

    private int dinosKilled = 0;
    public void SetDinosKilled(int dk) { dinosKilled = dk; }
    public int GetDinosKilled() { return dinosKilled; }

    private float loseThreshold=500f;

    private DifficultyEnum difficulty;

    private void Start()
    {
        ApplyDifficulty();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        UpdateGameTime();
        CheckLoseCondition();
    }

    private void UpdateGameTime() 
    {
        incrementTimer += Time.deltaTime;
        if (incrementTimer >= 1f) 
        {
            gameTime += 1;
            incrementTimer = 0f;
        }

    }

    private void CheckLoseCondition() 
    {
        if (dinosAlive > loseThreshold) 
        {
            EnterLoseState();
        }
    }

    public void IncrementDinosKilled() 
    {
        dinosKilled += 1;
    }

    public void IncrementDinosAlive() 
    {

    }
   
    
    private void EnterLoseState() 
    {
        MenuManager.Instance.EndGame();
    }

    public DifficultyEnum Difficulty { get => difficulty; set => difficulty = value; }

    public enum DifficultyEnum
    {
        Easy,
        Hard,
        Apocalypse
    }

    [Header("Difficulty Settings")]
    [SerializeField] private float babyBoomTriggerTime = 40f;
    [SerializeField] private float megaSpawnStartTime  = 80f;
    [SerializeField] private float megaSpawnInterval   = 15f;
    [SerializeField] private float migrationDelay      = 15f;
    [SerializeField] private int   megaSpawnCountMin   = 1;
    [SerializeField] private int   megaSpawnCountMax   = 4;

    public float BabyBoomTriggerTime => babyBoomTriggerTime;
    public float MegaSpawnStartTime  => megaSpawnStartTime;
    public float MegaSpawnInterval   => megaSpawnInterval;
    public float MigrationDelay      => migrationDelay;
    public int   MegaSpawnCountMin   => megaSpawnCountMin;
    public int   MegaSpawnCountMax   => megaSpawnCountMax;
    [SerializeField] private float breakTimeMin = 35f;
    [SerializeField] private float breakTimeMax = 60f;
    public float BreakTimeMin => breakTimeMin;
    public float BreakTimeMax => breakTimeMax;

    //TODO call this when game starts with ui
    public void ApplyDifficulty()
    {
        switch (difficulty)
        {
            case DifficultyEnum.Easy:
                babyBoomTriggerTime = 40f; megaSpawnStartTime = 80f; migrationDelay = 15f;
                megaSpawnInterval = 15f;   megaSpawnCountMin = 1;    megaSpawnCountMax = 4;
                breakTimeMin = 40f; breakTimeMax = 70f;
                break;
            case DifficultyEnum.Hard:
                babyBoomTriggerTime = 25f; megaSpawnStartTime = 55f; migrationDelay = 10f;
                megaSpawnInterval = 10f;   megaSpawnCountMin = 2;    megaSpawnCountMax = 5;
                breakTimeMin = 20f; breakTimeMax = 40f;
                break;
            case DifficultyEnum.Apocalypse:
                babyBoomTriggerTime = 12f; megaSpawnStartTime = 35f; migrationDelay = 5f;
                megaSpawnInterval = 6f;    megaSpawnCountMin = 3;    megaSpawnCountMax = 7;
                breakTimeMin = 8f; breakTimeMax = 20f;
                break;
        }
    }
}
