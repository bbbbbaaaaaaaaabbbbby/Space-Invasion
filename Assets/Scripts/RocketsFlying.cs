using UnityEngine;

public class RocketsFlying : MonoBehaviour
{
    public float speed;
    void Update()
    {
        transform.position += -transform.right *  speed * Time.deltaTime;
    }
}
