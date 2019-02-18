using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HYZ {
    namespace SoundInternal {
        public class SoundEventPool {
            private List<SoundEvent> pool;
            private int size;

            public SoundEventPool(int poolSize) {
                size = poolSize;

                pool = new List<SoundEvent>();
                for(int i = 0; i < size; ++i) {
                    pool.Add(new SoundEvent());
                }
            }

            public SoundEvent GetInstance() {
                if(pool.Count > 0) {
                    SoundEvent instance = pool[0];
                    pool.RemoveAt(0);

                    return instance;
                }

                return new SoundEvent();
            }

            public void Recycle(SoundEvent soundEvent) {
                if(pool.Count < size) {
                    pool.Add(soundEvent);
                }
            }
        }
    }
}