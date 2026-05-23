using System;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] GameObject tempMap; //TEMPORARY, WAIT FOR REAL MAP
    GameManager gameManager;

    private float spawnRate = 1f;
    private float timer = 0f;

    private void Start()
    {
        gameManager = GameManager.Instance;
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
        spawnRate = (2f/gameManager.GetgameTime);
        int dinosAlive = gameManager.GetDinosAlive();
        dinosAlive += 1;
        gameManager.SetDinosAlive((dinosAlive));
        Debug.Log(spawnRate);
    }
    
    private void SpawnDinosaur()
    {
        GameObject go = Instantiate(objectToSpawn, GetSpawnPosition(), Quaternion.identity);
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
        
