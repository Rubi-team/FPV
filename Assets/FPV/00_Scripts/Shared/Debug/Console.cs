using System.Diagnostics;
using UnityEditor;

namespace UnityEngine
{
    public static class Console
    {
        private const string infoColor = nameof(Color.white);
        private const string warningColor = nameof(Color.yellow);
        private const string errorColor = nameof(Color.red);

        // Liste des catégories et des niveaux de log
        public static readonly string[] Categories = { "UI", "Player", "UnityService" };
        public static readonly string[] LogLevels = { "Log", "Warning", "Error" };

        // Vérifie si un log doit être affiché
        private static bool IsLogEnabled(string category, string logType)
        {
            return EditorPrefs.GetBool($"Debug_{category}_{logType}", true);
        }

        [Conditional("DEBUG")]
        public static void Log(string category, object message)
        {
            if (IsLogEnabled(category, "Log"))
                Debug.Log(FormatMessageWithCategory(infoColor, category, message));
        }

        [Conditional("DEBUG")]
        public static void LogWarning(string category, object message)
        {
            if (IsLogEnabled(category, "Warning"))
                Debug.LogWarning(FormatMessageWithCategory(warningColor, category, message));
        }

        [Conditional("DEBUG")]
        public static void LogError(string category, object message)
        {
            if (IsLogEnabled(category, "Error"))
                Debug.LogError(FormatMessageWithCategory(errorColor, category, message));
        }

        private static string FormatMessageWithCategory(string color, string category, object message)
        {
            return $"<color={color}><b>[{category}]</b> {message}</color>";
        }
    }
    
}
