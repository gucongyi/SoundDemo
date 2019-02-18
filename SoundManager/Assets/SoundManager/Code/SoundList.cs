using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyEditor;

namespace HYZ {
    public class SoundList : MonoBehaviour {
#pragma warning disable 414
        [SerializeField]
        bool showHelp = false;
#pragma warning restore 414
        public bool advanced = false;

        [Message("Audio source used for SoundInfo that does not hold special effect and" +
            " can use the game standard audio source (most of the sounds)",
                 messageType = EasyEditor.MessageType.Info, id = "showHelp", value = true)]
        public AudioSource standardAudioSource;

        [Visibility("advanced", true)]
        [Message("Size of the pool containing standard audio sources. A value of -1 means that the pool will have the same size" +
            " as the SoundPlayer sound event pool.",
                 messageType = EasyEditor.MessageType.Info, id = "showHelp", value = true)]
        public int poolSize = -1;

        [Message("Sounds are looked-up by the asset name in the Project window",
                 messageType = EasyEditor.MessageType.Info, id = "showHelp", value = true)]
        public SoundInfo[] sounds;

        private Dictionary<SoundInfo, AudioSourcePool> soundPoolDic;

        public void Initialize(int standardPoolSize) {
            if(poolSize == -1) {
                poolSize = standardPoolSize;
            }

            InitializeAudioSourcePools();
        }

        private void InitializeAudioSourcePools() {
            soundPoolDic = new Dictionary<SoundInfo, AudioSourcePool>(sounds.Length);

            AudioSourcePool standardAudioSourcePool = new AudioSourcePool(standardAudioSource, poolSize, this.transform);

            for(int i = 0; i < sounds.Length; i++) {
                SoundInfo sound = sounds[i];
                if(sound.audioSource != null) {
                    soundPoolDic.Add(sound, new AudioSourcePool(sound.audioSource, sound.poolSize, this.transform));
                } else {
                    soundPoolDic.Add(sound, standardAudioSourcePool);
                }
            }
        }

        public Sound GetSound(string soundName) {
            Sound result = null;

            SoundInfo soundInfo = GetSoundInfo(soundName);

            if(soundInfo != null) {
                AudioSource audioSource = soundPoolDic[soundInfo].GetInstance();
                result = new Sound(soundInfo, audioSource, soundPoolDic[soundInfo]);
            }

            return result;
        }

        private SoundInfo GetSoundInfo(string soundName) {
            SoundInfo result = null;
            for(int i = 0; i < sounds.Length; i++) {
                if(sounds[i]==null)continue;
                if(sounds[i].name == soundName) {
                    result = sounds[i];
                    break;
                }
            }

            return result;
        }
    }

    public class AudioSourcePool {
        private AudioSource prefab;
        private List<AudioSource> pool;
        private int size;
        private Transform parent;

        public AudioSourcePool(AudioSource targetPrefab, int poolSize, Transform parent) {
            prefab = targetPrefab;
            size = poolSize;
            this.parent = parent;

            pool = new List<AudioSource>();
            for(int i = 0; i < size; ++i) {
                AudioSource audioSource = GameObject.Instantiate<AudioSource>(prefab);
                audioSource.transform.parent = parent;
                pool.Add(audioSource);
            }
        }

        public AudioSource GetInstance() {
            if(pool.Count > 0) {
                AudioSource audioSrc = pool[0];
                pool.RemoveAt(0);

                return audioSrc;
            }

            AudioSource audioSource = GameObject.Instantiate<AudioSource>(prefab);
            audioSource.transform.parent = parent;

            return audioSource;
        }

        public void Recycle(AudioSource audioSrc) {
            if(pool.Count < size) {
                pool.Add(audioSrc);
            } else {
                GameObject.Destroy(audioSrc.gameObject);
            }
        }
    }
}