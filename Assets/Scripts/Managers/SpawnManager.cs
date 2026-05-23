using System;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject dinoToSpawn;
    [SerializeField] private GameObject megaToSpawn;
    [Header("Mega Spawn")]
    [SerializeField] private float megaSpawnStartTime  = 90f;  // 1min30
    [SerializeField] private int   megaSpawnCountMin   = 1;
    [SerializeField] private int   megaSpawnCountMax   = 4;
    [SerializeField] private float megaSpawnInterval   = 15f;  // delay between megas


    [SerializeField] GameObject tempMap; //TEMPORARY, WAIT FOR REAL MAP
    private GameManager _gameManager;
    private AudioHandler _audioHandler;
    private WaveManager _waveManager;

    private float _spawnRate = 40f;
    private float _timer = 0f;

    private bool _megaSapwnTriggered = false;
    private int _megasToSpawn        = 0;
    private int _megasSpawned        = 0;
    private float _megaSpawnTimer    = 0f;

    private void OnEnable()
    {
        Dinosaur.OnDinoSpawnRequested   += SpawnDinosaurAt;
        DynoSoulsEvents.OnDinoKill      += OnDinoDied;
    }
    private void OnDisable()
    {
        Dinosaur.OnDinoSpawnRequested   -= SpawnDinosaurAt;
        DynoSoulsEvents.OnDinoKill      -= OnDinoDied;
    }

    private void Start()
    {
        _gameManager  = GameManager.Instance;
        _audioHandler = AudioHandler.Instance;
        _waveManager  = FindObjectOfType<WaveManager>();
    }

    private void Update()
    {
        HandleRegularSpawn();
        HandleMegaSpawn();
    }

    private void HandleRegularSpawn()
    {
        _timer += Time.deltaTime * 0.5f;
        if(_timer < _spawnRate) return;

        _timer = 0f;
        UpdateSpawnRate();
        SpawnDinosaur();
    }

    private void UpdateSpawnRate()
    {
        float t = Mathf.Max(_gameManager.GetgameTime, 1f);
        _spawnRate = Math.Max(5f, 40f / ( t* 0.05f +1 ));
        Debug.Log($"Spawn rate: {_spawnRate}");
    }

    private void SpawnDinosaur()
    {
        Instantiate(dinoToSpawn, GetSpawnPosition(), Quaternion.identity);
        _gameManager.SetDinosAlive(_gameManager.GetDinosAlive() + 1);
    }

    private void SpawnDinosaurAt(Vector3 pos, Dinosaur parent)
    {
        GameObject go = Instantiate(dinoToSpawn, pos, Quaternion.identity);
        Dinosaur dino = go.GetComponent<Dinosaur>();

        dino.SetParent(parent);
        parent.AddChild(dino);
        

        //audioHandler.PlayEffect(audioHandler.spawnEffect, "spawns");

        _gameManager.SetDinosAlive(_gameManager.GetDinosAlive() + 1);
    }

    private void OnDinoDied(Vector3 _)
    {
       _gameManager.SetDinosAlive(_gameManager.GetDinosAlive() -1);
    }

    private void HandleMegaSpawn()
    {
        if(_megaSapwnTriggered)
        {
            if(_megasSpawned >= _megasToSpawn) return;
            
            _megaSpawnTimer -= Time.deltaTime;
            if(_megaSpawnTimer > 0f) return;

            SpawnMegaDinosaur();
            _megasSpawned++;
            _megaSpawnTimer = megaSpawnInterval;
            return;
        }

        if(_gameManager.GetgameTime >= megaSpawnStartTime)
        {
            _megaSapwnTriggered = true;
            _megasToSpawn = UnityEngine.Random.Range(megaSpawnCountMin, megaSpawnCountMax + 1);
            _megaSpawnTimer = 0f;
            Debug.Log($"Mega wave triggered: {_megasToSpawn} megas incoming");
        }
    }
    private void SpawnMegaDinosaur()
    {
        GameObject go = Instantiate(megaToSpawn, GetSpawnPosition(), Quaternion.identity);
        MegaDinosaur mega = go.GetComponent<MegaDinosaur>();

        _gameManager.SetDinosAlive(_gameManager.GetDinosAlive() + 1);
        _waveManager?.ScheduleMegaMigration(mega);
        Debug.Log($"Megadino spawned, migration scheduled in {_waveManager?.name}");
    }


    
   /* private void IncrementSpawnRateWithTime()
    {
        float t = Mathf.Max(gameManager.GetgameTime, 1f);
        spawnRate = Math.Max(5f, 40f/ (t * 0.05f +1));
        int dinosAlive = gameManager.GetDinosAlive();
        dinosAlive += 1;
        gameManager.SetDinosAlive((dinosAlive));
        Debug.Log(spawnRate);
    }
    
    private void SpawnDinosaur()
    {
        GameObject go = Instantiate(objectToSpawn, GetSpawnPosition(), Quaternion.identity);
        //audioHandler.PlayEffect(audioHandler.spawnEffect,"spawns");
    }*/

    private Vector3 GetSpawnPosition() 
    {
        float randomX = UnityEngine.Random.Range(-100f,100f);
        float randomZ = UnityEngine.Random.Range(-100f, 100f);

        Vector3 spawnPos = new Vector3(randomX, 0f, randomZ);

        return spawnPos;
    }

    private void GetBounds() 
    {

    }

    [ContextMenu("Test Spawn Mega")]
    private void TestSpawnMega() => SpawnMegaDinosaur();
}
        
