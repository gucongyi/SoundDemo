
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using HYZ; 
public class MusicTrigger :MonoBehaviour
{
    public string musicName;
    public bool autoTrigger;
    public bool stopSelfMusicOnDestroy=true;
    public float stopFadeOutOnDestroy = -1;  
     
    private void Start()
    {
        if (autoTrigger)
        {
            PlayMusic();
        }
    }
  
    private void OnDisable()
    {
        if (!stopSelfMusicOnDestroy)
            return;

        if (GameSoundPlayer.Instance!=null&&GameSoundPlayer.Instance.currentMusicName == this.musicName)
            GameSoundPlayer.Instance.StopBGMusic(stopFadeOutOnDestroy);
        
    } 

    public void PlayMusic()
    { 
        GameSoundPlayer.Instance.PlayBgMusic(musicName);
    }
}
