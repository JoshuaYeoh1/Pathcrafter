using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Algorithms
{
    BreadthFirstSearch,
    Dijkstra,
    AStar,
}

public class PathfindTester : MonoBehaviour
{
    Grid grid;
    SquareGridController gridController;

    public GameManager gm;
    public PathRenderer pathRenderer;

    //public Transform startCursor;

    [Header("Position")]
    public Vector3 posOffset;

    [Header("Agent")]
    public SimpleVehicle agentVehicle;
    SeekFleeBehaviour agentSeek;
    Pathseeker agentPathseeker;
    Agent agent;

    [Header("Target")]
    public Transform goal;

    public Algorithms algorithm = Algorithms.AStar;

    [Range(0, 10)] public int msDelay; // artificial lag

    void Awake()
    {
        grid=GetComponent<Grid>();
        gridController=GetComponent<SquareGridController>();

        agentSeek=agentVehicle.GetComponent<SeekFleeBehaviour>();
        agentPathseeker=agentVehicle.GetComponent<Pathseeker>();
        agent=agentVehicle.GetComponent<Agent>();
    }

    void Start()
    {
        GameEventSystem.Current.PathseekerStartEvent += OnPathseekerStart;
        GameEventSystem.Current.PathseekerEndEvent += OnPathseekerEnd;
        GameEventSystem.Current.SelectAgentEvent += OnSelectAgent;
    }
    void OnDestroy()
    {
        GameEventSystem.Current.PathseekerStartEvent -= OnPathseekerStart;
        GameEventSystem.Current.PathseekerEndEvent -= OnPathseekerEnd;
        GameEventSystem.Current.SelectAgentEvent -= OnSelectAgent;
    }

    public void OnAlgorithmMenuChange(int i)
    {
        switch(i)
        {
            case 0: algorithm = Algorithms.AStar; break;
            case 1: algorithm = Algorithms.Dijkstra; break;
            case 2: algorithm = Algorithms.BreadthFirstSearch; break;
        }

        RenderPath();
    }

    public void RenderPath()
    {
        ISquareGrid sqrGrid = gridController.GetSquareGrid();
        List<Vector3Int> path = null;

        Vector3Int startPos = Vector3Int.RoundToInt(agentVehicle.Position-posOffset);
        Vector3Int goalPos = Vector3Int.RoundToInt(goal.position-posOffset);

        PathfindingResult result = new PathfindingResult();

        switch(algorithm)
        {
            case Algorithms.BreadthFirstSearch: result = Pathfinder_BFS.TryFindPath(startPos, goalPos, sqrGrid, out path, agent.currentSO, msDelay); break;
            case Algorithms.Dijkstra: result = Pathfinder_UCS.TryFindPath(startPos, goalPos, sqrGrid, out path, agent.currentSO, msDelay); break;
            case Algorithms.AStar: result = Pathfinder_AStar.TryFindPath(startPos, goalPos, sqrGrid, out path, Pathfinder_AStar.Heuristics.MANHATTAN, agent.currentSO, msDelay); break;
        }

        Debug.Log($"Cells processed: {result.cellsProcessed}");
        Debug.Log($"Time taken (ms): {result.millisecondsTaken}");

        if(result.pathFound)
        {
            pathRenderer.gameObject.SetActive(true);

            pathRenderer.RenderPath(path);

            agentPathseeker.SeekPath(path);

            AudioManager.Current.PlaySFX(SFXManager.Current.sfxUIRenderPath, agentVehicle.transform.position, false);
        }
        else
        {
            pathRenderer.gameObject.SetActive(false);

            if(agentSeekingPath) agentPathseeker.ForgetPath();
        }

        GameEventSystem.Current.OnRenderPath(result);
    }

    bool agentSeekingPath;

    void OnPathseekerStart()
    {
        agentSeekingPath=true;
    }
    void OnPathseekerEnd()
    {
        agentSeekingPath=false;

        pathRenderer.gameObject.SetActive(false);
    }
    
    void OnSelectAgent(AgentType type)
    {
        if(agentSeekingPath) RenderPath();
    }

    Vector3Int mouseCoord;

    void Update()
    {
        mouseCoord = GetMouseCoord();

        CheckMouseButtons();
        CheckNumberButtons();
    }

    void CheckMouseButtons()
    {
        if(IsPointerOverUI(Input.mousePosition)) return;
        if(!CoordHasCell(mouseCoord)) return;
        //if(cellData.Cost<=0) return;

        //////////////////////////////////////////////////////

        if(gm.sandboxMode)
        {
            if(Input.GetMouseButtonDown(0)) // brush paint start
            {
                if(cellData.TerrainType!=gm.selectedTerrain)
                {
                    gridController.SaveToHistory();
                }
            }

            if(Input.GetMouseButton(0)) // brush paint
            {
                if(cellData.TerrainType!=gm.selectedTerrain)
                {
                    gridController.TryChangeCellData(mouseCoord, gm.selectedTerrain);

                    if(agentSeekingPath) RenderPath();

                    SFXManager.Current.PlaySfxBlock(gm.selectedTerrain);
                }
            }
            else if(Input.GetMouseButtonDown(1)) // fill paint
            {   
                gridController.SaveToHistory();

                FillPaint(mouseCoord);

                if(agentSeekingPath) RenderPath();

                SFXManager.Current.PlaySfxBlock(gm.selectedTerrain);
            }
        }
        else
        {   
            if(Input.GetMouseButtonDown(0))
            {
                AudioManager.Current.PlaySFX(SFXManager.Current.sfxUITeleport, agentVehicle.transform.position, false);
            }
            if(Input.GetMouseButton(0)) // set agent position
            {
                agentVehicle.transform.position = mouseCoord + posOffset;

                agentSeek.target.position = agentVehicle.Position;

                agentPathseeker.ForgetPath();
            }
            else if(Input.GetMouseButtonDown(1)) // set goal position
            {
                goal.position = mouseCoord + posOffset;

                RenderPath();

                AudioManager.Current.PlaySFX(SFXManager.Current.sfxUITeleport, goal.position, false);
            }
        }
    }

    void CheckNumberButtons()
    {
        if(gm.sandboxMode)
        {
            for(int i=1; i<=7; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    GameEventSystem.Current.OnSelectTerrain((TerrainType)i);
                }
            }
        }
        else
        {
            for(int i=1; i<=8; i++)
            {
                if(Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    GameEventSystem.Current.OnSelectAgent((AgentType)i);
                }
            }
        }        
    }

    void FillPaint(Vector3Int coordinate)
    {
        ICellData initialCellData;

        if(!gridController.TryGetCellData(coordinate, out initialCellData)) return;

        TerrainType initialType = initialCellData.TerrainType;

        HashSet<Vector3Int> visited = new HashSet<Vector3Int>(); // To keep track of visited cells

        Queue<Vector3Int> queue = new Queue<Vector3Int>(); // Queue for breadth-first search

        queue.Enqueue(coordinate); // Enqueue initial coordinate

        while(queue.Count>0)
        {
            Vector3Int currentCoord = queue.Dequeue(); // Dequeue a coordinate

            Debug.Log("Processing cell at: " + currentCoord);

            if (visited.Contains(currentCoord))
            {
                Debug.Log("Skipping visited cell: " + currentCoord);
                continue;
            }

            visited.Add(currentCoord); // Mark current coordinate as visited

            ISquareGrid iSqrGrid = gridController.GetSquareGrid();

            List<CellData> neighbours = iSqrGrid.GetCellLinks(currentCoord);

            foreach(CellData neighbour in neighbours)
            {
                Debug.Log("Checking neighbour at: " + neighbour.Coordinate);

                if(neighbour.TerrainType==initialType && IsCardinalNeighbor(currentCoord, neighbour.Coordinate))
                {
                    Debug.Log("Filling neighbour at: " + neighbour.Coordinate);

                    gridController.TryChangeCellData(neighbour.Coordinate, gm.selectedTerrain); // Fill the neighbour with selected terrain
                    
                    if(!visited.Contains(neighbour.Coordinate))
                    {
                        queue.Enqueue(neighbour.Coordinate); // Enqueue the neighbour for further exploration
                    }
                }
                else
                {
                    Debug.Log("Skipping neighbour at: " + neighbour.Coordinate);
                }
            }
        }

        gridController.TryChangeCellData(coordinate, gm.selectedTerrain); // Fill itself
    }

    bool IsCardinalNeighbor(Vector3Int cell1, Vector3Int cell2)
    {
        // Check if the cells are adjacent in the cardinal directions (up, down, left, and right)
        return Mathf.Abs(cell1.x - cell2.x) + Mathf.Abs(cell1.y - cell2.y) == 1;
    }

    Vector3Int GetMouseCoord()
    {
        Vector3 cursorWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorWorldPos.z=0;

        Vector3Int mouseCoord = grid.WorldToCell(cursorWorldPos);
        // Debug.Log(mouseCoord);

        return mouseCoord;
    }

    ICellData cellData;

    bool CoordHasCell(Vector3Int coord)
    {
        if(gridController.TryGetCellData(coord, out cellData))
        {
            return true;
        }
        return false;
    }

    CellView selectedCell;

    void SelectCell(Vector3Int coord)
    {
        if(selectedCell) ToggleSelectedCellVisual(selectedCell, false);

        CellView cell = gridController.GetCellView(coord);

        selectedCell = cell;

        ToggleSelectedCellVisual(selectedCell, true);
    }

    void ToggleSelectedCellVisual(CellView cell, bool toggle)
    {
        if(toggle)
        {
            cell.transform.localScale *= .9f;
        }
        else
        {
            cell.transform.localScale = Vector3.one;
        }
    }

    List<RaycastResult> raycastResults = new List<RaycastResult>();

    bool IsPointerOverUI(Vector2 touchPos)
    {
        PointerEventData eventDataPos = new PointerEventData(EventSystem.current);

        eventDataPos.position = touchPos;

        EventSystem.current.RaycastAll(eventDataPos, raycastResults);

        if(raycastResults.Count>0) // if more than 0, then UI is touched
        {
            foreach(RaycastResult result in raycastResults)
            {
                if(result.gameObject.tag!="Dont Mind Me") return true; // ignore UI elements with this tag
            }
        }

        return false;
    }

    Vector3Int V2to3Int(Vector2Int v2int)
    {
        return new Vector3Int(v2int.x, v2int.y, 0);
    }
    Vector2Int V3to2Int(Vector3Int v3int)
    {
        return new Vector2Int(v3int.x, v3int.y);
    }
}
