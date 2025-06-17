using System.Linq;
using FMOD;
using UnityEngine;
using UnityEngine.UIElements;
using FPV.Shared;
using TMPro;
using Debug = UnityEngine.Debug;

namespace FPV
{
    internal class MainMenuView : View<MetagameApplication>
    {
        [SerializeField] private TextMeshProUGUI codeTextField;
        [SerializeField] private GameObject canvas;
        
        /*private Label titleLabel;
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
        }*/
        
        public void OnClickCreateRelay()
        {
            Broadcast(new CreateRelayEvent());
        }
        
        /// <summary>
        /// PK JE DOIS FAIRE ÇA ÇA ME RENDS FOU FUCK TEXT MESH PRO
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string CleanJoinCode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            // Supprime les espaces, sauts de ligne, caractères invisibles
            string cleaned = new string(input
                .Where(c =>
                    "6789BCDFGHJKLMNPQRTWbcdfghjklmnpqrtw".Contains(c))
                .ToArray());

            return cleaned.ToUpper(); // Optionnel, Relay est insensible à la casse
        }

        
        public void OnCodeInputFieldSubmitted()
        {
            string input = CleanJoinCode(codeTextField.text); // Envie de se foutre en l'air

            if (input.Length != 6) // Jtai cap le input field comme ça tu peux pas mettre + de 6 char
            {
                Debug.LogWarning("Le code doit contenir 6 caractères valides.");
                return;
            }

            Broadcast(new JoinRelayEvent(input));

            EnableButtonsAndInputField(false); // ça jten referais une stv
            
            
            // jsp ce que tu veux faire la mais jte le laisse
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                
            }
        }
        
        public void OnClickQuit()
        {
            Broadcast(new ApplicationQuitEvent());
        }

        public void OnClickCredit()
        {
            Debug.Log("Credits");
        }

        /// <summary>
        /// Enable or disable the buttons and input field
        /// </summary>
        /// <param name="enable"> true or false</param>
        internal void EnableButtonsAndInputField(bool enable)
        {
            canvas.SetActive(enable);
            
            /*codeTextField.SetEnabled(enable);
            createRelayButton.SetEnabled(enable);
            singlePlayerButton.SetEnabled(enable);
            quitButton.SetEnabled(enable);*/
        }
    }
}