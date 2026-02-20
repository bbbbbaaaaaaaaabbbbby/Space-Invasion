using UnityEngine;
using UnityEngine.SceneManagement;

public class Switch_Scenes : MonoBehaviour
{
    public int sceneNum;
    public void Switch()
    {
        SceneManager.LoadScene(sceneNum);
    }
}
