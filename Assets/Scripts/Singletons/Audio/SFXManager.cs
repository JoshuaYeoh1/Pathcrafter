using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Current;

    void Awake()
    {
        if(!Current) Current=this;
    }

    ////////////////////////////////////////////////////////////////////////////////////

    [Header("Blocks")]
    public AudioClip[] sfxCobweb;
    public AudioClip[] sfxGrass;
    public AudioClip[] sfxLava;
    public AudioClip[] sfxRail;
    public AudioClip[] sfxStone;
    public AudioClip[] sfxWater;
    public AudioClip[] sfxWood;

    bool canPlaySfxBlock=true;

    public void PlaySfxBlock(TerrainType type)
    {
        if(canPlaySfxBlock)
        {
            canPlaySfxBlock=false;

            CancelInvoke("SfxBlockCooldown");
            Invoke("SfxBlockCooldown", .1f);
        }
        else return;

        switch(type)
        {
            case TerrainType.Grass: AudioManager.Current.PlaySFX(sfxGrass, transform.position, false); break;
            case TerrainType.Wood: AudioManager.Current.PlaySFX(sfxWood, transform.position, false); break;
            case TerrainType.Stone: AudioManager.Current.PlaySFX(sfxStone, transform.position, false); break;
            case TerrainType.Rail: AudioManager.Current.PlaySFX(sfxRail, transform.position, false); break;
            case TerrainType.Cobweb: AudioManager.Current.PlaySFX(sfxCobweb, transform.position, false); break;
            case TerrainType.Water: AudioManager.Current.PlaySFX(sfxWater, transform.position, false); break;
            case TerrainType.Lava: AudioManager.Current.PlaySFX(sfxLava, transform.position, false); break;
            default: AudioManager.Current.PlaySFX(sfxStone, transform.position, false); break;
        }
    }

    void SfxBlockCooldown() //Invoked
    {
        canPlaySfxBlock=true;
    }

    [Header("UI")]
    public AudioClip[] sfxUIBtnClick;
    public AudioClip[] sfxUIBtnHover;
    public AudioClip[] sfxUIGoal;
    public AudioClip[] sfxUIRenderPath;
    public AudioClip[] sfxUITeleport;
    public AudioClip[] sfxUITransition;
    public AudioClip[] sfxUITween;
}
