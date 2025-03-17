using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CodeMonkey.Toolkit.TTextWriter {

    /// <summary>
    /// ** Text Writer **
    /// 
    /// Easily add a Text writing effect to some text object.
    /// Works with TextMesh, Text (UI), TextMeshPro and TextMeshProUGUI
    /// Just call the static function TextWriter.AddWriter();
    /// 
    /// You can add parameters to control the speed, invisible characters, 
    /// unscaled deltaTime, and a onCompleted callback
    /// </summary>
    public class TextWriter : MonoBehaviour {


        private static TextWriter instance;


        private static void Init() {
            if (instance == null) {
                GameObject gameObject = new GameObject("TextWriter", typeof(TextWriter));
                instance = gameObject.GetComponent<TextWriter>();
            }
        }

        public static TextWriterSingle AddWriter(Text uiText, string textToWrite, float timePerCharacter, bool withInvisibleCharacters, bool useUnscaledDeltaTime, Action onComplete) {
            Init();
            instance.RemoveWriterInstance(uiText);
            return instance.AddWriterInstance(uiText, textToWrite, timePerCharacter, withInvisibleCharacters, useUnscaledDeltaTime, onComplete);
        }

        public static TextWriterSingle AddWriter(TextMesh textMesh, string textToWrite, float timePerCharacter, bool withInvisibleCharacters, bool useUnscaledDeltaTime, Action onComplete) {
            Init();
            instance.RemoveWriterInstance(textMesh);
            return instance.AddWriterInstance(textMesh, textToWrite, timePerCharacter, withInvisibleCharacters, useUnscaledDeltaTime, onComplete);
        }

        public static TextWriterSingle AddWriter(TextMeshPro textMeshPro, string textToWrite, float timePerCharacter, bool withInvisibleCharacters, bool useUnscaledDeltaTime, Action onComplete) {
            Init();
            instance.RemoveWriterInstance(textMeshPro);
            return instance.AddWriterInstance(textMeshPro, textToWrite, timePerCharacter, withInvisibleCharacters, useUnscaledDeltaTime, onComplete);
        }

        public static TextWriterSingle AddWriter(TextMeshProUGUI textMeshProUGUI, string textToWrite, float timePerCharacter, bool withInvisibleCharacters, bool useUnscaledDeltaTime, Action onComplete) {
            Init();
            instance.RemoveWriterInstance(textMeshProUGUI);
            return instance.AddWriterInstance(textMeshProUGUI, textToWrite, timePerCharacter, withInvisibleCharacters, useUnscaledDeltaTime, onComplete);
        }

        public static void RemoveWriter(TextMesh textMesh) {
            instance.RemoveWriterInstance(textMesh);
        }

        public static void RemoveWriter(Text uiText) {
            instance.RemoveWriterInstance(uiText);
        }

        public static void RemoveWriter(TextMeshPro textMeshPro) {
            instance.RemoveWriterInstance(textMeshPro);
        }

        public static void RemoveWriter(TextMeshProUGUI textMeshProUGUI) {
            instance.RemoveWriterInstance(textMeshProUGUI);
        }




        private List<TextWriterSingle> textWriterSingleList;


        private void Awake() {
            instance = this;

            textWriterSingleList = new List<TextWriterSingle>();
        }


        private TextWriterSingle AddWriterInstance(Text uiText, string textToWrite, float timePerCharacter, bool withInvisibleCharacters, bool useUnscaledDeltaTime, Action onComplete) {
            TextWriterSingle textWriterSingle = new TextWriterSingle(textToWrite, timePerCharacter, withInvisibleCharacters, useUnscaledDeltaTime, onComplete);
            textWriterSingle.SetUIText(uiText);
            textWriterSingleList.Add(textWriterSingle);
            return textWriterSingle;
        }

        private TextWriterSingle AddWriterInstance(TextMesh textMesh, string textToWrite, float timePerCharacter, bool withInvisibleCharacters, bool useUnscaledDeltaTime, Action onComplete) {
            TextWriterSingle textWriterSingle = new TextWriterSingle(textToWrite, timePerCharacter, withInvisibleCharacters, useUnscaledDeltaTime, onComplete);
            textWriterSingle.SetTextMesh(textMesh);
            textWriterSingleList.Add(textWriterSingle);
            return textWriterSingle;
        }

        private TextWriterSingle AddWriterInstance(TextMeshPro textMeshPro, string textToWrite, float timePerCharacter, bool withInvisibleCharacters, bool useUnscaledDeltaTime, Action onComplete) {
            TextWriterSingle textWriterSingle = new TextWriterSingle(textToWrite, timePerCharacter, withInvisibleCharacters, useUnscaledDeltaTime, onComplete);
            textWriterSingle.SetTextMeshPro(textMeshPro);
            textWriterSingleList.Add(textWriterSingle);
            return textWriterSingle;
        }

        private TextWriterSingle AddWriterInstance(TextMeshProUGUI textMeshProUGUI, string textToWrite, float timePerCharacter, bool withInvisibleCharacters, bool useUnscaledDeltaTime, Action onComplete) {
            TextWriterSingle textWriterSingle = new TextWriterSingle(textToWrite, timePerCharacter, withInvisibleCharacters, useUnscaledDeltaTime, onComplete);
            textWriterSingle.SetTextMeshProUGUI(textMeshProUGUI);
            textWriterSingleList.Add(textWriterSingle);
            return textWriterSingle;
        }

        private void RemoveWriterInstance(TextMesh textMesh) {
            for (int i = 0; i < textWriterSingleList.Count; i++) {
                if (textWriterSingleList[i].GetTextMesh() == textMesh) {
                    textWriterSingleList.RemoveAt(i);
                    i--;
                }
            }
        }

        private void RemoveWriterInstance(Text uiText) {
            for (int i = 0; i < textWriterSingleList.Count; i++) {
                if (textWriterSingleList[i].GetUIText() == uiText) {
                    textWriterSingleList.RemoveAt(i);
                    i--;
                }
            }
        }

        private void RemoveWriterInstance(TextMeshPro textMeshPro) {
            for (int i = 0; i < textWriterSingleList.Count; i++) {
                if (textWriterSingleList[i].GetTextMeshPro() == textMeshPro) {
                    textWriterSingleList.RemoveAt(i);
                    i--;
                }
            }
        }

        private void RemoveWriterInstance(TextMeshProUGUI textMeshProUGUI) {
            for (int i = 0; i < textWriterSingleList.Count; i++) {
                if (textWriterSingleList[i].GetTextMeshProUGUI() == textMeshProUGUI) {
                    textWriterSingleList.RemoveAt(i);
                    i--;
                }
            }
        }

        private void Update() {
            for (int i = 0; i < textWriterSingleList.Count; i++) {
                TextWriterSingle single = textWriterSingleList[i];
                bool destroyInstance = single.Update();
                if (destroyInstance) {
                    textWriterSingleList.Remove(single);
                    i--;
                }
            }
        }

        /*
         * Represents a single TextWriter instance
         * */
        public class TextWriterSingle {


            private TextMesh textMesh;
            private Text uiText;
            private TextMeshPro textMeshPro;
            private TextMeshProUGUI textMeshProUGUI;
            private string textToWrite;
            private int characterIndex;
            private float timePerCharacter;
            private float timer;
            private bool withInvisibleCharacters;
            private bool useUnscaledDeltaTime;
            private Action onComplete;


            public TextWriterSingle(string textToWrite, float timePerCharacter, bool withInvisibleCharacters, bool useUnscaledDeltaTime, Action onComplete) {
                this.textToWrite = textToWrite;
                this.timePerCharacter = timePerCharacter;
                this.withInvisibleCharacters = withInvisibleCharacters;
                this.useUnscaledDeltaTime = useUnscaledDeltaTime;
                this.onComplete = onComplete;
                characterIndex = 0;
            }

            // Returns true on complete
            public bool Update() {
                if (useUnscaledDeltaTime) {
                    timer -= Time.unscaledDeltaTime;
                } else {
                    timer -= Time.deltaTime;
                }
                while (timer <= 0f) {
                    // Display next character
                    timer += timePerCharacter;
                    characterIndex++;
                    string text = textToWrite.Substring(0, characterIndex);
                    if (withInvisibleCharacters) {
                        text += "<color=#00000000>" + textToWrite.Substring(characterIndex) + "</color>";
                    }

                    if (textMesh != null) {
                        textMesh.text = text;
                    }
                    if (uiText != null) {
                        uiText.text = text;
                    }
                    if (textMeshPro != null) {
                        textMeshPro.SetText(text);
                    }
                    if (textMeshProUGUI != null) {
                        textMeshProUGUI.SetText(text);
                    }

                    if (characterIndex >= textToWrite.Length) {
                        // Entire string displayed
                        if (onComplete != null) onComplete();
                        return true;
                    }
                }

                return false;
            }

            public void SetTextMesh(TextMesh textMesh) {
                this.textMesh = textMesh;
            }

            public void SetUIText(Text uiText) {
                this.uiText = uiText;
            }

            public void SetTextMeshPro(TextMeshPro textMeshPro) {
                this.textMeshPro = textMeshPro;
            }

            public void SetTextMeshProUGUI(TextMeshProUGUI textMeshProUGUI) {
                this.textMeshProUGUI = textMeshProUGUI;
            }

            public TextMesh GetTextMesh() {
                return textMesh;
            }

            public Text GetUIText() {
                return uiText;
            }

            public TextMeshPro GetTextMeshPro() {
                return textMeshPro;
            }

            public TextMeshProUGUI GetTextMeshProUGUI() {
                return textMeshProUGUI;
            }

            public bool IsActive() {
                return characterIndex < textToWrite.Length;
            }

            public void WriteAllAndDestroy() {
                if (uiText != null) {
                    uiText.text = textToWrite;
                } else {
                    textMeshPro.SetText(textToWrite);
                }

                characterIndex = textToWrite.Length;
                if (onComplete != null) onComplete();

                if (uiText != null) {
                    TextWriter.RemoveWriter(uiText);
                } else {
                    TextWriter.RemoveWriter(textMeshPro);
                }

            }


        }


    }

}