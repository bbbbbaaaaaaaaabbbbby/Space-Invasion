using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints; // Перетащи 2 точки в инспекторе

    private int nextSpawnIndex = 0;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Выбираем точку спавна по очереди
        Transform spawn = spawnPoints.Length > 0 
            ? spawnPoints[nextSpawnIndex % spawnPoints.Length] 
            : null;

        Vector3 pos = spawn != null ? spawn.position : Vector3.zero;
        Quaternion rot = spawn != null ? spawn.rotation : Quaternion.identity;

        GameObject player = Instantiate(playerPrefab, pos, rot);
        NetworkServer.AddPlayerForConnection(conn, player);

        nextSpawnIndex++;
        
        Debug.Log($"🚀 Игрок заспавнен на точке {nextSpawnIndex}: {pos}");
    }
}