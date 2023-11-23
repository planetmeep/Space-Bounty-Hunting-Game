using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpaceStationPathfinder : MonoBehaviour
{
    public Tilemap walkableTilemap;
    public GameObject debugprefab;
    public Vector2 cellToWorldOffset;
    // Start is called before the first frame update
    void Start()
    {
        /*Instantiate(debugprefab, walkableTilemap.CellToWorld(Vector3Int.right), Quaternion.identity);
        Instantiate(debugprefab, walkableTilemap.CellToWorld(Vector3Int.up), Quaternion.identity);
        Instantiate(debugprefab, walkableTilemap.CellToWorld(Vector3Int.left), Quaternion.identity);
        Instantiate(debugprefab, walkableTilemap.CellToWorld(Vector3Int.down), Quaternion.identity);*/
        //Instantiate(debugprefab, walkableTilemap.CellToWorld(Vector3Int.zero), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        /*GameObject player = GameObject.Find("Player");
        Vector3 currentCellPos = walkableTilemap.GetCellCenterWorld(walkableTilemap.WorldToCell(player.transform.position));
        Instantiate(debugprefab, currentCellPos, Quaternion.identity);*/

    }


}
