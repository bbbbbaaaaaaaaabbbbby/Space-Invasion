using UnityEngine;
using System;

public class Spawning : MonoBehaviour
{
    System.Random rnd = new System.Random();
    public GameObject prefab;
    public GameObject[] spawnPoints;
    private GameObject spawned;
    private float rndValue;
    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Debug.Log(spawnPoints[i].name);
            rndValue = (float)(rnd.Next(100, 10000) / 1000);
            Debug.Log(rndValue);
            spawned = Instantiate(prefab, spawnPoints[i].transform.position, Quaternion.identity);
            Debug.Log(spawned.name);
            spawned.transform.localScale = new Vector3(spawned.transform.localScale.x * rndValue, spawned.transform.localScale.y * rndValue, spawned.transform.localScale.z * rndValue);
            Debug.Log(spawned.name);
        }
    }
}
