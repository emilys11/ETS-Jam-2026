using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] Meteorite meteoritePrefab;
    [SerializeField] Volcano volcanoPrefab;
    [SerializeField] Flood floodPrefab;
    [SerializeField] int meteoriteCostValue = 100;
    [SerializeField] int volcanoCostValue = 100;
    [SerializeField] int floodCostValue = 200;

    Player player;
    Collider spawnArea;
    

    void Start()
    {
        spawnArea = GetComponent<Collider>();
    }

    void Update()
    {
        
    }

    // https://discussions.unity.com/t/pick-random-point-inside-box-collider/708849
    public Vector3 RandomPointInBounds()
    {
        Bounds bounds = spawnArea.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void SpawnMeteorite()
    {
        if (DynoSoulsManager.currentAmount < meteoriteCostValue) return;
 
        Meteorite newMeteorite = Instantiate(meteoritePrefab, RandomPointInBounds(), Quaternion.identity);
        newMeteorite.TargetPos = player.Target.transform.position;

        DynoSoulsEvents.SpendCoins(meteoriteCostValue);
    }

    public void SpawnVolcano()
    {
        if (DynoSoulsManager.currentAmount < volcanoCostValue) return;

        Instantiate(volcanoPrefab, player.Target.transform.position, Quaternion.identity);

        DynoSoulsEvents.SpendCoins(volcanoCostValue);
    }

    public void SpawnFlood()
    {
        if (DynoSoulsManager.currentAmount < floodCostValue) return;

        Vector3 pos = new Vector3(-100f, 20, 0f);

        Instantiate(floodPrefab,pos, Quaternion.identity);

        DynoSoulsEvents.SpendCoins(floodCostValue);
    }

    public Player Player { set => player = value; }
}
