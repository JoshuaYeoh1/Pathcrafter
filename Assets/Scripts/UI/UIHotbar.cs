using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHotbar : MonoBehaviour
{
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

    //////////////////////////////////////////////////////////////////////////////////////

    public void SelectAgent(int i)
    {
        GameEventSystem.Current.OnSelectAgent((AgentType)i);
    }

    public void SelectTerrain(int i)
    {
        GameEventSystem.Current.OnSelectTerrain((TerrainType)i);
    }

    //////////////////////////////////////////////////////////////////////////////////////

    public bool sandboxHotbar;

    void OnSelectAgent(AgentType type)
    {
        if(!sandboxHotbar) HighlightItem((int)type-1);
    }

    void OnSelectTerrain(TerrainType type)
    {
        if(sandboxHotbar) HighlightItem((int)type-1);
    }

    //////////////////////////////////////////////////////////////////////////////////////

    public GameObject[] highlightGraphics;

    void HighlightItem(int i)
    {
        if(i>=highlightGraphics.Length) return;

        foreach(GameObject select in highlightGraphics)
        {
            if(select!=highlightGraphics[i])
            select.SetActive(false);
        }

        highlightGraphics[i].SetActive(true);
    }
}
