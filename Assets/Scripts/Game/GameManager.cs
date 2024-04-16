using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType
{
    Grass = 1,
    Wood = 2,
    Stone = 3,
    Rail = 4,
    Cobweb = 5,
    Water = 6,
    Lava = 7,
}

public enum AgentType
{
    Steve = 1,
    Shovel = 2,
    Axe = 3,
    Pickaxe = 4,
    Minecart = 5,
    Shears = 6,
    Boat = 7,
    Strider = 8,
}

public class GameManager : MonoBehaviour
{
    public bool sandboxMode;

    public void ToggleSandboxMode(bool toggle)
    {
        sandboxMode=toggle;
    }

    public AgentType selectedAgent = AgentType.Steve;
    public TerrainType selectedTerrain = TerrainType.Grass;

    void Start()
    {
        GameEventSystem.Current.SelectAgentEvent += OnSelectAgent;
        GameEventSystem.Current.SelectTerrainEvent += OnSelectTerrain;
    }

    void OnDestroy()
    {
        GameEventSystem.Current.SelectAgentEvent -= OnSelectAgent;
        GameEventSystem.Current.SelectTerrainEvent -= OnSelectTerrain;
    }

    void OnSelectAgent(AgentType type)
    {
        selectedAgent=type;
    }

    void OnSelectTerrain(TerrainType type)
    {
        selectedTerrain=type;
    }
}
