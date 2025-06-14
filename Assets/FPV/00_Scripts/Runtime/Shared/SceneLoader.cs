using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace FPV.Shared
{
    public static class SceneLoader
    {
        private static List<AsyncOperation> asyncLoads = new();

        public enum Scene
        {
            LoadingScene,
            Metagame,
            Game_Lobby,
            Game_Main,
            Game_Blocking,
            Game_LevelArt
        }

        public static async Task LoadScenesAdditiveAsync(Scene mainScene, Scene[] additiveScenes)
        {
            asyncLoads.Clear();

            // 1. Charger la scène de loading (Single)
            var loadingOp = SceneManager.LoadSceneAsync(nameof(Scene.LoadingScene), LoadSceneMode.Single);
            await loadingOp.AsTask();

            // 2. Lancer la scène principale en additive
            var mainLoadOp = SceneManager.LoadSceneAsync(mainScene.ToString(), LoadSceneMode.Additive);
            asyncLoads.Add(mainLoadOp);

            // 3. Lancer les scènes additives
            foreach (var scene in additiveScenes)
            {
                var op = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                asyncLoads.Add(op);
            }

            // 4. Attendre que tout soit chargé
            foreach (var op in asyncLoads) await op.AsTask();

            // 5. Définir la scène principale comme active
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainScene.ToString()));

            // 6. Décharger la scène de Loading
            var loadingScene = SceneManager.GetSceneByName(nameof(Scene.LoadingScene));
            if (loadingScene.IsValid() && loadingScene.isLoaded) SceneManager.UnloadSceneAsync(loadingScene);
        }

        public static float GetLoadingProgress()
        {
            if (asyncLoads.Count == 0) return 1f;

            var totalProgress = 0f;
            foreach (var operation in asyncLoads) totalProgress += operation.progress;
            return totalProgress / asyncLoads.Count;
        }

        private static Task AsTask(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<bool>();
            asyncOp.completed += _ => tcs.TrySetResult(true);
            return tcs.Task;
        }
    }
}