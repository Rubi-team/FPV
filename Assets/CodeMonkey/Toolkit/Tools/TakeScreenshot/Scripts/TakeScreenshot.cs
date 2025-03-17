using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace CodeMonkey.Toolkit.TTakeScreenshot {

    public class TakeScreenshot : MonoBehaviour {


        private static TakeScreenshot instance;


        public static TakeScreenshot Instance {
            get { Init(); return instance; }
            private set {
                instance = value;
            }
        }


        private static void Init() {
            if (instance == null) {
                GameObject gameObject = new GameObject(nameof(TakeScreenshot));
                instance = gameObject.AddComponent<TakeScreenshot>();
            }
        }





        private Camera takeScreenshotCamera;
        private bool takeScreenshot;
        private Rect rect;
        private Action<Texture2D> onScreenshotTaken;
        private string savePathPng;
        private bool withUI;
        private bool takeScreenshotUINextFrame;


        private void Awake() {
            Instance = this;
        }

        private void OnEnable() {
            RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        }

        private void OnDisable() {
            RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
        }

        private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext scriptableRenderContext, Camera camera) {
            if (takeScreenshot && (takeScreenshotCamera == null || takeScreenshotCamera == camera)) {
                takeScreenshot = false;

                Canvas[] canvasArray = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                if (!withUI) {
                    // Hide the UI
                    foreach (Canvas canvas in canvasArray) {
                        canvas.enabled = false;
                    }

                    if (takeScreenshotUINextFrame) {
                        takeScreenshot = true;
                        takeScreenshotUINextFrame = false;
                        return;
                    }
                }

                Texture2D screenshotTexture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
                screenshotTexture.ReadPixels(rect, 0, 0);
                screenshotTexture.Apply();

                if (!withUI) {
                    // Re-show the UI
                    foreach (Canvas canvas in canvasArray) {
                        canvas.enabled = true;
                    }
                }

                onScreenshotTaken?.Invoke(screenshotTexture);

                if (savePathPng != null) {
                    byte[] byteArray = screenshotTexture.EncodeToPNG();
                    System.IO.File.WriteAllBytes(savePathPng, byteArray);
                }
            }
        }

        public void TakeScreenshotTexture_Instance(Camera takeScreenshotCamera, Rect rect, Action<Texture2D> onScreenshotTaken, string savePathPng, bool withUI) {
            this.takeScreenshotCamera = takeScreenshotCamera;
            this.rect = rect;
            this.savePathPng = savePathPng;
            this.onScreenshotTaken = onScreenshotTaken;
            this.withUI = withUI;
            takeScreenshot = true;
            takeScreenshotUINextFrame = true;
        }

        public static void TakeScreenshotTexture(Camera takeScreenshotCamera, Rect rect, Action<Texture2D> onScreenshotTaken, string savePathPng = null, bool withUI = true) {
            Init();
            instance.TakeScreenshotTexture_Instance(takeScreenshotCamera, rect, onScreenshotTaken, savePathPng, withUI);
        }

        public static void TakeScreenshotTexture(Camera takeScreenshotCamera, int width, int height, Action<Texture2D> onScreenshotTaken, string savePathPng = null, bool withUI = true) {
            TakeScreenshotTexture(takeScreenshotCamera, new Rect(0, 0, width, height), onScreenshotTaken, savePathPng, withUI);
        }

        public static void TakeScreenshotTexture(Action<Texture2D> onScreenshotTaken, string savePathPng = null, bool withUI = true) {
            TakeScreenshotTexture(Camera.main, Screen.width, Screen.height, onScreenshotTaken, savePathPng, withUI);
        }

        public static void TakeScreenshotTexture(Rect rect, Action<Texture2D> onScreenshotTaken, string savePathPng = null, bool withUI = true) {
            TakeScreenshotTexture(Camera.main, rect, onScreenshotTaken, savePathPng, withUI);
        }

    }

}