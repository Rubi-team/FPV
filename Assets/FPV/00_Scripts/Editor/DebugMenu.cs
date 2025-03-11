// Classe Editor pour ajouter le menu "Debug"

using FPV;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using Console = FPV.Console;

namespace FPV
{
    [InitializeOnLoad]
    public static class DebugConsoleButton
    {
        static DebugConsoleButton()
        {
            EditorApplication.delayCall += AddButtonToConsole;
        }

        private static void AddButtonToConsole()
        {
            // Get the Console Window type
            Type consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            if (consoleWindowType == null) return;

            EditorWindow consoleWindow = EditorWindow.GetWindow(consoleWindowType);
            if (consoleWindow == null) return;

            // Get the root UI element of the Console window
            var root = consoleWindow.rootVisualElement;
            if (root == null) return;

            // Find the toolbar container
            var toolbar = root.Q("Toolbar") ?? root.Q("topToolbarContainer") ?? root;
            if (toolbar == null) return;

            // Check if the button already exists
            if (toolbar.Q<Button>("DebugFilterButton") != null) return;

            // Create the button
            Button debugButton = null;
            debugButton = new Button(() =>
            {
                ShowDebugDropdown(debugButton);
            })
            {
                text = "⚙️ Debug",
                name = "DebugFilterButton"
            };

            // Apply styles to keep it small and aligned
            debugButton.style.width = 80;     // Fixed width
            debugButton.style.height = 22;    // Match Unity button size
            debugButton.style.marginLeft = 4; // Small spacing
            debugButton.style.flexShrink = 0; // Prevent stretching
            debugButton.style.alignSelf = Align.Center; // Center in the toolbar

            // Add the button at the end of the toolbar
            toolbar.Add(debugButton);

            // Force UI refresh
            consoleWindow.Repaint();
        }

        private static void ShowDebugDropdown(Button button)
        {
            GenericMenu menu = new GenericMenu();

            string[] categories = { "UI", "Player", "UnityService" };
            string[] logLevels = { "Log", "Warning", "Error" };

            foreach (var category in categories)
            {
                // Check if all logs in this category are enabled
                bool allEnabled = true;
                foreach (var level in logLevels)
                {
                    string key = $"Debug_{category}_{level}";
                    if (!EditorPrefs.GetBool(key, true))
                    {
                        allEnabled = false;
                        break;
                    }
                }

                // "Toggle All" option
                menu.AddItem(new GUIContent($"{category} / Toggle All"), allEnabled, () =>
                {
                    bool newState = !allEnabled; // Flip state
                    foreach (var level in logLevels)
                    {
                        string key = $"Debug_{category}_{level}";
                        EditorPrefs.SetBool(key, newState);
                    }
                    Debug.Log($"[Debug Settings] {category}: {(newState ? "Enabled All" : "Disabled All")}");
                });

                // Individual log level toggles
                foreach (var level in logLevels)
                {
                    string key = $"Debug_{category}_{level}";
                    bool isEnabled = EditorPrefs.GetBool(key, true);
                    menu.AddItem(new GUIContent($"{category} / {level}"), isEnabled, () =>
                    {
                        bool newState = !EditorPrefs.GetBool(key, true);
                        EditorPrefs.SetBool(key, newState);
                        Debug.Log($"[Debug Settings] {category} - {level}: {(newState ? "Enabled" : "Disabled")}");
                    });
                }

                menu.AddSeparator(""); // Separate categories
            }

            menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }
    }
}


