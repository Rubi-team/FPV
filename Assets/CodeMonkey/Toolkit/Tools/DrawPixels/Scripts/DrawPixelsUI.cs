using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TDrawPixels {

    public class DrawPixelsUI : MonoBehaviour {


        public const string PATH = "/CodeMonkey/Toolkit/Tools/DrawPixels";


        [SerializeField] private Texture2D colorsTexture;
        [SerializeField] private Camera saveCamera;
        [SerializeField] private Button smallBrushButton;
        [SerializeField] private Button mediumBrushButton;
        [SerializeField] private Button largeBrushButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Image selectedColorImage;


        private void Awake() {
            smallBrushButton.onClick.AddListener(() => {
                DrawPixels.Instance.SetCursorSize(DrawPixels.CursorSize.Small);
            });
            mediumBrushButton.onClick.AddListener(() => {
                DrawPixels.Instance.SetCursorSize(DrawPixels.CursorSize.Medium);
            });
            largeBrushButton.onClick.AddListener(() => {
                DrawPixels.Instance.SetCursorSize(DrawPixels.CursorSize.Large);
            });

            saveButton.onClick.AddListener(() => {
                DrawPixels.Instance.SaveImage((Texture2D texture2D) => {
                    rawImage.GetComponent<RawImage>().texture = texture2D;

                    byte[] byteArray = texture2D.EncodeToPNG();
                    System.IO.File.WriteAllBytes(Application.dataPath + PATH + "/PixelImage.png", byteArray);
                });
                //SaveImageCamera(100, 100);
            });
        }

        private void Start() {
            DrawPixels.Instance.OnColorChanged += DrawPixels_OnColorChanged;

            UpdateSelectedColor();
            LoadPixelRawImage();
        }

        private void LoadPixelRawImage() {
            if (System.IO.File.Exists(Application.dataPath + PATH + "/PixelImage.png")) {
                Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                texture2D.filterMode = FilterMode.Point;

                byte[] byteArray = System.IO.File.ReadAllBytes(Application.dataPath + PATH + "/PixelImage.png");
                texture2D.LoadImage(byteArray);

                rawImage.texture = texture2D;
            }
        }

        private void DrawPixels_OnColorChanged(object sender, System.EventArgs e) {
            UpdateSelectedColor();
        }

        private void UpdateSelectedColor() {
            Vector2 pixelCoordinates = DrawPixels.Instance.GetColorUV();
            pixelCoordinates.x *= colorsTexture.width;
            pixelCoordinates.y *= colorsTexture.height;
            selectedColorImage.color = colorsTexture.GetPixel((int)pixelCoordinates.x, (int)pixelCoordinates.y);
        }

        // Optional function that uses a Camera to save a Texture
        private void SaveImageCamera(int width, int height) {
            RenderTexture renderTexture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
            saveCamera.targetTexture = renderTexture;
            saveCamera.enabled = true;
            saveCamera.Render();

            RenderTexture.active = renderTexture;
            Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, width, height);
            screenshotTexture.ReadPixels(rect, 0, 0);
            screenshotTexture.Apply();

            saveCamera.enabled = false;
            rawImage.texture = renderTexture;

            byte[] byteArray = screenshotTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.dataPath + PATH + "/PixelImage.png", byteArray);
        }

    }

}