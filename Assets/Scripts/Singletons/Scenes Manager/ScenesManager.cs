using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes // must be in the same order as in the build settings, and case sensitive
{
    Scene,
    MainMenu,
}

public class ScenesManager : MonoBehaviour
{
    public Animator transitionAnimator;
    public GameObject transitionCanvas;
    public CanvasGroup transitionCanvasGroup;

    public bool isTransitioning;
    int transitionTypes=1;

    public static ScenesManager Current;

    void Awake()
    {
        if(!Current) Current=this;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayTransitionIn();
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) ReloadScene();
        // if(Input.GetKeyDown(KeyCode.KeypadPlus)) LoadNextScene();
        // if(Input.GetKeyDown(KeyCode.KeypadMinus)) LoadPrevScene();
    }

    public void PlayTransition(string type, int i, bool quit=false)
    {
        playingTransitionRt = StartCoroutine(PlayingTransition(type, i, quit));
    }
    public void PlayTransitionIn()
    {
        PlayTransition("in", Random.Range(1, transitionTypes+1)); // choose a random transition
    }
    public void PlayTransitionOut(bool quit=false)
    {
        PlayTransition("out", Random.Range(1, transitionTypes+1), quit);
    }

    Coroutine playingTransitionRt;
    IEnumerator PlayingTransition(string type, int i, bool quit=false)
    {
        ShowTransition(true);

        transitionAnimator.Play(type+i, 0);

        if(type=="in")
        {
            yield return new WaitForSecondsRealtime(.1f);
            yield return new WaitForSecondsRealtime(GetTransitionLength());

            ShowTransition(false);
        }

        else if(type=="out")
        {
            yield return new WaitForSecondsRealtime(.1f);
            // FadeAudio(musicSource, true, GetTransitionLength(), 0);
            // FadeAudio(ambSource, true, GetTransitionLength(), 0);
            yield return new WaitForSecondsRealtime(GetTransitionLength());

            if(quit) Quit(false);
        }
    }

    public void TransitionTo(Scenes scene, bool anim = true)
    {
        int sceneIndex = (int)scene;
        TransitionTo(sceneIndex, anim);
    }

    public void TransitionTo(int sceneIndex, bool anim=true)
    {
        if(!isTransitioning) playingTransitionRt = StartCoroutine(TransitioningTo(sceneIndex, anim));
    }

    IEnumerator TransitioningTo(int sceneIndex, bool anim)
    {
        if(anim)
        {
            PlayTransitionOut();

            yield return new WaitForSecondsRealtime(.1f);
            yield return new WaitForSecondsRealtime(GetTransitionLength());
        }

        SceneManager.LoadScene(sceneIndex);

        //if(anim) PlayTransition("in", Random.Range(1, transitionTypes+1));

        // ChangeMusic();

        // ToggleAmb(false);
        // if(!IsSceneMainMenu()) ToggleAmb(true);
    }

    public void ShowTransition(bool toggle)
    {
        if(!toggle) CancelTransition();

        transitionAnimator.gameObject.SetActive(toggle);
        transitionCanvasGroup.interactable=toggle;
        transitionCanvasGroup.blocksRaycasts=toggle;
        isTransitioning=toggle;

        if(toggle) CancelTransition();
    }

    public void CancelTransition()
    {
        if(playingTransitionRt!=null) StopCoroutine(playingTransitionRt);
        transitionAnimator.Play("cancel", 0);
    }

    public void LoadNextScene()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if(nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            TransitionTo(nextIndex);
        }
    }

    public void LoadPrevScene()
    {
        int prevIndex = SceneManager.GetActiveScene().buildIndex - 1;

        if(prevIndex>=0)
        {
            TransitionTo(prevIndex);
        }
    }

    public void ReloadScene()
    {
        if(!IsSceneMainMenu())
        TransitionTo(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        if(!IsSceneMainMenu())
        TransitionTo(Scenes.MainMenu);
    }

    public bool IsSceneMainMenu()
    {
        return SceneManager.GetActiveScene().name == Scenes.MainMenu.ToString();
    }

    public Scenes StringToEnum(string str)
    {
        return (Scenes)System.Enum.Parse(typeof(Scenes), str);
    }

    public float GetTransitionLength()
    {
        return transitionAnimator.GetCurrentAnimatorStateInfo(0).length;
    }

    public void Quit(bool anim=true)
    {
        if(anim) PlayTransitionOut(true);

        else
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }

    // public void RandomScene()
    // {
    //     if(!isTransitioning) TransitionTo(Random.Range(0,6));
    // }
}
