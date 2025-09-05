using UnityEngine;

[CreateAssetMenu(menuName="Agent Template")]

public class AgentTemplateSO : ScriptableObject
{
    public Sprite agentIcon;
    
    public int grassCost;
    public int woodCost;
    public int stoneCost;
    public int railCost;
    public int cobwebCost;
    public int waterCost;
    public int lavaCost;
}
