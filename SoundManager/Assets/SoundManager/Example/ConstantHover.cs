using UnityEngine;
using System.Collections;

namespace HYZ {
    namespace SoundExample {
        /// <summary>
        /// Component to make the gameobject gently hover in Y direction according to a sine function
        /// </summary>
        public class ConstantHover : MonoBehaviour {
            /// <summary>
            /// If should start at random position in the sine, otherwise starts at 0
            /// </summary>
            public bool startAtRandomDistance = false;

            /// <summary>
            /// Original local Y position, can be used if you need to move the object locally
            /// </summary>
            public float origY = 0;

            /// <summary>
            /// Maximum distance it can hover
            /// </summary>
            public float hoverDistance = 0.3f;

            /// <summary>
            /// Time it should take to hover 1 time up and 1 time down
            /// </summary>
            public float timePerHover = 1f;

            private float startTime = 0;

            private bool on = true;

            private float lastTimeDelta = 0;

            // Use this for initialization
            void Start() {
                Reset();
            }

            // Update is called once per frame
            void Update() {
                if(!on) {
                    return;
                }

                Vector3 position = transform.localPosition;

                lastTimeDelta = (Time.time - startTime);

                position.y = origY + Mathf.Sin((lastTimeDelta / timePerHover) * (Mathf.PI * 2f)) * hoverDistance;

                transform.localPosition = position;
            }

            /// <summary>
            /// Restart the hovering
            /// </summary>
            public void Reset() {
                startTime = Time.time;

                if(startAtRandomDistance) {
                    startTime += Random.Range(0, timePerHover);
                }

                origY = transform.localPosition.y;
            }

            /// <summary>
            /// Toggle hovering on or off
            /// </summary>
            /// <param name="_on"></param>
            public void Toggle(bool _on) {
                on = _on;

                if(on) {
                    startTime = Time.time - lastTimeDelta;
                }
            }
            /// <summary>
            /// If hovering is on
            /// </summary>
            /// <returns></returns>
            public bool IsOn() {
                return on;
            }
        }
    }
}