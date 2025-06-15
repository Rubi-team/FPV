using UnityEngine;

namespace FPV
{
    public class MainCamera : MonoBehaviour
    {
        // Make it dontdestroy on load and an instance of this class, if it already exists we destroy this one
        private static MainCamera _instance;

        public static MainCamera Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MainCamera>();
                    if (_instance == null)
                    {
                        var obj = new GameObject("MainCamera");
                        _instance = obj.AddComponent<MainCamera>();
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}