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
        if (Input.GetMouseButtonDown(0) &&  can_fire)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    IEnumerator ShootCoroutine()
    {
        can_fire = false;
        Shot();
        yield return new WaitForSeconds(0.2f);
        can_fire = true;
    }

    private void Shot()
    {
        Quaternion rot = Quaternion.Euler(transform.rotation.eulerAngles.z, transform.rotation.eulerAngles.y + 90f, transform.rotation.eulerAngles.x);
        GameObject rocket = Instantiate(rocketPrefab, firePoint.position, rot);
        GameObject rocket2 = Instantiate(rocketPrefab, firePoint2.position, rot);
        rocket.GetComponent<RocketStats>().owner = gameObject;
        rocket2.GetComponent<RocketStats>().owner = gameObject;
        rocket.GetComponent<RocketStats>().damage = GetComponent<UserStats>().dmg;
        rocket2.GetComponent<RocketStats>().damage = GetComponent<UserStats>().dmg;

    }
}
