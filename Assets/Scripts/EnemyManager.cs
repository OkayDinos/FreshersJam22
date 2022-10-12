using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    List<GameObject> enemies = new List<GameObject>();

    public static EnemyManager singleton;

    WorldManager worldManager;

    float timer;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        worldManager = WorldManager.singleton;

        timer = 5;
    }

    public void SpawnEnemies(Vector3 _playerPos, int _amount)
    {
        for (int i = 0; i < _amount; i++)
        {
            float spawnPosX;

            if (Random.value > 0.5f)
            {
                spawnPosX = _playerPos.x + 20 + (Random.value * 50);
            }
            else
            {
                spawnPosX = _playerPos.x - 20 - (Random.value * 50);
            }
            
            SpawnEnemy(new Vector3(spawnPosX, 1, Random.Range(1, 2)));
        }
    }

    void SpawnEnemy(Vector3 _position)
    {
        GameObject enemy = Instantiate(enemyPrefab, _position, Quaternion.identity);

        enemies.Add(enemy);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameState.Playing) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 20;

            SpawnEnemies(worldManager.playerRef.transform.position, 1);
        }

        List<GameObject> enemiesToDelete = new List<GameObject>();

        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyController>().toDelete == true)
            {
                enemiesToDelete.Add(enemy);
            }
            else
            {
                if (enemy.transform.position.x > worldManager.playerRef.transform.position.x + 90)
                {
                    enemy.transform.position = new Vector3(worldManager.playerRef.transform.position.x - 80, 1, Random.Range(1, 2));
                }
                else if (enemy.transform.position.x < worldManager.playerRef.transform.position.x - 90)
                {
                    enemy.transform.position = new Vector3(worldManager.playerRef.transform.position.x + 80, 1, Random.Range(1, 2));
                }
            }
        }

        foreach (GameObject enemy in enemiesToDelete)
        {
            enemies.Remove(enemy);
            Destroy(enemy);

            SpawnEnemies(worldManager.playerRef.transform.position, 1);
        }
    }
}
