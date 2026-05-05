using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [Header("Flight Physics")]
    public float forwardSpeed = 20f;
    public float maxTurnSpeed = 60f;
    public float turnAcceleration = 120f;
    public float turnDecay = 5f;

    [Header("Camera")]
    public GameObject playerCameraPrefab;
    public bool spawnCameraOnStart = true;

    private Rigidbody rb;
    private Vector2 inputAxes;
    private Vector2 currentTurnVelocity;
    private bool canControl = false;

    void Awake() => rb = GetComponent<Rigidbody>();

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        if (spawnCameraOnStart && playerCameraPrefab != null) 
            SpawnCamera();
        
        var gameState = FindObjectOfType<NetworkGameState>();
        if (gameState != null && gameState.gameStarted)
        {
            EnableControl();
        }
    }

    public void EnableControl()
    {
        canControl = true;
        enabled = true;
        Debug.Log("🎮 [PC] Управление ВКЛЮЧЕНО: " + gameObject.name);
    }

    void Update()
    {
        if (!isLocalPlayer || !canControl || !Application.isFocused) return;

        inputAxes.x = Input.GetAxisRaw("Horizontal"); // A/D ←/→
        inputAxes.y = Input.GetAxisRaw("Vertical");   // W/S ↑/↓

        ProcessTurn(inputAxes);

        if (Input.GetMouseButton(0))
            CmdFire();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer || !canControl) return;

        rb.linearVelocity = transform.forward * forwardSpeed;
    }

    void ProcessTurn(Vector2 axes)
    {
        if (axes != Vector2.zero)
        {
            currentTurnVelocity.x = Mathf.MoveTowards(currentTurnVelocity.x, axes.y * maxTurnSpeed, turnAcceleration * Time.deltaTime);
            currentTurnVelocity.y = Mathf.MoveTowards(currentTurnVelocity.y, axes.x * maxTurnSpeed, turnAcceleration * Time.deltaTime);
        }
        else
        {
            currentTurnVelocity.x = Mathf.MoveTowards(currentTurnVelocity.x, 0, turnDecay * Time.deltaTime);
            currentTurnVelocity.y = Mathf.MoveTowards(currentTurnVelocity.y, 0, turnDecay * Time.deltaTime);
        }

        if (currentTurnVelocity.sqrMagnitude > 0.01f)
        {
            Vector3 rotDelta = new Vector3(currentTurnVelocity.x, currentTurnVelocity.y, 0) * Time.deltaTime;
            transform.rotation *= Quaternion.Euler(rotDelta);
        }
    }

    [Command]
    void CmdFire()
    {
        Debug.Log($"🔫 [SERVER] Выстрел от {gameObject.name}");
    }

    void SpawnCamera()
    {
        GameObject camObj = Instantiate(playerCameraPrefab, transform.position, Quaternion.identity);
        camObj.GetComponent<CameraFollow>()?.SetTarget(transform);
        var cam = camObj.GetComponent<Camera>();
        if (cam) { cam.tag = "MainCamera"; cam.gameObject.SetActive(true); }
    }
}