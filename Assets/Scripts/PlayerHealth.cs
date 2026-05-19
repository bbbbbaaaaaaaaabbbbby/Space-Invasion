using Mirror;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar] public int maxHealth = 100;
    [SyncVar(hook = nameof(OnHealthChanged))] public int currentHealth;

    public event System.Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage, GameObject attacker)
    {
        if (!isServer) return;

        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void OnHealthChanged(int oldVal, int newVal) { }

    [Server]
    void Die()
    {
        OnDeath?.Invoke();

        var gameState = GameObject.FindObjectOfType<NetworkGameState>();
        if (gameState != null)
        {
            gameState.AnnounceLoser(connectionToClient);
        }

        NetworkServer.Destroy(gameObject);
    }
}