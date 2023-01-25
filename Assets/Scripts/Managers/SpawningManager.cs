using System.Collections;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class SpawningManager : MonoBehaviour
{
    [SerializeField] float timeBetweenSpawns;

    public static Vector2 despawnPosition = new Vector2(-4, -8);

    GameManager gameManager;
    Enemy.Pool enemyPool;
    Player player;
    PlayerSpawningPoint playerSpawningPoint;
    SpawningPoint[] spawningPoints;

    Coroutine spawningCoroutineObject;

    [Inject]
    public void Setup(GameManager gameManager, Enemy.Pool enemyPool, Player player)
    {
        this.gameManager = gameManager;
        this.enemyPool = enemyPool;
        this.player = player;
    }

    private void Awake()
    {
        spawningPoints = GetComponentsInChildren<SpawningPoint>();
        playerSpawningPoint = GetComponentInChildren<PlayerSpawningPoint>();
    }

    private void OnEnable()
    {
        gameManager.GameRunStarted += SpawnPlayer;
        gameManager.GameRunStarted += StartSpawningEnemies;
        gameManager.GameRunEnded += StopSpawningEnemies;
        Enemy.EnemyDied += DespawnEnemy;
    }

    private void OnDisable()
    {
        gameManager.GameRunStarted -= SpawnPlayer;
        gameManager.GameRunStarted -= StartSpawningEnemies;
        gameManager.GameRunEnded -= StopSpawningEnemies;
        Enemy.EnemyDied -= DespawnEnemy;
    }

    [ContextMenu("Spawn Player")]
    public void SpawnPlayer()
    {
        player.transform.position = playerSpawningPoint.transform.position;
        player.ResetStats();
    }

    [ContextMenu("Start Spawning Enemies")]
    public void StartSpawningEnemies()
    {
        spawningCoroutineObject = StartCoroutine(SpawningCoroutine());
    }

    [ContextMenu("Stop Spawning Enemies")]
    public void StopSpawningEnemies()
    {
        StopCoroutine(spawningCoroutineObject);
    }

    private IEnumerator SpawningCoroutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(timeBetweenSpawns);

            SpawnEnemy();
        }
    }

    [ContextMenu("Spawn Enemy")]
    public void SpawnEnemy()
    {
        var enemy = enemyPool.Spawn();
        enemy.ResetStats();
        enemy.transform.position = spawningPoints[GetRandomIndex()].transform.position;
        enemy.StartMoving();
    }

    public void DespawnEnemy(Enemy enemy)
    {
        enemy.transform.position = despawnPosition;
        enemyPool.Despawn(enemy);
        enemy.StopMoving();
    }

    private int GetRandomIndex()
    {
        return Random.Range(0, 5);
    }
}
