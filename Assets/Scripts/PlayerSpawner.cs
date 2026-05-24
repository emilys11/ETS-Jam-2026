using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] Meteorite meteoritePrefab;

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
        Meteorite newMeteorite = Instantiate(meteoritePrefab, RandomPointInBounds(), Quaternion.identity);
        newMeteorite.TargetPos = player.Target.transform.position;
    }

    public Player Player { set => player = value; }
}
