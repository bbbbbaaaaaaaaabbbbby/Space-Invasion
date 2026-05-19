using Mirror;
using UnityEngine;

public class EnemyIndicator : NetworkBehaviour
{
    [Header("Обводка")]
    public Renderer outlineRenderer;      // Дочерний меш (чуть больше оригинала)

    [Header("Маркер над головой")]
    public GameObject marker;             // Canvas/стрелка/точка над кораблём
    public Vector3 markerOffset = new Vector3(0, 2.5f, 0);

    void Start()
    {
        if (isLocalPlayer)
        {
            // Себя не подсвечиваем
            if (outlineRenderer != null) outlineRenderer.enabled = false;
            if (marker != null) marker.SetActive(false);
        }
        else
        {
            // Враг — включаем всё
            if (outlineRenderer != null)
            {
                outlineRenderer.enabled = true;
                outlineRenderer.material.EnableKeyword("_EMISSION");
                outlineRenderer.material.SetColor("_EmissionColor", Color.red * 2f);
            }

            if (marker != null)
            {
                marker.SetActive(true);
                // Привязываем маркер к позиции корабля
                marker.transform.SetParent(transform);
                marker.transform.localPosition = markerOffset;
            }
        }
    }
}