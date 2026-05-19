using UnityEngine;

public class PlanetLiving : MonoBehaviour
{ 
    public int _hp = 100;
    public GameObject boomPrefab;
    private int _value;

    private void Awake()
    {
        _hp = (int)(transform.localScale.x) * 8;
        _value = _hp / 20;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(200, gameObject);
        }
        if (collision.gameObject.CompareTag("Rocket"))
        {
            Debug.Log(_hp);
            Debug.Log("1");
            GameObject rocket = collision.gameObject.transform.parent.gameObject;
            _hp -= rocket.GetComponent<RocketStats>().damage;
            
            if (_hp <= 0)
            {
                rocket.GetComponent<RocketStats>().owner.GetComponent<UserStats>().points += _value;
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }
    
}
