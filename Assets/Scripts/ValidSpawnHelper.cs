using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

public class ValidSpawnHelper : MonoBehaviour
{
    [Header("Tilemap Layers")]
    [SerializeField] Tilemap[] availableTileMap;
    [SerializeField] Tilemap[] nonAvailableTileMap;
    public static ValidSpawnHelper Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            //Debug.LogWarning($"Duplicate ValidSpawnHelper found on {gameObject.name}. Destroying the duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool ValidSpawnLocation(Vector3 position)
    {
        Tilemap referenceMap = nonAvailableTileMap.FirstOrDefault() ?? availableTileMap.FirstOrDefault();
        if (referenceMap == null)
        {
            //Debug.LogError("No tilemaps assigned to ValidSpawnHelper fields!");
            return false;
        }
        Vector3Int gridPos = referenceMap.WorldToCell(position);
        foreach (var item in nonAvailableTileMap)
        {
            if (item.HasTile(gridPos))
            {
                //Debug.LogWarning($"Hit an obstacle/wall tile on layer [{item.name}]! Invalid spot.");
                return false;
            }
        }
        foreach (var item in availableTileMap)
        {
            if (item.HasTile(gridPos))
            {
                //Debug.LogWarning($"Hit a clean floor tile on layer [{item.name}]! Valid spot.");
                return true;
            }
        }
        //Debug.LogWarning("Hit empty void space.");
        return false;
    }
}
