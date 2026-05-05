using UnityEngine;
using System;
using System.Collections;

public class Spawning : MonoBehaviour
{
    [Header("Planet Settings")]
    public GameObject prefab;
    public float minScale = 1f;
    public float maxScale = 10f;
    
    [Header("Spawn Settings")]
    public float spawnInterval = 5f;      // Интервал между спавнами
    public int maxPlanets = 20;           // Максимум планет на сцене
    public bool spawnOnStart = true;      // Спавнить ли сразу при старте
    
    private GameObject[] spawnPoints;
    private System.Random rnd = new System.Random();
    private int currentPlanetCount = 0;

    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("⚠️ Spawning: Не найдены точки спавна с тегом 'SpawnPoint'");
            return;
        }

        // Первичный спавн при старте
        if (spawnOnStart)
        {
            SpawnAllPoints();
        }

        // Запускаем периодический спавн
        StartCoroutine(SpawnLoop());
    }

    // Первичный спавн на всех точках
    void SpawnAllPoints()
    {
        foreach (var point in spawnPoints)
        {
            SpawnPlanetAt(point.transform.position);
        }
    }

    // Корутина периодического спавна
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentPlanetCount < maxPlanets)
            {
                // Случайная точка спавна
                Vector3 spawnPos = GetRandomSpawnPosition();
                SpawnPlanetAt(spawnPos);
            }
            else
            {
                Debug.Log("🛑 Достигнут лимит планет: " + maxPlanets);
            }
        }
    }

    // Спавн одной планеты
    void SpawnPlanetAt(Vector3 position)
    {
        float scale = UnityEngine.Random.Range(minScale, maxScale); // Или rnd.Next для System.Random
        
        GameObject planet = Instantiate(prefab, position, Quaternion.identity);
        planet.transform.localScale = Vector3.one * scale;
        
        // Случайный поворот для разнообразия
        planet.transform.rotation = UnityEngine.Random.rotation;
        
        // Добавляем компонент для отслеживания уничтожения
        planet.AddComponent<PlanetTracker>().spawner = this;
        
        currentPlanetCount++;
        
        Debug.Log($"🪐 Планета заспавнена: поз={position}, размер={scale:F1}, всего={currentPlanetCount}");
    }

    // Случайная позиция из доступных точек
    Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints.Length == 0) return Vector3.zero;
        
        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        return spawnPoints[index].transform.position;
    }

    // Вызывается когда планета уничтожена
    public void OnPlanetDestroyed()
    {
        currentPlanetCount--;
        Debug.Log($"💥 Планета уничтожена. Осталось: {currentPlanetCount}");
    }
}

// Вспомогательный компонент на планете
public class PlanetTracker : MonoBehaviour
{
    [HideInInspector] public Spawning spawner;

    void OnDestroy()
    {
        if (spawner != null)
            spawner.OnPlanetDestroyed();
    }
}