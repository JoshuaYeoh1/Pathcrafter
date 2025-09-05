using System;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    public static GameEventSystem Current;

    void Awake()
    {
        Current = this;
        Debug.Log("GameEventSystem has awoken");
    }

    public event Action<PathfindingResult> RenderPathEvent;
    
    public void OnRenderPath(PathfindingResult result)
    {
        RenderPathEvent?.Invoke(result);
    }

    public event Action AgentModeEvent;
    public event Action SandboxModeEvent;

    public void OnAgentMode()
    {
        AgentModeEvent?.Invoke();
    }
    public void OnSandboxMode()
    {
        SandboxModeEvent?.Invoke();
    }
    
    public event Action<AgentType> SelectAgentEvent;
    public event Action<TerrainType> SelectTerrainEvent;

    public void OnSelectAgent(AgentType type)
    {
        SelectAgentEvent?.Invoke(type);
    }
    public void OnSelectTerrain(TerrainType type)
    {
        SelectTerrainEvent?.Invoke(type);
    }
    
    public event Action PathseekerStartEvent;
    public event Action PathseekerEndEvent;

    public void OnPathseekerStart()
    {
        PathseekerStartEvent?.Invoke();
    }
    public void OnPathseekerEnd()
    {
        PathseekerEndEvent?.Invoke();
    }
}