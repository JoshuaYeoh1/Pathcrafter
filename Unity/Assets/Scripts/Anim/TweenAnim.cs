using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenAnim : MonoBehaviour
{
    [Header("Position")]
    public bool animPos;
    public Vector3 inPos;
    public Vector3 outPos;
    Vector3 defPos;

    [Header("Rotation")]
    public bool animRot;
    public Vector3 inRot;
    public Vector3 outRot;
    Vector3 defRot;

    [Header("Scale")]
    public bool animScale;
    public Vector3 inScale;
    public Vector3 outScale;
    Vector3 defScale;

    void Awake()
    {
        defPos = transform.position;
        defRot = transform.eulerAngles;
        defScale = transform.localScale;
    }

    void Start()
    {
        Reset(); // Must put in start otherwise buttonanim dissappears in mobile only
    }

    public void Reset()
    {
        if(animPos) transform.position = inPos;
        if(animRot) transform.eulerAngles = inRot;
        if(animScale) transform.localScale = inScale;
    }

    public void TweenIn(float time)
    {
        Reset();

        if(time>0)
        {
            LeanTween.cancel(gameObject);

            if(animPos) LeanTween.move(gameObject, defPos, time).setEaseOutExpo().setIgnoreTimeScale(true);
            if(animRot) LeanTween.rotate(gameObject, defRot, time).setEaseInOutSine().setIgnoreTimeScale(true);
            if(animScale) LeanTween.scale(gameObject, defScale, time).setEaseOutCubic().setIgnoreTimeScale(true);

            AudioManager.Current.PlaySFX(SFXManager.Current.sfxUITween, transform.position, false);
        }
        else
        {
            if(animPos) transform.position = defPos;
            if(animRot) transform.eulerAngles = defRot;
            if(animScale) transform.localScale = defScale;
        }
    }

    public void TweenOut(float time)
    {
        TweenIn(0);

        if(time>0)
        {
            LeanTween.cancel(gameObject);

            if(animPos) LeanTween.move(gameObject, outPos, time).setEaseInExpo().setIgnoreTimeScale(true).setOnComplete(Reset);
            if(animRot) LeanTween.rotate(gameObject, outRot, time).setEaseInOutSine().setIgnoreTimeScale(true).setOnComplete(Reset);
            if(animScale) LeanTween.scale(gameObject, outScale, time).setEaseInCubic().setIgnoreTimeScale(true).setOnComplete(Reset);

            AudioManager.Current.PlaySFX(SFXManager.Current.sfxUITween, transform.position, false);
        }
        else
        {
            if(animPos) transform.position = outPos;
            if(animRot) transform.eulerAngles = outRot;
            if(animScale) transform.localScale = outScale;
        }
    }

    //[Button] // requires Odin Inspector????
    [ContextMenu("Record Current Position")]
    void RecordCurrentPosition()
    {
        inPos=transform.position;
        outPos=transform.position;
    }

    //[Button]
    [ContextMenu("Record Current Rotation")]
    void RecordCurrentRotation()
    {
        inRot=transform.eulerAngles;
        outRot=transform.eulerAngles;
    }

    //[Button]
    [ContextMenu("Record Current Scale")]
    void RecordCurrentScale()
    {
        inScale=transform.position;
        outScale=transform.position;
    }
    
}
