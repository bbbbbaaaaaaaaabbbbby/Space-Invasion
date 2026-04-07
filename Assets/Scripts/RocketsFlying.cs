using UnityEngine;

public class RocketsFlying : MonoBehaviour
{
    void Update()
    {
        transform.position += -transform.right *  GetComponent<RocketStats>().speed * Time.deltaTime;
    }
}
