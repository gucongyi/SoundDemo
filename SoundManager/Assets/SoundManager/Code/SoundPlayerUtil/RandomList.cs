using System.Collections.Generic;
using UnityEngine;


namespace HYZ {
    namespace SoundInternal {
        /// <summary>
        /// List that can be randomly picked from until it's empty, then it will refill again
        /// </summary>
        /// <typeparam name="T">Object type to store in the list</typeparam>
        public class RandomList<T> {
            List<T> list;

            List<T> source;

            /// <summary>
            /// If true will print log information
            /// </summary>
            public bool debug = false;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="_source">List with objects to refill the list</param>
            public RandomList(List<T> _source) {
                source = _source;

                list = new List<T>(source.Capacity);

                Reset();
            }

            /// <summary>
            /// Force empty the list and refill again
            /// </summary>
            public void Reset() {
                list.Clear();

                list.AddRange(source);
            }

            /// <summary>
            /// Get random item from the list
            /// </summary>
            /// <returns></returns>
            public T GetAny() {
                int index = Random.Range(0, list.Count - 1);

                T ret = list[index];

                list.RemoveAt(index);

                if(debug) {
                    //Debug.Log("randomlist ret " + ret);
                }

                if(list.Count == 0) {
                    if(debug) {
                        //Debug.Log("randomlist reset");
                    }

                    Reset();
                }

                return ret;
            }

            /// <summary>
            /// Get list of objects used to refill
            /// </summary>
            /// <returns></returns>
            public List<T> GetSource() {
                return new List<T>(source);
            }

            /// <summary>
            /// Get list of objects that are currently remaining for picking
            /// </summary>
            /// <returns></returns>
            public List<T> GetRemaining() {
                return new List<T>(list);
            }

            /// <summary>
            /// Remove object from the currently remaining objects. Note that the object will be added again during a refill.
            /// </summary>
            /// <param name="option"></param>
            public void RemoveFromRemaining(T option) {
                list.Remove(option);
            }

            /// <summary>
            /// Add object to the currently remaining item. Note that this object will not be added on the next refill.
            /// </summary>
            /// <param name="option"></param>
            public void AddToRemaining(T option) {
                list.Add(option);
            }
        }
    }
}