using System;
using System.Collections.Generic;
using FPV.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FPV
{
    [InitializeOnLoad]
    public static class DebugConsoleButton
    {
        private static readonly string CategoriesKey = "Debug_Categories";

        static DebugConsoleButton()
        {
            EditorApplication.delayCall += AddButtonToConsole;
        }

        private static void AddButtonToConsole()
        {
            Type consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            if (consoleWindowType == null) return;

            EditorWindow consoleWindow = EditorWindow.GetWindow(consoleWindowType);
            if (consoleWindow == null) return;

            var root = consoleWindow.rootVisualElement;
            if (root == null) return;

            var toolbar = root.Q("Toolbar") ?? root.Q("topToolbarContainer") ?? root;
            if (toolbar == null) return;

            if (toolbar.Q<Button>("DebugFilterButton") != null) return;

            Button debugButton = null;
            debugButton = new Button(() =>
            {
                ShowDebugDropdown(debugButton);
            })
            {
                text = "⚙️ Debug",
                name = "DebugFilterButton"
            };

            debugButton.style.width = 80;
            debugButton.style.height = 22;
            debugButton.style.marginLeft = 4;
            debugButton.style.flexShrink = 0;
            debugButton.style.alignSelf = Align.Center;

            toolbar.Add(debugButton);
            consoleWindow.Repaint();
        }

        private static void ShowDebugDropdown(Button button)
        {
            GenericMenu menu = new GenericMenu();

            List<string> categories = GetCategories();
            string[] logLevels = { "Log", "Warning", "Error" };

            foreach (var category in categories)
            {
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

                menu.AddItem(new GUIContent($"{category} / Toggle All"), allEnabled, () =>
                {
                    bool newState = !allEnabled;
                    foreach (var level in logLevels)
                    {
                        string key = $"Debug_{category}_{level}";
                        EditorPrefs.SetBool(key, newState);
                    }
                    Debug.Log($"[Debug Settings] {category}: {(newState ? "Enabled All" : "Disabled All")}");
                });

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

                menu.AddItem(new GUIContent($"{category} / ❌ Remove Category"), false, () =>
                {
                    RemoveCategory(category);
                });

                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent("➕ Add Category..."), false, () =>
            {
                DebugCategoryWindow.ShowWindow();
            });


            menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        private static List<string> GetCategories()
        {
            string saved = EditorPrefs.GetString(CategoriesKey, "UI,Player,UnityService");
            return new List<string>(saved.Split(','));
        }

        private static void SaveCategories(List<string> categories)
        {
            EditorPrefs.SetString(CategoriesKey, string.Join(",", categories));
        }

        private static void AddCategory(string category)
        {
            List<string> categories = GetCategories();
            if (!categories.Contains(category))
            {
                categories.Add(category);
                SaveCategories(categories);
                Debug.Log($"[Debug Settings] Added new category: {category}");
            }
            else
            {
                Debug.LogWarning($"[Debug Settings] Category '{category}' already exists.");
            }
        }

        private static void RemoveCategory(string category)
        {
            List<string> categories = GetCategories();
            if (categories.Contains(category))
            {
                categories.Remove(category);
                SaveCategories(categories);

                string[] logLevels = { "Log", "Warning", "Error" };
                foreach (var level in logLevels)
                {
                    string key = $"Debug_{category}_{level}";
                    EditorPrefs.DeleteKey(key);
                }

                Debug.Log($"[Debug Settings] Removed category: {category}");
            }
        }

        private static string PromptCategoryName()
        {
            return EditorUtility.DisplayDialogComplex("New Debug Category", "Enter a new category name:", "OK", "Cancel", "") == 0
                ? EditorUtility.DisplayDialog("New Category", "Type the category name in the Console", "OK")
                    ? System.Console.ReadLine() // Fake input, Unity doesn't support text input directly here
                    : ""
                : "";
        }
    }
}
