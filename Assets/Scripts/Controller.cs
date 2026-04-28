using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour
{
    public Camera controlCamera;
    public float cursorPlaneDistance = 15f;

    public float forwardSpeed = 20f;
    public float rotationSpeed = 5f;
    public float maxBankAngle = 35f;
    public float bankSmoothness = 4f;
    public float minDistanceToCursor = 2f;

    private Rigidbody rb;
    private Vector3 targetPoint;
    private float currentBankAngle;
    private Quaternion targetRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 2f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
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

        if (dist < minDistanceToCursor)
            direction = direction.normalized * minDistanceToCursor;
        else
            direction = direction.normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);

        Vector3 localDir = transform.InverseTransformDirection(direction);
        float targetBank = -Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg * (maxBankAngle / 45f);
        targetBank = Mathf.Clamp(targetBank, -maxBankAngle, maxBankAngle);
        currentBankAngle = Mathf.Lerp(currentBankAngle, targetBank, Time.deltaTime * bankSmoothness);

        targetRotation = lookRotation * Quaternion.Euler(0, 0, currentBankAngle);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * forwardSpeed * Time.fixedDeltaTime);

        float maxAngleStep = rotationSpeed * 180f * Time.fixedDeltaTime;
        Quaternion smoothRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, maxAngleStep);
        rb.MoveRotation(smoothRotation);
    }
}