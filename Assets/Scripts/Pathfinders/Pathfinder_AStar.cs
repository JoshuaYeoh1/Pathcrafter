using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Threading;

public static class Pathfinder_AStar
{
    public enum Heuristics
    {
        NONE = 0,
        MANHATTAN = 1
    }

    // Func<Vector3Int, Vector3Int, float> is a delegate.
    // See https://learn.microsoft.com/en-us/dotnet/api/system.func-2?view=net-8.0
    static readonly Dictionary<Heuristics, Func<Vector3Int, Vector3Int, float>> HEURISTICS = new();

    static float Manhattan(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Static class can have static constructor, which is called
    // RIGHT BEFORE the first call of any of its static field,
    // properties and methods. Now you know.
    static Pathfinder_AStar()
    {
        HEURISTICS.Add(Heuristics.NONE, (a,b)=>0); // Anonymous method
        HEURISTICS.Add(Heuristics.MANHATTAN, Manhattan);
    }

    public static PathfindingResult TryFindPath(Vector3Int startCoordinate, Vector3Int goalCoordinate, ISquareGrid grid, out List<Vector3Int> path, Heuristics heuristicType, AgentTemplateSO agentSO, int msDelay=0)
    {
        // 1. Copy content from Pathfinder_UCS
        // 2. Apply the necessary changes to 'priority' calculation
        // Refer to lecture slides :)

        PriorityQueue<Vector3Int> frontier = new PriorityQueue<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int?> cameFrom = new Dictionary<Vector3Int, Vector3Int?>();
        Dictionary<Vector3Int, float> costSoFar = new Dictionary<Vector3Int, float>();

        // 1. Put `startCoordinate` in `frontier` (priority queue) with `priority` = `0`.
        frontier.Enqueue(startCoordinate, 0);

        // 2. Add to `cameFrom` (dictionary) with key = `currentNode.Coordinate` value = `null`
        cameFrom[startCoordinate]=null;

        // 3. Add to `costSoFar` (dictionary) with key = `currentNode.Coordinate` value = `0`
        costSoFar[startCoordinate]=0;

        int cellsProcessed = 0; // tracker variable

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Step 2
        // Repeat while `frontier` not empty:
        while(frontier.Count > 0)
        {
            // 1. Get `coordinate` from `frontier.
            // NOTE: REPLACE WITH CORRECT VALUE.
            Vector3Int coordinate = frontier.Dequeue();

            cellsProcessed++;

            if(msDelay>0) Thread.Sleep(msDelay);

            // 2. Early exit: if `coordinate` equals `goalCoordinate`, break out of loop.
            if(coordinate == goalCoordinate) break;

            if(IsTerrainImpassable(grid.GetCell(coordinate).TerrainType, agentSO)) break;

            // Get `connections` from `grid` using `coordinate`.
            List<CellData> neighbours = grid.GetCellLinks(coordinate);

            // For each `neighbour` of `cell`:
            foreach(CellData neighbour in neighbours)
            {
                // 1. If `neighbour` not passable, skip this iteration.
                if(IsTerrainImpassable(neighbour.TerrainType, agentSO))
                    continue;
                
                
                // if orthogonal neighbors of this neighbour are not passable,
                // AND neighbour is diagonal,
                // Skip it
                if(!AreOrthogonalNeighboursPassable(neighbour, grid, agentSO) &&
                    IsNextPathDiagonal(coordinate, neighbour.Coordinate))
                    continue;
                    
                                
                // 2. Calculate `calcCost` = `costSoFar[cell.Coordinate]` + `neighbour.Cost`
                float newCost = costSoFar[coordinate] + GetTerrainCost(neighbour.TerrainType, agentSO);

                // 3. If `costSoFar` DOES NOT contain value for key `neighbour.Coordinate` OR `calcCost` < `costSoFar[neighbour.Coordinate]`:
                if(!costSoFar.ContainsKey(neighbour.Coordinate) || newCost<costSoFar[neighbour.Coordinate])
                {
                    costSoFar[neighbour.Coordinate] = newCost;
                    cameFrom[neighbour.Coordinate] = coordinate;

                    float priority = newCost + Heuristic(heuristicType, neighbour.Coordinate, goalCoordinate);

                    frontier.Enqueue(neighbour.Coordinate, priority);
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

    static float Heuristic(Heuristics heuristicType, Vector3Int a, Vector3Int b)
    {
        return HEURISTICS[heuristicType](a, b);
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