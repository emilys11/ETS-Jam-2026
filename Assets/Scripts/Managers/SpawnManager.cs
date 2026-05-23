using System;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] Dinosaur objectToSpawn;
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
            SpawnDinosaurAt(GetSpawnPosition());
            timer = 0f;
        }
    }
    
    private void IncrementSpawnRateWithTime()
    {
        float t = Mathf.Max(gameManager.GetgameTime, 1f);
        spawnRate = Math.Max(5f, 40f/ (t * 0.05f +1));
        //spawnRate = Mathf.Pow((1/gameManager.GetgameTime),0.35f)*3f;
        int dinosAlive = gameManager.GetDinosAlive();
        dinosAlive += 1;
        gameManager.SetDinosAlive((dinosAlive));
    }
    
    private Vector3 GetSpawnPosition() 
    {
        float randomX = UnityEngine.Random.Range(-10f,10f);
        float randomY = UnityEngine.Random.Range(-10f, 10f);

        Vector3 spawnPos = new Vector3(randomX, randomY, 0f);

        return spawnPos;
    }

    private void GetBounds() 
    {

    }
}
        
