#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    internal static class HD_Settings_Shortcuts
    {
        #region Properties
        [System.Serializable]
        private class HD_ShortcutsSettings
        {
            public KeyCode ToggleGameObjectActiveStateKeyCode = KeyCode.Mouse2;
            public KeyCode ToggleLockStateKeyCode = KeyCode.F1;
            public KeyCode ChangeTagLayerKeyCode = KeyCode.F2;
            public KeyCode RenameSelectedGameObjectsKeyCode = KeyCode.F3;
        }
        private static HD_ShortcutsSettings shortcutsSettings = new();
        #endregion

        #region Initialization
        public static void Initialize()
        {
            LoadSettings();
            LoadHierarchyDesignerManagerGameObjectCaches();
        }

        private static void LoadHierarchyDesignerManagerGameObjectCaches()
        {
            HD_Manager_GameObject.ToggleGameObjectActiveStateKeyCodeCache = ToggleGameObjectActiveStateKeyCode;
            HD_Manager_GameObject.ToggleLockStateKeyCodeCache = ToggleLockStateKeyCode;
            HD_Manager_GameObject.ChangeTagLayerKeyCodeCache = ChangeTagLayerKeyCode;
            HD_Manager_GameObject.RenameSelectedGameObjectsKeyCodeCache = RenameSelectedGameObjectsKeyCode;
        }
        #endregion

        #region Accessors
        public static KeyCode ToggleGameObjectActiveStateKeyCode
        {
            get => shortcutsSettings.ToggleGameObjectActiveStateKeyCode;
            set
            {
                if (shortcutsSettings.ToggleGameObjectActiveStateKeyCode != value)
                {
                    shortcutsSettings.ToggleGameObjectActiveStateKeyCode = value;
                    HD_Manager_GameObject.ToggleGameObjectActiveStateKeyCodeCache = value;
                }
            }
        }

        public static KeyCode ToggleLockStateKeyCode
        {
            get => shortcutsSettings.ToggleLockStateKeyCode;
            set
            {
                if (shortcutsSettings.ToggleLockStateKeyCode != value)
                {
                    shortcutsSettings.ToggleLockStateKeyCode = value;
                    HD_Manager_GameObject.ToggleLockStateKeyCodeCache = value;
                }
            }
        }

        public static KeyCode ChangeTagLayerKeyCode
        {
            get => shortcutsSettings.ChangeTagLayerKeyCode;
            set
            {
                if (shortcutsSettings.ChangeTagLayerKeyCode != value)
                {
                    shortcutsSettings.ChangeTagLayerKeyCode = value;
                    HD_Manager_GameObject.ChangeTagLayerKeyCodeCache = value;
                }
            }
        }

        public static KeyCode RenameSelectedGameObjectsKeyCode
        {
            get => shortcutsSettings.RenameSelectedGameObjectsKeyCode;
            set
            {
                if (shortcutsSettings.RenameSelectedGameObjectsKeyCode != value)
                {
                    shortcutsSettings.RenameSelectedGameObjectsKeyCode = value;
                    HD_Manager_GameObject.RenameSelectedGameObjectsKeyCodeCache = value;
                }
            }
        }
        #endregion

        #region Save and Load
        public static void SaveSettings()
        {
            string dataFilePath = HD_Common_File.GetSavedDataFilePath(HD_Common_Constants.ShortcutSettingsTextFileName);
            string json = JsonUtility.ToJson(shortcutsSettings, true);
            File.WriteAllText(dataFilePath, json);
            AssetDatabase.Refresh();
        }

        public static void LoadSettings()
        {
            string dataFilePath = HD_Common_File.GetSavedDataFilePath(HD_Common_Constants.ShortcutSettingsTextFileName);
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                HD_ShortcutsSettings loadedSettings = JsonUtility.FromJson<HD_ShortcutsSettings>(json);
                shortcutsSettings = loadedSettings;
            }
            else
            {
                SetDefaultSettings();
            }
        }

        private static void SetDefaultSettings()
        {
            shortcutsSettings = new()
            {
                ToggleGameObjectActiveStateKeyCode = KeyCode.Mouse2,
                ToggleLockStateKeyCode = KeyCode.F1,
                ChangeTagLayerKeyCode = KeyCode.F2,
                RenameSelectedGameObjectsKeyCode = KeyCode.F3,
            };
        }
        #endregion

        #region Minor Shortcuts
        #pragma warning disable IDE0051
        #region Windows
        [Shortcut("Hierarchy Designer/Open Hierarchy Designer Window", KeyCode.Alpha1, ShortcutModifiers.Alt)]
        private static void OpenHierarchyDesignerWindow()
        {
            HD_Window_Main.OpenWindow();
        }

        [Shortcut("Hierarchy Designer/Open Folder Panel", KeyCode.Alpha2, ShortcutModifiers.Alt)]
        private static void OpenFolderManagerPanel()
        {
            HD_Window_Main.SwitchWindow(HD_Window_Main.CurrentWindow.Folders);
            HD_Window_Main.OpenWindow();
        }

        [Shortcut("Hierarchy Designer/Open Separator Panel", KeyCode.Alpha3, ShortcutModifiers.Alt)]
        private static void OpenSeparatorManagerPanel()
        {
            HD_Window_Main.SwitchWindow(HD_Window_Main.CurrentWindow.Separators);
            HD_Window_Main.OpenWindow();
        }

        [Shortcut("Hierarchy Designer/Open Tools Panel")]
        private static void OpenToolsPanel()
        {
            HD_Window_Main.SwitchWindow(HD_Window_Main.CurrentWindow.Tools);
            HD_Window_Main.OpenWindow();
        }

        [Shortcut("Hierarchy Designer/Open Presets Panel")]
        private static void OpenPresetsPanel()
        {
            HD_Window_Main.SwitchWindow(HD_Window_Main.CurrentWindow.Presets);
            HD_Window_Main.OpenWindow();
        }

        [Shortcut("Hierarchy Designer/Open Preset Creator Panel")]
        private static void OpenPresetCreatorPanel()
        {
            HD_Window_Main.SwitchWindow(HD_Window_Main.CurrentWindow.PresetCreator);
            HD_Window_Main.OpenWindow();
        }

        [Shortcut("Hierarchy Designer/Open General Settings Panel")]
        private static void OpenGeneralSettingsPanel()
        {
            HD_Window_Main.SwitchWindow(HD_Window_Main.CurrentWindow.GeneralSettings);
            HD_Window_Main.OpenWindow();
        }

        [Shortcut("Hierarchy Designer/Open Design Settings Panel")]
        private static void OpenDesignSettingsPanel()
        {
            HD_Window_Main.SwitchWindow(HD_Window_Main.CurrentWindow.DesignSettings);
            HD_Window_Main.OpenWindow();
        }

        [Shortcut("Hierarchy Designer/Open Shortcut Settings Panel")]
        private static void OpenShortcutSettingsPanel()
        {
            HD_Window_Main.SwitchWindow(HD_Window_Main.CurrentWindow.ShortcutSettings);
            HD_Window_Main.OpenWindow();
        }

        [Shortcut("Hierarchy Designer/Open Advanced Settings Panel")]
        private static void OpenAdvancedSettingsPanel()
        {
            HD_Window_Main.SwitchWindow(HD_Window_Main.CurrentWindow.AdvancedSettings);
            HD_Window_Main.OpenWindow();
        }

        [Shortcut("Hierarchy Designer/Open Rename Tool Window")]
        private static void OpenRenameToolWindow()
        {
            HD_Window_Rename.OpenWindow(null, true, 0);
        }
        #endregion

        #region Create
        [Shortcut("Hierarchy Designer/Create All Folders")]
        private static void CreateAllHierarchyFolders() => HD_Common_Menu.CreateAllFolders();

        [Shortcut("Hierarchy Designer/Create Default Folder")]
        private static void CreateDefaultHierarchyFolder() => HD_Common_Menu.CreateDefaultFolder();

        [Shortcut("Hierarchy Designer/Create Missing Folders")]
        private static void CreateMissingHierarchyFolders() => HD_Common_Menu.CreateMissingFolders();

        [Shortcut("Hierarchy Designer/Create All Separators")]
        private static void CreateAllHierarchySeparators() => HD_Common_Menu.CreateAllSeparators();

        [Shortcut("Hierarchy Designer/Create Default Separator")]
        private static void CreateDefaultHierarchySeparator() => HD_Common_Menu.CreateDefaultSeparator();

        [Shortcut("Hierarchy Designer/Create Missing Separators")]
        private static void CreateMissingHierarchySeparators() => HD_Common_Menu.CreateMissingSeparators();
        #endregion

        #region Refresh
        [Shortcut("Hierarchy Designer/Refresh All GameObjects' Data", KeyCode.R, ShortcutModifiers.Shift)]
        private static void RefreshAllGameObjectsData() => HD_Common_Menu.RefreshAllGameObjectsData();

        [Shortcut("Hierarchy Designer/Refresh Selected GameObject's Data", KeyCode.R, ShortcutModifiers.Alt)]
        private static void RefreshSelectedGameObjectsData() => HD_Common_Menu.RefreshSelectedGameObjectsData();

        [Shortcut("Hierarchy Designer/Refresh Selected Main Icon")]
        private static void RefreshMainIconForSelectedGameObject() => HD_Common_Menu.RefreshSelectedMainIcon();

        [Shortcut("Hierarchy Designer/Refresh Selected Component Icons")]
        private static void RefreshComponentIconsForSelectedGameObjects() => HD_Common_Menu.RefreshSelectedComponentIcons();

        [Shortcut("Hierarchy Designer/Refresh Selected Hierarchy Tree Icon")]
        private static void RefreshHierarchyTreeIconForSelectedGameObjects() => HD_Common_Menu.RefreshSelectedHierarchyTreeIcon();

        [Shortcut("Hierarchy Designer/Refresh Selected Tag")]
        private static void RefreshTagForSelectedGameObjects() => HD_Common_Menu.RefreshSelectedTag();

        [Shortcut("Hierarchy Designer/Refresh Selected Layer")]
        private static void RefreshLayerForSelectedGameObjects() => HD_Common_Menu.RefreshSelectedLayer();
        #endregion

        #region Transform
        [Shortcut("Hierarchy Designer/Transform GameObject into a Folder")]
        private static void TransformGameObjectIntoAFolder() => HD_Common_Menu.TransformGameObjectIntoAFolder();

        [Shortcut("Hierarchy Designer/Transform Folder into a GameObject")]
        private static void TransformFolderIntoAGameObject() => HD_Common_Menu.TransformFolderIntoAGameObject();

        [Shortcut("Hierarchy Designer/Transform GameObject into a Separator")]
        private static void TransformGameObjectIntoASeparator() => HD_Common_Menu.TransformGameObjectIntoASeparator();

        [Shortcut("Hierarchy Designer/Transform Separator into a GameObject")]
        private static void TransformSeparatorIntoAGameObject() => HD_Common_Menu.TransformSeparatorIntoAGameObject();
        #endregion

        #region General
        [Shortcut("Hierarchy Designer/Expand All GameObjects", KeyCode.E, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void ExpandAllGameObjects() => HD_Common_Menu.GeneralExpandAll();

        [Shortcut("Hierarchy Designer/Collapse All GameObjects", KeyCode.C, ShortcutModifiers.Shift | ShortcutModifiers.Alt)]
        private static void CollapseAllGameObjects() => HD_Common_Menu.GeneralCollapseAll();
        #endregion
        #endregion
    }
}
#endif