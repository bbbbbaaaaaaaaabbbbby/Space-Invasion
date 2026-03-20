using UnityEngine;

public class CannonToCursor : MonoBehaviour
{
    private Camera _cam;
    public float rotSpeed;

    private void Start()
    {
        _cam = Camera.main;
    }
    
    
    void Update()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            Vector3 direction = hitPoint - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.forward);
        }
    }
}
