using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FPV.Shared
{
    public static class NetworkSceneManager
    {
        private static NetworkManager m_NetworkManager => NetworkManager.Singleton;

        private static TaskCompletionSource<bool> m_SceneLoadCompletionSource;
        private static string m_TargetSceneName;
        private static bool m_IsWaitingForScene = false;

        private static HashSet<ulong> m_LoadedClients = new();
        private static int m_ExpectedClientsCount;

        public static event Action OnAllClientsLoaded;

        private static void Init()
        {
            if (m_NetworkManager == null)
            {
                Debug.LogError("NetworkManager.Singleton is null. Cannot initialize NetworkSceneManager.");
                return;
            }

            if (m_NetworkManager.SceneManager == null)
            {
                Debug.LogError("SceneManager is null on NetworkManager. Cannot hook into scene events.");
                return;
            }

            // S'abonner qu'une seule fois
            m_NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
            m_NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
        }


        /// <summary>
        /// Charge une scène et attend que tous les clients aient fini de la charger.
        /// </summary>
        public static async Task<bool> LoadNetworkSceneAsync(string sceneName)
        {
            Init();

            if (m_NetworkManager == null || m_NetworkManager.SceneManager == null)
                return false;

            if (m_IsWaitingForScene)
            {
                Debug.LogWarning("Une autre scène est déjà en cours de chargement.");
                return false;
            }

            m_IsWaitingForScene = true;
            m_TargetSceneName = sceneName;
            m_LoadedClients.Clear();

            m_ExpectedClientsCount = m_NetworkManager.ConnectedClients.Count;

            m_SceneLoadCompletionSource = new TaskCompletionSource<bool>();

            var status = m_NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogError($"Échec du chargement de la scène {sceneName}. Statut : {status}");
                m_IsWaitingForScene = false;
                m_SceneLoadCompletionSource.SetResult(false);
            }

            return await m_SceneLoadCompletionSource.Task;
        }

        private static void OnSceneEvent(SceneEvent sceneEvent)
        {
            if (!m_IsWaitingForScene || sceneEvent.SceneName != m_TargetSceneName)
                return;

            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadComplete:
                    m_LoadedClients.Add(sceneEvent.ClientId);

                    Debug.Log($"Client {sceneEvent.ClientId} a chargé la scène {sceneEvent.SceneName}.");

                    if (m_LoadedClients.Count == m_ExpectedClientsCount)
                    {
                        m_IsWaitingForScene = false;
                        Debug.Log($"Tous les clients ont chargé la scène {m_TargetSceneName}.");
                        m_SceneLoadCompletionSource?.SetResult(true);
                        OnAllClientsLoaded?.Invoke();
                    }

                    break;

                case SceneEventType.LoadEventCompleted:
                    // Optionnel : peut être utile pour des nettoyages ou validations supplémentaires
                    break;
            }
        }
    }
}