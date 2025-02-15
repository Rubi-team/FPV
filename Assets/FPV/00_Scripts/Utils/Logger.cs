using UnityEngine;

namespace Utils
{
    [AddComponentMenu("_Ase/Utils/Logger")]
    public class Logger : MonoBehaviour
    {

        [Header("Settings")] [SerializeField] private bool _showLogs;

        public void Log(object message, Object sender = null)
        {
            if (_showLogs)
            {
                Debug.Log(message, sender);
            }
        }

        public void WarningLog(object message, Object sender = null)
        {
            if (_showLogs)
            {
                Debug.LogWarning(message, sender);
            }
        }

        public void ErrorLog(object message, Object sender = null)
        {
            if (_showLogs)
            {
                Debug.LogError(message, sender);
            }
        }
    }
}

