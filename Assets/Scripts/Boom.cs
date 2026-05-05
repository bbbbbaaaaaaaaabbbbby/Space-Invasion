using UnityEngine;

public class Boom : MonoBehaviour
{
    public GameObject explosionPref;
    private GameObject explosionEffect;
    public float lifetime;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Colliding"))
        {
            Debug.Log("1");
            explosionEffect = Instantiate(explosionPref,  transform.position, transform.rotation);
            // int hp = collision.gameObject.GetComponent<PlanetLiving>()._hp;
            // hp -= GetComponent<RocketStats>().damage;
            Destroy(explosionEffect, lifetime);
            Destroy(transform.parent.gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            explosionEffect = Instantiate(explosionPref,  transform.position, transform.rotation);
            Destroy(explosionEffect, lifetime);
            Destroy(collision.gameObject);
            Destroy(transform.parent.gameObject);
        }
    }
}
