using Mirror;
using UnityEngine;
using System.Linq;

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
            Die(attacker);
        }
    }

    void OnHealthChanged(int oldVal, int newVal)
    {
        // Обновление UI здоровья
    }

    [Server]
    void Die(GameObject killer)
    {
        OnDeath?.Invoke();
        
        // Просто передаём netId
        uint deadNetId = netId;
        uint killerNetId = killer != null ? killer.GetComponent<NetworkIdentity>().netId : 0;
        
        RpcPlayerDied(deadNetId, killerNetId);
    }

    [ClientRpc]
    void RpcPlayerDied(uint deadNetId, uint killerNetId)
    {
        // Находим NetworkIdentity по netId через NetworkServer (если сервер) или NetworkClient
        NetworkIdentity deadIdentity = GetNetworkIdentity(deadNetId);
        NetworkIdentity killerIdentity = GetNetworkIdentity(killerNetId);

        // Проверяем, это мы или нет
        if (deadIdentity != null && deadIdentity.isLocalPlayer)
        {
            ShowLoseScreen();
        }
        else if (killerIdentity != null && killerIdentity.isLocalPlayer)
        {
            ShowWinScreen();
        }
    }

    // ✅ Исправлено: поиск NetworkIdentity через NetworkClient/NetworkServer
    NetworkIdentity GetNetworkIdentity(uint netId)
    {
        // На клиенте ищем через NetworkClient.spawned
        if (NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity clientIdentity))
            return clientIdentity;
        
        // На сервере ищем через NetworkServer.spawned
        if (NetworkServer.spawned.TryGetValue(netId, out NetworkIdentity serverIdentity))
            return serverIdentity;
        
        return null;
    }

    void ShowLoseScreen()
    {
        Debug.Log("Я проиграл?");
        GameUIManager.Instance?.ShowLosePanel();
        Debug.Log("💀 Ты проиграл!");
    }

    void ShowWinScreen()
    {
        Debug.Log("Я непогрешим");
        GameUIManager.Instance?.ShowWinPanel();
        Debug.Log("🏆 Ты победил!");
    }
}