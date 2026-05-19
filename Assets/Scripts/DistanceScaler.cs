using UnityEngine;

public class DistanceScaler : MonoBehaviour
{
    [Header("Настройки")]
    public float baseSize = 50f;      // Размер Image при расстоянии 10 метров
    public float referenceDistance = 10f; // Расстояние, при котором размер = baseSize
    public float minSize = 20f;       // Минимальный размер (вблизи)
    public float maxSize = 100f;      // Максимальный размер (вдали)

    private Camera mainCamera;
    private RectTransform rectTransform;

    void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // Расстояние до камеры
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);

        // Формула: чем дальше, тем больше (чтобы на экране был одинаковый размер)
        float scaleFactor = distance / referenceDistance;
        float newSize = Mathf.Clamp(baseSize * scaleFactor, minSize, maxSize);

        rectTransform.sizeDelta = new Vector2(newSize, newSize);
    }
}