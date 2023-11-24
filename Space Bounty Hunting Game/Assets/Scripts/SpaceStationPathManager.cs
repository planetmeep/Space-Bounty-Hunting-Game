using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpaceStationPathManager : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject debugprefab;

    public Vector3[] PathToTarget(Transform start, Transform end) 
    {
        //GameObject[] debugPrefabs = GameObject.FindGameObjectsWithTag("debug");
        //foreach (GameObject prefab in debugPrefabs) Destroy(prefab);

        Vector2Int startCellPos = (Vector2Int)tilemap.WorldToCell(start.position);
        Vector2Int endCellPos = (Vector2Int)tilemap.WorldToCell(end.position);

        Dictionary<Vector2Int, Vector2Int> BFSDictionary = BreadthFirstSearch.BFS(startCellPos, tilemap);
        Dictionary<Vector2Int, Vector2Int> shortestPathDict = BreadthFirstSearch.ShortestPath(startCellPos, endCellPos, BFSDictionary);

        Vector2Int[] shortestPathArray = BreadthFirstSearch.ShortestPathArray(shortestPathDict);
        List<Vector3> pathWorldArray = new List<Vector3>();
        if (shortestPathArray != null) 
        {
            foreach(Vector2Int node in shortestPathArray) 
            {
                Vector3 nodeWorldPos = tilemap.GetCellCenterWorld((Vector3Int)node);
                pathWorldArray.Add(nodeWorldPos);
            }      
        }

        /*foreach (Vector3 node in pathWorldArray) 
        {
            Instantiate(debugprefab, node, Quaternion.identity);
        }*/
        return pathWorldArray.ToArray();
    }


}
