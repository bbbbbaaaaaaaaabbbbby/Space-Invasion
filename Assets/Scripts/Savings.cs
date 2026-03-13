using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Savings : MonoBehaviour
{
    private string json;
    private string key;
    private string path;
    private SavingData _savingData;
    
    private void Awake()
    {
        Load();
        transform.position = new Vector3(_savingData.x, _savingData.y, _savingData.z);
        
    }

    private void OnApplicationQuit()
    {
        _savingData = new SavingData()
        {
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z,
        };
        Save();
    }
    
    private void Save()
    {
        key = "Amount.json";
        json = JsonConvert.SerializeObject(_savingData, Formatting.Indented);
        // JObject obj = JObject.Parse(json);
        path = Path.Combine(Application.persistentDataPath, key);
        using (StreamWriter writer = new(path))
        {
            writer.Write(json);
        }
        Debug.Log(path);
    }
    
    private void Load()
    {
        key = "Amount.json";
        path = Path.Combine(Application.persistentDataPath, key);
        using (StreamReader reader = new(path))
        {
            json = reader.ReadToEnd();
            Debug.Log(json);
            _savingData = JsonConvert.DeserializeObject<SavingData>(json);
        }
    }
    
}

public class SavingData
{
    public float x;
    public float y;
    public float z;
}
