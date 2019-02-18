using UnityEngine;
using System.Collections;

namespace HYZ {
    public class Sound {
        public SoundInfo soundInfo;
        public AudioSource audioSource;
        public AudioSourcePool audioSourcePool;

        public Sound(SoundInfo soundinfo, AudioSource audioSource, AudioSourcePool audioSourcePool) {
            this.soundInfo = soundinfo;
            this.audioSource = audioSource;
            this.audioSourcePool = audioSourcePool;
        }

        public void RecycleAudioSource() {
            audioSourcePool.Recycle(this.audioSource);
        }
    }
}