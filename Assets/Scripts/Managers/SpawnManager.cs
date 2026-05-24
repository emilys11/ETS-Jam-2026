
using UnityEngine;
using UnityEngine.Pool;

/*public class SpawnManager : MonoBehaviour
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
    [SerializeField] private float _spawnRate = 5f;
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
        //float t = Mathf.Max(_gameManager.GetgameTime, 1f);
        //_spawnRate = Mathf.Max(5f, 5.0f / (t * 0.05f + 1));
        if(_gameManager.GetgameTime == 0f) { return; }
        _spawnRate = Mathf.Pow((1 / _gameManager.GetgameTime), 0.35f) * 3f;
        //_spawnRate = Mathf.Pow(1f/_gameManager.GetgameTime,2))*3f;
        Debug.Log($"Spawn rate updated: {_spawnRate}"); // Décommenter si tu veux voir le rate descendre
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
}*/
using System;
using UnityEngine;
using UnityEngine.Pool; // Required for native pooling

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Dinosaur dinoPrefab;       // Changed from GameObject to Component
    [SerializeField] private MegaDinosaur megaPrefab;   // Changed from GameObject to Component

    [Header("Mega Spawn")]
    [SerializeField] private float megaSpawnStartTime = 90f;
    [SerializeField] private int megaSpawnCountMin = 1;
    [SerializeField] private int megaSpawnCountMax = 4;
    [SerializeField] private float megaSpawnInterval = 15f;

    private GameManager _gameManager;
    private AudioHandler _audioHandler;
    private WaveManager _waveManager;

    [Header("Debug")]
    [SerializeField] private float _spawnRate = 40f;
    [SerializeField] private float _timer = 0f;

    private bool _megaSpawnTriggered = false;
    private int _megasToSpawn = 0;
    private int _megasSpawned = 0;
    private float _megaSpawnTimer = 0f;

    private bool isFirstSpawn = true;
    [SerializeField] private int firstSpawnAmount = 20;

    private IObjectPool<Dinosaur> _dinoPool;
    private IObjectPool<MegaDinosaur> _megaPool;

    private void Awake()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        // Pool for Regular Dinosaurs
        _dinoPool = new ObjectPool<Dinosaur>(
            createFunc: () => Instantiate(dinoPrefab),
            actionOnGet: (dino) => dino.gameObject.SetActive(true),
            actionOnRelease: (dino) => dino.gameObject.SetActive(false),
            actionOnDestroy: (dino) => Destroy(dino.gameObject),
            collectionCheck: false,
            defaultCapacity: 500,
            maxSize: 500
        );

        // Pool for Mega Dinosaurs
        _megaPool = new ObjectPool<MegaDinosaur>(
            createFunc: () => Instantiate(megaPrefab),
            actionOnGet: (mega) => mega.gameObject.SetActive(true),
            actionOnRelease: (mega) => mega.gameObject.SetActive(false),
            actionOnDestroy: (mega) => Destroy(mega.gameObject),
            collectionCheck: false,
            defaultCapacity: 20,
            maxSize: 100
        );
    }

    private void OnEnable()
    {
        Dinosaur.OnDinoSpawnRequested += SpawnDinosaurAt;
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
        _audioHandler = AudioHandler.Instance;
        _waveManager = FindAnyObjectByType<WaveManager>();

        _timer = _spawnRate;

        if (_gameManager == null) Debug.LogError("SpawnManager: GameManager est INTROUVABLE !");
        if (dinoPrefab == null) Debug.LogError("SpawnManager: Il manque le Prefab dinoPrefab !");
    }

    private void Update()
    {
        if (_gameManager == null) return;

        HandleRegularSpawn();
        HandleMegaSpawn();
    }

    private void HandleRegularSpawn()
    {
        _timer += Time.deltaTime * 0.5f;
        if (_timer < _spawnRate) return;
        _timer = 0f;
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
        if (_gameManager.GetgameTime <= 0.1f)
            return;
        _spawnRate = Mathf.Pow(1f / _gameManager.GetgameTime, 0.35f) * 3f;
    }

    
    private void SpawnDinosaur()
    {
        if (dinoPrefab == null) return;

        Dinosaur dino = _dinoPool.Get();
        dino.transform.position = GetSpawnPosition();
        dino.transform.rotation = Quaternion.identity;

        dino.ConfigurePool(_dinoPool);

        _gameManager.SetDinosAlive(_gameManager.GetDinosAlive() + 1);
    }

    
    private void SpawnDinosaurAt(Vector3 pos, Dinosaur parent)
    {
        if (dinoPrefab == null) return;

        Dinosaur dino = _dinoPool.Get();
        dino.transform.position = pos;
        dino.transform.rotation = Quaternion.identity;

        dino.ConfigurePool(_dinoPool);
        dino.SetParent(parent);
        parent.AddChild(dino);

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
        if (_megaSpawnTriggered)
        {
            if (_megasSpawned >= _megasToSpawn) return;

            _megaSpawnTimer -= Time.deltaTime;
            if (_megaSpawnTimer > 0f) return;

            SpawnMegaDinosaur();
            _megasSpawned++;
            _megaSpawnTimer = megaSpawnInterval;
            return;
        }

        if (_gameManager.GetgameTime >= megaSpawnStartTime)
        {
            _megaSpawnTriggered = true;
            _megasToSpawn = UnityEngine.Random.Range(megaSpawnCountMin, megaSpawnCountMax + 1);
            _megaSpawnTimer = 0f;
            Debug.Log($"Mega wave triggered: {_megasToSpawn} megas incoming");
        }
    }

    // Spawning a mega dinosaur from the pool
    private void SpawnMegaDinosaur()
    {
        if (megaPrefab == null) return;

        MegaDinosaur mega = _megaPool.Get();
        mega.transform.position = GetSpawnPosition();
        mega.transform.rotation = Quaternion.identity;

        // CRITICAL: Pass the pool reference to the mega dino
        mega.ConfigurePool(_megaPool);

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