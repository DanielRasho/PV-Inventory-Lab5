using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections;

public class GridMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction iceAction;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float timeToMove = 0.2f;

    [Header("Tilemaps")]
    [SerializeField] private GridManager gridManager;

    [Header("Ice")]
    [SerializeField] private float iceSpawnDelay = 0.06f;

    private bool isMoving;
    private bool isSpawningIce;
    private Vector2 lastDir = Vector2.right;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction  = playerInput.actions["Move"];
        iceAction   = playerInput.actions["Ice"]; // add this action to your Input Action Asset
        animator    = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input != Vector2.zero)
            lastDir = SnapToCardinal(input);

        animator.SetBool("isWalking", isMoving);
        animator.SetFloat("InputX", lastDir.x);
        animator.SetFloat("InputY", lastDir.y);

        if (!isMoving)
            Move(input);

        if (iceAction.WasPressedThisFrame() && !isSpawningIce)
            StartCoroutine(SpawnIceRow(gridManager.GetCurrentCell(transform), Vector2Int.RoundToInt(lastDir)));
    }

    // ---- Movement ----

    Vector2 SnapToCardinal(Vector2 raw)
    {
        if (Mathf.Abs(raw.x) > Mathf.Abs(raw.y))
            return raw.x > 0 ? Vector2.right : Vector2.left;
        else
            return raw.y > 0 ? Vector2.up : Vector2.down;
    }

    void Move(Vector2 raw)
    {
        if (raw == Vector2.zero) return;

        Vector2Int dir        = Vector2Int.RoundToInt(SnapToCardinal(raw));
        Vector2Int currentCell = gridManager.GetCurrentCell(transform);
        Vector2Int targetCell  = currentCell + dir;

        if (!gridManager.IsWalkable(targetCell)) return;

        Vector3 targetWorld = gridManager.CellToWorld(targetCell);
        StartCoroutine(SmoothMove(targetWorld));
    }

    private IEnumerator SmoothMove(Vector3 target)
    {
        isMoving = true;
        Vector3 start   = transform.position;
        float elapsed   = 0f;

        while (elapsed < timeToMove)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / timeToMove);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }

    // ---- Ice Spawning ----
    private IEnumerator SpawnIceRow(Vector2Int origin, Vector2Int dir)
    {
        isSpawningIce = true;
        Vector2Int firstCell = origin + dir;

        // Decide mode based on what's immediately in front
        bool shouldBreak = !gridManager.IsWalkable(firstCell) && gridManager.HasIce(firstCell);

        if (shouldBreak)
            yield return BreakIceRow(origin, dir);
        else
            yield return BuildIceRow(origin, dir);

        isSpawningIce = false;
    }

    private IEnumerator BuildIceRow(Vector2Int origin, Vector2Int dir)
    {
        Vector2Int current = origin + dir;

        while (!gridManager.IsWall(current) && !gridManager.HasEnemy(current) && !gridManager.HasIce(current))
        {
            gridManager.AddIce(current);
            yield return new WaitForSeconds(iceSpawnDelay);
            current += dir;
        }
    }

    private IEnumerator BreakIceRow(Vector2Int origin, Vector2Int dir)
    {
        Vector2Int current = origin + dir;

        while (gridManager.HasIce(current))
        {
            gridManager.RemoveIce(current);
            yield return new WaitForSeconds(iceSpawnDelay);
            current += dir;
        }
    }
}