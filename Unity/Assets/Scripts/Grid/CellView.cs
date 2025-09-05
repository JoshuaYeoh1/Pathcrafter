using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellView : MonoBehaviour
{
    public ICellData BackingData { get; private set; }

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        CheckAnim();
    }

    TerrainType prevType;

    void CheckAnim()
    {
        if(prevType!=BackingData.TerrainType)
        {
            prevType=BackingData.TerrainType;
        }
        else return;

        anim.Play(BackingData.TerrainType.ToString(), 0, 0);
    }

    public void SetData(ICellData cell)
    {
        BackingData = cell;

        transform.position = BackingData.Coordinate; // 2. Set transform position to cell coordinate.
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        if(!Application.isPlaying) return;
        //if(BackingData.Cost<0) return;
        
        foreach(CellData neighbour in BackingData.Neighbours)
        {
            //if(neighbour==null || neighbour.Cost<0) continue;
            if(neighbour==null) continue;

            var start = new Vector3(BackingData.Coordinate.x+0.5f, BackingData.Coordinate.y + 0.5f);
            var end = new Vector3(neighbour.Coordinate.x + 0.5f, neighbour.Coordinate.y + 0.5f);

            Debug.DrawLine(start, end, new Color(0.0f, 0.5f, 0.0f,1.0f));
        }
    }

#endif

}
