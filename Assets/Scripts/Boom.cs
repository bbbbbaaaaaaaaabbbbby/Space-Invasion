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
        }
        Destroy(explosionEffect, lifetime);
        Destroy(transform.parent.gameObject);
    }
}
