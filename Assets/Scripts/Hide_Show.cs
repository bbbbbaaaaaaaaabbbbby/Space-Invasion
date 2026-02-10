using UnityEngine;

public class Hide_Show : MonoBehaviour
{
    private bool showUp = true;
    public GameObject obj;
    public GameObject obj2;
    public void Hide_Or_Show()
    {
        obj.SetActive(showUp);
        showUp = !showUp;
        obj2.SetActive(showUp);
    }
}
