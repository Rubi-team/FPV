using System.Collections;
using System.IO;
using System.Linq;
using Unity.Multiplayer;
using UnityEngine;
using UnityEngine.Networking;

namespace FPV.Shared
{
    /// <summary>
    /// A configuration Manager for easily accessing dynamic configurations that alter the behaviour of the app
    /// </summary>
    public class ConfigurationManager
    {
        #region DeveloperSetupFileData

        /// <summary>
        /// Name of the configuration file
        /// </summary>
        public const string k_DevConfigFile = "StartupConfiguration.json";

        /// <summary>
        /// Where the default configuration file is stored.
        /// </summary>
        public static readonly string k_DevConfigFileDefault = Path.Combine(Application.dataPath,
            Path.Combine("Resources", Path.Combine("DefaultConfigurations", k_DevConfigFile)));

        /// <summary>
        /// Identifier of "Override Multiplayer Role" settings
        /// </summary>
        public const string k_OverrideMultiplayerRole = "OverrideMultiplayerRole";

        /// <summary>
        /// Identifier of "Host mode" settings
        /// </summary>
        public const string k_ModeHost = "StartAsHost";

        /// <summary>
        /// Identifier of "Server mode" settings
        /// </summary>
        public const string k_ModeServer = "StartAsServer";

        /// <summary>
        /// Identifier of "Client mode" settings
        /// </summary>
        public const string k_ModeClient = "StartAsClient";

        /// <summary>
        /// Players the server expects in a match
        /// </summary>
        public const string k_MaxPlayers = "MaxPlayers";

        /// <summary>
        /// Port where the game runs on the server 
        /// </summary>
        public const string k_Port = "Port";

        /// <summary>
        /// Are bots allowed in the match?
        /// </summary>
        public const string k_EnableBots = "EnableBots";

        /// <summary>
        /// IP address of the server 
        /// </summary>
        public const string k_ServerIP = "ServerIP";

        /// <summary>
        /// Will the game startup behaviour change according to the settings?
        /// </summary>
        public const string k_Autoconnect = "AutoConnect";

        /// <summary>
        /// Will the server stay open to allow reconnection of disconnected players?
        /// </summary>
        public const string k_AllowReconnection = "AllowReconnection";

        #endregion

        /// <summary>
        /// Meta-configuration file used to automate processes
        /// </summary>
        private JSONNode m_Config;

        private string m_ConfigFilePath;

        /// <summary>
        /// Initializes the ConfigurationManager
        /// </summary>
        /// <param name="configFilePath">path of the configuration file</param>
        public ConfigurationManager(string configFilePath)
        {
            LoadConfigurationFromFile(configFilePath, false, false);
        }

        /// <summary>
        /// Initializes the ConfigurationManager
        /// </summary>
        /// <param name="configFilePath">path of the configuration file</param>
        /// <param name="keepUninitialized">if true, the initialization is not performed</param>
        public ConfigurationManager(string configFilePath, bool keepUninitialized)
        {
            m_ConfigFilePath = configFilePath;
            if (keepUninitialized) return;
            LoadConfigurationFromFile(configFilePath, false, false);
        }

        /// <summary>
        /// Initializes the ConfigurationManager
        /// </summary>
        /// <param name="routineRunner">A MonoBehaviour that will run the loading routine</param>
        /// <param name="configFilePath">path of the configuration file</param>
        /// <param name="onFinished">Callback invoked when the configuration is loaded</param>
        public ConfigurationManager(MonoBehaviour routineRunner, string configFilePath,
            System.Action<ConfigurationManager> onFinished = null)
        {
#if !UNITY_EDITOR
            string configPathOnMobile = Path.Combine(Application.streamingAssetsPath, "Client", configFilePath);
#if UNITY_ANDROID
            Debug.Log($"Loading config on Android platforms, from {configPathOnMobile}");
            routineRunner.StartCoroutine(LoadConfigurationFromFileOnNonDesktop(configPathOnMobile, onFinished));
            return;
#endif
#if UNITY_IOS
            LoadConfigurationOnIOS(configPathOnMobile, onFinished);
            return;
#endif
#endif
            LoadConfigurationFromFile(configFilePath, false, false);
            onFinished?.Invoke(this);
        }

        private void LoadConfigurationOnIOS(string configFilePath,
            System.Action<ConfigurationManager> onFinished = null)
        {
            Debug.Log($"Loading config on iOS platform, from {configFilePath}");
            LoadConfigurationFromFile(configFilePath, false, false);
            onFinished?.Invoke(this);
        }

        private IEnumerator LoadConfigurationFromFileOnNonDesktop(string configFilePath,
            System.Action<ConfigurationManager> onFinished)
        {
            m_ConfigFilePath = configFilePath;
            using (var www = UnityWebRequest.Get(configFilePath))
            {
                yield return www.SendWebRequest();
                m_Config = JSONNode.Parse(www.downloadHandler.text);
                Debug.Log("[Non-desktop] config: " + m_Config.ToString(""));
                onFinished?.Invoke(this);
            }
        }

        /// <summary>
        /// Loads the configuration file
        /// </summary>
        /// <param name="configFilePath">Path of the configuration file.</param>
        /// <param name="createIfNotExists">If true, creates the configuration file when it doesn't exist.</param>
        /// <param name="updateIfOutdated">If true, new default settings will be integrated in the existing configuration.</param>
        private void LoadConfigurationFromFile(string configFilePath, bool createIfNotExists, bool updateIfOutdated)
        {
            m_ConfigFilePath = configFilePath;
            var templatePath = Path.Combine("DefaultConfigurations", configFilePath.Split('.')[0]);
            if (!File.Exists(configFilePath))
            {
                if (!createIfNotExists)
                    throw new FileNotFoundException(
                        $"{configFilePath} not found, please open the Bootstrapper Window to create a default configuration file (menu Window > Multiplayer > Bootstrapper). If you're using multiplayer Play Mode and this is a virtual player, please copy the StartupConfiguration.json file from the root folder of the project of the Main Editor, then use the Multiplayer Play Mode window to navigate to the Virtual Player's folder, and paste it there.");

                m_Config = JSONNode.Parse(Resources.Load<TextAsset>(templatePath).text);
                JSONUtilities.WriteJSONToFile(configFilePath, m_Config, false);
                return;
            }

            m_Config = JSONUtilities.ReadJSONFromFile(configFilePath);

            if (!updateIfOutdated) return;

            /*
             * Since user settings may change between versions, we need to be sure that we update them
             * when new ones come up.
             */
            var template = JSONNode.Parse(Resources.Load<TextAsset>(templatePath).text);
            var newSettings = template.Keys.Except(m_Config.Keys);
            foreach (var item in newSettings) m_Config[item] = template[item].Value;
            JSONUtilities.WriteJSONToFile(configFilePath, m_Config, false);
        }

        public MultiplayerRoleFlags GetMultiplayerRole()
        {
            if (GetBool(k_OverrideMultiplayerRole))
            {
                if (GetBool(k_ModeServer)) return MultiplayerRoleFlags.Server;
                if (GetBool(k_ModeHost)) return MultiplayerRoleFlags.ClientAndServer;
                if (GetBool(k_ModeClient)) return MultiplayerRoleFlags.Client;
            }

            return MultiplayerRolesManager.ActiveMultiplayerRoleMask;
        }

        /// <summary>
        /// Removes a key from the configuration
        /// </summary>
        /// <param name="key">The configuration key.</param>
        public void Remove(string key)
        {
            m_Config.Remove(key);
        }

        /// <summary>
        /// Checks if a key exists in the configuration
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>True if the key exists, false otherwise</returns>
        public bool Contains(string key)
        {
            return m_Config.Keys.Any(k => k == key);
        }

        /// <summary>
        /// Sets the value of a key in the configuration
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <param name="value">The value</param>
        /// <remarks>value must implement ToString()</remarks>
        public void Set(string key, object value)
        {
            m_Config[key] = value.ToString();
        }

        /// <summary>
        /// Gets the value of a key in the configuration
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>the value of the string in the configuration, as string</returns>
        public string GetString(string key)
        {
            return m_Config[key].Value;
        }

        /// <summary>
        /// Gets the value of a key in the configuration
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>the value of the string in the configuration, as bool</returns>
        public bool GetBool(string key)
        {
            return m_Config[key].AsBool;
        }

        /// <summary>
        /// Gets the value of a key in the configuration
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>the value of the string in the configuration, as int</returns>
        public int GetInt(string key)
        {
            return m_Config[key].AsInt;
        }

        /// <summary>
        /// Gets the value of a key in the configuration
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>the value of the string in the configuration, as float</returns>
        public float GetFloat(string key)
        {
            return m_Config[key].AsFloat;
        }

        /// <summary>
        /// Saves the confiuration as a JSON file
        /// </summary>
        /// <param name="singleLine">If true, the JSON is saved as a one-liner</param>
        public void SaveAsJSON(bool singleLine)
        {
            SaveAsJSON(m_ConfigFilePath, singleLine);
        }

        private void SaveAsJSON(string path, bool singleLine)
        {
            JSONUtilities.WriteJSONToFile(path, m_Config, singleLine);
        }

        /// <summary>
        /// Overwrites the existing configuration with a new one
        /// </summary>
        /// <param name="newConfiguration">The new configuration to use</param>
        public void Overwrite(JSONNode newConfiguration)
        {
            m_Config = JSONNode.Parse(newConfiguration.ToString());
        }

        public override string ToString()
        {
            return m_Config.ToString("");
        }
    }
}