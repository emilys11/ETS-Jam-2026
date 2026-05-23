using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] Meteorite meteoritePrefab;

    Player player;
    Collider spawnArea;

    Dictionary<Meteorite, Vector3> meteoriteList = new Dictionary<Meteorite, Vector3>();
    float meteoriteSpeed = 30.0f;

    void Start()
    {
        spawnArea = GetComponent<Collider>();
    }

    void Update()
    {
        float step = meteoriteSpeed * Time.deltaTime;

        foreach (KeyValuePair<Meteorite, Vector3> entry in meteoriteList)
        {
            Debug.Log(entry.Value);
            entry.Key.transform.position = Vector3.MoveTowards(entry.Key.transform.position, entry.Value, step);
        }
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
        meteoriteList.Add(newMeteorite, player.Target.transform.position);
    }

    public Player Player { set => player = value; }
}
