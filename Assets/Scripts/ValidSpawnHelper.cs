using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

public class ValidSpawnHelper : MonoBehaviour
{
    [Header("Tilemap Layers")]
    [SerializeField] Tilemap[] availableTileMap;
    [SerializeField] Tilemap[] nonAvailableTileMap;
    [SerializeField] Tilemap baseLayer;

    private Vector3Int[] cachedValidPositions;

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
    private void Start()
    {
        CacheValidPositions();
    }


    public Vector3 GetRandomValidSpawnLocation()
    {
        int randomIndex = UnityEngine.Random.Range(0, cachedValidPositions.Length);
        return cachedValidPositions[randomIndex];
    }
    public bool ValidSpawnLocation(Vector3 position)
    {
        Vector3Int gridPos = baseLayer.WorldToCell(position);
        for (int i = 0; i < cachedValidPositions.Length; i++)
        {
            if (cachedValidPositions[i] == gridPos)
            {  
                return true; 
            }
        }
        return false;
    }

    private bool ValidSpawnLocationLayer(Vector3 position)
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
    private bool ValidSpawnLocationLayer(Vector3Int position)
    {
        Tilemap referenceMap = nonAvailableTileMap.FirstOrDefault() ?? availableTileMap.FirstOrDefault();
        if (referenceMap == null)
        {
            //Debug.LogError("No tilemaps assigned to ValidSpawnHelper fields!");
            return false;
        }
        Vector3Int gridPos = position;// referenceMap.WorldToCell(position);
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
    private void CacheValidPositions()
    {
        Tilemap referenceMap = baseLayer;
        if (referenceMap == null) return;

        referenceMap.CompressBounds();
        BoundsInt bounds = referenceMap.cellBounds;

        int nbTileValide = 0;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (ValidSpawnLocationLayer(pos))
            {
                nbTileValide++;
            }
            /*Vector3 worldPos = referenceMap.CellToWorld(pos) + referenceMap.tileAnchor;
            if (ValidSpawnLocationLayer(worldPos))
            {
                nbTileValide++;
            }*/
        }
        cachedValidPositions = new Vector3Int[nbTileValide];
        int index = 0;
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (ValidSpawnLocationLayer(pos))
            {
                cachedValidPositions[index++] = pos;
            }
            /*Vector3 worldPos = referenceMap.CellToWorld(pos) + referenceMap.tileAnchor;
            if (ValidSpawnLocationLayer(worldPos))
            {
                cachedValidPositions[index++] = worldPos;
            }*/
        }
    }
}
