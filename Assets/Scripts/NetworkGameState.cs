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
    
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    
        // Если игра уже началась — сразу включаем управление
        var gameState = FindObjectOfType<NetworkGameState>();
        if (gameState != null && gameState.gameStarted)
        {
            var pc = GetComponent<PlayerController>();
            if (pc != null) pc.EnableControl();
        }
    }
}