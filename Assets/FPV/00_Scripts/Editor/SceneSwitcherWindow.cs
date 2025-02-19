using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EditorScripts
{
    public class SceneSwitcherWindow : EditorWindow
    {
        private string[] allScenePaths;
        private Dictionary<string, bool> favoriteScenes = new Dictionary<string, bool>();
        private Vector2 scrollPosition;

        [MenuItem("Tools/SceneSwitcher")]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneSwitcherWindow>();
            window.titleContent = new GUIContent("Scene Switcher");
            window.Show();
        }

        private void OnEnable()
        {
            LoadFavorites();
            RefreshScenes();
        }

        private void LoadFavorites()
        {
            foreach (string scenePath in AssetDatabase.FindAssets("t:Scene").Select(AssetDatabase.GUIDToAssetPath))
            {
                if (!favoriteScenes.ContainsKey(scenePath))
                {
                    favoriteScenes[scenePath] = EditorPrefs.GetBool($"FavoriteScene_{scenePath}", false);
                }
            }
        }

        private void SaveFavorites()
        {
            foreach (var kvp in favoriteScenes)
            {
                EditorPrefs.SetBool($"FavoriteScene_{kvp.Key}", kvp.Value);
            }
        }

        private void RefreshScenes()
        {
            allScenePaths = AssetDatabase.FindAssets("t:Scene")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !path.Contains("/Others/") && !path.StartsWith("Packages/"))
                .ToArray();
        }

        private void OnGUI()
        {
            if (allScenePaths == null || allScenePaths.Length == 0)
            {
                EditorGUILayout.HelpBox("No scenes found. Please add scenes to the project.", MessageType.Info);
                if (GUILayout.Button("Refresh"))
                {
                    RefreshScenes();
                }
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            var sortedScenes = allScenePaths.OrderByDescending(path => favoriteScenes.ContainsKey(path) && favoriteScenes[path]).ToArray();

            foreach (string scenePath in sortedScenes)
            {
                GUILayout.BeginHorizontal();
                
                bool isFavorite = favoriteScenes.ContainsKey(scenePath) && favoriteScenes[scenePath];
                bool newFavorite = GUILayout.Toggle(isFavorite, "", GUILayout.Width(20));
                if (newFavorite != isFavorite)
                {
                    favoriteScenes[scenePath] = newFavorite;
                    SaveFavorites();
                }
                
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (GUILayout.Button(sceneName, GUILayout.Width(150), GUILayout.Height(30)))
                {
                    OpenScene(scenePath);
                }
                
                GUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
            
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh Scenes", GUILayout.Width(150f), GUILayout.Height(30f)))
            {
                RefreshScenes();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
    }
}
