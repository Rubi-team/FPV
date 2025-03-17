using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace CodeMonkey.Toolkit.TWebRequests {

    /// <summary>
    /// ** Web Requests **
    /// 
    /// Contact a URL and get a response with Text or Images.
    /// 
    /// This can be used for building tons of things, like a Game version checker, 
    /// a dynamic message from the developer, show latest workshop items, 
    /// really anything that contacts a server to get or send data.
    /// 
    /// And you can do all that without having to update the Game build.
    /// 
    /// Just use the functions inside this WebRequests class.
    /// </summary>
    public static class WebRequests {


        private class WebRequestsMonoBehaviour : MonoBehaviour { }


        private static WebRequestsMonoBehaviour webRequestsMonoBehaviour;

        private static void Init() {
            if (webRequestsMonoBehaviour == null) {
                GameObject gameObject = new GameObject("WebRequests");
                webRequestsMonoBehaviour = gameObject.AddComponent<WebRequestsMonoBehaviour>();
            }
        }

        public static void Get(string url, Action<string> onError, Action<string> onSuccess) {
            Init();
            webRequestsMonoBehaviour.StartCoroutine(GetCoroutine(url, onError, onSuccess));
        }

        private static IEnumerator GetCoroutine(string url, Action<string> onError, Action<string> onSuccess) {
            using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url)) {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                    unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                    unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                    // Error
                    onError(unityWebRequest.error);
                } else {
                    onSuccess(unityWebRequest.downloadHandler.text);
                }
            }
        }

        public static void Post(string url, Dictionary<string, string> formFields, Action<string> onError, Action<string> onSuccess) {
            Init();
            webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePost(url, formFields, onError, onSuccess));
        }

        public static void Post(string url, string postData, string contentType, Action<string> onError, Action<string> onSuccess) {
            Init();
            webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePost(url, postData, contentType, onError, onSuccess));
        }

        public static void PostJson(string url, string jsonData, Action<string> onError, Action<string> onSuccess) {
            Init();
            webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePostJson(url, jsonData, onError, onSuccess));
        }

        private static IEnumerator GetCoroutinePost(string url, Dictionary<string, string> formFields, Action<string> onError, Action<string> onSuccess) {
            using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, formFields)) {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                    unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                    unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                    // Error
                    onError(unityWebRequest.error);
                } else {
                    onSuccess(unityWebRequest.downloadHandler.text);
                }
            }
        }

        private static IEnumerator GetCoroutinePost(string url, string postData, string contentType, Action<string> onError, Action<string> onSuccess) {
            using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, postData, contentType)) {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                    unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                    unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                    // Error
                    onError(unityWebRequest.error);
                } else {
                    onSuccess(unityWebRequest.downloadHandler.text);
                }
            }
        }

        private static IEnumerator GetCoroutinePostJson(string url, string jsonData, Action<string> onError, Action<string> onSuccess) {
            using (UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST")) {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                unityWebRequest.SetRequestHeader("Content-Type", "application/json");

                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                    unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                    unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                    // Error
                    onError(unityWebRequest.error);
                } else {
                    onSuccess(unityWebRequest.downloadHandler.text);
                }
            }
        }

        public static void Put(string url, string bodyData, Action<string> onError, Action<string> onSuccess) {
            Init();
            webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePut(url, bodyData, onError, onSuccess));
        }

        private static IEnumerator GetCoroutinePut(string url, string bodyData, Action<string> onError, Action<string> onSuccess) {
            using (UnityWebRequest unityWebRequest = UnityWebRequest.Put(url, bodyData)) {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                    unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                    unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                    // Error
                    onError(unityWebRequest.error);
                } else {
                    onSuccess(unityWebRequest.downloadHandler.text);
                }
            }
        }

        public static void GetTexture(string url, Action<string> onError, Action<Texture2D> onSuccess) {
            Init();
            webRequestsMonoBehaviour.StartCoroutine(GetTextureCoroutine(url, onError, onSuccess));
        }

        private static IEnumerator GetTextureCoroutine(string url, Action<string> onError, Action<Texture2D> onSuccess) {
            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url)) {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                    unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                    unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                    // Error
                    onError(unityWebRequest.error);
                } else {
                    DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                    onSuccess(downloadHandlerTexture.texture);
                }
            }
        }

        public static void GetSprite(string url, Action<string> onError, Action<Sprite> onSuccess) {
            Init();
            webRequestsMonoBehaviour.StartCoroutine(GetSpriteCoroutine(url, onError, onSuccess));
        }

        private static IEnumerator GetSpriteCoroutine(string url, Action<string> onError, Action<Sprite> onSuccess) {
            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url)) {
                yield return unityWebRequest.SendWebRequest();

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                    unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                    unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                    // Error
                    onError(unityWebRequest.error);
                } else {
                    DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                    Sprite sprite = Sprite.Create(downloadHandlerTexture.texture, new Rect(0, 0, downloadHandlerTexture.texture.width, downloadHandlerTexture.texture.height), new Vector2(.5f, .5f));
                    onSuccess(sprite);
                }
            }
        }

    }

}