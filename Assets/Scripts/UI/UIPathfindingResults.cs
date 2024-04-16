using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPathfindingResults : MonoBehaviour
{
    public TextMeshProUGUI pathFoundTMP;
    public List<GameObject> childFields = new List<GameObject>();

    public TextMeshProUGUI cellsProcessedTMP;
    
    public TextMeshProUGUI timeTakenTMP;
    
    void Start()
    {
        GameEventSystem.Current.RenderPathEvent += OnRenderPath;
    }
    void OnDestroy()
    {
        GameEventSystem.Current.RenderPathEvent -= OnRenderPath;
    }

    void OnRenderPath(PathfindingResult result)
    {
        if(result.pathFound)
        {
            pathFoundTMP.text = "Yes";
            pathFoundTMP.color = Color.green;
        }
        else
        {
            pathFoundTMP.text = "No";
            pathFoundTMP.color = Color.red;
        }
        
        foreach(GameObject field in childFields)
        {
            field.SetActive(result.pathFound);
        }

        cellsProcessedTMP.text = result.cellsProcessed.ToString();

        timeTakenTMP.text = result.millisecondsTaken.ToString();
    }
}
