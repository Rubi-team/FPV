// Classe Editor pour ajouter le menu "Debug"

using FPV;
using UnityEditor;
using UnityEngine;

public class DebugMenu
{
    [MenuItem("Debug/Log Settings", priority = 0)]
    public static void ShowDebugSettings()
    {
        DebugSettingsWindow.ShowWindow();
    }
}

// Fenêtre d'options pour le filtrage des logs
public class DebugSettingsWindow : EditorWindow
{
    private static bool[,] debugSettings;

    public static void ShowWindow()
    {
        var window = GetWindow<DebugSettingsWindow>("Debug Settings");
        window.Show();
    }

    private void OnEnable()
    {
        // Charger les préférences
        debugSettings = new bool[Console.Categories.Length, Console.LogLevels.Length];
        for (int i = 0; i < Console.Categories.Length; i++)
        {
            for (int j = 0; j < Console.LogLevels.Length; j++)
            {
                string key = $"Debug_{Console.Categories[i]}_{Console.LogLevels[j]}";
                debugSettings[i, j] = EditorPrefs.GetBool(key, true);
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Debug Log Settings", EditorStyles.boldLabel);

        for (int i = 0; i < Console.Categories.Length; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Console.Categories[i], GUILayout.Width(150));

            for (int j = 0; j < Console.LogLevels.Length; j++)
            {
                bool newValue = GUILayout.Toggle(debugSettings[i, j], Console.LogLevels[j], GUILayout.Width(80));
                if (newValue != debugSettings[i, j])
                {
                    debugSettings[i, j] = newValue;
                    EditorPrefs.SetBool($"Debug_{Console.Categories[i]}_{Console.LogLevels[j]}", newValue);
                }
            }

            GUILayout.EndHorizontal();
        }
    }
}