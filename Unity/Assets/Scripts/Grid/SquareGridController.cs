using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MapData
{
    public static readonly int Rows = 20;
    public static readonly int ColumnsPerRow = 20;

    static List<TerrainType> GetMapData()
    {
        List<TerrainType> terrainTypeList = new List<TerrainType>();

        for(int i=0; i<Rows*ColumnsPerRow; i++)
        {
            //randomTerrainTypes.Add((TerrainType)Random.Range(1,8));
            terrainTypeList.Add(TerrainType.Grass);
        }

        return terrainTypeList;
    }

    public static readonly List<TerrainType> Raw = GetMapData();
}

public class SquareGridController : MonoBehaviour
{
    public ISquareGrid GetSquareGrid() => squareGrid; // Lambda expression approach since it is a one liner that returns an object.

    SquareGrid squareGrid;

    [SerializeField] CellView cellPrefab;

    Dictionary<Vector3Int, CellView> cellViewStorage;

    void Awake()
    {
        // last param is true to make the generated map
        // look the same as MapData.Raw above.
        squareGrid = new(MapData.Raw, MapData.ColumnsPerRow, true);
        MakeCellViews();
    }

    void MakeCellViews()
    {
        cellViewStorage = new Dictionary<Vector3Int, CellView>();

        foreach(var cell in squareGrid.Cells) // for each 'cell' in squareGrid Cells
        {
            CellView cellView = Instantiate(cellPrefab, transform); // 'cellView' = instantiate from 'cellPrefab' and set this gameObject as parent

            cellView.SetData(cell.Value); // pass 'cell' to 'cellView'

            cellViewStorage[cell.Key] = cellView; // add 'cellView' to 'cellViewStorage', key is 'cell' coordinate
        }
    }

    public CellView GetCellView(Vector3Int coord) => cellViewStorage[coord];

    public bool TryGetCellData(Vector3Int coord, out ICellData cellData)
    => squareGrid.TryGetCellData(coord, out cellData);

    public bool TryChangeCellData(Vector3Int coord, TerrainType terrainType)
    => squareGrid.TryChangeCellData(coord, terrainType);

    public void SaveToHistory()
    {
        squareGrid.SaveToHistory(); // Save current state to history before making changes
    }
    
    public void Undo()
    {
        squareGrid.Undo();
    }
    
}