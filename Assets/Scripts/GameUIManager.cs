using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    // public GameObject hudPanel; // Обычный HUD (здоровье и т.д.)

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        HideAllPanels();
        // hudPanel?.SetActive(true);
    }

    public void ShowWinPanel()
    {
        Debug.Log("Win Panel");
        HideAllPanels();
        winPanel?.SetActive(true);
        Time.timeScale = 0f; // Останавливаем игру
    }

    public void ShowLosePanel()
    {
        Debug.Log("Lose Panel");
        HideAllPanels();
        losePanel?.SetActive(true);
        Time.timeScale = 0f;
    }

    void HideAllPanels()
    {
        winPanel?.SetActive(false);
        losePanel?.SetActive(false);
        // hudPanel?.SetActive(false);
    }
}