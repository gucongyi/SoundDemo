using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyEditor;

namespace HYZ {
    public class SoundInfo : ScriptableObject {

#pragma warning disable 414
        [SerializeField]
        bool showHelp = false;
#pragma warning restore 414

        public List<AudioClip> audioClips = new List<AudioClip>();
        private SoundInternal.RandomList<AudioClip> randomAudioClips;

        [Inspector(group = "Sound Basic Parameters")]
        public bool looping = false;
        public float delayAtStart = 0f;

        [Visibility("looping", true)]
        public bool randomClipWhenLooping = false;
        [Visibility("looping", true)]
        public bool delayAtNextStartWhenLooping = false;
        [Visibility("looping", true)]
        public bool ignoreDelayAtFirstWhenLooping = false;

        [Inspector(group = "Sound Customization")]
        public bool fading = false;
        [BeginHorizontal]
        [Visibility("fading", true)]
        public float fadeInTime = 0f;
        [EndHorizontal]
        [Visibility("fading", true)]
        public float fadeOutTime = 0f;

        [Space(10f)]
        [Message(text = "If not modified, default volume is 1.",
                 id = "showHelp", value = true)]
        public bool customVolume = false;
        [Visibility("customVolume", true)]
        [Range(0f, 1f)]
        public float volume = 1f;

        [Space(10f)]
        public bool randomPitch = false;
        [BeginHorizontal]
        [Visibility("randomPitch", true)]
        public float minPitch = 1f;
        [EndHorizontal]
        [Visibility("randomPitch", true)]
        public float maxPitch = 1f;

        [Inspector(group = "Specific Source")]
        [Message(text = "If you want to set some special settings on this sound you can drag an Audio Source here",
                 id = "showHelp", value = true)]
        public AudioSource audioSource;
        [Visibility("AudioSourceNotNull")]
        [Message(text = "If you can predict how many copy of this sound can be played at the same time, set the size of this pool " +
            "to avoid instanciation at runtime.",
                 id = "showHelp", value = true)]
        public int poolSize;

        private bool AudioSourceNotNull() {
            return audioSource != null;
        }

        public AudioClip GetAudioClip() {
            if(randomAudioClips == null) {
                randomAudioClips = new SoundInternal.RandomList<AudioClip>(audioClips);
            }

            return randomAudioClips.GetAny();
        }

        public float GetVolume() {
            if(customVolume) {
                return volume;
            } else {
                return 1.0f;
            }
        }

        public float GetRandomPitch() {
            return Random.Range(minPitch, maxPitch);
        }
    }
}