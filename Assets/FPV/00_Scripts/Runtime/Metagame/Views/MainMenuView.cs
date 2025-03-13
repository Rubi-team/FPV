using UnityEngine;
using UnityEngine.UI;
using DEBUG;
using TMPro;
using UnityEngine.Serialization;

namespace FPV
{
    internal class MainMenuView : View<MetagameApplication>
    {
        [Header("MainMenu Elements")]
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_InputField codeTextField;
        [SerializeField] private Button createRelayButton;
        [SerializeField] private Button singlePlayerButton;
        [SerializeField] private Button quitButton;
        

        void OnEnable()
        {
            createRelayButton.onClick.AddListener(OnClickCreateRelay);
            singlePlayerButton.onClick.AddListener(OnClickStartSinglePlayer);
            codeTextField.onSubmit.AddListener(OnCodeInputFieldSubmitted);
            quitButton.onClick.AddListener(OnClickQuit);
            
        }
        
        void OnDisable()
        {
            createRelayButton.onClick.RemoveListener(OnClickCreateRelay);
            singlePlayerButton.onClick.RemoveListener(OnClickStartSinglePlayer);
            codeTextField.onEndEdit.RemoveListener(OnCodeInputFieldSubmitted);
            quitButton.onClick.RemoveListener(OnClickQuit);
        }
        
        void OnCodeInputFieldSubmitted(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length != 6)
                return;

            //console.Log("Broadcasting JoinRelayEvent with code: " + input);
            Broadcast(new JoinRelayEvent(input));
            EnableButtonsAndInputField(false);
        }

        
        void OnClickCreateRelay()
        {
            //console?.Log("Broadcasting CreateRelayEvent");
            Broadcast(new CreateRelayEvent());
        }
        

        void OnClickStartSinglePlayer()
        {
            Broadcast(new StartSinglePlayerModeEvent());
        }

        void OnClickQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
        
        /// <summary>
        /// Enable or disable the buttons and input field
        /// </summary>
        /// <param name="enable"> true or false</param>
        internal void EnableButtonsAndInputField(bool enable)
        {
            createRelayButton.interactable = enable;
            singlePlayerButton.interactable = enable;
            codeTextField.interactable = enable;
            quitButton.interactable = enable;
        }

    }
}
