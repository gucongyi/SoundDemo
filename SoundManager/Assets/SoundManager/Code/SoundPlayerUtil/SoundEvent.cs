using UnityEngine;
using System.Collections;

namespace HYZ {
    namespace SoundInternal {
        public class SoundEvent {
            public System.Action<SoundEvent> onComplete;

            private string name;
            private int eventID;
            private Transform attachPoint;
            private Vector3 position;
            private Sound sound;
            private SoundFade fade; 
            private bool isDelaying = false;
            private float startTime = 0f;

            public string Name { get { return name; } }
            public int EventID { get { return eventID; } }
            public Sound Sound { get { return sound; } }

            public float TargetVolume { get; private set; }

            public SoundEvent() {
                eventID = -1;
                sound = null;

                fade = new SoundFade();

            }

            public void Start(int id, Sound aSound, Transform parent = null, Vector3 position = new Vector3(), bool mute = false,bool isLoopStart=false) {
                name = aSound.soundInfo.name;
                eventID = id;
                attachPoint = parent;
                this.position = position;
                sound = aSound;
                TargetVolume = sound.soundInfo.GetVolume();

                PrepareAudioSource(isLoopStart);
                ToggleMute(mute);

                if(sound.soundInfo.delayAtStart <= 0 || ( !isLoopStart && sound.soundInfo.looping && sound.soundInfo.ignoreDelayAtFirstWhenLooping)||(isLoopStart&&sound.soundInfo.looping&&!sound.soundInfo.delayAtNextStartWhenLooping)) 
                {
                    sound.audioSource.Play();
                }
                else
                {
                    isDelaying = true;
                    startTime = Time.realtimeSinceStartup;
                }

                fade.FadeIn(sound.audioSource, TargetVolume, sound.soundInfo.fading ? sound.soundInfo.fadeInTime : 0f);
            }

            void PrepareAudioSource(bool isLoopStart ) {
                AudioSource audioSource = sound.audioSource;
                SoundInfo soundInfo = sound.soundInfo; 
                audioSource.transform.position = position;

                if(!isLoopStart || (soundInfo.looping&&soundInfo.randomClipWhenLooping)) 
                    audioSource.clip = soundInfo.GetAudioClip();

                audioSource.volume = soundInfo.GetVolume();
                audioSource.loop = false;
                if(soundInfo.looping && !(soundInfo.delayAtNextStartWhenLooping||soundInfo.randomClipWhenLooping))
                {
                    audioSource.loop = true;
                }

                if(soundInfo.randomPitch) {
                    audioSource.pitch = soundInfo.GetRandomPitch();
                }
            }

            public void Mute(float fadeout = -1f) {
                fade.FadeOut(fadeout == -1 ? 0f : fadeout);
                if(fadeout == -1) {
                    fade.FadeOut(0f);
                } else {
                    fade.FadeOut(fadeout);
                }
            }

            public void UnMute(float fadein = -1f) {
                fade.FadeIn(sound.audioSource, TargetVolume, fadein == -1 ? 0f : fadein, GetVolume());
            }

            public void Stop(float fadeout = -1f) {
                fade.onFadeComplete += this.OnFadeComplete;

                if(fadeout == -1) {
                    fade.FadeOut(sound.soundInfo.fading ? sound.soundInfo.fadeOutTime : 0f);
                } else {
                    fade.FadeOut(fadeout);
                }
            }

            public void SetPosition(Vector3 position) {
                sound.audioSource.transform.position = position;
            }

            public void SetVolume(float volume, bool stopFade) {
                if(stopFade) {
                    fade.Stop();
                    sound.audioSource.volume = volume * SoundPlayer.RelativeVolume;
                }
            }

            public float GetVolume() {
                return sound.audioSource.volume;
            }

            public bool IsFadingIn() {
                return fade.CurrentState == SoundFade.FadeState.FadeIn;
            }

            public bool IsFadingOut() {
                return fade.CurrentState == SoundFade.FadeState.FadeOut;
            }

            public void SetPitch(float pitch) {
                sound.audioSource.pitch = pitch;
            }

            public void ToggleMute(bool mute) {
                sound.audioSource.mute = mute;
            }

            public void Update() {
                if(isDelaying) {
                    if(Time.realtimeSinceStartup - startTime >= sound.soundInfo.delayAtStart) {
                        isDelaying = false;
                        sound.audioSource.Play();
                    }
                } else {
                    if(sound.audioSource != null) {
                        if(!sound.audioSource.loop && !sound.audioSource.isPlaying && !isDelaying) {
                            fade.Stop();
                            if(sound.soundInfo.looping)
                            {
                                Start(eventID, sound, attachPoint, this.position, sound.audioSource.mute,true);
                                return;
                            }
                            else
                            { 
                                attachPoint = null;
                                CallOnComplete();
                            }
                        }
                    }

                    fade.Update();

                    if(attachPoint != null) {
                        sound.audioSource.transform.position = attachPoint.position;
                    }
                }
            }

            void OnFadeComplete(SoundFade.FadeState fadeState) {
                fade.onFadeComplete -= this.OnFadeComplete;

                if(fadeState == SoundFade.FadeState.FadeOut) {
                    attachPoint = null;
                    CallOnComplete();
                }
            }

            void CallOnComplete() {
                if(onComplete != null) {
                    onComplete(this);
                }
            }
        }

        public class SoundEventIDGenerator {
            private int curInUseNumber;

            public SoundEventIDGenerator() {
                curInUseNumber = 0;
            }

            public int GetID() {
                curInUseNumber++;
                if(curInUseNumber >= int.MaxValue) {
                    curInUseNumber = 0;
                    //Debug.LogError("Current in use id is out of range, force to start at 0!");
                }
                return curInUseNumber;
            }
        }
    }
}