using UnityEngine;

public class RocketsFlying : MonoBehaviour
{
    public float speed;
    public GameObject explosionPref;
    private GameObject explosionEffect;
    public float lifetime;
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Colliding"))
        {
            explosionEffect = Instantiate(explosionPref,  transform.position, transform.rotation);
        }
        Destroy(explosionEffect, lifetime);
        Destroy(gameObject);
    }
}
