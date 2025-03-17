using CodeMonkey.Toolkit.TBlockerUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeMonkey.Toolkit.TQuestionDialog.Demo {

    public class Demo : MonoBehaviour {


        [SerializeField] private Button simpleQuestionButton;
        [SerializeField] private TextMeshProUGUI simpleQuestionTextMeshProUGUI;
        [SerializeField] private Button reallySureQuestionButton;
        [SerializeField] private Button infoButton;


        private void Awake() {
            simpleQuestionButton.onClick.AddListener(() => {
                BlockerUI.Show();
                QuestionDialogUI.ShowQuestionYesNo("Do you like dogs?",
                    () => {
                        BlockerUI.Hide();
                        simpleQuestionTextMeshProUGUI.text = "Simple Question\nAnswered: No";
                    },
                    () => {
                        BlockerUI.Hide();
                        simpleQuestionTextMeshProUGUI.text = "Simple Question\nAnswered: Yes";
                    });
            });
            reallySureQuestionButton.onClick.AddListener(() => {
                BlockerUI.Show();
                QuestionDialogUI.ShowQuestionYesNo("Do you want to do this?",
                    BlockerUI.Hide,
                    () => {
                        QuestionDialogUI.ShowQuestionYesNo("Are you REALLY sure you want to do this?",
                            BlockerUI.Hide,
                            () => {
                                QuestionDialogUI.ShowQuestionOkCancel("Ok you did this!",
                                    BlockerUI.Hide,
                                    BlockerUI.Hide);
                            });
                    });
            });
            infoButton.onClick.AddListener(() => {
                BlockerUI.Show();
                QuestionDialogUI.ShowQuestionOkCancel("This is an info message!\nThe buttons are Ok - Cancel instead of Yes - No\nI can still write whatever I want here and do anything on click.",
                    BlockerUI.Hide,
                    BlockerUI.Hide);
            });
        }
    }

}