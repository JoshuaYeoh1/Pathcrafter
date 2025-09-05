using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingResult
{
    public List<Vector3Int> path { get; set; }
    public bool pathFound { get; set; }
    public int cellsProcessed { get; set; }
    public long millisecondsTaken { get; set; }

    public PathfindingResult()
    {
        pathFound = false;
        path = new List<Vector3Int>();
        cellsProcessed = 0;
        millisecondsTaken = 0;
    }
}
