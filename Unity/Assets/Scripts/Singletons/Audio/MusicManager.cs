using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Current;

    AudioSource musicSource;
    float defVolume;

    void Awake()
    {
        if(!Current) Current=this;

        musicSource = GetComponent<AudioSource>();
        defVolume = musicSource.volume;
        musicSource.loop=false;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////

    [Header("Music")]
    public bool musicEnabled=true;
    public AudioClip[] idleMusics;
    //public AudioClip[] combatMusics;

    List<AudioClip> currentClips = new List<AudioClip>();

    void Start()
    {
        if(HasClips(idleMusics)) SwapMusic(idleMusics);
    }

    public void SwapMusic(AudioClip[] clips)
    {
        if(currentClips.Count>0)
        {
            currentClips.Clear();
        }
        
        if(HasClips(clips))
        {
            currentClips.AddRange(clips);
            RestartMusic();
        }
    }

    void RestartMusic()
    {
        if(currentClips.Count>0)
        {
            musicSource.volume = defVolume;
            musicSource.clip = currentClips[Random.Range(0, currentClips.Count)];
            musicSource.Play();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////

    void Update()
    {
        if(musicEnabled) UpdateShuffleMusic();
    }

    void UpdateShuffleMusic()
    {
        if(!musicSource.isPlaying) RestartMusic();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////

    public void ChangeMusic(AudioClip[] clips, float fadeOutTime=3)
    {
        AudioManager.Current.TweenVolume(musicSource, 0, fadeOutTime);

        if(changingMusicRt!=null) StopCoroutine(changingMusicRt);
        changingMusicRt = StartCoroutine(ChangingMusic(clips, fadeOutTime));
    }
    
    Coroutine changingMusicRt;
    IEnumerator ChangingMusic(AudioClip[] clips, float fadeOutTime)
    {
        if(fadeOutTime>0) yield return new WaitForSecondsRealtime(fadeOutTime);
        if(HasClips(clips)) SwapMusic(clips);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////

    public void PlayMusic(AudioClip[] clips)
    {
        ChangeMusic(clips, 0);
    }

    public void StopMusic(float fadeOutTime=3)
    {
        ChangeMusic(null, fadeOutTime);

        if(currentClips.Count>0)
        {
            currentClips.Clear();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool HasClips(AudioClip[] clips)
    {
        return clips!=null && clips.Length>0;
    }
}
