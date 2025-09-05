using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;

public static class Pathfinder_BFS
{
    // returns true when path is found, false otherwise
    // usage is similar to how Dictionary.TryGetValue is used!
    public static PathfindingResult TryFindPath(Vector3Int startCoordinate, Vector3Int goalCoordinate, ISquareGrid grid, out List<Vector3Int> path, AgentTemplateSO agentSO, int msDelay=0)
    {
        Queue<Vector3Int> frontier = new Queue<Vector3Int>();

        // 1. Put startCoordinate in queue:frontier
        frontier.Enqueue(startCoordinate);

        // 2. Add entry to dictionary:cameFrom. key = currentNode, value = none
        // Note that the Value is Vector3Int? instead of Vector3Int
        Dictionary<Vector3Int, Vector3Int?> cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
        cameFrom[startCoordinate] = null;

        int cellsProcessed = 0; // tracker variable

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // 3. While queue:frontier not empty
        while(frontier.Count>0)
        {
            // a. get cellCoordinate from queue:frontier
            Vector3Int coordinate = frontier.Dequeue();

            cellsProcessed++;

            if(msDelay>0) Thread.Sleep(msDelay);

            // b. Early exit: if coordinate equals goalCoordinate, break out of loop
            if (coordinate == goalCoordinate) break;

            if(IsTerrainImpassable(grid.GetCell(coordinate).TerrainType, agentSO)) break;

            // c. get connections from grid using cellCoordinate
            List<CellData> neighbours = grid.GetCellLinks(coordinate);

            // d. for each neighbour in connections
            foreach(CellData neighbour in neighbours)
            {
                // if neighbour NOT passable, skip
                if(IsTerrainImpassable(neighbour.TerrainType, agentSO)) continue;


                // if orthogonal neighbors of this neighbour are not passable,
                // AND neighbour is diagonal,
                // Skip it
                if(!AreOrthogonalNeighboursPassable(neighbour, grid, agentSO) &&
                    IsNextPathDiagonal(coordinate, neighbour.Coordinate))
                    continue;
                    

                // if neighbour NOT in dictionary:cameFrom
                if(!cameFrom.ContainsKey(neighbour.Coordinate))
                {
                    // add neighbour.Coordinate to queue:frontier
                    frontier.Enqueue(neighbour.Coordinate);

                    cameFrom[neighbour.Coordinate] = coordinate;
                }
            }
        }

        stopwatch.Stop();
        long millisecondsTaken = stopwatch.ElapsedMilliseconds;

        // return path
        path = new List<Vector3Int>();

        bool pathFound = PathProcessor.TryGetPath(cameFrom, startCoordinate, goalCoordinate, ref path);

        PathfindingResult result = new PathfindingResult();
        result.pathFound = pathFound;
        result.path = path;
        result.cellsProcessed = cellsProcessed;
        result.millisecondsTaken = millisecondsTaken;

        return result;
    }

    static bool IsTerrainImpassable(TerrainType terrainType, AgentTemplateSO agentSO)
    {
        return GetTerrainCost(terrainType, agentSO) >= 100;
    }
    
    static float GetTerrainCost(TerrainType terrainType, AgentTemplateSO agentSO)
    {
        switch(terrainType)
        {
            case TerrainType.Grass: return agentSO.grassCost;
            case TerrainType.Wood: return agentSO.woodCost;
            case TerrainType.Stone: return agentSO.stoneCost;
            case TerrainType.Rail: return agentSO.railCost;
            case TerrainType.Cobweb: return agentSO.cobwebCost;
            case TerrainType.Water: return agentSO.waterCost;
            case TerrainType.Lava: return agentSO.lavaCost;
            default: return 0; // Default cost for unknown terrain types
        }
    }

    static bool AreOrthogonalNeighboursPassable(CellData cell, ISquareGrid grid, AgentTemplateSO agentSO)
    {
        List<Vector3> orthogonalDirs = new()
        {
            new Vector3Int(1,0,0),     // Right
            new Vector3Int(0,-1,0),    // Bottom
            new Vector3Int(-1,0,0),    // Left
            new Vector3Int(0,1,0),     // Up
        };

        foreach(Vector3 orthogonalDir in orthogonalDirs)
        {
            Vector3Int neighbourCoord = Vector3Int.RoundToInt(cell.Coordinate + orthogonalDir);

            CellData neighbour = grid.GetCell(neighbourCoord);
        
            if(neighbour!=null) // if within the grid bounds
            {
                if(IsTerrainImpassable(neighbour.TerrainType, agentSO))
                {
                    return false;
                }
            }
        }
        
        return true; // All orthogonal neighbors are passable
    }

    static bool IsNextPathDiagonal(Vector3Int currentCoord, Vector3Int nextCoord)
    {
        int dx = Mathf.Abs(nextCoord.x - currentCoord.x);
        int dy = Mathf.Abs(nextCoord.y - currentCoord.y);

        // If both dx and dy are non-zero, it's diagonal
        return dx != 0 && dy != 0;
    }
}