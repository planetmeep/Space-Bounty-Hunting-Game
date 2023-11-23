using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpaceStationPathManager : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject debugprefab;
    public GameObject startNode;
    public Vector2Int startNodeCellPos;
    Dictionary<Vector2Int, Vector2Int> BFSDictionary;
    List<GameObject> debugTiles;
    // Start is called before the first frame update

    void Start()
    {

        startNode.transform.position = tilemap.GetCellCenterWorld(tilemap.WorldToCell(startNode.transform.position));
        startNodeCellPos = (Vector2Int)tilemap.WorldToCell(startNode.transform.position);
        BFSDictionary = BreadthFirstSearch.BFS(startNodeCellPos, tilemap);
        debugTiles = new List<GameObject>();
        PathToPlayer();
    }



    // Update is called once per frame
    void Update()
    {
        PathToPlayer();
        /*GameObject player = GameObject.Find("Player");
        Vector3 currentCellPos = walkableTilemap.GetCellCenterWorld(walkableTilemap.WorldToCell(player.transform.position));
        Instantiate(debugprefab, currentCellPos, Quaternion.identity);*/

    }

    public void PathToPlayer() 
    {
        foreach (GameObject go in debugTiles) 
        {
            Destroy(go);
        }
        debugTiles.Clear();

        if (BFSDictionary != null) 
        {
            GameObject player = GameObject.Find("Player");
            Vector2Int playerCellPos = (Vector2Int)tilemap.WorldToCell(player.transform.position);
            Dictionary<Vector2Int, Vector2Int> shortestPathDict = BreadthFirstSearch.ShortestPath(startNodeCellPos, playerCellPos, BFSDictionary);

            Vector2Int[] shortestPathArray = BreadthFirstSearch.ShortestPathArray(shortestPathDict);

            if (shortestPathArray != null) 
            {
                foreach(Vector2Int node in shortestPathArray) 
                {
                    Vector3 nodeWorldPos = tilemap.GetCellCenterWorld((Vector3Int)node);
                    
                    GameObject newTile = Instantiate(debugprefab, nodeWorldPos, Quaternion.identity);
                    debugTiles.Add(newTile);
                }      
            }
        }
    }

}
