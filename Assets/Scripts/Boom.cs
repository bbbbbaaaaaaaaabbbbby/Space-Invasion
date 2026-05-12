using UnityEngine;
using Mirror;

public class Boom : MonoBehaviour
{
    public GameObject explosionPref;
    private GameObject explosionEffect;
    public float lifetime;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[BOOM] Столкновение с: {collision.gameObject.name}");

        // ✅ Только сервер обрабатывает урон и удаляет ракету
        if (NetworkServer.active)
        {
            // Берём урон и владельца из родительской ракеты
            RocketStats rocketStats = transform.parent?.GetComponent<RocketStats>();
            int damage = (rocketStats != null) ? rocketStats.damage : 10;
            GameObject owner = (rocketStats != null) ? rocketStats.owner : null;

            // ✅ Ищем здоровье у игрока (включая родителей, т.к. коллайдер может быть на дочернем объекте корабля)
            PlayerHealth health = collision.gameObject.GetComponentInParent<PlayerHealth>();
            if (health != null)
            {
                Debug.Log($"[BOOM] Наношу урон {damage} игроку");
                health.TakeDamage(damage, owner);
            }

            // Эффект взрыва (не сетевой — просто партиклы)
            if (explosionPref != null)
            {
                explosionEffect = Instantiate(explosionPref, transform.position, transform.rotation);
                Destroy(explosionEffect, lifetime);
            }

            // ✅ Уничтожаем ракету сетевым способом (видно всем клиентам)
            if (transform.parent != null)
                NetworkServer.Destroy(transform.parent.gameObject);
            else
                NetworkServer.Destroy(gameObject);
        }
    }
}