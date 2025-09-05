using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathseeker : MonoBehaviour
{
    SeekFleeBehaviour seek;

    void Awake()
    {
        seek=GetComponent<SeekFleeBehaviour>();
    }

    List<Vector3Int> path;
    int pathIndex;

    public void SeekPath(List<Vector3Int> newPath)
    {
        path = newPath;
        pathIndex=0;

        GameEventSystem.Current.OnPathseekerStart();
    }

    public float continueRange=.25f;

    void Update()
    {
        if(path==null || path.Count==0) return;

        if(Vector3.Distance(transform.position, seek.target.position) < continueRange)
        {
            if(pathIndex >= path.Count-1) // Check if at the end or beyond
            {
                ForgetPath();

                AudioManager.Current.PlaySFX(SFXManager.Current.sfxUIGoal, transform.position, false);

                return;
            }
            
            pathIndex++; // to the next one
            
            SetSeekPos(path[pathIndex]);
        }
    }

    public Vector3 seekOffset;

    void SetSeekPos(Vector3 pos)
    {
        seek.target.position = pos + seekOffset;
    }

    public void ForgetPath()
    {
        path=null;

        GameEventSystem.Current.OnPathseekerEnd();
    }
}
