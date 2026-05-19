using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [Header("Network")]
    public NetworkGameState networkState;
    public TMP_InputField ipInputField;
    public TMP_InputField portInputField;

    [Header("UI Panels")]
    public GameObject menuPanel;
    public GameObject lobbyPanel;
    public GameObject playerDisconnectedPanel; // ✅ "Противник вышел — победа"
    public GameObject hostDisconnectedPanel;   // ✅ "Хост отключился"

    [Header("Buttons")]
    public Button hostBtn;
    public Button joinBtn;
    public Button startBtn;
    public Button playerDisconnectBtn; // Кнопка "В меню" на панели победы
    public Button hostDisconnectBtn;   // Кнопка "В меню" на панели хоста

    [Header("Text (TMP)")]
    public TMP_Text statusText;

    private bool wasConnected = false;

    void Start()
    {
        if (!ValidateUI()) return;
        
        if (networkState == null)
            networkState = FindObjectOfType<NetworkGameState>();

        hostBtn.onClick.AddListener(OnHost);
        joinBtn.onClick.AddListener(OnJoin);
        startBtn.onClick.AddListener(HandleStart);
        playerDisconnectBtn?.onClick.AddListener(ReturnToMenu);
        hostDisconnectBtn?.onClick.AddListener(ReturnToMenu);
        
        if (ipInputField != null) ipInputField.text = "localhost";
        if (portInputField != null) portInputField.text = "7777";

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
        CheckConnection();

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

    // =================== ПРОВЕРКА СОЕДИНЕНИЯ ===================

    void CheckConnection()
    {
        // Был подключён, но соединение пропало
        if (wasConnected && !NetworkClient.isConnected && !NetworkClient.isConnecting)
        {
            wasConnected = false;
            
            // Определяем: я хост или клиент?
            if (NetworkServer.active)
            {
                // Я хост — значит клиент отключился
                OnPlayerDisconnected();
            }
            else
            {
                // Я клиент — значит хост упал
                OnHostDisconnected();
            }
        }

        if (NetworkClient.isConnected)
            wasConnected = true;
    }

    // =================== ОТКЛЮЧЕНИЕ ИГРОКА (хост видит) ===================

    void OnPlayerDisconnected()
    {
        Debug.Log("👤 Противник вышел!");
        
        Time.timeScale = 0f;
        
        menuPanel?.SetActive(false);
        lobbyPanel?.SetActive(false);
        hostDisconnectedPanel?.SetActive(false);
        playerDisconnectedPanel?.SetActive(true);
        
        statusText.text = "Противник вышел — победа!";
    }

    // =================== ОТКЛЮЧЕНИЕ ХОСТА (клиент видит) ===================

    void OnHostDisconnected()
    {
        Debug.Log("❌ Хост отключился!");
        
        if (NetworkManager.singleton != null)
            NetworkManager.singleton.StopClient();

        Time.timeScale = 0f;
        
        menuPanel?.SetActive(false);
        lobbyPanel?.SetActive(false);
        playerDisconnectedPanel?.SetActive(false);
        hostDisconnectedPanel?.SetActive(true);
        
        statusText.text = "Хост отключился";
    }

    // =================== ВОЗВРАТ В МЕНЮ ===================

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;

        if (NetworkManager.singleton != null)
        {
            if (NetworkServer.active)
                NetworkManager.singleton.StopHost();
            else
                NetworkManager.singleton.StopClient();
        }

        StartCoroutine(LoadMenuScene());
    }

    IEnumerator LoadMenuScene()
    {
        yield return null;
        SceneManager.LoadScene(0);
    }

    // =================== ОСТАЛЬНОЕ ===================

    void OnHost()
    {
        if (NetworkManager.singleton == null)
        {
            Debug.LogError("❌ NetworkManager не найден!");
            return;
        }

        ApplyPort();
        NetworkManager.singleton.StartHost();
        GoToLobby();
        
        var transport = Transport.active;
        if (transport != null)
            statusText.text = $"Хост: {NetworkManager.singleton.networkAddress}:{GetPort()}";
    }

    void OnJoin()
    {
        if (NetworkManager.singleton == null)
        {
            Debug.LogError("❌ NetworkManager не найден!");
            return;
        }

        string ip = (ipInputField != null && !string.IsNullOrEmpty(ipInputField.text)) 
            ? ipInputField.text 
            : "localhost";

        NetworkManager.singleton.networkAddress = ip;
        ApplyPort();

        if (NetworkClient.isConnected || NetworkClient.isConnecting)
            NetworkManager.singleton.StopClient();

        StartCoroutine(SafeConnect());
    }

    IEnumerator SafeConnect()
    {
        yield return null;

        if (NetworkManager.singleton == null) yield break;

        try
        {
            NetworkManager.singleton.StartClient();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Ошибка подключения: {ex.Message}");
            statusText.text = "Ошибка подключения!";
            yield break;
        }

        GoToLobby();
    }

    void HandleStart()
    {
        if (networkState != null && NetworkServer.active)
            networkState.StartGame();
    }

    void OnGameStarted()
    {
        Time.timeScale = 1f;
        menuPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        EnableLocalPlayer();
        Debug.Log("✅ Матч начался!");
    }

    void OnPlayerCountUpdated(int count)
    {
        statusText.text = $"Игроков: {count}/2";
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
        playerDisconnectedPanel?.SetActive(false);
        hostDisconnectedPanel?.SetActive(false);
        Time.timeScale = 0f;
        startBtn.interactable = false;
        statusText.text = "Введите IP:Порт и нажмите Join";
    }

    void EnableLocalPlayer()
    {
        if (NetworkClient.localPlayer != null)
        {
            var pc = NetworkClient.localPlayer.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.EnableControl();
                return;
            }
        }

        var allPlayers = FindObjectsOfType<PlayerController>();
        foreach (var pc in allPlayers)
        {
            if (pc.isLocalPlayer)
            {
                pc.EnableControl();
                break;
            }
        }
    }

    void ApplyPort()
    {
        if (portInputField == null) return;
        
        if (ushort.TryParse(portInputField.text, out ushort port))
        {
            var transport = Transport.active;
            
            if (transport is TelepathyTransport telepathy)
                telepathy.port = port;
            else if (transport is kcp2k.KcpTransport kcp)
                kcp.Port = port;
            else if (transport is Mirror.SimpleWeb.SimpleWebTransport swt)
                swt.port = port;
        }
    }

    ushort GetPort()
    {
        var transport = Transport.active;
        
        if (transport is TelepathyTransport telepathy)
            return telepathy.port;
        else if (transport is kcp2k.KcpTransport kcp)
            return kcp.Port;
        else if (transport is Mirror.SimpleWeb.SimpleWebTransport swt)
            return swt.port;
        
        return 7777;
    }

    bool ValidateUI()
    {
        bool ok = menuPanel && lobbyPanel && statusText && hostBtn && joinBtn && startBtn;
        if (!ok) Debug.LogError("❌ GameFlowManager: Проверь привязки UI в Inspector!");
        return ok;
    }
    
    public void ShowPlayerDisconnectedPanel()
    {
        Debug.Log("👤 Противник вышел");
    
        Time.timeScale = 0f;
    
        menuPanel?.SetActive(false);
        lobbyPanel?.SetActive(false);
        hostDisconnectedPanel?.SetActive(false);
        playerDisconnectedPanel?.SetActive(true);
    
        statusText.text = "Противник вышел";
    }
}