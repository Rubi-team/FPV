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
            var consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            if (consoleWindowType == null) return;

            var consoleWindow = EditorWindow.GetWindow(consoleWindowType);
            if (consoleWindow == null) return;

            var root = consoleWindow.rootVisualElement;
            if (root == null) return;

            var toolbar = root.Q("Toolbar") ?? root.Q("topToolbarContainer") ?? root;
            if (toolbar == null) return;

            if (toolbar.Q<Button>("DebugFilterButton") != null) return;

            Button debugButton = null;
            debugButton = new Button(() => { ShowDebugDropdown(debugButton); })
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
            var menu = new GenericMenu();

            var categories = GetCategories();
            string[] logLevels = { "Log", "Warning", "Error" };

            foreach (var category in categories)
            {
                var allEnabled = true;
                foreach (var level in logLevels)
                {
                    var key = $"Debug_{category}_{level}";
                    if (!EditorPrefs.GetBool(key, true))
                    {
                        allEnabled = false;
                        break;
                    }
                }

                menu.AddItem(new GUIContent($"{category} / Toggle All"), allEnabled, () =>
                {
                    var newState = !allEnabled;
                    foreach (var level in logLevels)
                    {
                        var key = $"Debug_{category}_{level}";
                        EditorPrefs.SetBool(key, newState);
                    }

                    Debug.Log($"[Debug Settings] {category}: {(newState ? "Enabled All" : "Disabled All")}");
                });

                foreach (var level in logLevels)
                {
                    var key = $"Debug_{category}_{level}";
                    var isEnabled = EditorPrefs.GetBool(key, true);
                    menu.AddItem(new GUIContent($"{category} / {level}"), isEnabled, () =>
                    {
                        var newState = !EditorPrefs.GetBool(key, true);
                        EditorPrefs.SetBool(key, newState);
                        Debug.Log($"[Debug Settings] {category} - {level}: {(newState ? "Enabled" : "Disabled")}");
                    });
                }

                menu.AddItem(new GUIContent($"{category} / ❌ Remove Category"), false,
                    () => { RemoveCategory(category); });

                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent("➕ Add Category..."), false, () => { DebugCategoryWindow.ShowWindow(); });


            menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
        }

        private static List<string> GetCategories()
        {
            var saved = EditorPrefs.GetString(CategoriesKey, "UI,Player,UnityService");
            return new List<string>(saved.Split(','));
        }

        private static void SaveCategories(List<string> categories)
        {
            EditorPrefs.SetString(CategoriesKey, string.Join(",", categories));
        }

        public static void AddCategory(string category)
        {
            var categories = GetCategories();
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
            var categories = GetCategories();
            if (categories.Contains(category))
            {
                categories.Remove(category);
                SaveCategories(categories);

                string[] logLevels = { "Log", "Warning", "Error" };
                foreach (var level in logLevels)
                {
                    var key = $"Debug_{category}_{level}";
                    EditorPrefs.DeleteKey(key);
                }

                Debug.Log($"[Debug Settings] Removed category: {category}");
            }
        }
    }
}