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

    [Header("Stats")]
    public float fire_rate = 0.2f;

    private bool can_fire = true;
    private UserStats userStats;

    
    
    void Start()
    {
        userStats = GetComponent<UserStats>();
        if (userStats != null)
            fire_rate = userStats.fire_rate;
    }

    void Update()
    {
        // ✅ Только свой корабль стреляет по нажатию Space
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space) && can_fire)
        {
            // Отправляем команду на сервер
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

    // ✅ Выполняется на сервере — все клиенты увидят ракеты
    [Command]
    void CmdShoot()
    {
        Quaternion rot = transform.rotation * Quaternion.Euler(0, 90, 0);

        GameObject rocket1 = Instantiate(rocketPrefab, firePoint.position, rot);
        GameObject rocket2 = Instantiate(rocketPrefab, firePoint2.position, rot);

        // Спавним в сети
        NetworkServer.Spawn(rocket1);
        NetworkServer.Spawn(rocket2);

        // Передаём параметры
        SetupRocket(rocket1);
        SetupRocket(rocket2);

        // Автоудаление через 5 секунд
        StartCoroutine(DestroyLater(rocket1, 5f));
        StartCoroutine(DestroyLater(rocket2, 5f));
    }

    void SetupRocket(GameObject rocket)
    {
        RocketStats stats = rocket.GetComponent<RocketStats>();
        if (stats == null) return;

        stats.owner = gameObject;
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