using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    public float spawnRate = 3f;
    public int maxEnemies = 5;

    public Transform[] spawnPoints;
    public Transform player;

    public float spawnDuration = 20f;

    private int currentEnemies = 0;
    private float timer = 0f;
    private bool canSpawn = true;

    void Start()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        currentEnemies = enemies.Length;

        foreach (Enemy e in enemies)
        {
            e.spawner = this;
            e.player = player;
        }

        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnRate);
    }

    void Update()
    {
        if (!canSpawn) return;

        timer += Time.deltaTime;

        if (timer >= spawnDuration)
        {
            canSpawn = false;
            CancelInvoke(nameof(SpawnEnemy));
            Debug.Log("⛔ Spawn detenido");
        }
    }

    void SpawnEnemy()
    {
        if (!canSpawn) return;
        if (currentEnemies >= maxEnemies) return;

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No hay spawn points asignados");
            return;
        }

        int index = Random.Range(0, spawnPoints.Length);

        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPoints[index].position,
            Quaternion.identity
        );

        currentEnemies++;

        Enemy e = enemy.GetComponent<Enemy>();

        if (e != null)
        {
            e.spawner = this;
            e.player = player;
        }
    }

    public void EnemyDied()
    {
        currentEnemies--;

        if (currentEnemies < 0)
            currentEnemies = 0;
    }
}