using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    public enum AIMode { RandomBounce, EveryNTiles, Chase }

    [Header("References")]
    [SerializeField] private GridManager gridState;
    [SerializeField] private Tilemap ground;
    [SerializeField] private Transform player;

    [Header("Settings")]
    [SerializeField] private AIMode mode = AIMode.RandomBounce;
    [SerializeField] private float moveInterval = 0.35f;
    [SerializeField] private int changeDirEveryN = 4;

    private Vector2Int gridPos;
    private Vector2Int currentDir = Vector2Int.right;
    private int stepCount = 0;
    private Animator animator;

    private static readonly Vector2Int[] Dirs = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    void Start()
    {
        animator = GetComponent<Animator>();
        gridPos = WorldToGrid(transform.position);
        // Snap visual position to grid center immediately
        transform.position = GridToWorld(gridPos);
        gridState.RegisterEnemy(gridPos);
        StartCoroutine(AILoop());
    }

    void OnDestroy() => gridState.UnregisterEnemy(gridPos);

    // ---- AI Loop ----

    IEnumerator AILoop()
    {
        while (true)
        {
            Vector2Int next = mode switch
            {
                AIMode.RandomBounce => StepRandomBounce(),
                AIMode.EveryNTiles  => StepEveryN(),
                AIMode.Chase        => StepChase(),
                _                   => gridPos
            };

            if (next != gridPos)
            {
                currentDir = next - gridPos;
                gridState.MoveEnemy(gridPos, next);
                gridPos = next;

                // Wait for the slide to fully finish before deciding next step
                yield return SmoothMove(GridToWorld(gridPos));
                CheckPlayerCollision();
            }
            else
            {
                // Trapped or no path — wait a bit before retrying
                yield return new WaitForSeconds(moveInterval);
            }
        }
    }

    void CheckPlayerCollision()
    {
        Vector2Int playerCell = WorldToGrid(player.position);
        if (playerCell == gridPos)
        {
            Debug.Log("Player caught!"); // hook into your game manager here
        }
    }

    // ---- Behaviors ----

    Vector2Int StepRandomBounce()
    {
        Vector2Int next = gridPos + currentDir;
        if (gridState.IsWalkable(next)) return next;

        // Bumped — pick random valid direction excluding current
        var options = new List<Vector2Int>(Dirs);
        Shuffle(options);
        foreach (var dir in options)
        {
            Vector2Int candidate = gridPos + dir;
            if (gridState.IsWalkable(candidate))
            {
                currentDir = dir;
                return candidate;
            }
        }
        return gridPos; // trapped
    }

    Vector2Int StepEveryN()
    {
        Vector2Int next = gridPos + currentDir;
        bool blocked     = !gridState.IsWalkable(next);
        bool timeToTurn  = stepCount >= changeDirEveryN;

        if (blocked || timeToTurn)
        {
            var options = new List<Vector2Int>(Dirs);
            Shuffle(options);
            foreach (var dir in options)
            {
                Vector2Int candidate = gridPos + dir;
                if (gridState.IsWalkable(candidate))
                {
                    currentDir = dir;
                    stepCount = 0;
                    return candidate;
                }
            }
            return gridPos;
        }

        stepCount++;
        return next;
    }

    Vector2Int StepChase()
    {
        Vector2Int target = WorldToGrid(player.position);
        return BFS(gridPos, target);
    }

    Vector2Int BFS(Vector2Int start, Vector2Int goal)
    {
        if (start == goal) return start;

        var visited = new HashSet<Vector2Int> { start };
        var queue   = new Queue<(Vector2Int pos, Vector2Int firstStep)>();

        foreach (var dir in Dirs)
        {
            Vector2Int n = start + dir;
            if (gridState.IsWalkable(n))
            {
                queue.Enqueue((n, n));
                visited.Add(n);
            }
        }

        while (queue.Count > 0)
        {
            var (current, firstStep) = queue.Dequeue();
            if (current == goal) return firstStep;

            foreach (var dir in Dirs)
            {
                Vector2Int n = current + dir;
                if (!visited.Contains(n) && gridState.IsWalkable(n))
                {
                    visited.Add(n);
                    queue.Enqueue((n, firstStep));
                }
            }
        }

        return gridPos; // no path, stay
    }

    // ---- Helpers ----
    
    IEnumerator SmoothMove(Vector3 target)
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < moveInterval)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / moveInterval);
            yield return null;
        }

        transform.position = target;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    Vector3 GridToWorld(Vector2Int pos) =>
        ground.CellToWorld((Vector3Int)pos) + (Vector3)(ground.cellSize * 0.5f);

    Vector2Int WorldToGrid(Vector3 world) =>
        (Vector2Int)ground.WorldToCell(world);
}