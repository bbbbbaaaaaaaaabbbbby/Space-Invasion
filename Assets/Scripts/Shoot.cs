using System.Collections;
using Mirror;
using UnityEngine;

public class Shoot : NetworkBehaviour
{
    [Header("Prefabs")]
    public GameObject rocketPrefab;

    [Header("Fire Points")]
    public Transform firePoint;
    public Transform firePoint2;

    [Header("Spawn Settings")]
    public float spawnOffset = 3f; // ✅ Ракета спавнится ВПЕРЕДИ корабля

    [Header("Stats")]
    public float fire_rate = 0.2f;

    private bool can_fire = true;
    private UserStats userStats;
    public KeyCode shootButton;

    void Start()
    {
        userStats = GetComponent<UserStats>();
        if (userStats != null)
            fire_rate = userStats.fire_rate;
    }

    void FixedUpdate()
    {
        shootButton = GetComponent<UserStats>().shootButton;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(shootButton) && can_fire)
        {
            CmdShoot();
            StartCoroutine(ShootCooldown());
        }
    }

    IEnumerator ShootCooldown()
    {
        can_fire = false;
        yield return new WaitForSeconds(fire_rate);
        can_fire = true;
    }

    [Command]
    void CmdShoot()
    {
        // ✅ Спавним ВПЕРЕДИ точки огня, а не внутри корабля
        Vector3 spawnPos1 = firePoint.position + firePoint.forward * spawnOffset;
        Vector3 spawnPos2 = firePoint2.position + firePoint2.forward * spawnOffset;

        Quaternion rot = transform.rotation * Quaternion.Euler(0, 90, 0);

        GameObject rocket1 = Instantiate(rocketPrefab, spawnPos1, rot);
        GameObject rocket2 = Instantiate(rocketPrefab, spawnPos2, rot);

        NetworkServer.Spawn(rocket1);
        NetworkServer.Spawn(rocket2);

        // ✅ Передаём owner — ВАЖНО для игнорирования коллизии
        SetupRocket(rocket1);
        SetupRocket(rocket2);

        // Удаляем через 5 секунд
        StartCoroutine(DestroyLater(rocket1, 5f));
        StartCoroutine(DestroyLater(rocket2, 5f));
    }

    void SetupRocket(GameObject rocket)
    {
        RocketStats stats = rocket.GetComponent<RocketStats>();
        if (stats == null) return;

        stats.owner = gameObject; // ✅ Кто выстрелил
        if (userStats != null)
            stats.damage = userStats.dmg;
    }

    IEnumerator DestroyLater(GameObject rocket, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rocket != null)
            NetworkServer.Destroy(rocket);
    }
}