using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenAnimSequence : MonoBehaviour
{
    public List<TweenAnim> animsList = new List<TweenAnim>();
    
    public bool startOnEnable=true;
    public float startAnimDelay=1;
    public float animTime=.5f;
    public float nextAnimOffsetTime=-.25f;

    void OnEnable()
    {
        if(startOnEnable)
        {
            if(playDelayRt!=null) StopCoroutine(playDelayRt);
            playDelayRt = StartCoroutine(PlayDelay(startAnimDelay));
        }
    }

    Coroutine playDelayRt;
    IEnumerator PlayDelay(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Play();
    }

    public void Play()
    {
        if(tweeningInRt!=null) StopCoroutine(tweeningInRt);
        tweeningInRt = StartCoroutine(TweeningIn());
    }

    Coroutine tweeningInRt;
    IEnumerator TweeningIn()
    {
        foreach(TweenAnim anim in animsList)
        {
            anim.Reset();
        }

        for(int i=0; i<animsList.Count; i++)
        {
            animsList[i].TweenIn(animTime);
            yield return new WaitForSecondsRealtime(animTime+nextAnimOffsetTime);
        }
    }

    public void Reverse()
    {
        if(tweeningOutRt!=null) StopCoroutine(tweeningOutRt);
        tweeningOutRt = StartCoroutine(TweeningOut());
    }

    Coroutine tweeningOutRt;
    IEnumerator TweeningOut()
    {
        foreach(TweenAnim anim in animsList)
        {
            anim.TweenIn(0);
        }

        for(int i=animsList.Count-1; i>=0; i--)
        {
            animsList[i].TweenOut(animTime);
            yield return new WaitForSecondsRealtime(animTime+nextAnimOffsetTime);
        }
    }
}
