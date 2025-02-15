using System;
using UnityEngine;

namespace Utils
{
    public class BaseInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        // A script to inherit from to make a singleton

        public static T Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
            OnAwake();
        }

        private void OnAwake()
        {
            
        }
    }
}