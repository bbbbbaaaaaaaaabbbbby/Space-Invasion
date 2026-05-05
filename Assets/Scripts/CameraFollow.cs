using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset")]
    public Vector3 offset = new Vector3(0, 5, -8); // Ближе к кораблю
    public bool useLocalOffset = true;

    [Header("Hard Follow")]
    [Tooltip("0 = мягко, 1 = жёстко прилипает")]
    [Range(0f, 1f)]
    public float stiffness = 0.9f; // Жёсткость следования
    
    [Tooltip("Мгновенное обновление позиции")]
    public bool instantPosition = false; // Прилипает моментально
    
    [Tooltip("Мгновенное обновление поворота")]
    public bool instantRotation = false;

    [Header("Optional Smooth")]
    public float positionSmooth = 15f; // Высокое значение = почти мгновенно
    public float rotationSmooth = 12f;
    public float lookAhead = 3f;

    void LateUpdate()
    {
        if (target == null) return;

        // Рассчитываем желаемую позицию
        Vector3 desiredOffset = useLocalOffset 
            ? target.TransformDirection(offset) 
            : offset;
        
        Vector3 desiredPosition = target.position + desiredOffset;

        // Позиция: либо мгновенно, либо очень резко
        if (instantPosition)
        {
            transform.position = desiredPosition;
        }
        else
        {
            // Жёсткое следование через Lerp с высоким stiffness
            float t = Mathf.Lerp(0.5f, 1f, stiffness);
            transform.position = Vector3.Lerp(
                transform.position, 
                desiredPosition, 
                positionSmooth * t * Time.deltaTime
            );
        }

        // Поворот: смотрим на точку впереди корабля
        Vector3 lookTarget = target.position + target.forward * lookAhead;
        Vector3 direction = lookTarget - transform.position;
        
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(direction);

            if (instantRotation)
            {
                transform.rotation = desiredRotation;
            }
            else
            {
                float t = Mathf.Lerp(0.5f, 1f, stiffness);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    desiredRotation, 
                    rotationSmooth * t * Time.deltaTime
                );
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        // Мгновенно телепортируем камеру при смене цели
        if (target != null && instantPosition)
        {
            Vector3 desiredOffset = useLocalOffset 
                ? target.TransformDirection(offset) 
                : offset;
            transform.position = target.position + desiredOffset;
        }
    }
}