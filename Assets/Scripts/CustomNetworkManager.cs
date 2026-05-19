using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    private int nextSpawnIndex = 0;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Transform spawn = spawnPoints.Length > 0 
            ? spawnPoints[nextSpawnIndex % spawnPoints.Length] 
            : null;

        Vector3 pos = spawn != null ? spawn.position : Vector3.zero;
        Quaternion rot = spawn != null ? spawn.rotation : Quaternion.identity;

        GameObject player = Instantiate(playerPrefab, pos, rot);
        NetworkServer.AddPlayerForConnection(conn, player);

        nextSpawnIndex++;
        Debug.Log($"🚀 Игрок заспавнен: {pos}");
    }

    // ✅ Клиент отключился — хост показывает панель "Противник вышел"
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        var gameState = FindObjectOfType<NetworkGameState>();
        bool gameWasRunning = gameState != null && gameState.gameStarted;

        // Базовый метод удаляет игрока
        base.OnServerDisconnect(conn);

        // Если игра шла — показываем хосту панель отключения игрока
        if (gameWasRunning)
        {
            var gameFlow = FindObjectOfType<GameFlowManager>();
            if (gameFlow != null)
            {
                gameFlow.ShowPlayerDisconnectedPanel();
            }
        }
    }
}