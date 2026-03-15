using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Tilemaps")] 
    [SerializeField] private Tilemap ground;
    [SerializeField] private Tilemap collision;
    [SerializeField] private Tilemap iceCubesTilemap;
    [SerializeField] private TileBase iceTile;

    private HashSet<Vector2Int> staticWalls  = new();
    private HashSet<Vector2Int> iceBlocks    = new();
    private HashSet<Vector2Int> enemyPositions = new();

    void Awake()
    {
        LoadFromTilemap(collision, staticWalls);
        LoadFromTilemap(iceCubesTilemap, iceBlocks);
    }

    private void LoadFromTilemap(Tilemap tilemap, HashSet<Vector2Int> target)
    {
        // CompressBounds shrinks the cellBounds to only tiles that exist
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
                target.Add((Vector2Int)pos);
        }
    }

    // --- Ice (dynamic) ---
    public void AddIce(Vector2Int pos)
    {
        iceCubesTilemap.SetTile((Vector3Int)pos, iceTile);
        iceBlocks.Add(pos);
    }

    public void RemoveIce(Vector2Int pos)
    {
        iceBlocks.Remove(pos);
        iceCubesTilemap.SetTile((Vector3Int)pos, null);
    }

    public bool HasIce(Vector2Int pos) => iceBlocks.Contains(pos);

    // --- Enemies ---
    public void RegisterEnemy(Vector2Int pos)    => enemyPositions.Add(pos);
    public void UnregisterEnemy(Vector2Int pos)  => enemyPositions.Remove(pos);
    public void MoveEnemy(Vector2Int from, Vector2Int to)
    {
        enemyPositions.Remove(from);
        enemyPositions.Add(to);
    }
    public bool HasEnemy(Vector2Int pos) => enemyPositions.Contains(pos);

    // --- Queries ---
    public bool IsWalkable(Vector2Int pos) =>
        ground.HasTile((Vector3Int)pos) &&
        !staticWalls.Contains(pos) &&
        !iceBlocks.Contains(pos) &&
        !enemyPositions.Contains(pos);

    public bool IsWall(Vector2Int pos) => staticWalls.Contains(pos);
    
    public Vector2Int GetCurrentCell(Transform t) =>
        (Vector2Int)ground.WorldToCell(t.position);

    public Vector3 CellToWorld(Vector2Int cell) =>
        ground.CellToWorld((Vector3Int)cell) + (ground.cellSize * 0.5f);
}