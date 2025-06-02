using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace FPV.Shared
{
    public static class SceneLoader
    {
        private class LoadingMonoBehaviour : MonoBehaviour { }

        private static List<AsyncOperation> asyncLoads = new List<AsyncOperation>();

        public enum Scene
        {
            Loading,
            Metagame,
            Game_Lobby,
            Game_Main,
            Game_Blocking,
            Game_LevelArt
        }

        public static async Task LoadScenesAdditiveAsync(Scene mainScene, Scene[] additiveScenes)
        {
            asyncLoads.Clear();

            // Load main scene
            var mainLoadOperation = SceneManager.LoadSceneAsync(mainScene.ToString(), LoadSceneMode.Single);
            asyncLoads.Add(mainLoadOperation);
        
            // Attend que la scène principale soit chargée
            await mainLoadOperation.AsTask();

            // Load additive scenes
            foreach (var scene in additiveScenes)
            {
                var asyncLoad = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                asyncLoads.Add(asyncLoad);
            }

            // Attend que toutes les scènes additives soient chargées
            foreach (var operation in asyncLoads.Skip(1)) // Skip la scène principale qui est déjà chargée
            {
                await operation.AsTask();
            }
        }

        public static float GetLoadingProgress()
        {
            if (asyncLoads.Count == 0) return 1f;

            float totalProgress = 0f;
            foreach (var operation in asyncLoads)
            {
                totalProgress += operation.progress;
            }
            return totalProgress / asyncLoads.Count;
        }

        // Helper extension method pour convertir AsyncOperation en Task
        private static Task AsTask(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<bool>();
            asyncOp.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}