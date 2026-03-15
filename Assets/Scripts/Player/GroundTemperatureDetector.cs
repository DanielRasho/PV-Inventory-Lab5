using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class GroundTemperatureByTilemap : MonoBehaviour
{
    [Header("Tilemaps de piso")]
    [SerializeField] private Tilemap groundNeutralTilemap;
    [SerializeField] private Tilemap groundColdTilemap;
    [SerializeField] private Tilemap groundHotTilemap;

    [Header("Detección")]
    [SerializeField] private Vector3 feetOffset = new Vector3(0f, -0.2f, 0f);

    [Header("Tiempos")]
    [SerializeField] private float slowInterval = 3f;
    [SerializeField] private float fastInterval = 2f;

    [Header("Temperatura")]
    [SerializeField] private int currentTemperature = 0;
    [SerializeField] private int minTemperature = -5;
    [SerializeField] private int maxTemperature = 5;

    [Header("UI de derrota")]
    [SerializeField] private GameObject burnedCanvas;
    [SerializeField] private GameObject frozenCanvas;

    [Header("Referencias")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Animator arrowAnimator;

    private float timer = 0f;
    private GroundType lastGroundType = GroundType.None;
    private bool lastMovingState = false;
    private bool gameEnded = false;
    private FreeMovement freeMovement;


    private enum GroundType
    {
        None,
        Neutral,
        Hot,
        Cold
    }

    private void Awake()
    {
        UpdateTemperatureUI();
    }

    private void Start()
    {
        if (burnedCanvas != null)
            burnedCanvas.SetActive(false);

        if (frozenCanvas != null)
            frozenCanvas.SetActive(false);

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        freeMovement = GetComponent<FreeMovement>();
    }

    private void Update()
    {
        if (gameEnded) return;

        GroundType currentGround = DetectGroundType();
        bool isMoving = IsPlayerMoving();

        if (currentGround != lastGroundType || isMoving != lastMovingState)
        {
            timer = 0f;
            lastGroundType = currentGround;
            lastMovingState = isMoving;
        }

        if (currentGround == GroundType.Neutral || currentGround == GroundType.None)
            return;

        float currentInterval = GetCurrentInterval(currentGround, isMoving);

        timer += Time.deltaTime;

        if (timer >= currentInterval)
        {
            ApplyTemperatureEffect(currentGround, isMoving);
            timer = 0f;
        }
    }

    private GroundType DetectGroundType()
    {
        Vector3 checkPosition = transform.position + feetOffset;

        if (IsOnTilemap(groundHotTilemap, checkPosition))
            return GroundType.Hot;

        if (IsOnTilemap(groundColdTilemap, checkPosition))
            return GroundType.Cold;

        if (IsOnTilemap(groundNeutralTilemap, checkPosition))
            return GroundType.Neutral;

        return GroundType.None;
    }

    private bool IsOnTilemap(Tilemap tilemap, Vector3 worldPosition)
    {
        if (tilemap == null) return false;

        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return tilemap.GetTile(cellPosition) != null;
    }

    //private bool IsPlayerMoving()
    //{
    //    if (Keyboard.current == null) return false;

    //    return Keyboard.current.wKey.isPressed ||
    //           Keyboard.current.aKey.isPressed ||
    //           Keyboard.current.sKey.isPressed ||
    //           Keyboard.current.dKey.isPressed;
    //}



    private bool IsPlayerMoving()
    {
        if (Keyboard.current == null) return false;

        bool wasdPressed = Keyboard.current.wKey.isPressed ||
                           Keyboard.current.aKey.isPressed ||
                           Keyboard.current.sKey.isPressed ||
                           Keyboard.current.dKey.isPressed;

        bool isDashing = freeMovement != null && freeMovement.IsDashing;


        return wasdPressed || isDashing;
    }

    private void ApplyTemperatureEffect(GroundType groundType, bool isMoving)
    {
        switch (groundType)
        {
            case GroundType.Hot:
                currentTemperature += 1;
                break;

            case GroundType.Cold:
                currentTemperature -= 1;
                break;
        }

        currentTemperature = Mathf.Clamp(currentTemperature, minTemperature, maxTemperature);
        UpdateTemperatureUI();
        CheckEndState();
    }

    private float GetCurrentInterval(GroundType groundType, bool isMoving)
    {
        switch (groundType)
        {
            case GroundType.Hot:
                return isMoving ? fastInterval : slowInterval;

            case GroundType.Cold:
                return isMoving ? slowInterval : fastInterval;

            default:
                return slowInterval;
        }
    }

    private void CheckEndState()
    {
        if (currentTemperature >= maxTemperature)
        {
            gameEnded = true;

            if (burnedCanvas != null)
                burnedCanvas.SetActive(true);

            DisablePlayerInput();
            return;
        }

        if (currentTemperature <= minTemperature)
        {
            gameEnded = true;

            if (frozenCanvas != null)
                frozenCanvas.SetActive(true);

            DisablePlayerInput();
        }
    }

    private void DisablePlayerInput()
    {
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
    }

    private void UpdateTemperatureUI()
    {
        if (arrowAnimator == null) return;

        float normalizedTemperature = Mathf.InverseLerp(minTemperature, maxTemperature, currentTemperature);
        arrowAnimator.SetFloat("TemperatureNormalized", normalizedTemperature);
    }

    public void ApplyDashTemperature(int amount)
    {
        if (gameEnded) return;

        GroundType currentGround = DetectGroundType();

        if (currentGround == GroundType.Hot)
        {
            currentTemperature += amount;
            currentTemperature = Mathf.Clamp(currentTemperature, minTemperature, maxTemperature);
            UpdateTemperatureUI();
            CheckEndState();
        }
    }
}