using Mirror;
using UnityEngine;
using System;

public class NetworkGameState : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnGameStartedChanged))]
    public bool gameStarted = false;

    [SyncVar(hook = nameof(OnPlayerCountChanged))]
    public int playerCount = 0;

    public event Action OnGameStart;
    public event Action<int> OnPlayerCountUpdate;

    void OnGameStartedChanged(bool oldVal, bool newVal)
    {
        if (newVal) OnGameStart?.Invoke();
    }

    void OnPlayerCountChanged(int oldVal, int newVal)
    {
        OnPlayerCountUpdate?.Invoke(newVal);
    }

    [Server]
    public void StartGame()
    {
        if (!gameStarted && playerCount >= 2)
            gameStarted = true;
    }

    [Server]
    public void UpdatePlayerCount(int count)
    {
        playerCount = count;
    }

    // =================== КОНЕЦ ИГРЫ ===================

    [Server]
    public void AnnounceLoser(NetworkConnectionToClient loserConn)
    {
        // Проигравшему — поражение
        TargetShowLose(loserConn);

        // Победителю — победа
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn != loserConn && conn.isReady)
            {
                TargetShowWin(conn);
            }
        }
    }

    [Server]
    public void AnnounceWinnerByDisconnect(NetworkConnectionToClient disconnectedConn)
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn != disconnectedConn && conn.isReady)
            {
                TargetShowWin(conn);
            }
        }
    }

    [TargetRpc]
    void TargetShowLose(NetworkConnection target)
    {
        GameUIManager.Instance?.ShowLose();
    }

    [TargetRpc]
    void TargetShowWin(NetworkConnection target)
    {
        GameUIManager.Instance?.ShowWin();
    }
}