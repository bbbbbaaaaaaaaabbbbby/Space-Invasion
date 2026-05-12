using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameFlowManager : MonoBehaviour
{
    [Header("Network")]
    public NetworkGameState networkState;
    public TMP_InputField ipInputField;   // Поле ввода IP
    public TMP_InputField portInputField; // ✅ Поле ввода порта

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
        
        // Заполняем значения по умолчанию
        if (ipInputField != null)
            ipInputField.text = "localhost";
        
        if (portInputField != null)
            portInputField.text = "7777";

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
        if (NetworkManager.singleton == null)
        {
            Debug.LogError("❌ NetworkManager не найден!");
            return;
        }

        // ✅ Применяем порт из поля ввода
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

        // ✅ Берём IP и порт из полей ввода
        string ip = (ipInputField != null && !string.IsNullOrEmpty(ipInputField.text)) 
            ? ipInputField.text 
            : "localhost";

        NetworkManager.singleton.networkAddress = ip;
        ApplyPort();

        if (NetworkClient.isConnected || NetworkClient.isConnecting)
        {
            NetworkManager.singleton.StopClient();
        }

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
        {
            networkState.StartGame();
        }
    }

    // ✅ Применяет порт из поля ввода к транспорту
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
            
            Debug.Log($"🔌 Порт установлен: {port}");
        }
        else
        {
            Debug.LogWarning("⚠️ Неверный порт, используется 7777");
        }
    }

    // ✅ Получает текущий порт из транспорта
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
        statusText.text = "Введите IP:Порт и нажмите Join";
    }

    void EnableLocalPlayer()
    {
        // ✅ Исправлено: ищем через NetworkClient, а не случайный FindObjectOfType
        if (NetworkClient.localPlayer != null)
        {
            var pc = NetworkClient.localPlayer.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.EnableControl();
                Debug.Log("🎮 Управление включено для локального игрока (хост/клиент)");
                return;
            }
        }

        // Fallback: если по какой-то причине localPlayer не найден
        var allPlayers = FindObjectsOfType<PlayerController>();
        foreach (var pc in allPlayers)
        {
            if (pc.isLocalPlayer)
            {
                pc.EnableControl();
                Debug.Log("🎮 Управление включено (fallback)");
                break;
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