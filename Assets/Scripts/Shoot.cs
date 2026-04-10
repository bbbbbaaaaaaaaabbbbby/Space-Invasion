using System.Collections;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject rocketPrefab;
    public Transform firePoint;
    public Transform firePoint2;
    private bool CanFire = true;
    void Update()
    {
        if (Input.GetMouseButtonDown(0) &&  CanFire)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    IEnumerator ShootCoroutine()
    {
        CanFire = false;
        Shot();
        yield return new WaitForSeconds(5f);
        CanFire = true;
    }

    private void Shot()
    {
        Quaternion rot = Quaternion.Euler(transform.rotation.eulerAngles.z, transform.rotation.eulerAngles.y + 90f, transform.rotation.eulerAngles.x);
        GameObject rocket = Instantiate(rocketPrefab, firePoint.position, rot);
        GameObject rocket2 = Instantiate(rocketPrefab, firePoint2.position, rot);
    }
}
