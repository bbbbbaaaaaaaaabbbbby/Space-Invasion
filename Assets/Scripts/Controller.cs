using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour
{
    public Camera controlCamera;
    public float cursorPlaneDistance = 15f;

    public float forwardSpeed = 20f;
    public float rotationSpeed = 180f;          // Макс. скорость поворота (град/сек)
    public float minRotationSpeed = 20f;        // Мин. скорость у центра экрана
    public float rotationResponseSmoothing = 5f; // Плавность изменения скорости поворота
    public float maxBankAngle = 35f;
    public float bankSmoothness = 4f;
    public float minDistanceToCursor = 2f;

    private Rigidbody rb;
    private Vector3 targetPoint;
    private float currentBankAngle;
    private Quaternion targetRotation;
    private float rotationMultiplier = 1f; // Текущий множитель скорости (0..1)

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 2f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var stats = GetComponent<UserStats>();
        if (stats != null) forwardSpeed = stats.speed;
    }

    void Start()
    {
        if (controlCamera == null) controlCamera = Camera.main;
        targetPoint = transform.position + transform.forward * cursorPlaneDistance;
        targetRotation = transform.rotation;
        currentBankAngle = 0f;
    }

    void Update()
    {
        if (controlCamera == null) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cursorPlaneDistance;
        targetPoint = controlCamera.ScreenToWorldPoint(mousePos);

        Vector3 direction = targetPoint - transform.position;
        float dist = direction.magnitude;

        if (dist < 0.001f) return;

        direction.Normalize();
        if (dist < minDistanceToCursor)
            direction *= minDistanceToCursor;

        // 1. Защита от переворотов при вертикальном взгляде
        Vector3 upRef = Vector3.up;
        if (Mathf.Abs(Vector3.Dot(direction.normalized, Vector3.up)) > 0.95f)
            upRef = transform.up;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized, upRef);

        // 2. Плавный крен через линейную проекцию
        Vector3 localDir = transform.InverseTransformDirection(direction.normalized);
        float targetBank = -localDir.x * maxBankAngle;
        targetBank = Mathf.Clamp(targetBank, -maxBankAngle, maxBankAngle);

        currentBankAngle = Mathf.Lerp(currentBankAngle, targetBank, Time.deltaTime * bankSmoothness);
        targetRotation = lookRotation * Quaternion.Euler(0, 0, currentBankAngle);

        // 3. 🆕 Динамическая скорость поворота по удалению курсора от центра экрана
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        float cursorDistFromCenter = Vector2.Distance(mousePos, screenCenter);
        float maxPossibleDist = Mathf.Max(Screen.width, Screen.height) * 0.5f;

        float centerFactor = Mathf.Clamp01(cursorDistFromCenter / maxPossibleDist);
        // Сглаживаем множитель, чтобы скорость не дёргалась при резких движениях мыши
        rotationMultiplier = Mathf.Lerp(rotationMultiplier, centerFactor, Time.deltaTime * rotationResponseSmoothing);
    }

    void FixedUpdate()
    {
        if (GetComponent<UserStats>().speed > 0)
        rb.MovePosition(rb.position + transform.forward * forwardSpeed * Time.fixedDeltaTime);

        // 🆕 Применяем динамическую скорость поворота
        float dynamicRotationSpeed = Mathf.Lerp(minRotationSpeed, rotationSpeed, rotationMultiplier);
        float maxAngleStep = dynamicRotationSpeed * Time.fixedDeltaTime;

        Quaternion smoothRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, maxAngleStep);
        rb.MoveRotation(smoothRotation);
    }
}