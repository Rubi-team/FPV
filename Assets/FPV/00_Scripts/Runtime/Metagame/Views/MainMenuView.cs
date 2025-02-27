using UnityEditor.UI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

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

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            m_Root = uiDocument.rootVisualElement;

            CodeInputField = m_Root.Q<TextField>("codeTextField");
            
            CreateRelayButton = m_Root.Q<Button>("createRelayButton");
            CreateRelayButton.RegisterCallback<ClickEvent>(OnClickCreateRelay);

            m_SinglePlayerButton = m_Root.Q<Button>("singlePlayerButton");
            m_SinglePlayerButton.RegisterCallback<ClickEvent>(OnClickStartSinglePlayer);

            m_QuitButton = m_Root.Q<Button>("quitButton");
            m_QuitButton.RegisterCallback<ClickEvent>(OnClickQuit);

            m_TitleLabel = m_Root.Query<Label>("titleLabel");
            m_TitleLabel.text = Constants.GAME_NAME;

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
    }
}
