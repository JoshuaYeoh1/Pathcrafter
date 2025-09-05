using System.Collections.Generic;
using UnityEngine;

public static class PathProcessor
{
    // path is a ref List<Vector3Int>. This means the List is passed by reference.
    public static bool TryGetPath(Dictionary<Vector3Int, Vector3Int?> dict,
        Vector3Int startCoordinate, Vector3Int goalCoordinate, ref List<Vector3Int> path)
    {
        // First check: if dict does not contain goal, do not continue and return false
        // Otherwise, assemble the path and return true
        if (dict.ContainsKey(goalCoordinate))
        {
            // If contain goal, it means we can traverse backwards from goal to start.
            // Traverse backwards, each time we traverse a node, add it to 'path'.
            Vector3Int fromCoordinate = goalCoordinate;
            while (fromCoordinate != startCoordinate)
            {
                path.Add(fromCoordinate);
                fromCoordinate = dict[fromCoordinate].Value;
            }

            // The starting point is not technically required, as when querying path
            // using pathfinding algorithm, the agent is already at the starting point.
            // However, we need this for path visualization purpose.
            path.Add(startCoordinate);

            // The list needs to be reversed so it goes from start to goal.
            path.Reverse();
            return true;
        }
        else return false;
    }
}