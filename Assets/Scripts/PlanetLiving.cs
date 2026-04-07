using UnityEngine;

public class PlanetLiving : MonoBehaviour
{ 
    public int _hp = 100;

    private void Awake()
    {
        _hp = (int)(transform.localScale.x) * 8;
    }

    private void Update()
    {
        // Debug.Log(_hp);
        if (_hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Rocket"))
        {
            Debug.Log("1");
            _hp -= collision.gameObject.GetComponent<RocketStats>().damage;
        }
    }
    
}
