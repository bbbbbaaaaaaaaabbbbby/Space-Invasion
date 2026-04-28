using UnityEngine;

public class HideAndShowObject : MonoBehaviour
{
    public GameObject obj;
    public KeyCode keyCode;

    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
