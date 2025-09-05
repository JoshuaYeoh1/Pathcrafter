using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICellData
{
    Vector3Int Coordinate { get; }
    //int Cost { get; }
    List<CellData> Neighbours { get; }
    TerrainType TerrainType { get; }
}

[System.Serializable]
public class CellData : ICellData
{
    public Vector3Int Coordinate { get; private set; }
    //public int Cost { get; private set; }
    public List<CellData> Neighbours { get; private set; }
    public TerrainType TerrainType { get; set; }

    public CellData(Vector3Int coordinate, TerrainType terrainType)
    {
        Coordinate = coordinate;
        TerrainType = terrainType;
    }

    // public void UpdateCost(int cost)
    // {
    //     Cost = cost;
    // }

    public void UpdateTerrainType(TerrainType terrainType)
    {
        TerrainType = terrainType;
    }

    public void UpdateNeighbours(List<CellData> neighbours)
    {
        Neighbours = neighbours;
    }
}