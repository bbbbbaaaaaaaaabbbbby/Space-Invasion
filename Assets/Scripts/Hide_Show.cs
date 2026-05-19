using UnityEngine;

public class Hide_Show : MonoBehaviour
{
    public GameObject obj;
    public GameObject obj2;
    public GameObject obj3;
    public GameObject last_scene;
    public void Hide_Or_ShowFirst()
    {
        obj.SetActive(!obj.activeSelf!);
        obj2.SetActive(!obj2.activeSelf);
        last_scene = obj2;
    }
	public void Hide_Or_ShowSecond()
    {
        obj.SetActive(!obj.activeSelf);
        obj3.SetActive(!obj3.activeSelf);
        last_scene = obj3;
    }

    public void SwitchAndKill()
    {
        obj.SetActive(!obj.activeSelf);
        obj2.SetActive(!obj2.activeSelf);
        obj3.SetActive(!obj3.activeSelf);
        last_scene.SetActive(!last_scene.activeSelf);
        Destroy(obj);
    }


    public void HideSpecial()
    {
        obj.SetActive(!obj.activeSelf);
        obj3.SetActive(!obj3.activeSelf);
        obj2.SetActive(!obj2.activeSelf);
    }
    
	public void Hide_Or_ShowLastScene()
    {
        obj.SetActive(!obj.activeSelf);
        last_scene.SetActive(!last_scene.activeSelf);
    }
}
