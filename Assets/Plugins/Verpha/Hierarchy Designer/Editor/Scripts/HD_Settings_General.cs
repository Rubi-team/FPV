#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    internal static class HD_Settings_General
    {
        #region Properties
        [System.Serializable]
        private class HD_GeneralSettings
        {
            #region Core
            public HierarchyLayoutMode LayoutMode = HierarchyLayoutMode.Split;
            public HierarchyTreeMode TreeMode = HierarchyTreeMode.Default;
            #endregion

            #region General
            public bool EnableGameObjectMainIcon = true;
            public bool EnableGameObjectComponentIcons = true;
            public bool EnableHierarchyTree = true;
            public bool EnableGameObjectTag = true;
            public bool EnableGameObjectLayer = true;
            public bool EnableHierarchyRows = true;
            public bool EnableHierarchyLines = true;
            public bool EnableHierarchyButtons = true;
            public bool EnableMajorShortcuts = true;
            public bool DisableHierarchyDesignerDuringPlayMode = true;
            #endregion

            #region Filtering
            public bool ExcludeFolderProperties = true;
            public List<string> ExcludedComponents = new (){ "Transform", "RectTransform", "CanvasRenderer" };
            public int MaximumComponentIconsAmount = 10;
            public List<string> ExcludedTags = new();
            public List<string> ExcludedLayers = new();
            #endregion
        }
        public enum HierarchyLayoutMode { Consecutive, Docked, Split };
        public enum HierarchyTreeMode { Minimal, Default};

        private static HD_GeneralSettings generalSettings = new();
        #endregion

        #region Initialization
        public static void Initialize()
        {
            LoadSettings();
            LoadSettingsIntoCaches();
        }

        private static void LoadSettingsIntoCaches()
        {
            HD_Manager_GameObject.LayoutModeCache = LayoutMode;
            HD_Manager_GameObject.TreeModeCache = TreeMode;
            HD_Manager_GameObject.EnableGameObjectMainIconCache = EnableGameObjectMainIcon;
            HD_Manager_GameObject.EnableGameObjectComponentIconsCache = EnableGameObjectComponentIcons;
            HD_Manager_GameObject.EnableHierarchyTreeCache = EnableHierarchyTree;
            HD_Manager_GameObject.EnableGameObjectTagCache = EnableGameObjectTag;
            HD_Manager_GameObject.EnableGameObjectLayerCache = EnableGameObjectLayer;
            HD_Manager_GameObject.EnableHierarchyRowsCache = EnableHierarchyRows;
            HD_Manager_GameObject.EnableHierarchyLinesCache = EnableHierarchyLines;
            HD_Manager_GameObject.EnableHierarchyButtonsCache = EnableHierarchyButtons;
            HD_Manager_GameObject.EnableMajorShortcutsCache = EnableMajorShortcuts;
            HD_Manager_GameObject.DisableHierarchyDesignerDuringPlayModeCache = DisableHierarchyDesignerDuringPlayMode;
            HD_Manager_GameObject.ExcludeFolderProperties = ExcludeFolderProperties;
            HD_Manager_GameObject.ExcludedComponentsCache = ExcludedComponents;
            HD_Manager_GameObject.MaximumComponentIconsAmountCache = MaximumComponentIconsAmount;
            HD_Manager_GameObject.ExcludedTagsCache = ExcludedTags;
            HD_Manager_GameObject.ExcludedLayersCache = ExcludedLayers;
        }
        #endregion

        #region Accessors
        #region Core
        public static HierarchyLayoutMode LayoutMode
        {
            get => generalSettings.LayoutMode;
            set
            {
                if (generalSettings.LayoutMode != value)
                {
                    generalSettings.LayoutMode = value;
                    HD_Manager_GameObject.LayoutModeCache = value;
                }
            }
        }

        public static HierarchyTreeMode TreeMode
        {
            get => generalSettings.TreeMode;
            set
            {
                if (generalSettings.TreeMode != value)
                {
                    generalSettings.TreeMode = value;
                    HD_Manager_GameObject.TreeModeCache = value;
                }
            }
        }
        #endregion

        #region General
        public static bool EnableGameObjectMainIcon
        {
            get => generalSettings.EnableGameObjectMainIcon;
            set
            {
                if (generalSettings.EnableGameObjectMainIcon != value)
                {
                    generalSettings.EnableGameObjectMainIcon = value;
                    HD_Manager_GameObject.EnableGameObjectMainIconCache = value;
                }
            }
        }

        public static bool EnableGameObjectComponentIcons
        {
            get => generalSettings.EnableGameObjectComponentIcons;
            set
            {
                if (generalSettings.EnableGameObjectComponentIcons != value)
                {
                    generalSettings.EnableGameObjectComponentIcons = value;
                    HD_Manager_GameObject.EnableGameObjectComponentIconsCache = value;
                }
            }
        }

        public static bool EnableHierarchyTree
        {
            get => generalSettings.EnableHierarchyTree;
            set
            {
                if (generalSettings.EnableHierarchyTree != value)
                {
                    generalSettings.EnableHierarchyTree = value;
                    HD_Manager_GameObject.EnableHierarchyTreeCache = value;
                }
            }
        }

        public static bool EnableGameObjectTag
        {
            get => generalSettings.EnableGameObjectTag;
            set
            {
                if (generalSettings.EnableGameObjectTag != value)
                {
                    generalSettings.EnableGameObjectTag = value;
                    HD_Manager_GameObject.EnableGameObjectTagCache = value;
                }
            }
        }

        public static bool EnableGameObjectLayer
        {
            get => generalSettings.EnableGameObjectLayer;
            set
            {
                if (generalSettings.EnableGameObjectLayer != value)
                {
                    generalSettings.EnableGameObjectLayer = value;
                    HD_Manager_GameObject.EnableGameObjectLayerCache = value;
                }
            }
        }

        public static bool EnableHierarchyRows
        {
            get => generalSettings.EnableHierarchyRows;
            set
            {
                if (generalSettings.EnableHierarchyRows != value)
                {
                    generalSettings.EnableHierarchyRows = value;
                    HD_Manager_GameObject.EnableHierarchyRowsCache = value;
                }
            }
        }

        public static bool EnableHierarchyLines
        {
            get => generalSettings.EnableHierarchyLines;
            set
            {
                if (generalSettings.EnableHierarchyLines != value)
                {
                    generalSettings.EnableHierarchyLines = value;
                    HD_Manager_GameObject.EnableHierarchyLinesCache = value;
                }
            }
        }

        public static bool EnableHierarchyButtons
        {
            get => generalSettings.EnableHierarchyButtons;
            set
            {
                if (generalSettings.EnableHierarchyButtons != value)
                {
                    generalSettings.EnableHierarchyButtons = value;
                    HD_Manager_GameObject.EnableHierarchyButtonsCache = value;
                }
            }
        }

        public static bool EnableMajorShortcuts
        {
            get => generalSettings.EnableMajorShortcuts;
            set
            {
                if (generalSettings.EnableMajorShortcuts != value)
                {
                    generalSettings.EnableMajorShortcuts = value;
                    HD_Manager_GameObject.EnableMajorShortcutsCache = value;
                }
            }
        }

        public static bool DisableHierarchyDesignerDuringPlayMode
        {
            get => generalSettings.DisableHierarchyDesignerDuringPlayMode;
            set
            {
                if (generalSettings.DisableHierarchyDesignerDuringPlayMode != value)
                {
                    generalSettings.DisableHierarchyDesignerDuringPlayMode = value;
                    HD_Manager_GameObject.DisableHierarchyDesignerDuringPlayModeCache = value;
                }
            }
        }
        #endregion

        #region Filtering
        public static bool ExcludeFolderProperties
        {
            get => generalSettings.ExcludeFolderProperties;
            set
            {
                if (generalSettings.ExcludeFolderProperties != value)
                {
                    generalSettings.ExcludeFolderProperties = value;
                    HD_Manager_GameObject.ExcludeFolderProperties = value;
                }
            }
        }

        public static List<string> ExcludedComponents
        {
            get => generalSettings.ExcludedComponents;
            set
            {
                if (generalSettings.ExcludedComponents != value)
                {
                    generalSettings.ExcludedComponents = value;
                    HD_Manager_GameObject.ExcludedComponentsCache = value;
                    HD_Manager_GameObject.ClearGameObjectDataCache();
                }
            }
        }

        public static int MaximumComponentIconsAmount
        {
            get => generalSettings.MaximumComponentIconsAmount;
            set
            {
                int clampedValue = Mathf.Clamp(value, 1, 20);
                if (generalSettings.MaximumComponentIconsAmount != clampedValue)
                {
                    generalSettings.MaximumComponentIconsAmount = clampedValue;
                    HD_Manager_GameObject.MaximumComponentIconsAmountCache = value;
                }
            }
        }

        public static List<string> ExcludedTags
        {
            get => generalSettings.ExcludedTags;
            set
            {
                if (generalSettings.ExcludedTags != value)
                {
                    generalSettings.ExcludedTags = value;
                    HD_Manager_GameObject.ExcludedTagsCache = value;
                }
            }
        }

        public static List<string> ExcludedLayers
        {
            get => generalSettings.ExcludedLayers;
            set
            {
                if (generalSettings.ExcludedLayers != value)
                {
                    generalSettings.ExcludedLayers = value;
                    HD_Manager_GameObject.ExcludedLayersCache = value;
                }
            }
        }
        #endregion
        #endregion

        #region Save and Load
        public static void SaveSettings()
        {
            string dataFilePath = HD_Common_File.GetSavedDataFilePath(HD_Common_Constants.GeneralSettingsTextFileName);
            string json = JsonUtility.ToJson(generalSettings, true);
            File.WriteAllText(dataFilePath, json);
            AssetDatabase.Refresh();
        }

        public static void LoadSettings()
        {
            string dataFilePath = HD_Common_File.GetSavedDataFilePath(HD_Common_Constants.GeneralSettingsTextFileName);
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                HD_GeneralSettings loadedSettings = JsonUtility.FromJson<HD_GeneralSettings>(json);
                loadedSettings.LayoutMode = HD_Common_Checker.ParseEnum(loadedSettings.LayoutMode.ToString(), HierarchyLayoutMode.Docked);
                loadedSettings.TreeMode = HD_Common_Checker.ParseEnum(loadedSettings.TreeMode.ToString(), HierarchyTreeMode.Default);
                generalSettings = loadedSettings;
            }
            else
            {
                SetDefaultSettings();
            }
        }

        private static void SetDefaultSettings()
        {
            generalSettings = new()
            {
                LayoutMode = HierarchyLayoutMode.Split,
                TreeMode = HierarchyTreeMode.Default,
                EnableGameObjectMainIcon = true,
                EnableGameObjectComponentIcons = true,
                EnableHierarchyTree = true,
                EnableGameObjectTag = true,
                EnableGameObjectLayer = true,
                EnableHierarchyRows = true,
                EnableHierarchyLines = true,
                EnableHierarchyButtons = true,
                EnableMajorShortcuts = true,
                DisableHierarchyDesignerDuringPlayMode = true,
                ExcludeFolderProperties = true,
                ExcludedComponents = new() { "Transform", "RectTransform", "CanvasRenderer" },
                MaximumComponentIconsAmount = 10,
                ExcludedTags = new(),
                ExcludedLayers = new()
            };
        }
        #endregion
    }
}
#endif