using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] List<WorldTile> prefabTilesList = new List<WorldTile>();

    List<PlacedWorldTile> tilesList = new List<PlacedWorldTile>();

    int worldSize;

    float worldCentre;

    public GameObject playerRef;

    EnemyManager enemyManager;

    [SerializeField] GameObject platform;

    public static WorldManager singleton;

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

    // Start is called before the first frame update
    void Start()
    {
        enemyManager = EnemyManager.singleton;

        GenerateWorld();

        enemyManager.SpawnEnemies(playerRef.transform.position, 10);
    }

    void GenerateWorld()
    {
        worldSize = 0;

        int targetWorldSize = 20;
        
        Vector3 nextTileSpawnPos = new Vector3(0, 0, 0);

        while (targetWorldSize > worldSize)
        {
            int random = Random.Range(0, prefabTilesList.Count);
            Vector3 currentTileSpawnPos = nextTileSpawnPos + new Vector3(((prefabTilesList[random].tileLength - 1) * 10) / 2, 0, 4f);

            Quaternion currentTileSpawnRot = Quaternion.identity;

            if (random == 2)
            {
                currentTileSpawnRot = Quaternion.Euler(0, 180, 0);
            }

            GameObject tile = Instantiate(prefabTilesList[random].prefabTile, currentTileSpawnPos, currentTileSpawnRot);
            tilesList.Add(new PlacedWorldTile { tile = tile, length = prefabTilesList[random].tileLength });
            nextTileSpawnPos.x += 10 * prefabTilesList[random].tileLength;

            worldSize += prefabTilesList[random].tileLength;
        }

        worldCentre = (worldSize * 10) / 2;

        playerRef.transform.SetPositionAndRotation(new Vector3(worldCentre, 1, 1), Quaternion.identity);

        playerRef.GetComponent<BaseCharacterController>().Begin(worldCentre);
    }

    void MoveTile(bool _rightDirection) // false = left direction
    {
        if (_rightDirection)
        {
            PlacedWorldTile movedTile = tilesList[0];
            tilesList.RemoveAt(0);

            Vector3 endPosition = tilesList[tilesList.Count - 1].tile.transform.position + new Vector3(((tilesList[tilesList.Count - 1].length - 1) * 10) / 2, 0, 0);

            movedTile.tile.transform.position = new Vector3(endPosition.x + 10 + (((movedTile.length - 1) * 10) / 2), 0, 4);

            tilesList.Add(movedTile);
        }
        else
        {
            PlacedWorldTile movedTile = tilesList[tilesList.Count - 1];
            tilesList.RemoveAt(tilesList.Count - 1);

            Vector3 endPosition = tilesList[0].tile.transform.position - new Vector3(((tilesList[0].length - 1) * 10) / 2, 0, 0);

            movedTile.tile.transform.position = new Vector3(endPosition.x - 10 - (((movedTile.length - 1) * 10) / 2), 0, 4);

            tilesList.Insert(0, movedTile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        worldCentre = playerRef.transform.position.x;

        platform.transform.position = new Vector3(worldCentre, 0, 0);

        if (tilesList[0].tile.transform.position.x < worldCentre - ((worldSize + 1) * 10 / 2))
        {
            MoveTile(true);
        }
        else if (tilesList[tilesList.Count - 1].tile.transform.position.x > worldCentre + ((worldSize + 1) * 10 / 2))
        {
            MoveTile(false);
        }
    }
}
