using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public static class BreadthFirstSearch
{

    public static Dictionary<Vector2Int, Vector2Int> BFS(Vector2Int start, Vector2Int end, Tilemap tilemap) 
    {
        // start is cell coordinates

        Dictionary<Vector2Int, Vector2Int> backwardPath = new Dictionary<Vector2Int, Vector2Int>();
        // key: child cell, value: parent cell


        List<Vector2Int> queue = new List<Vector2Int>();
        List<Vector2Int> exhausted = new List<Vector2Int>();


        queue.Add(start);
        exhausted.Add(start);

        while (queue.Count > 0) 
        {
            Vector2Int previousPos = queue[0];

            queue.RemoveAt(0);

            Vector2Int[] neighbors = GetNeighbors(previousPos, exhausted.ToArray(), queue.ToArray(), tilemap);
            foreach (Vector2Int neighbor in neighbors) 
            {
                queue.Add(neighbor);
                exhausted.Add(neighbor);
                backwardPath.Add(neighbor, previousPos);
            }
        }

        Dictionary<Vector2Int, Vector2Int> forwardPath = new Dictionary<Vector2Int, Vector2Int>();
        // key: parent cell, value: child cell

        Vector2Int childCell = end;
        while (childCell != start) 
        {
            forwardPath.Add(backwardPath[childCell], childCell);
            childCell = backwardPath[childCell]; 
        }

        return forwardPath;

    }

    private static bool TileValidUp(Vector2Int currentPos, Tilemap tilemap, Vector2Int[] exhausted, Vector2Int[] queue)
    {
        List<Vector2Int> exhaustedList = new List<Vector2Int>();
        exhaustedList.AddRange(exhausted);
        List<Vector2Int> queueList = new List<Vector2Int>();
        exhaustedList.AddRange(queue);

        Vector2Int stepUpVector = new Vector2Int(currentPos.x, currentPos.y + 1);
        if (tilemap.GetTile((Vector3Int)stepUpVector) == null) return false;
        if (exhaustedList.Contains(stepUpVector)) return false;
        if (queueList.Contains(stepUpVector)) return false;

        return true;
    }

    private static bool TileValidDown(Vector2Int currentPos, Tilemap tilemap, Vector2Int[] exhausted, Vector2Int[] queue)
    {
        List<Vector2Int> exhaustedList = new List<Vector2Int>();
        exhaustedList.AddRange(exhausted);
        List<Vector2Int> queueList = new List<Vector2Int>();
        exhaustedList.AddRange(queue);

        Vector2Int stepDownVector = new Vector2Int(currentPos.x, currentPos.y - 1);
        if (tilemap.GetTile((Vector3Int)stepDownVector) == null) return false;
        if (exhaustedList.Contains(stepDownVector)) return false;
        if (queueList.Contains(stepDownVector)) return false;

        return true;
    }

    private static bool TileValidLeft(Vector2Int currentPos, Tilemap tilemap, Vector2Int[] exhausted, Vector2Int[] queue)
    {
        List<Vector2Int> exhaustedList = new List<Vector2Int>();
        exhaustedList.AddRange(exhausted);
        List<Vector2Int> queueList = new List<Vector2Int>();
        exhaustedList.AddRange(queue);

        Vector2Int stepLeftVector = new Vector2Int(currentPos.x - 1, currentPos.y);
        if (tilemap.GetTile((Vector3Int)stepLeftVector) == null) return false;
        if (exhaustedList.Contains(stepLeftVector)) return false;
        if (queueList.Contains(stepLeftVector)) return false;

        return true;
    }

    private static bool TileValidRight(Vector2Int currentPos, Tilemap tilemap, Vector2Int[] exhausted, Vector2Int[] queue)
    {
        List<Vector2Int> exhaustedList = new List<Vector2Int>();
        exhaustedList.AddRange(exhausted);
        List<Vector2Int> queueList = new List<Vector2Int>();
        exhaustedList.AddRange(queue);

        Vector2Int stepRightVector = new Vector2Int(currentPos.x + 1, currentPos.y);
        if (tilemap.GetTile((Vector3Int)stepRightVector) == null) return false;
        if (exhaustedList.Contains(stepRightVector)) return false;
        if (queueList.Contains(stepRightVector)) return false;

        return true;
    }

    public static Vector2Int[] GetNeighbors(Vector2Int currentPos, Vector2Int[] exhausted, Vector2Int[] queue, Tilemap tilemap) 
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (TileValidUp(currentPos, tilemap, exhausted, queue)) neighbors.Add(new Vector2Int(currentPos.x, currentPos.y + 1));
        if (TileValidDown(currentPos, tilemap, exhausted, queue)) neighbors.Add(new Vector2Int(currentPos.x, currentPos.y - 1));
        if (TileValidLeft(currentPos, tilemap, exhausted, queue)) neighbors.Add(new Vector2Int(currentPos.x - 1, currentPos.y));
        if (TileValidRight(currentPos, tilemap, exhausted, queue)) neighbors.Add(new Vector2Int(currentPos.x + 1, currentPos.y));

        return neighbors.ToArray();
    }
}
