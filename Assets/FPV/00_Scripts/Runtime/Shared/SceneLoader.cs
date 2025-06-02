using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPV.Shared
{
    public static class SceneLoader
    {
        private class LoadingMonoBehaviour : MonoBehaviour
        {
        }

        private static AsyncOperation asyncLoad;

        public enum Scene
        {
            Loading,
            Metagame,
            Game_Lobby,
            Game_Main,
            Game_Blocking,
            Game_LevelArt
        }

        private static Action m_LoaderCallback;

        public static void LoadScene(Scene scene, Action callback = null)
        {
            m_LoaderCallback = () =>
            {
                var loadingObject = new GameObject("LoadingMonoBehaviour");
                loadingObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
            };

            // Load Loading Scene
            SceneManager.LoadScene(nameof(Scene.Loading));
        }

        public static float GetLoadingProgress()
        {
            if (asyncLoad != null) return asyncLoad.progress;
            return 1f;
        }

        private static IEnumerator LoadSceneAsync(Scene scene)
        {
            yield return null;

            asyncLoad = SceneManager.LoadSceneAsync(scene.ToString());

            while (asyncLoad is { isDone: false }) yield return null;
        }


        public static void OnLoaderCallback()
        {
            if (m_LoaderCallback != null)
            {
                m_LoaderCallback.Invoke();
                m_LoaderCallback = null;
            }
            else
            {
                Debug.LogError("Loader callback was not set before calling OnLoaderCallback!");
            }
        }
    }
}