using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TWebRequests.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        [SerializeField] private Image image;
        [SerializeField] private RawImage rawImage;
        [SerializeField] private Button getURLButton;
        [SerializeField] private Button getJsonButton;
        [SerializeField] private Button getTextureButton;
        [SerializeField] private Button getSpriteButton;
        [SerializeField] private Button postURLButton;


        private void Start() {
            getURLButton.onClick.AddListener(() => {
                // Contact URL with a simple GET request, returns data from that web page, in this example it's just some HTML code.
                string url = "https://unitycodemonkey.com/unitywebrequest.php";
                textMeshProUGUI.text = "Contacting " + url + "...\n";
                WebRequests.Get(url,
                    (string error) => {
                        textMeshProUGUI.text += "ERROR: " + error;
                    },
                    (string success) => {
                        textMeshProUGUI.text += "SUCCESS:\n" + success;
                    });
            });

            getJsonButton.onClick.AddListener(() => {
                // Contact URL and get some JSON which is then parsed into an object of type JsonData.
                string url = "https://unitycodemonkey.com/unitywebrequest.php?q=json";
                textMeshProUGUI.text = "Contacting " + url + "...\n";
                WebRequests.Get(url,
                    (string error) => {
                        textMeshProUGUI.text += "ERROR: " + error;
                    },
                    (string success) => {
                        textMeshProUGUI.text += "SUCCESS!\nRaw: " + success + "\n";
                        JsonData jsonData = JsonUtility.FromJson<JsonData>(success);
                        textMeshProUGUI.text += "jsonData.code: " + jsonData.code + "\n";
                        textMeshProUGUI.text += "jsonData.text: " + jsonData.text + "\n";
                    });
            });

            getTextureButton.onClick.AddListener(() => {
                // Contact URL with a image and get it as a Texture2D
                textMeshProUGUI.text = "Getting Texture...\n";
                string url = "https://codemonkeyexternal.blob.core.windows.net/external/Misc/monkey.jpg";
                rawImage.texture = null;
                WebRequests.GetTexture(
                    url,
                    (string error) => {
                        textMeshProUGUI.text += "ERROR: " + error;
                    },
                    (Texture2D texture2D) => {
                        textMeshProUGUI.text += "Got Texture";
                        rawImage.texture = texture2D;
                    });
            });

            getSpriteButton.onClick.AddListener(() => {
                // Contact URL with a image and get it as a Sprite 
                textMeshProUGUI.text = "Getting Sprite...\n";
                string url = "https://codemonkeyexternal.blob.core.windows.net/external/Misc/monkeyHead.png";
                image.sprite = null;
                WebRequests.GetSprite(
                    url,
                    (string error) => {
                        textMeshProUGUI.text += "ERROR: " + error;
                    },
                    (Sprite sprite) => {
                        textMeshProUGUI.text += "Got Sprite";
                        image.sprite = sprite;
                    });
            });

            postURLButton.onClick.AddListener(() => {
                // Contact URL and send some JSON data as POST, then get the result which in this case the server returns as JSON as well
                JsonData jsonData = new JsonData {
                    code = 56,
                    text = "Hello and Welcome, I'm your Code Monkey!"
                };
                string jsonDataString = JsonUtility.ToJson(jsonData);
                textMeshProUGUI.text = "Sending POST JSON with data: " + jsonDataString + "\n";
                string url = "https://unitycodemonkey.com/unitywebrequest.php";
                WebRequests.PostJson(url,
                    jsonDataString,
                    (string error) => {
                        textMeshProUGUI.text += "ERROR: " + error;
                    },
                    (string success) => {
                        textMeshProUGUI.text += "SUCCESS: " + success;
                    });
            });
        }


        // JSON Data Class
        [Serializable]
        public class JsonData {
            public int code;
            public string text;
        }
    }

}