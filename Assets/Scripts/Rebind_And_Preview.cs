using UnityEngine;
using TMPro;

public class Rebind_And_Preview : MonoBehaviour
{
    private bool showUp = false;
    private bool isSwitchingShooting = false;
    public KeyCode shootingKey;
    public GameObject settings_Panel;
    public TMP_Text buttonPreview;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            showUp = !showUp;
            if (showUp) Time.timeScale = 0f;
            else Time.timeScale = 1f;
            settings_Panel.SetActive(showUp);
        }
        SwitchShooting();
        buttonPreview.text = shootingKey.ToString();
    }

    public void SwitchShooting()
    {
        if (isSwitchingShooting)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    isSwitchingShooting = false;
                    shootingKey = key;
                    break;
                }
            }
        }
        
    }

    public void ShootingSwitch()
    {
        isSwitchingShooting = true;
    }
}
