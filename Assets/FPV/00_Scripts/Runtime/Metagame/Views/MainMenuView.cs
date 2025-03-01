using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Utils;
using Button = UnityEngine.UIElements.Button;
using DEBUG;

namespace FPV
{
    internal class MainMenuView : View<MetagameApplication>
    {
        internal TextField CodeInputField { get; private set; }
        Button CreateRelayButton;
        Button m_QuitButton;
        Button m_SinglePlayerButton;
        Label m_TitleLabel;
        VisualElement m_Root;
        
        [Header("Debug")] [SerializeField] LogHandler _logHandler;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            m_Root = uiDocument.rootVisualElement;
            
            m_TitleLabel = m_Root.Query<Label>("titleLabel");
            m_TitleLabel.text = CONSTANTS.GAME_NAME;

            // Get the input field
            CodeInputField = m_Root.Q<TextField>("codeTextField");

// Register callback for when the code input field is submitted (e.g., when the user presses Enter)
            CodeInputField.RegisterCallback<ChangeEvent<string>>(OnCodeInputFieldSubmitted);

            
            CreateRelayButton = m_Root.Q<Button>("createRelayButton");
            CreateRelayButton.RegisterCallback<ClickEvent>(OnClickCreateRelay);

            m_SinglePlayerButton = m_Root.Q<Button>("singlePlayerButton");
            m_SinglePlayerButton.RegisterCallback<ClickEvent>(OnClickStartSinglePlayer);

            m_QuitButton = m_Root.Q<Button>("quitButton");
            m_QuitButton.RegisterCallback<ClickEvent>(OnClickQuit);

            

            //CustomNetworkManager.OnConfigurationLoaded += OnGameConfigurationLoaded;
        }

        void OnGameConfigurationLoaded()
        {
            DisableControlsUnsupportedInAutoconnectMode();
        }

        void OnDisable()
        {
            m_QuitButton.UnregisterCallback<ClickEvent>(OnClickQuit);
            //CustomNetworkManager.OnConfigurationLoaded -= OnGameConfigurationLoaded;
        }
        
        void OnCodeInputFieldSubmitted(ChangeEvent<string> evt)
        {
            if (string.IsNullOrEmpty(evt.newValue) || evt.newValue.Length != 6)
                return;
            
            Broadcast(new JoinRelayEvent(evt.newValue));
            EnableButtonsAndInputField(false);
        }
        
        void OnClickCreateRelay(ClickEvent evt)
        {
            //Broadcast(new CreateRelayEvent());
        }
        

        void OnClickStartSinglePlayer(ClickEvent evt)
        {
            Broadcast(new StartSinglePlayerModeEvent());
        }

        void OnClickQuit(ClickEvent evt)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        internal void DisableControlsUnsupportedInAutoconnectMode()
        {
            /*if (!CustomNetworkManager.Singleton.AutoConnectOnStartup)
            {
                return;
            }*/
            m_SinglePlayerButton.SetEnabled(false);
        }
        
        /// <summary>
        /// Enable or disable the buttons and input field
        /// </summary>
        /// <param name="enable"> true or false</param>
        internal void EnableButtonsAndInputField(bool enable)
        {
            CreateRelayButton.SetEnabled(enable);
            m_SinglePlayerButton.SetEnabled(enable);
            CodeInputField.SetEnabled(enable);
        }
    }
}
