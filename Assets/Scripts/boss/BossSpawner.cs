using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;       // Prefab przeciwnika
    public Transform spawnPoint;         // Miejsce, gdzie przeciwnik się pojawi
    public float spawnInterval = 5f;     // Czas między kolejnymi przeciwnikami

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private System.Collections.IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}