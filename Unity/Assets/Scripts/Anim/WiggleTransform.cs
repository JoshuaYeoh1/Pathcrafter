using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiggleTransform : MonoBehaviour
{
    Vector3 seed;

    [Header("Position")]
    public bool wigglePos;
    public Vector3 posFrequency;
    public Vector3 posMagnitude;
    Vector3 defPos;

    [Header("Rotation")]
    public bool wiggleRot;
    public Vector3 rotFrequency;
    public Vector3 rotMagnitude;
    Vector3 defRot;

    [Header("Scale")]
    public bool wiggleScale;
    public float scaleFrequency;
    public float scaleMagnitude;
    Vector3 defScale;

    [Header("Time")]
    public bool ignoreTime;
    float time;
    
    void Awake()
    {
        seed = new Vector3(Random.value*999, Random.value*999, Random.value*999);

        defPos=transform.localPosition;
        defRot=transform.localEulerAngles;
        defScale=transform.localScale;
    }

    void Update()
    {
        if(ignoreTime)
        {
            time=Time.unscaledTime;
        }
        else
        {
            time=Time.time;
        }

        WigglePos();
        WiggleRot();
        WiggleScale();
    }

    public float Wiggle(float seed, float freq, float mag, float offset=0)
    {
        if(freq!=0 && mag!=0)
        {
            return (Mathf.PerlinNoise(seed, time * freq)*2-1) * mag + offset;
        }
        return offset;
    }

    void WigglePos()
    {
        if(!wigglePos)
        {
            transform.localPosition = defPos;
            return;
        }

        if(posFrequency==Vector3.zero || posMagnitude==Vector3.zero) return;
        
        Vector3 wiggle = new Vector3
        (
            Wiggle(seed.x, posFrequency.x, posMagnitude.x),
            Wiggle(seed.y, posFrequency.y, posMagnitude.y),
            Wiggle(seed.z, posFrequency.z, posMagnitude.z)
        );

        transform.localPosition = defPos + wiggle;
    }

    void WiggleRot()
    {
        if(!wiggleRot)
        {
            transform.localEulerAngles = defRot;
            return;
        }
        
        if(rotFrequency==Vector3.zero || rotMagnitude==Vector3.zero) return;
        
        Vector3 wiggle = new Vector3
        (
            Wiggle(seed.x, rotFrequency.x, rotMagnitude.x),
            Wiggle(seed.y, rotFrequency.y, rotMagnitude.y),
            Wiggle(seed.z, rotFrequency.z, rotMagnitude.z)
        );

        transform.localEulerAngles = defRot + wiggle;
    }

    void WiggleScale()
    {
        if(!wiggleScale)
        {
            transform.localScale = defScale;
            return;
        }
        
        if(scaleFrequency==0 || scaleMagnitude==0) return;
        
        Vector3 wiggle = new Vector3
        (
            Wiggle(seed.x, scaleFrequency, scaleMagnitude, scaleMagnitude),
            Wiggle(seed.x, scaleFrequency, scaleMagnitude, scaleMagnitude),
            Wiggle(seed.x, scaleFrequency, scaleMagnitude, scaleMagnitude)
        );

        transform.localScale = defScale + wiggle;
    }

    public void ShakePos(float time)
    {
        if(shakingPosRt!=null) StopCoroutine(shakingPosRt);
        shakingPosRt = StartCoroutine(ShakingPos(time));
    }
    Coroutine shakingPosRt;
    IEnumerator ShakingPos(float time)
    {
        wigglePos=true;
        yield return new WaitForSeconds(time);
        wigglePos=false;
    }

    public void ShakeRot(float time)
    {
        if(shakingRotRt!=null) StopCoroutine(shakingRotRt);
        shakingRotRt = StartCoroutine(ShakingRot(time));
    }
    Coroutine shakingRotRt;
    IEnumerator ShakingRot(float time)
    {
        wiggleRot=true;
        yield return new WaitForSeconds(time);
        wiggleRot=false;
    }

    public void ShakeScale(float time)
    {
        if(shakingScaleRt!=null) StopCoroutine(shakingScaleRt);
        shakingScaleRt = StartCoroutine(ShakingScale(time));
    }
    Coroutine shakingScaleRt;
    IEnumerator ShakingScale(float time)
    {
        wiggleScale=true;
        yield return new WaitForSeconds(time);
        wiggleScale=false;
    }
    
}
