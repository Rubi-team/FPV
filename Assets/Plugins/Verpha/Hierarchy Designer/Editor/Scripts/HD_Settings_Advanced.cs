#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    internal static class HD_Settings_Advanced
    {
        #region Properties
        [System.Serializable]
        private class HD_AdvancedSettings
        {
            #region Core
            public HierarchyDesignerLocation HierarchyLocation = HierarchyDesignerLocation.Tools;
            public UpdateMode MainIconUpdateMode = UpdateMode.Dynamic;
            public UpdateMode ComponentsIconsUpdateMode = UpdateMode.Dynamic;
            public UpdateMode HierarchyTreeUpdateMode = UpdateMode.Dynamic;
            public UpdateMode TagUpdateMode = UpdateMode.Dynamic;
            public UpdateMode LayerUpdateMode = UpdateMode.Dynamic;
            #endregion

            #region Main Icon
            public bool EnableDynamicBackgroundForGameObjectMainIcon = true;
            public bool EnablePreciseRectForDynamicBackgroundForGameObjectMainIcon = true;
            #endregion

            #region Component Icons
            public bool EnableCustomizationForGameObjectComponentIcons = true;
            public bool EnableTooltipOnComponentIconHovered = true;
            public bool EnableActiveStateEffectForComponentIcons = true;
            public bool DisableComponentIconsForInactiveGameObjects = true;
            #endregion

            #region Runtime Folder
            public bool EnableCustomInspectorUI = true;
            public bool EnableEditorUtilities = true;
            #endregion

            #region Separator
            public bool IncludeBackgroundImageForGradientBackground = true;
            #endregion

            #region Hierarchy Tools
            public bool ExcludeFoldersFromCountSelectToolCalculations = true;
            public bool ExcludeSeparatorsFromCountSelectToolCalculations = true;
            #endregion
        }
        public enum HierarchyDesignerLocation { Author, Plugins, Tools, TopBar, Window };
        public enum UpdateMode { Dynamic, Smart }
        private static HD_AdvancedSettings advancedSettings = new();
        #endregion

        #region Initialization
        public static void Initialize()
        {
            LoadSettings();

            string currentBaseHierarchyDesigner = ReadBaseHierarchyDesigner();
            string expectedBaseHierarchyDesigner = GetBaseHierarchyDesigner(advancedSettings.HierarchyLocation);
            if (currentBaseHierarchyDesigner != expectedBaseHierarchyDesigner)
            {
                GenerateConstantsFile(HierarchyLocation);
            }

            LoadHierarchyDesignerManagerGameObjectCaches();
        }

        private static string ReadBaseHierarchyDesigner()
        {
            string filePath = HD_Common_File.GetScriptsFilePath(HD_Common_Constants.ConstantClassTextFileName);
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    if (line.Contains("public const string AssetLocation ="))
                    {
                        int startIndex = line.IndexOf("\"") + 1;
                        int endIndex = line.LastIndexOf("\"");
                        return line[startIndex..endIndex];
                    }
                }
            }
            return null;
        }

        private static string GetBaseHierarchyDesigner(HierarchyDesignerLocation hierarchyLocation)
        {
            return hierarchyLocation switch
            {
                HierarchyDesignerLocation.Author => "Verpha/Hierarchy Designer",
                HierarchyDesignerLocation.Plugins => "Plugins/Hierarchy Designer",
                HierarchyDesignerLocation.Tools => "Tools/Hierarchy Designer",
                HierarchyDesignerLocation.TopBar => "Hierarchy Designer/Open Window",
                HierarchyDesignerLocation.Window => "Window/Hierarchy Designer",
                _ => "Tools/Hierarchy Designer"
            };
        }

        public static void GenerateConstantsFile(HierarchyDesignerLocation tempHierarchyLocation)
        {
            string filePath = HD_Common_File.GetScriptsFilePath(HD_Common_Constants.ConstantClassTextFileName);
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].TrimStart().StartsWith("public const string AssetLocation"))
                    {
                        lines[i] = $"        public const string AssetLocation = \"{GetBaseHierarchyDesigner(tempHierarchyLocation)}\";";
                        break;
                    }
                }
                File.WriteAllLines(filePath, lines, Encoding.UTF8);
            }
            AssetDatabase.Refresh();
        }

        private static void LoadHierarchyDesignerManagerGameObjectCaches()
        {
            HD_Manager_GameObject.MainIconUpdateModeCache = MainIconUpdateMode;
            HD_Manager_GameObject.ComponentsIconsUpdateModeCache = ComponentsIconsUpdateMode;
            HD_Manager_GameObject.HierarchyTreeUpdateModeCache = HierarchyTreeUpdateMode;
            HD_Manager_GameObject.TagUpdateModeCache = TagUpdateMode;
            HD_Manager_GameObject.LayerUpdateModeCache = LayerUpdateMode;
            HD_Manager_GameObject.EnableDynamicBackgroundForGameObjectMainIconCache = EnableDynamicBackgroundForGameObjectMainIcon;
            HD_Manager_GameObject.EnablePreciseRectForDynamicBackgroundForGameObjectMainIconCache = EnablePreciseRectForDynamicBackgroundForGameObjectMainIcon;
            HD_Manager_GameObject.DisableComponentIconsForInactiveGameObjectsCache = DisableComponentIconsForInactiveGameObjects;
            HD_Manager_GameObject.EnableCustomizationForGameObjectComponentIconsCache = EnableCustomizationForGameObjectComponentIcons;
            HD_Manager_GameObject.EnableTooltipOnComponentIconHoveredCache = EnableTooltipOnComponentIconHovered;
            HD_Manager_GameObject.EnableActiveStateEffectForComponentIconsCache = EnableActiveStateEffectForComponentIcons;
            HD_Manager_GameObject.IncludeBackgroundImageForGradientBackgroundCache = IncludeBackgroundImageForGradientBackground;
        }
        #endregion

        #region Accessors
        #region Core
        public static HierarchyDesignerLocation HierarchyLocation
        {
            get => advancedSettings.HierarchyLocation;
            set
            {
                if (advancedSettings.HierarchyLocation != value)
                {
                    advancedSettings.HierarchyLocation = value;
                }
            }
        }

        public static UpdateMode MainIconUpdateMode
        {
            get => advancedSettings.MainIconUpdateMode;
            set
            {
                if (advancedSettings.MainIconUpdateMode != value)
                {
                    advancedSettings.MainIconUpdateMode = value;
                    HD_Manager_GameObject.MainIconUpdateModeCache = value;
                }
            }
        }

        public static UpdateMode ComponentsIconsUpdateMode
        {
            get => advancedSettings.ComponentsIconsUpdateMode;
            set
            {
                if (advancedSettings.ComponentsIconsUpdateMode != value)
                {
                    advancedSettings.ComponentsIconsUpdateMode = value;
                    HD_Manager_GameObject.ComponentsIconsUpdateModeCache = value;
                }
            }
        }

        public static UpdateMode HierarchyTreeUpdateMode
        {
            get => advancedSettings.HierarchyTreeUpdateMode;
            set
            {
                if (advancedSettings.HierarchyTreeUpdateMode != value)
                {
                    advancedSettings.HierarchyTreeUpdateMode = value;
                    HD_Manager_GameObject.HierarchyTreeUpdateModeCache = value;
                }
            }
        }

        public static UpdateMode TagUpdateMode
        {
            get => advancedSettings.TagUpdateMode;
            set
            {
                if (advancedSettings.TagUpdateMode != value)
                {
                    advancedSettings.TagUpdateMode = value;
                    HD_Manager_GameObject.TagUpdateModeCache = value;
                }
            }
        }

        public static UpdateMode LayerUpdateMode
        {
            get => advancedSettings.LayerUpdateMode;
            set
            {
                if (advancedSettings.LayerUpdateMode != value)
                {
                    advancedSettings.LayerUpdateMode = value;
                    HD_Manager_GameObject.LayerUpdateModeCache = value;
                }
            }
        }
        #endregion

        #region Main Icon
        public static bool EnableDynamicBackgroundForGameObjectMainIcon
        {
            get => advancedSettings.EnableDynamicBackgroundForGameObjectMainIcon;
            set
            {
                if (advancedSettings.EnableDynamicBackgroundForGameObjectMainIcon != value)
                {
                    advancedSettings.EnableDynamicBackgroundForGameObjectMainIcon = value;
                    HD_Manager_GameObject.EnableDynamicBackgroundForGameObjectMainIconCache = value;
                }
            }
        }

        public static bool EnablePreciseRectForDynamicBackgroundForGameObjectMainIcon
        {
            get => advancedSettings.EnablePreciseRectForDynamicBackgroundForGameObjectMainIcon;
            set
            {
                if (advancedSettings.EnablePreciseRectForDynamicBackgroundForGameObjectMainIcon != value)
                {
                    advancedSettings.EnablePreciseRectForDynamicBackgroundForGameObjectMainIcon = value;
                    HD_Manager_GameObject.EnablePreciseRectForDynamicBackgroundForGameObjectMainIconCache = value;
                }
            }
        }
        #endregion

        #region Component Icons
        public static bool EnableCustomizationForGameObjectComponentIcons
        {
            get => advancedSettings.EnableCustomizationForGameObjectComponentIcons;
            set
            {
                if (advancedSettings.EnableCustomizationForGameObjectComponentIcons != value)
                {
                    advancedSettings.EnableCustomizationForGameObjectComponentIcons = value;
                    HD_Manager_GameObject.EnableCustomizationForGameObjectComponentIconsCache = value;
                }
            }
        }

        public static bool EnableTooltipOnComponentIconHovered
        {
            get => advancedSettings.EnableTooltipOnComponentIconHovered;
            set
            {
                if (advancedSettings.EnableTooltipOnComponentIconHovered != value)
                {
                    advancedSettings.EnableTooltipOnComponentIconHovered = value;
                    HD_Manager_GameObject.EnableTooltipOnComponentIconHoveredCache = value;
                }
            }
        }

        public static bool EnableActiveStateEffectForComponentIcons
        {
            get => advancedSettings.EnableActiveStateEffectForComponentIcons;
            set
            {
                if (advancedSettings.EnableActiveStateEffectForComponentIcons != value)
                {
                    advancedSettings.EnableActiveStateEffectForComponentIcons = value;
                    HD_Manager_GameObject.EnableActiveStateEffectForComponentIconsCache = value;
                }
            }
        }

        public static bool DisableComponentIconsForInactiveGameObjects
        {
            get => advancedSettings.DisableComponentIconsForInactiveGameObjects;
            set
            {
                if (advancedSettings.DisableComponentIconsForInactiveGameObjects != value)
                {
                    advancedSettings.DisableComponentIconsForInactiveGameObjects = value;
                    HD_Manager_GameObject.DisableComponentIconsForInactiveGameObjectsCache = value;
                }
            }
        }
        #endregion

        #region Folder
        public static bool EnableCustomInspectorGUI
        {
            get => advancedSettings.EnableCustomInspectorUI;
            set
            {
                if (advancedSettings.EnableCustomInspectorUI != value)
                {
                    advancedSettings.EnableCustomInspectorUI = value;
                }
            }
        }

        public static bool IncludeEditorUtilitiesForHierarchyDesignerRuntimeFolder
        {
            get => advancedSettings.EnableEditorUtilities;
            set
            {
                if (advancedSettings.EnableEditorUtilities != value)
                {
                    advancedSettings.EnableEditorUtilities = value;
                }
            }
        }
        #endregion

        #region Separator
        public static bool IncludeBackgroundImageForGradientBackground
        {
            get => advancedSettings.IncludeBackgroundImageForGradientBackground;
            set
            {
                if (advancedSettings.IncludeBackgroundImageForGradientBackground != value)
                {
                    advancedSettings.IncludeBackgroundImageForGradientBackground = value;
                    HD_Manager_GameObject.IncludeBackgroundImageForGradientBackgroundCache = value;
                }
            }
        }
        #endregion

        #region Hierarchy Tools
        public static bool ExcludeFoldersFromCountSelectToolCalculations
        {
            get => advancedSettings.ExcludeFoldersFromCountSelectToolCalculations;
            set
            {
                if (advancedSettings.ExcludeFoldersFromCountSelectToolCalculations != value)
                {
                    advancedSettings.ExcludeFoldersFromCountSelectToolCalculations = value;
                }
            }
        }

        public static bool ExcludeSeparatorsFromCountSelectToolCalculations
        {
            get => advancedSettings.ExcludeSeparatorsFromCountSelectToolCalculations;
            set
            {
                if (advancedSettings.ExcludeSeparatorsFromCountSelectToolCalculations != value)
                {
                    advancedSettings.ExcludeSeparatorsFromCountSelectToolCalculations = value;
                }
            }
        }
        #endregion
        #endregion

        #region Save and Load
        public static void SaveSettings()
        {
            string dataFilePath = HD_Common_File.GetSavedDataFilePath(HD_Common_Constants.AdvancedSettingsTextFileName);
            string json = JsonUtility.ToJson(advancedSettings, true);
            File.WriteAllText(dataFilePath, json);
            AssetDatabase.Refresh();
        }

        public static void LoadSettings()
        {
            string dataFilePath = HD_Common_File.GetSavedDataFilePath(HD_Common_Constants.AdvancedSettingsTextFileName);
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                HD_AdvancedSettings loadedSettings = JsonUtility.FromJson<HD_AdvancedSettings>(json);
                loadedSettings.HierarchyLocation = HD_Common_Checker.ParseEnum(loadedSettings.HierarchyLocation.ToString(), HierarchyDesignerLocation.Tools);
                loadedSettings.MainIconUpdateMode = HD_Common_Checker.ParseEnum(loadedSettings.MainIconUpdateMode.ToString(), UpdateMode.Dynamic);
                loadedSettings.ComponentsIconsUpdateMode = HD_Common_Checker.ParseEnum(loadedSettings.ComponentsIconsUpdateMode.ToString(), UpdateMode.Dynamic);
                loadedSettings.HierarchyTreeUpdateMode = HD_Common_Checker.ParseEnum(loadedSettings.HierarchyTreeUpdateMode.ToString(), UpdateMode.Dynamic);
                loadedSettings.TagUpdateMode = HD_Common_Checker.ParseEnum(loadedSettings.TagUpdateMode.ToString(), UpdateMode.Dynamic);
                loadedSettings.LayerUpdateMode = HD_Common_Checker.ParseEnum(loadedSettings.LayerUpdateMode.ToString(), UpdateMode.Dynamic);
                advancedSettings = loadedSettings;
            }
            else
            {
                SetDefaultSettings();
            }
        }

        private static void SetDefaultSettings()
        {
            advancedSettings = new()
            {
                HierarchyLocation = HierarchyDesignerLocation.Tools,
                MainIconUpdateMode = UpdateMode.Dynamic,
                ComponentsIconsUpdateMode = UpdateMode.Dynamic,
                HierarchyTreeUpdateMode = UpdateMode.Dynamic,
                TagUpdateMode = UpdateMode.Dynamic,
                LayerUpdateMode = UpdateMode.Dynamic,
                EnableDynamicBackgroundForGameObjectMainIcon = true,
                EnablePreciseRectForDynamicBackgroundForGameObjectMainIcon = true,
                EnableCustomizationForGameObjectComponentIcons = true,
                EnableTooltipOnComponentIconHovered = true,
                EnableActiveStateEffectForComponentIcons = true,
                DisableComponentIconsForInactiveGameObjects = true,
                EnableCustomInspectorUI = true,
                EnableEditorUtilities = true,
                IncludeBackgroundImageForGradientBackground = true,
                ExcludeFoldersFromCountSelectToolCalculations = true,
                ExcludeSeparatorsFromCountSelectToolCalculations = true,
            };
        }
        #endregion
    }
}
#endif