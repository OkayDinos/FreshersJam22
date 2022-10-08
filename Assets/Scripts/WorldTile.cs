using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New World Tile", menuName = "Custom/World Tile")]
public class WorldTile : ScriptableObject
{
    public GameObject prefabTile;
    
    public int tileLength;
}
