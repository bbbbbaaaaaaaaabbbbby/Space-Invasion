using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameFlowManager : MonoBehaviour
{
    [Header("Network")]
    public NetworkGameState networkState;

    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject lobbyPanel;

    [Header("Buttons")]
    public Button hostBtn;
    public Button joinBtn;
    public Button startBtn;

    [Header("Text (TMP)")]
    public TMP_Text statusText;

    void Start()
    {
        if (!ValidateUI()) return;
        
        if (networkState == null)
            networkState = FindObjectOfType<NetworkGameState>();

        hostBtn.onClick.AddListener(OnHost);
        joinBtn.onClick.AddListener(OnJoin);
        startBtn.onClick.AddListener(HandleStart);
        
        ResetUI();

        if (networkState != null)
        {
            networkState.OnGameStart += OnGameStarted;
            networkState.OnPlayerCountUpdate += OnPlayerCountUpdated;
        }
    }

    void OnDestroy()
    {
        if (networkState != null)
        {
            networkState.OnGameStart -= OnGameStarted;
            networkState.OnPlayerCountUpdate -= OnPlayerCountUpdated;
        }
    }

    void Update()
    {
        if (networkState == null) return;
        if (networkState.gameStarted) return;
        if (!NetworkServer.active) return;

        int current = NetworkManager.singleton ? NetworkManager.singleton.numPlayers : 0;
        if (current != networkState.playerCount)
        {
            networkState.UpdatePlayerCount(current);
            startBtn.interactable = (current >= 2);
        }
    }

    void OnPlayerCountUpdated(int count)
    {
        statusText.text = $"Игроков: {count}/2";
    }

    void OnGameStarted()
    {
        Time.timeScale = 1f;
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        EnableLocalPlayer();
        Debug.Log("✅ Матч начался!");
    }

    void OnHost()
    {
        if (NetworkManager.singleton == null) return;
        NetworkManager.singleton.StartHost();
        GoToLobby();
    }

    void OnJoin()
    {
        if (NetworkManager.singleton == null) return;
        NetworkManager.singleton.StartClient();
        GoToLobby();
    }

    void HandleStart()
    {
        if (networkState != null && NetworkServer.active)
        {
            networkState.StartGame();
        }
    }

    void GoToLobby()
    {
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        statusText.text = "Ожидание игроков...";
    }

    void ResetUI()
    {
        menuPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        Time.timeScale = 0f;
        startBtn.interactable = false;
        statusText.text = "Подключение: 0/2";
    }

    void EnableLocalPlayer()
    {
        // ✅ ИСПРАВЛЕНО: Ищем ВСЕХ PlayerController и включаем локального
        var players = FindObjectsOfType<PlayerController>();
        foreach (var pc in players)
        {
            if (pc.isLocalPlayer)
            {
                pc.EnableControl();
                Debug.Log("🎮 [GFM] Управление включено для локального игрока");
            }
        }
    }

    bool ValidateUI()
    {
        bool ok = menuPanel && lobbyPanel && statusText && hostBtn && joinBtn && startBtn;
        if (!ok) Debug.LogError("❌ GameFlowManager: Проверь привязки UI в Inspector!");
        return ok;
    }
}