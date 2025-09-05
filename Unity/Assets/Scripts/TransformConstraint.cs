using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TransformConstraint : MonoBehaviour
{
    public Transform constrainTo;
    public bool fixedUpdate=true;

    [Header("Weights")]
    public Vector3 positionMult;
    public Vector3 rotationMult, scaleMult;

    [Header("Offsets")]
    public Vector3 positionOffset;
    public Vector3 rotationOffset, scaleOffset;

#if UNITY_EDITOR
    void OnEnable()
    {
        EditorApplication.update += EditorUpdate;
    }

    void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;
    }

    void EditorUpdate()
    {
        if(!Application.isPlaying) Constraint();
    }
#endif

    void Update()
    {
        if(Application.isPlaying && !fixedUpdate) Constraint();
    }

    void FixedUpdate()
    {
        if(Application.isPlaying && fixedUpdate) Constraint();
    }

    void Constraint()
    {
        if(constrainTo)
        {
            if(positionMult!=Vector3.zero)
            transform.position = new Vector3
            (
                constrainTo.position.x*positionMult.x + positionOffset.x,
                constrainTo.position.y*positionMult.y + positionOffset.y,
                constrainTo.position.z*positionMult.z + positionOffset.z
            );
            
            if(rotationMult!=Vector3.zero)
            transform.rotation = Quaternion.Euler
            (
                constrainTo.eulerAngles.x*rotationMult.x + rotationOffset.x,
                constrainTo.eulerAngles.y*rotationMult.y + rotationOffset.y,
                constrainTo.eulerAngles.z*rotationMult.z + rotationOffset.z
            );
            
            if(scaleMult!=Vector3.zero)
            transform.localScale = new Vector3 
            (
                constrainTo.localScale.x*scaleMult.x + scaleOffset.x,
                constrainTo.localScale.y*scaleMult.y + scaleOffset.y,
                constrainTo.localScale.z*scaleMult.z + scaleOffset.z
            );
        }
    }
}
