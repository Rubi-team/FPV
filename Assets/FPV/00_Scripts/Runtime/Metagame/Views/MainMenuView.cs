using FMOD;
using UnityEngine;
using UnityEngine.UIElements;
using FPV.Shared;

namespace FPV
{
    internal class MainMenuView : View<MetagameApplication>
    {
        private Label titleLabel;
        private TextField codeTextField;
        private Button createRelayButton;
        private Button singlePlayerButton;
        private Button quitButton;
        private VisualElement m_Root;


        private void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            m_Root = uiDocument.rootVisualElement;

            titleLabel = m_Root.Query<Label>("titleLabel");
            titleLabel.text = FPV_CONSTANTS.GAME_NAME;

            codeTextField = m_Root.Query<TextField>("codeTextField");
            codeTextField.RegisterCallback<KeyDownEvent>(OnCodeInputFieldSubmitted);

            createRelayButton = m_Root.Query<Button>("createRelayButton");
            createRelayButton.RegisterCallback<ClickEvent>(OnClickCreateRelay);

            singlePlayerButton = m_Root.Query<Button>("singlePlayerButton");
            singlePlayerButton.RegisterCallback<ClickEvent>(OnClickStartSinglePlayer);

            quitButton = m_Root.Query<Button>("quitButton");
            quitButton.RegisterCallback<ClickEvent>(OnClickQuit);
        }

        private void OnDisable()
        {
            codeTextField.UnregisterCallback<KeyDownEvent>(OnCodeInputFieldSubmitted);
            createRelayButton.UnregisterCallback<ClickEvent>(OnClickCreateRelay);
            singlePlayerButton.UnregisterCallback<ClickEvent>(OnClickStartSinglePlayer);
            quitButton.UnregisterCallback<ClickEvent>(OnClickQuit);
        }

        private void OnCodeInputFieldSubmitted(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
            {
                var input = codeTextField.value;
                if (string.IsNullOrEmpty(input) || input.Length != 6)
                    return;

                // console.Log("Broadcasting JoinRelayEvent with code: " + input);
                Broadcast(new JoinRelayEvent(input));
                EnableButtonsAndInputField(false);
            }
        }


        private void OnClickCreateRelay(ClickEvent evt)
        {
            //console?.Log("Broadcasting CreateRelayEvent");
            Broadcast(new CreateRelayEvent());
        }


        private void OnClickStartSinglePlayer(ClickEvent evt)
        {
            Broadcast(new StartSinglePlayerModeEvent());
        }

        private void OnClickQuit(ClickEvent evt)
        {
            Broadcast(new ApplicationQuitEvent());
        }

        /// <summary>
        /// Enable or disable the buttons and input field
        /// </summary>
        /// <param name="enable"> true or false</param>
        internal void EnableButtonsAndInputField(bool enable)
        {
            codeTextField.SetEnabled(enable);
            createRelayButton.SetEnabled(enable);
            singlePlayerButton.SetEnabled(enable);
            quitButton.SetEnabled(enable);
        }
    }
}