using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CodeMonkey.Toolkit.TFunctionTimer;

namespace CodeMonkey.Toolkit.TInputWindow {

    /// <summary>
    /// ** Input Window **
    /// 
    /// Use this window to help you receive input from the player for something like 
    /// getting a Player Name, naming a Car or Pet, or really anything.
    /// 
    /// You can limit what characters that players can use and how many characters.
    /// 
    /// Just call the static function InputWindowUI.Show();
    /// 
    /// The BlockerUI helps block further clicks on UI buttons but is not required.
    /// The class also has auto-initialization, you can either manually drag the prefab onto your Canvas, 
    /// or it will automatically spawn when you first use the static function.
    /// Just make sure the prefab exists inside a Resources folder and is named exactly InputWindowUI
    /// </summary>
    public class InputWindowUI : MonoBehaviour {


        public const string ALPHABET_LOWER_CASE = "abcdefghijklmnopqrstuvxywz";
        public const string ALPHABET_UPPER_CASE = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
        public const string NUMBERS = "0123456789";


        private static InputWindowUI instance;


        private static void Init() {
            if (instance == null) {
                Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
                if (canvas == null) {
                    Debug.LogError("No Canvas was found in Scene! InputWindowUI needs a Canvas to work.");
                    return;
                }
                InputWindowUI inputWindowUI = Resources.Load<InputWindowUI>(nameof(InputWindowUI));
                if (inputWindowUI == null) {
                    Debug.LogError("Could not find InputWindowUI in Resources! Is the prefab inside a folder named exactly 'Resources'? And is the prefab named exactly '" + nameof(InputWindowUI) + "'?");
                    return;
                }
                instance = Instantiate(inputWindowUI, canvas.transform);
            }
        }



        [SerializeField] private Button okBtn;
        [SerializeField] private Button cancelBtn;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private bool listenToKeyInputs;


        private Action okClickAction;
        private Action cancelClickAction;


        private void Awake() {
            instance = this;

            okBtn.onClick.AddListener(() => {
                okClickAction?.Invoke(); 
            });
            cancelBtn.onClick.AddListener(() => {
                cancelClickAction?.Invoke();
            });

            Hide();
        }

        private void Update() {
            if (listenToKeyInputs) {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                    okClickAction?.Invoke();
                }
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    cancelClickAction?.Invoke();
                }
            }
        }

        private void Show_Instance(string titleString, string inputString, string validCharacters, int characterLimit, Action onCancel, Action<string> onOk) {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();

            titleText.text = titleString;

            inputField.characterLimit = characterLimit;
            inputField.onValidateInput = (string text, int charIndex, char addedChar) => {
                return ValidateChar(validCharacters, addedChar);
            };

            inputField.text = inputString;
            inputField.Select();
            inputField.ActivateInputField();
            FunctionTimer.Create(() => {
                // Select Input on next frame, sometimes selecting on same frame as setting it active doesn't work
                inputField.Select();
                inputField.ActivateInputField();
            }, .01f);

            okClickAction = () => {
                Hide();
                onOk(inputField.text);
            };

            cancelClickAction = () => {
                Hide();
                onCancel();
            };
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private char ValidateChar(string validCharacters, char addedChar) {
            if (validCharacters.IndexOf(addedChar) != -1) {
                // Valid
                return addedChar;
            } else {
                // Invalid
                return '\0';
            }
        }

        private bool IsVisible_Instance() {
            return gameObject.activeSelf;
        }




        public static void Show(string titleString, string inputString, string validCharacters, int characterLimit, Action onCancel, Action<string> onOk) {
            Init();
            instance.Show_Instance(titleString, inputString, validCharacters, characterLimit, onCancel, onOk);
        }

        public static void Show(string titleString, int defaultInt, Action onCancel, Action<int> onOk) {
            Init();
            instance.Show_Instance(titleString, defaultInt.ToString(), "0123456789-+", 20, onCancel,
                (string inputText) => {
                    // Try to Parse input string
                    if (int.TryParse(inputText, out int _i)) {
                        onOk(_i);
                    } else {
                        onOk(defaultInt);
                    }
                }
            );
        }

        public static bool IsVisible() {
            Init();
            return instance.IsVisible_Instance();
        }

    }

}