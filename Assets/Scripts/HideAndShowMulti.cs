using UnityEngine;

public class HideAndShowMulti : MonoBehaviour
{
    private bool showUp = true;
    public GameObject obj;
    public GameObject[] obj2;
    public void Hide_Or_Show()
    {
        obj.SetActive(showUp);
        showUp = !showUp;
        for (int i = 0; i < obj2.Length; i++)
        {
            obj2[i].SetActive(showUp);
        }
    }
}