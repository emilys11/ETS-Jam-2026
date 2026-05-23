using System;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] GameObject tempMap; //TEMPORARY, WAIT FOR REAL MAP
    GameManager gameManager;
    AudioHandler audioHandler;

    private float spawnRate = 40f;
    private float timer = 0f;

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
    private void SpawnDinosaurAt(Vector3 pos, Dinosaur parent)
    {
        GameObject go = Instantiate(objectToSpawn, pos, Quaternion.identity);
        
        go.GetComponent<Dinosaur>().SetParent(parent);
        parent.AddChild(go.GetComponent<Dinosaur>());

        //audioHandler.PlayEffect(audioHandler.spawnEffect, "spawns");

        int dinosAlive =  gameManager.GetDinosAlive();
        gameManager.SetDinosAlive(dinosAlive + 1);
    }
    private void OnDinoDied(Vector3 _)
    {
        int dinosAlive = gameManager.GetDinosAlive();
        gameManager.SetDinosAlive(dinosAlive - 1);
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        audioHandler = AudioHandler.Instance;
    }

    private void Update()
    {
        timer += Time.deltaTime * 0.5f;
        if (timer >= spawnRate)
        {
            IncrementSpawnRateWithTime();
            SpawnDinosaur();
            timer = 0f;
        }
    }
    
    private void IncrementSpawnRateWithTime()
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
    }

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
}
        
