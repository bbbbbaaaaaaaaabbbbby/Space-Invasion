using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 20f;
    public float rotationSpeed = 10f;
    public float mouseSensitivity = 1f;
    public float deadZone = 0.05f;
    
    public float maxBankAngle = 45f;
    public float bankSpeed = 5f;
    
    private Rigidbody rb;
    private Camera mainCamera;
    private float currentBank = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        float mouseX = (Input.mousePosition.x - Screen.width / 2f) / (Screen.width / 2f);
        float mouseY = (Input.mousePosition.y - Screen.height / 2f) / (Screen.height / 2f);
        
        Vector3 targetDir;
        if (Mathf.Abs(mouseX) > deadZone || Mathf.Abs(mouseY) > deadZone)
        {
            Vector3 right = mainCamera.transform.right;
            Vector3 up = mainCamera.transform.up;
            Vector3 forward = mainCamera.transform.forward;
            
            targetDir = forward + (right * mouseX + up * mouseY) * mouseSensitivity;
            targetDir.Normalize();
        }
        else
        {
            targetDir = rb.transform.forward;
        }
        
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * 100f * Time.fixedDeltaTime));
        
        float targetBank = -mouseX * maxBankAngle;
        currentBank = Mathf.Lerp(currentBank, targetBank, bankSpeed * Time.fixedDeltaTime);
        
        Vector3 euler = rb.rotation.eulerAngles;
        euler.z = currentBank;
        rb.MoveRotation(Quaternion.Euler(euler));
        
        rb.linearVelocity = rb.transform.forward * speed;
    }
}