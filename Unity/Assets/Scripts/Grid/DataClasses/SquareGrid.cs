using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISquareGrid
{
    public CellData GetCell(Vector3Int coordinate);
    public List<CellData> GetCellLinks(Vector3Int coordinate);
};

public class SquareGrid : ISquareGrid
{
    public CellData GetCell(Vector3Int coordinate)
    {
        cellStorage.TryGetValue(coordinate, out CellData cell);
        return cell;
    }

    public List<CellData> GetCellLinks(Vector3Int coordinate)
    {
        cellLinks.TryGetValue(coordinate, out List<CellData> list);
        return list;
    }

    public static readonly List<Vector3Int> DIRECTIONS = new()
    {
        new Vector3Int(1,0,0),     // Right
        new Vector3Int(0,-1,0),    // Bottom
        new Vector3Int(-1,0,0),    // Left
        new Vector3Int(0,1,0),     // Up
        new Vector3Int(1,1,0),     // Top right
        new Vector3Int(-1,1,0),    // Top left
        new Vector3Int(-1,-1,0),   // Bottom left
        new Vector3Int(1,-1,0)     // Bottom right
        // TODO Cardinal direction support
    };
    
    Dictionary<Vector3Int, CellData> cellStorage;
    Dictionary<Vector3Int, List<CellData>> cellLinks;
    Stack<Dictionary<Vector3Int, TerrainType>> historyStack;

    public Dictionary<Vector3Int, CellData> Cells { get=> cellStorage; }

    public SquareGrid(List<TerrainType> mapData, int columnsPerRow, bool flipRow)
    {
        Generate(mapData, columnsPerRow, flipRow);
        Link();
        historyStack = new Stack<Dictionary<Vector3Int, TerrainType>>();
    }

    // NOT INCLUDED: Proper checks to ensure mapData fits columnsPerRow.
    void Generate(List<TerrainType> mapData, int columnsPerRow, bool flipRow)
    {
        cellStorage = new Dictionary<Vector3Int, CellData>();

        int rows = mapData.Count / columnsPerRow;

        int rows_0b = rows - 1;

        for(int i=rows_0b; i>=0; i--)
        {
            int revRow=flipRow ? rows_0b-i : i;

            for(int j=0; j<columnsPerRow; j++)
            {
                Vector3Int coord = new Vector3Int(j, revRow, 0);    // Make coordinate using j and i

                TerrainType terrainType = mapData[(i * rows) + j];                 // Find the terrainType from mapData

                CellData c = new CellData(coord, terrainType);             // Make cell and

                cellStorage[coord] = c;                             // store it to collection
            }
        }
    }

    void Link()
    {
        cellLinks = new Dictionary<Vector3Int, List<CellData>>();

        foreach(KeyValuePair<Vector3Int, CellData> kvp in cellStorage) // For each 'cell' in cellStorage
        {
            List<CellData> list = new List<CellData>(); // Make 'list' of CellData

            // Get 'cellCoord' = 'cell' coordinate
            Vector3Int cellCoord = kvp.Key;
            CellData cell = kvp.Value;

            for(int i=0; i<8; i++) // Loop 8 times, for every direction, iterate i
            {
                Vector3Int neighbourCoord = cellCoord + DIRECTIONS[i]; // 'neighbourCoord' = 'cellCoord' + DIRECTIONS[i]

                if(cellStorage.TryGetValue(neighbourCoord, out CellData nCell)) // Try get 'nCell' from 'cellStorage', key is 'neighbourCoord'
                {
                    if(nCell!=null) // if 'nCell' exist,
                    {
                        list.Add(nCell); //  add 'nCell' to 'list'
                    }
                }
            }

            cellLinks[cellCoord] = list; // Store 'list' to 'cellLinks', key is 'cellCoord'

            cell.UpdateNeighbours(list); // update 'cell' neighbours 'list'
        }
    }

    public bool TryGetCellData(Vector3Int coord, out ICellData cellData)
    {
        if(cellStorage.TryGetValue(coord, out CellData cell)) // Try get 'cell' from 'cellStorage', key is 'coord'
        {
            cellData = cell; // if 'cell' exist, assign 'cell' to cellData
            return true;
        }
        else // otherwise return false
        {
            cellData = null;
            return false;
        }
    }
    
    public bool TryChangeCellData(Vector3Int coord, TerrainType terrainType)
    {
        if(cellStorage.TryGetValue(coord, out CellData cell)) // Try get 'cell' from 'cellStorage', key is 'coord'
        {
            cell.UpdateTerrainType(terrainType); // if 'cell' exist, update cell values
            return true;
        }
        else // otherwise return false
        {
            return false;
        }
    }

    public void SaveToHistory()
    {
        // Create a snapshot of current map data
        Dictionary<Vector3Int, TerrainType> snapshot = new Dictionary<Vector3Int, TerrainType>();

        foreach(var kvp in cellStorage)
        {
            snapshot[kvp.Key] = kvp.Value.TerrainType;
        }

        // Push snapshot to history stack
        historyStack.Push(snapshot);
    }

    public void Undo()
    {
        if(historyStack.Count > 0)
        {
            // Pop the last snapshot from history
            Dictionary<Vector3Int, TerrainType> snapshot = historyStack.Pop();

            // Restore map data from the snapshot
            foreach(var kvp in snapshot)
            {
                Vector3Int coord = kvp.Key;
                TerrainType terrainType = kvp.Value;
                cellStorage[coord].UpdateTerrainType(terrainType);
            }
        }
    }
}
