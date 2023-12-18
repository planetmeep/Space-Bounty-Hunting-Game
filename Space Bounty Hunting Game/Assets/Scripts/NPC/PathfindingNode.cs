using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode : MonoBehaviour
{
    public Transform target;
    public SpaceStationPathManager pathManager;
    public Vector3Int previousTargetPos;
    public bool pathfindingEnabled;
    public Vector3[] currentPath;
    public int pointsIndex;

    // Start is called before the first frame update
    void Awake()
    {
        previousTargetPos = pathManager.tilemap.WorldToCell(target.position);
        if (pathfindingEnabled) currentPath = pathManager.PathToTarget(transform, target);
        pointsIndex = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pathfindingEnabled) return;
        if (CheckTargetMoved()) 
        {
            currentPath = pathManager.PathToTarget(transform, target);
            pointsIndex = 1;
        }
        previousTargetPos = pathManager.tilemap.WorldToCell(target.position);

    }
    
    public bool CheckTargetMoved() 
    {
        return pathManager.tilemap.WorldToCell(target.position) != previousTargetPos;
    }
}
