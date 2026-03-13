using System.Collections;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject rocketPrefab;
    public Transform firePoint;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    IEnumerator ShootCoroutine()
    {
        Shot();
        yield return new WaitForSeconds(5f);
    }

    private void Shot()
    {
        Quaternion rot = Quaternion.Euler(transform.rotation.eulerAngles.z, transform.rotation.eulerAngles.y + 90f, transform.rotation.eulerAngles.x);
        GameObject rocket = Instantiate(rocketPrefab, firePoint.position, rot);
    }
}
