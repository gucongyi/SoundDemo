using UnityEngine;
using System.Collections;

namespace HYZ {
    namespace SoundInternal {
        public class SoundFade {

            public enum FadeState {
                None,
                FadeIn,
                FadeOut
            }

            public delegate void SoundFadeHandler(FadeState state);
            public event SoundFadeHandler onFadeComplete;

            private AudioSource targetAudioSource;

            private float fadeSpeed;
            private float targetVolume;
            private float volumeDiff;

            private bool doFade = false;

            public FadeState CurrentState { get { return curState; } }
            private FadeState curState;

            public SoundFade() {
                targetAudioSource = null;
                fadeSpeed = 0;
                targetVolume = 0;

                curState = FadeState.None;
            }

            public void FadeIn(AudioSource audioSrc, float targetVol, float duration, float initialVolume = 0f) {
                curState = FadeState.FadeIn;
                targetAudioSource = audioSrc;
                targetAudioSource.volume = initialVolume;
                InitFade(targetVol, duration);
            }

            public void FadeOut(float duration) {
                curState = FadeState.FadeOut;
                InitFade(0, duration);
            }

            public void Stop() {
                doFade = false;
            }

            public void Update() {
                if(doFade) {
                    volumeDiff = Mathf.MoveTowards(volumeDiff, 0, Time.deltaTime * fadeSpeed);
                    if(Mathf.Abs(volumeDiff) <= 0.001f) {
                        volumeDiff = 0;
                        targetAudioSource.volume = targetVolume * SoundPlayer.RelativeVolume;
                        doFade = false;

                        CallOnComplete();
                    } else {
                        targetAudioSource.volume = (targetVolume - volumeDiff) * SoundPlayer.RelativeVolume;
                    }
                }
            }

            private void InitFade(float targetVol, float duration) {
                if(duration <= 0.001f) {
                    doFade = false;
                    targetAudioSource.volume = targetVol * SoundPlayer.RelativeVolume;

                    CallOnComplete();
                } else {
                    doFade = true;
                    targetVolume = targetVol;
                    volumeDiff = targetVolume - targetAudioSource.volume;

                    fadeSpeed = Mathf.Abs(volumeDiff / duration);
                }
            }

            private void CallOnComplete() {
                if(onFadeComplete != null) {
                    onFadeComplete(curState);
                }
            }
        }
    }
}