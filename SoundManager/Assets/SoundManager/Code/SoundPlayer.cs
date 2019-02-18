using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EasyEditor;
using HYZ.SoundInternal;

namespace HYZ {
    public class SoundPlayer : MonoBehaviour {
        private delegate void SoundPlayerHandler(bool mute); 

#pragma warning disable 414
        [SerializeField]
        bool showHelp = false;
#pragma warning restore 414
       
        [Message(text = "This option determine if the sound player plays Music or normal sounds. " +
            "This is used to detect which sound player should be mute when muting music or sounds in settings",
                 id = "showHelp", value = true)]
        [SerializeField]
        private bool playMusic = false;

        [Message(text = "SoundPlayer can hold several SoundList in order to better organize sounds. Sound List can be child of the" +
            " SoundPlayer or can be prefabs in the Project panel.",
                 id = "showHelp", value = true)]
        [SerializeField]
        private SoundList[] soundLists;

        [Message(text = "Should be equal to the maximum of sounds you want to simultaneously play.",
                 id = "showHelp", value = true)]
        [SerializeField]
        private int soundEventPoolSize = 10;

        [Message(text = "Will call Initialize OnAwake if set to true.",
            id = "showHelp", value = true)]
        [SerializeField]
        private bool autoInitialize = true;

        private SoundEventPool soundEventPool;
        private List<int> curPlayingTraceableSoundIds;
        private List<SoundEvent> curPlayingTraceableSounds;
        private List<int> soundEventsToStop;
        private SoundEventIDGenerator eventIDGenerator;

        private bool mute;

        #region Static	
        private static event SoundPlayerHandler onSoundsMute;
        private static event SoundPlayerHandler onMusicMute;
        private static bool sSoundMute = false;
        private static bool sMusicdMute = false;

        private static float relativeVolume = 1.0f;
        float customVolumeValue;
        SoundInfo soundInfo;
        /// <summary>
        /// Vaule from 0.0f to 1.0f.
        /// </summary>
        public static float RelativeVolume {
            get { return relativeVolume; }
            set {
                relativeVolume = value;
                relativeVolume = Mathf.Clamp01(relativeVolume);
            }
        }

        public static void ToggleSoundsMute(bool mute) {
            sSoundMute = mute;

            if(onSoundsMute != null)
                onSoundsMute(mute);
        }

        public static void ToggleMusicMute(bool mute) {
            sMusicdMute = mute;

            if(onMusicMute != null)
                onMusicMute(mute);
        }
        #endregion

        #region Interfaces

        public Action<int> OnSoundComplete;

        /// <summary>
        /// Play the sound with the specified name.
        /// </summary>
        public int Play(string name,Action<SoundEvent> OnCompleteCallback=null) {
            return Play(name,OnCompleteCallback, null, default(Vector3));
        }

        /// <summary>
        /// Play the sound with the specified name and attach the audio source to a transform (used for 3d audio source).
        /// </summary>
        public int Play(string name, Transform parent,Action<SoundEvent> OnCompleteCallback=null) {
            return Play(name,OnCompleteCallback, parent, default(Vector3));
        }

        /// <summary>
        /// Play the sound at a specific position.
        /// </summary>
        public int Play(string name, Vector3 position,Action<SoundEvent> OnCompleteCallback=null) {
            return Play(name,OnCompleteCallback, null, position);
        }

        /// <summary>
        /// Stop the sound with the specified soundEventID. If no fadeout is specified it will use the SoundInfo fade out property.
        /// </summary>
        public void Stop(int soundEventID, float fadeout = -1f) {
            if(CurrentlyPlayingSoundEvent(soundEventID)) {
                if(!soundEventsToStop.Contains(soundEventID)) {
                    SoundEvent soundEvent = GetSoundEventFromID(soundEventID);
                    soundEvent.Stop(fadeout);
                }
            }
        }

        /// <summary>
        /// Stop the sound with the specified name. If no fadeout is specified it will use the SoundInfo fade out property.
        /// </summary>
        public void Stop(string name, float fadeout = -1f) {
            for(int i = 0; i < curPlayingTraceableSoundIds.Count; i++) {
                int soundEventID = curPlayingTraceableSoundIds[i];
                if(!soundEventsToStop.Contains(soundEventID)) {
                    if(curPlayingTraceableSounds[i].Name == name) {
                        curPlayingTraceableSounds[i].Stop(fadeout);
                    }
                }
            }
        }

        /// <summary>
        /// Mute the sound with the specified name. The sound will keep playing while silent.
        /// </summary>
        public void Mute(string name, float fadeout = -1f) {
            for(int i = 0; i < curPlayingTraceableSoundIds.Count; i++) {
                int soundEventID = curPlayingTraceableSoundIds[i];
                if(!soundEventsToStop.Contains(soundEventID)) {
                    if(curPlayingTraceableSounds[i].Name == name) {
                        curPlayingTraceableSounds[i].Mute(fadeout);
                    }
                }
            }
        }

        /// <summary>
        /// Mute the sound with the specified soundEventID. The sound will keep playing while silent. 
        /// Can be fade out if a value is specifiee
        /// </summary>
        public void Mute(int soundEventID, float fadeout = -1f) {
            if(CurrentlyPlayingSoundEvent(soundEventID)) {
                if(!soundEventsToStop.Contains(soundEventID)) {
                    SoundEvent soundEvent = GetSoundEventFromID(soundEventID);
                    soundEvent.Mute(fadeout);
                }
            }
        }

        /// <summary>
        /// Unmute the sound with the specified name.
        /// </summary>
        public void UnMute(string name, float fadein = -1f) {
            for(int i = 0; i < curPlayingTraceableSoundIds.Count; i++) {
                int soundEventID = curPlayingTraceableSoundIds[i];
                if(!soundEventsToStop.Contains(soundEventID)) {
                    if(curPlayingTraceableSounds[i].Name == name) {
                        curPlayingTraceableSounds[i].UnMute(fadein);
                    }
                }
            }
        }

        /// <summary>
        /// Unmute the sound with the specified id.
        /// </summary>
        public void UnMute(int soundEventID, float fadein = -1f) {
            if(CurrentlyPlayingSoundEvent(soundEventID)) {
                if(!soundEventsToStop.Contains(soundEventID)) {
                    SoundEvent soundEvent = GetSoundEventFromID(soundEventID);
                    soundEvent.UnMute(fadein);
                }
            }
        }

        /// <summary>
        /// Stop all the sounds attached to the SoundPlayer
        /// </summary>
        public void StopAll() {
            for(int i = 0; i < curPlayingTraceableSoundIds.Count; i++) {
                int soundEventID = curPlayingTraceableSoundIds[i];
                if(!soundEventsToStop.Contains(soundEventID)) {
                    curPlayingTraceableSounds[i].Stop();
                }
            }
        }

        /// <summary>
        /// Check if the specified sound is playing
        /// </summary>
        public bool IsSoundPlaying(int soundEventID) {
            if(CurrentlyPlayingSoundEvent(soundEventID)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the specified sound is playing
        /// </summary>
        public bool IsSoundPlaying(string name) {
            for(int i = 0; i < curPlayingTraceableSounds.Count; i++) {
                if(curPlayingTraceableSounds[i].Name == name) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the volume of a specified sound. If stopFade is set to false, the volume will not be modified if
        /// the sound is fading (in or out). If it is set to true, the fading will be stopped and the volume immediatly
        /// set to the volume parameter value.
        /// </summary>
        public void SetVolume(int soundEventID, float volume, bool stopFade = false) {
            if(CurrentlyPlayingSoundEvent(soundEventID)) {
                SoundEvent soundEvent = GetSoundEventFromID(soundEventID);
                soundEvent.SetVolume(volume, stopFade);
            }
        }

        /// <summary>
        /// Gets the volume of the specified sound.
        /// </summary>
        public float GetVolume(int soundEventID) {
            float result = -1f;

            if(CurrentlyPlayingSoundEvent(soundEventID)) {
                SoundEvent soundEvent = GetSoundEventFromID(soundEventID);
                result = soundEvent.GetVolume();
            }

            return result;
        }

        /// <summary>
        /// Determines whether the sound instance is fading in.
        /// </summary>
        public bool IsFadingIn(int soundEventID) {
            bool result = false;

            if(CurrentlyPlayingSoundEvent(soundEventID)) {
                SoundEvent soundEvent = GetSoundEventFromID(soundEventID);
                result = soundEvent.IsFadingIn();
            }

            return result;
        }

        /// <summary>
        /// Determines whether the sound instance is fading out.
        /// </summary>
        public bool IsFadingOut(int soundEventID) {
            bool result = false;

            if(CurrentlyPlayingSoundEvent(soundEventID)) {
                SoundEvent soundEvent = GetSoundEventFromID(soundEventID);
                result = soundEvent.IsFadingOut();
            }

            return result;
        }

        /// <summary>
        /// Get the sound target volume. The target volume is the volume a sound is aiming for when fading in our out.
        /// It is specified in the SoundInfo Custom Volume sound. If Custom Volume is not specified then the value is set
        /// to 1.
        /// </summary>
        public float GetTargetVolume(int soundEventID) {
            float result = -1f;

            if(CurrentlyPlayingSoundEvent(soundEventID)) {
                SoundEvent soundEvent = GetSoundEventFromID(soundEventID);
                result = soundEvent.TargetVolume;
            }

            return result;
        }

        /// <summary>
        /// Sets the pitch of the specified sound.
        /// </summary>
        public void SetPitch(int soundEventID, float pitch) {
            if(CurrentlyPlayingSoundEvent(soundEventID)) {
                SoundEvent soundEvent = GetSoundEventFromID(soundEventID);
                soundEvent.SetPitch(pitch);
            }
        }

        public void SetVolumeForSoundPlayer(float volume, bool stopFade = false) {
            for(int i = 0; i < curPlayingTraceableSounds.Count; i++) {
                curPlayingTraceableSounds[i].SetVolume(volume, stopFade);
            }
        }

        #endregion

        void Awake() { 
            if (this.autoInitialize) {
                Initialize();
            }
        }

        bool initialized = false;
        public virtual void Initialize() {
            if(!initialized) {
                initialized = true;

                if(playMusic) {
                    this.mute = sMusicdMute;
                    SoundPlayer.onMusicMute += this.OnMuteStateChange;
                } else {
                    this.mute = sSoundMute;
                    SoundPlayer.onSoundsMute += this.OnMuteStateChange;
                }

                soundEventPool = new SoundEventPool(soundEventPoolSize);

                for(int i = 0; i < soundLists.Length; i++) {
                    if(soundLists[i].transform.parent != this.transform) {
                        soundLists[i] = (SoundList)GameObject.Instantiate(soundLists[i], Vector3.zero, Quaternion.identity);
                        soundLists[i].transform.parent = this.transform;
                    }

                    soundLists[i].Initialize(soundEventPoolSize);
                }

                curPlayingTraceableSoundIds = new List<int>(soundEventPoolSize);
                curPlayingTraceableSounds = new List<SoundEvent>(soundEventPoolSize);

                soundEventsToStop = new List<int>(soundEventPoolSize);
                eventIDGenerator = new SoundEventIDGenerator();
            }
        }

        void OnDestroy() {
            if(playMusic) {
                SoundPlayer.onMusicMute -= this.OnMuteStateChange;
            } else {
                SoundPlayer.onSoundsMute -= this.OnMuteStateChange;
            }
        }

        void Update() {
            if(initialized) {
                UpdateSoundEvents();
            }
        }

        void LateUpdate() {
            if(initialized) {
                CleanCompletedSoundEvents();
            }
        }

        private void UpdateSoundEvents() {
            for(int i = 0; i < curPlayingTraceableSounds.Count; i++) {
                curPlayingTraceableSounds[i].Update();
            }
        }

        private void CleanCompletedSoundEvents() {
            for(int i = 0; i < soundEventsToStop.Count; i++) {
                int soundEventID = soundEventsToStop[i];
                SoundEvent soundEvent = GetSoundEventFromID(soundEventID);

                soundEvent.Sound.RecycleAudioSource();
                soundEventPool.Recycle(soundEvent);

                RemoveSoundFromTraceableSounds(soundEventID);

                if(OnSoundComplete != null) {
                    OnSoundComplete(soundEventID);
                }
            }

            if(soundEventsToStop.Count > 0) {
                soundEventsToStop.Clear();
            }
        }

        private void AddSoundToTraceableSounds(int soundEventID, SoundEvent soundEvent) {
            curPlayingTraceableSoundIds.Add(soundEventID);
            curPlayingTraceableSounds.Add(soundEvent);
        }

        private void RemoveSoundFromTraceableSounds(int soundEventID) {
            int findId = curPlayingTraceableSoundIds.FindIndex(e => e == soundEventID);
            if(findId != -1) {
                curPlayingTraceableSounds.RemoveAt(findId);
                curPlayingTraceableSoundIds.RemoveAt(findId);
            }
        }

        private bool CurrentlyPlayingSoundEvent(int soundEventID) {
            return curPlayingTraceableSoundIds.FindIndex(e => e == soundEventID) != -1;
        }

        private SoundEvent GetSoundEventFromID(int soundEventID) {
            int findId = curPlayingTraceableSoundIds.FindIndex(e => e == soundEventID);
            return findId != -1 ? curPlayingTraceableSounds[findId] : null;
        }

        private int Play(string name,Action<SoundEvent> OnCompleteCallback=null, Transform parent = null, Vector3 position = new Vector3()) {
            int soundEventID = -1;
            Sound sound = null;

            for(int i = 0; i < soundLists.Length; i++) {
                if (soundLists[i] == null) break;
                sound = soundLists[i].GetSound(name);
                if(sound != null) {
                    break;
                }
            }

            if(sound != null) {
                soundEventID = StartSoundEvent(sound, parent, position,OnCompleteCallback);
            }

            return soundEventID;
        }
        public void SetCustomVolume(float customVolumeValue)
        {
            this.customVolumeValue = customVolumeValue;
            if (this.soundInfo!=null)
            {
                this.soundInfo.customVolume = true;
                this.soundInfo.volume = this.customVolumeValue;
                SetVolumeForSoundPlayer(this.soundInfo.volume, true);
            }
        }
        private int StartSoundEvent(Sound sound, Transform parent, Vector3 position,Action<SoundEvent> OnCompleteCallback=null) {
            int soundEventID = eventIDGenerator.GetID();
            SoundEvent soundEvent = soundEventPool.GetInstance();
            soundEvent.onComplete += this.OnSoundEventComplete;

            if (OnCompleteCallback != null)
                soundEvent.onComplete += OnCompleteCallback;
            this.soundInfo = sound.soundInfo;
            sound.soundInfo.customVolume = true;
            sound.soundInfo.volume = this.customVolumeValue;
            soundEvent.Start(soundEventID, sound, parent, position, mute);
            AddSoundToTraceableSounds(soundEventID, soundEvent);

            return soundEventID;
        }

        void OnSoundEventComplete(SoundEvent soundEvent) {
            if(curPlayingTraceableSounds.Find(e => e.EventID == soundEvent.EventID) != null) {
                soundEventsToStop.Add(soundEvent.EventID); 
                soundEvent.onComplete = null;
            }
        }

        void OnMuteStateChange(bool mute) {
            if(this.mute != mute) {
                for(int i = 0; i < curPlayingTraceableSounds.Count; i++) {
                    curPlayingTraceableSounds[i].ToggleMute(mute);
                }

                this.mute = mute;
            }
        }
    }
}