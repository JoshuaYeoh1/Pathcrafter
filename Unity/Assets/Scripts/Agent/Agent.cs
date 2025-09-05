using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    SimpleVehicle vehicle;
    public SpriteRenderer sr;

    public AgentTemplateSO[] agentSO;
    public AgentTemplateSO currentSO;

    void Awake()
    {
        vehicle=GetComponent<SimpleVehicle>();

        currentSO = agentSO[0];
    }

    void Start()
    {
        GameEventSystem.Current.SelectAgentEvent += OnSelectAgent;
    }
    void OnDestroy()
    {
        GameEventSystem.Current.SelectAgentEvent -= OnSelectAgent;
    }
    
    void OnSelectAgent(AgentType type)
    {
        currentSO = agentSO[(int)type-1];

        sr.sprite = currentSO.agentIcon;

        vehicle.maxSpeed = vehicle.defSpeed;
    }

    int collCount;

    void OnTriggerEnter2D(Collider2D other)
    {
        ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit2D);

        collCount++;

        if(Time.time>1)
        {
            CellView cell = other.GetComponent<CellView>();
            if(!cell) return;

            SFXManager.Current.PlaySfxBlock(cell.BackingData.TerrainType);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        CellView cell = other.GetComponent<CellView>();
        if(!cell) return;

        float speedMult;

        switch(cell.BackingData.TerrainType)
        {
            case TerrainType.Grass: speedMult = 1-(currentSO.grassCost/100f); break;
            case TerrainType.Wood: speedMult = 1-(currentSO.woodCost/100f); break;
            case TerrainType.Stone: speedMult = 1-(currentSO.stoneCost/100f); break;
            case TerrainType.Rail: speedMult = 1-(currentSO.railCost/100f); break;
            case TerrainType.Cobweb: speedMult = 1-(currentSO.cobwebCost/100f); break;
            case TerrainType.Water: speedMult = 1-(currentSO.waterCost/100f); break;
            case TerrainType.Lava: speedMult = 1-(currentSO.lavaCost/100f); break;
            default: speedMult=1; break;
        }

        //Debug.Log($"speedMult: {speedMult}");

        vehicle.maxSpeed = vehicle.defSpeed * speedMult;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);

        if(collCount>0) collCount--;

        if(collCount<1)
        {
            vehicle.maxSpeed = vehicle.defSpeed;
        }
    }
}
