using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CodeMonkey.Toolkit.TQuestionDialog {

    /// <summary>
    /// ** Question Dialog **
    /// 
    /// Ask your player any question.
    /// Then run whatever logic you want on Yes/No/Ok/Cancel
    /// Just call the static functions QuestionDialogUI.ShowQuestionYesNo();
    /// or QuestionDialogUI.ShowQuestionOkCancel();
    /// 
    /// This class also has auto-initalization, 
    /// meaning you can either place it in the scene by default, 
    /// or when you call a function it will spawn the prefab from the Resources folder.
    /// 
    /// Just make sure the prefab is named exactly "QuestionDialogUI" and is placed on a 
    /// folder named exactly "Resources", otherwise it won't work.
    /// </summary>
    public class QuestionDialogUI : MonoBehaviour {


        private static QuestionDialogUI instance;


        private static void Init() {
            if (instance == null) {
                Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
                if (canvas == null) {
                    Debug.LogError("No Canvas was found in Scene! QuestionDialogUI needs a Canvas to work.");
                    return;
                }
                QuestionDialogUI questionDialogUI = Resources.Load<QuestionDialogUI>(nameof(QuestionDialogUI));
                if (questionDialogUI == null) {
                    Debug.LogError("Could not find QuestionDialogUI in Resources! Is the prefab inside a folder named exactly 'Resources'? And is the prefab named exactly '" + nameof(QuestionDialogUI) + "'?");
                    return;
                }
                instance = Instantiate(questionDialogUI, canvas.transform);
            }
        }


        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button leftBtn;
        [SerializeField] private TextMeshProUGUI rightTextMeshProUGUI;
        [SerializeField] private TextMeshProUGUI leftTextMeshProUGUI;


        private Action rightAction;
        private Action leftAction;


        private void Awake() {
            instance = this;

            rightButton.onClick.AddListener(() => {
                Hide();
                rightAction?.Invoke();
            });
            leftBtn.onClick.AddListener(() => {
                Hide();
                leftAction?.Invoke();
            });

            Hide();
        }

        private void ShowQuestion(string questionText, string leftButtonText, string rightButtonText, Action leftAction, Action rightAction) {
            gameObject.SetActive(true);

            rightTextMeshProUGUI.text = rightButtonText;
            leftTextMeshProUGUI.text = leftButtonText;

            textMeshProUGUI.text = questionText;

            this.rightAction = rightAction;
            this.leftAction = leftAction;

            transform.SetAsLastSibling();
        }

        private void ShowQuestionYesNo_Instance(string questionText, Action noAction, Action yesAction) {
            ShowQuestion(questionText, "NO", "YES", noAction, yesAction);
        }

        private void ShowQuestionOkCancel_Instance(string questionText, Action cancelAction, Action okAction) {
            ShowQuestion(questionText, "CANCEL", "OK", cancelAction, okAction);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }




        public static void ShowQuestionYesNo(string questionText, Action noAction, Action yesAction) {
            Init();
            instance.ShowQuestionYesNo_Instance(questionText, noAction, yesAction);
        }

        public static void ShowQuestionOkCancel(string questionText, Action cancelAction, Action okAction) {
            Init();
            instance.ShowQuestionOkCancel_Instance(questionText, cancelAction, okAction);
        }

    }

}