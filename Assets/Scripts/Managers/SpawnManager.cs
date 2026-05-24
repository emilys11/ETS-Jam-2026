
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject dinoToSpawn;
    [SerializeField] private GameObject megaToSpawn;

    [Header("Mega Spawn")]
    [SerializeField] private float megaSpawnStartTime = 90f;  // 1min30
    [SerializeField] private int megaSpawnCountMin = 1;
    [SerializeField] private int megaSpawnCountMax = 4;
    [SerializeField] private float megaSpawnInterval = 15f;  // delay between megas

    private GameManager _gameManager;
    private AudioHandler _audioHandler;
    private WaveManager _waveManager;

    [Header("Debug")]
    [SerializeField] private float _spawnRate = 40f;
    [SerializeField] private float _timer = 0f;

    private bool _megaSapwnTriggered = false;
    private int _megasToSpawn = 0;
    private int _megasSpawned = 0;
    private float _megaSpawnTimer = 0f;

    private float _nextMegaWaveTime = 0f;

    bool isFirstSpawn = true;
    [SerializeField] int firstSpawnAmount = 20;

    private void OnEnable()
    {
        Dinosaur.OnDinoSpawnRequested += SpawnDinosaurAt;

        // Sécurité si DynoSoulsEvents est introuvable au lancement
        try { DynoSoulsEvents.OnDinoKill += OnDinoDied; } catch { }
    }

    private void OnDisable()
    {
        Dinosaur.OnDinoSpawnRequested -= SpawnDinosaurAt;
        try { DynoSoulsEvents.OnDinoKill -= OnDinoDied; } catch { }
    }

    
    private void Start()
    {
        _gameManager = GameManager.Instance;

        megaSpawnStartTime = _gameManager.MegaSpawnStartTime;
        megaSpawnInterval = _gameManager.MegaSpawnInterval;
        megaSpawnCountMin = _gameManager.MegaSpawnCountMin;
        megaSpawnCountMax = _gameManager.MegaSpawnCountMax;


        _audioHandler = AudioHandler.Instance;
        _waveManager = FindAnyObjectByType<WaveManager>();

        // FIX: On triche pour faire spawn le tout 1er dino IMMÉDIATEMENT au début du jeu !
        _timer = _spawnRate;

        if (_gameManager == null) Debug.LogError("SpawnManager: GameManager est INTROUVABLE !");
        if (dinoToSpawn == null) Debug.LogError("SpawnManager: Il manque le Prefab dinoToSpawn !");
    }

    private void Update()
    {
        // Si le GameManager a planté, on arrête d'essayer de spawn pour pas spammer la console d'erreurs
        if (_gameManager == null) return;

        HandleRegularSpawn();
        HandleMegaSpawn();
    }

    private void HandleRegularSpawn()
    {
        _timer += Time.deltaTime * 0.5f;
        if (_timer < _spawnRate) return;
        _timer = 0f; // Reset du timer
        UpdateSpawnRate();
        if (isFirstSpawn)
        {
            for (int i = 0; i < firstSpawnAmount; i++)
            {
                SpawnDinosaur();
            }
            isFirstSpawn = false;
        }
        else
        {
            SpawnDinosaur();
        }
    }

    private void UpdateSpawnRate()
    {
        float t = Mathf.Max(_gameManager.GetgameTime, 1f);
        _spawnRate = Mathf.Max(5f, 5.0f / (t * 0.05f + 1));
        // Debug.Log($"Spawn rate updated: {_spawnRate}"); // Décommenter si tu veux voir le rate descendre
    }

    private void SpawnDinosaur()
    {
        if (dinoToSpawn == null) return;
        Instantiate(dinoToSpawn, GetSpawnPosition(), Quaternion.identity);
        _gameManager.SetDinosAlive(_gameManager.GetDinosAlive() + 1);
    }

    private void SpawnDinosaurAt(Vector3 pos, Dinosaur parent)
    {
        if (dinoToSpawn == null) return;
        GameObject go = Instantiate(dinoToSpawn, pos, Quaternion.identity);
        Dinosaur dino = go.GetComponent<Dinosaur>();

        if (dino != null)
        {
            dino.SetParent(parent);
            parent.AddChild(dino);
        }

        if (_gameManager != null)
            _gameManager.SetDinosAlive(_gameManager.GetDinosAlive() + 1);
    }

    private void OnDinoDied(Vector3 _)
    {
        if (_gameManager != null)
            _gameManager.SetDinosAlive(_gameManager.GetDinosAlive() - 1);
    }

    private void HandleMegaSpawn()
    {
        if (_megaSapwnTriggered)
        {
            if (_megasSpawned >= _megasToSpawn)
            {
                _megaSapwnTriggered = false;
                _megasSpawned = 0;
                float randomBreak = Random.Range(_gameManager.BreakTimeMin, _gameManager.BreakTimeMax);

                _nextMegaWaveTime = _gameManager.GetgameTime + randomBreak;
                Debug.Log($"mega wav done. Next in {randomBreak:F1}s");
                return;
            }

            _megaSpawnTimer -= Time.deltaTime;
            if (_megaSpawnTimer > 0f) return;

            SpawnMegaDinosaur();
            _megasSpawned++;
            _megaSpawnTimer = megaSpawnInterval;
            return;
        }

        float triggerTime = _nextMegaWaveTime > 0f ? _nextMegaWaveTime : megaSpawnStartTime;

        if (_gameManager.GetgameTime >= triggerTime)
        {
            _megaSapwnTriggered = true;
            _megasToSpawn = UnityEngine.Random.Range(megaSpawnCountMin, megaSpawnCountMax + 1);
            _megaSpawnTimer = 0f; // Force le premier mega à spawn tout de suite
            Debug.Log($"Mega wave triggered: {_megasToSpawn} megas incoming");
        }
    }

    private void SpawnMegaDinosaur()
    {
        if (megaToSpawn == null) return;
        GameObject go = Instantiate(megaToSpawn, GetSpawnPosition(), Quaternion.identity);
        MegaDinosaur mega = go.GetComponent<MegaDinosaur>();

        _gameManager.SetDinosAlive(_gameManager.GetDinosAlive() + 1);

        if (_waveManager != null)
        {
            _waveManager.ScheduleMegaMigration(mega);
            Debug.Log($"Megadino spawned, migration scheduled.");
        }
    }

    
    private Vector3 GetSpawnPosition()
    {
        return ValidSpawnHelper.Instance.GetRandomValidSpawnLocation();
    }

    [ContextMenu("Test Spawn Mega")]
    private void TestSpawnMega() => SpawnMegaDinosaur();
}