using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils
{
    [Flags]
    public enum LogLevel
    {
        Logs = 1, // 001
        Warnings = 2, // 010
        Errors = 4 // 100
    }

    [AddComponentMenu("_Ase/Utils/Logger")]
    public class LogHandler : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private LogLevel logLevel;

        public void Log(object message, Object sender = null)
        {
            if ((logLevel & LogLevel.Logs) != 0) Debug.Log(message, sender);
        }

        public void Warning(object message, Object sender = null)
        {
            if ((logLevel & LogLevel.Warnings) != 0) Debug.LogWarning(message, sender);
        }

        public void Error(object message, Object sender = null)
        {
            if ((logLevel & LogLevel.Errors) != 0) Debug.LogError(message, sender);
        }
    }
}