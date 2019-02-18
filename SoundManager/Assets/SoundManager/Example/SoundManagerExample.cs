using UnityEngine;
using System.Collections;

namespace HYZ {
    public class SoundManagerExample : MonoBehaviour {
        [SerializeField]
        private SoundPlayer soundPlayer;

        [SerializeField]
        private Transform attachTarget;
        [SerializeField]
        private Transform playPos;

        private bool mute = false;

        int curId = -1;

        void Awake() {
            soundPlayer.OnSoundComplete += HandleOnSoundComplete;
        }

        void OnGUI() {
            if(GUI.Button(new Rect(0, 0, 300, 50), "Play water")) {
                curId = soundPlayer.Play("water");
            }
            if(GUI.Button(new Rect(0, 50, 300, 50), "Play laugh")) {
                curId = soundPlayer.Play("laugh");
            }
            if(GUI.Button(new Rect(300, 50, 300, 50), "Change Cur Sound Pitch")) {
                soundPlayer.SetPitch(curId, 0.5f);
            }
            if(GUI.Button(new Rect(0, 100, 300, 50), "Play laugh transform")) {
                curId = soundPlayer.Play("laugh", attachTarget);
            }
            if(GUI.Button(new Rect(300, 100, 300, 50), "Play water 2D")) {
                curId = soundPlayer.Play("water2D");
            }
            if(GUI.Button(new Rect(0, 150, 300, 50), "Play water position")) {
                curId = soundPlayer.Play("water", playPos.position);
            }
            if(GUI.Button(new Rect(300, 150, 300, 50), "Play laugh 2D")) {
                curId = soundPlayer.Play("laugh2D");
            }
            if(GUI.Button(new Rect(0, 200, 300, 50), "stop current sound")) {
                soundPlayer.Stop(curId);
            }
            if(GUI.Button(new Rect(0, 250, 300, 50), "Toggle Mute")) {
                mute = !mute;
                SoundPlayer.ToggleMusicMute(mute);
                SoundPlayer.ToggleSoundsMute(mute);
            }
            if(GUI.Button(new Rect(0, 300, 300, 50), "Stop every sounds")) {
                soundPlayer.StopAll();
            }
        }

        void HandleOnSoundComplete(int soundID) {
            //Debug.Log("Sound " + soundID + " finished to play.");
        }
    }
}