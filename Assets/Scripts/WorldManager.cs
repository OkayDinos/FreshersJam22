using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabTilesList = new List<GameObject>();

    List<GameObject> tilesList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Vector3 spawnPos = new Vector3(0,0,0);
        for (int i = 0; i < 3; i++)
        {
            GameObject tile = Instantiate(prefabTilesList[0], spawnPos, Quaternion.identity);
            tilesList.Add(tile);
            spawnPos.x += 10;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
