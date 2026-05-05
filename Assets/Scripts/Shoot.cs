using System.Collections;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject rocketPrefab;
    public Transform firePoint;
    public Transform firePoint2;
    private bool can_fire = true;
    public float fire_rate;

    void Start()
    {
        fire_rate = GetComponent<UserStats>().fire_rate;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) &&  can_fire)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    IEnumerator ShootCoroutine()
    {
        can_fire = false;
        Shot();
        yield return new WaitForSeconds(fire_rate);
        can_fire = true;
    }

    private void Shot()
    {
        // ✅ Берём поворот корабля как есть (ракета летит туда же, куда смотрит корабль)
        Quaternion rot = transform.rotation * Quaternion.Euler(0, 90, 0);
        
        GameObject rocket = Instantiate(rocketPrefab, firePoint.position, rot);
        GameObject rocket2 = Instantiate(rocketPrefab, firePoint2.position, rot);
        
        rocket.GetComponent<RocketStats>().owner = gameObject;
        rocket2.GetComponent<RocketStats>().owner = gameObject;
        rocket.GetComponent<RocketStats>().damage = GetComponent<UserStats>().dmg;
        rocket2.GetComponent<RocketStats>().damage = GetComponent<UserStats>().dmg;
    }
}
