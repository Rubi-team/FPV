#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    internal class HD_Window_Main : EditorWindow
    {
        #region Properties
        #region General
        public enum CurrentWindow { Home, About, Folders, Separators, Tools, Presets, PresetCreator, GeneralSettings, DesignSettings, ShortcutSettings, AdvancedSettings }
        private static CurrentWindow currentWindow;
        private static string cachedCurrentWindowLabel;
        private Vector2 headerButtonsScroll;
        private Dictionary<string, Action> utilitiesMenuItems;
        private Dictionary<string, Action> configurationsMenuItems;
        private const int primaryButtonsHeight = 30;
        private const int secondaryButtonsHeight = 25;
        private const int defaultMarginSpacing = 5;
        private const float moveItemInListButtonWidth = 25;
        private const float createButtonWidth = 52;
        private const float removeButtonWidth = 60;
        private readonly int[] fontSizeOptions = new int[15];
        #endregion

        #region Home
        private Vector2 homeScroll;
        private string patchNotes = string.Empty;
        private const float titleAspectRatio = 1f;
        private const float titleWidthPercentage = 1f;
        private const float titleMinWidth = 256f;
        private const float titleMaxWidth = 512f;
        private const float titleMinHeight = 128f;
        private const float titleMaxHeight = 400f;
        #endregion

        #region About
        private Vector2 aboutSummaryScroll;
        private Vector2 aboutPatchNotesScroll;
        private Vector2 aboutMyOtherAssetsScroll;

        private const string folderText =
            "Folders are great for organizing multiple GameObjects of the same or similar type (e.g., static environment objects, reflection probes, and so on).\n\n" +
            "Folders have a script called 'Hierarchy Designer Folder' with a main variable, 'Flatten Folder.'" +
            "If 'Flatten Folder' is true, in the Flatten Event (Awake or Start method), the folder will FREE all GameObject children, and once that is complete, it will destroy the folder.\n";

        private const string separatorText =
            "Separators are visual dividers; they are meant to organize your scenes and provide clarity.\n\n" +
            "Separators are editor-only and will NOT be included in your game's build. Therefore, do not use them as GameObject parents; instead, use folders.\n";

        private const string savedDataText =
            "Settings and Custom Presets are saved in the 'Saved Data' folder (located at: Assets/.../Hierarchy Designer/Editor/Saved Data) as .json files.\n\n" +
            "To export Hierarchy Designer's data to another project, simply copy and paste the .json files into the other project's saved data folder, and then restart the editor.\n";

        private const string additionalNotesText =
            "Hierarchy Designer is currently in development, and more features and improvements are coming soon.\n\n" +
            "Hierarchy Designer is an Editor-Only tool (with the exception of the Hierarchy Designer Folder script) and will not affect your build or game.\n\n" +
            "Like most editor tool, it will slightly affect performance (EDITOR ONLY). Disabling features you don't use or setting their update values to 'smart' will greatly improve performance, especially in larger scenes.\n\n" +
            "If you have any questions or would like to report a bug, you may email me at: VerphaSuporte@outlook.com.\n\nIf you like Hierarchy Designer, please rate it on the Store.";
        #endregion

        #region Folders
        private Vector2 folderMainScroll;
        private Vector2 foldersListScroll;
        private const float folderCreationLabelWidth = 110;
        private Dictionary<string, HD_Settings_Folders.HD_FolderData> tempFolders;
        private List<string> foldersOrder;
        private string newFolderName = "";
        private Color newFolderTextColor = HD_Settings_Design.FolderDefaultTextColor;
        private int newFolderFontSize = HD_Settings_Design.FolderDefaultFontSize;
        private FontStyle newFolderFontStyle = HD_Settings_Design.FolderDefaultFontStyle;
        private Color newFolderIconColor = HD_Settings_Design.FolderDefaultImageColor;
        private HD_Settings_Folders.FolderImageType newFolderImageType = HD_Settings_Design.FolderDefaultImageType;
        private bool folderHasModifiedChanges = false;
        private Color tempFolderGlobalTextColor = Color.white;
        private int tempFolderGlobalFontSize = 12;
        private FontStyle tempFolderGlobalFontStyle = FontStyle.Normal;
        private Color tempFolderGlobalIconColor = Color.white;
        private HD_Settings_Folders.FolderImageType tempGlobalFolderImageType = HD_Settings_Folders.FolderImageType.Default;
        #endregion

        #region Separators
        private Vector2 separatorMainScroll;
        private Vector2 separatorsListScroll;
        private const float separatorCreationLabelWidth = 160;
        private Dictionary<string, HD_Settings_Separators.HD_SeparatorData> tempSeparators;
        private List<string> separatorsOrder;
        private bool separatorHasModifiedChanges = false;
        private string newSeparatorName = "";
        private Color newSeparatorTextColor = HD_Settings_Design.SeparatorDefaultTextColor;
        private bool newSeparatorIsGradient = HD_Settings_Design.SeparatorDefaultIsGradientBackground;
        private Color newSeparatorBackgroundColor = HD_Settings_Design.SeparatorDefaultBackgroundColor;
        private Gradient newSeparatorBackgroundGradient = HD_Common_Color.CopyGradient(HD_Settings_Design.SeparatorDefaultBackgroundGradient);
        private int newSeparatorFontSize = HD_Settings_Design.SeparatorDefaultFontSize;
        private FontStyle newSeparatorFontStyle = HD_Settings_Design.SeparatorDefaultFontStyle;
        private TextAnchor newSeparatorTextAnchor = HD_Settings_Design.SeparatorDefaultTextAnchor;
        private HD_Settings_Separators.SeparatorImageType newSeparatorImageType = HD_Settings_Design.SeparatorDefaultImageType;
        private Color tempSeparatorGlobalTextColor = Color.white;
        private bool tempSeparatorGlobalIsGradient = false;
        private Color tempSeparatorGlobalBackgroundColor = Color.gray;
        private Gradient tempSeparatorGlobalBackgroundGradient = new();
        private int tempSeparatorGlobalFontSize = 12;
        private FontStyle tempSeparatorGlobalFontStyle = FontStyle.Normal;
        private TextAnchor tempSeparatorGlobalTextAnchor = TextAnchor.MiddleCenter;
        private HD_Settings_Separators.SeparatorImageType tempSeparatorGlobalImageType = HD_Settings_Separators.SeparatorImageType.Default;
        #endregion

        #region Tools
        private Vector2 toolsMainScroll;
        private const float labelWidth = 80;
        private HierarchyDesigner_Attribute_Tools selectedCategory = HierarchyDesigner_Attribute_Tools.Activate;
        private int selectedActionIndex = 0;
        private readonly List<string> availableActionNames = new();
        private readonly List<MethodInfo> availableActionMethods = new();
        private static readonly Dictionary<HierarchyDesigner_Attribute_Tools, List<(string Name, MethodInfo Method)>> cachedActions = new();
        private static bool cacheInitialized = false;
        #endregion

        #region Presets
        private Vector2 presetsMainScroll;
        private const float presetslabelWidth = 130;
        private const float presetsToggleLabelWidth = 205;
        private int selectedPresetIndex = 0;
        private string[] presetNames;
        private bool applyToFolders = true;
        private bool applyToSeparators = true;
        private bool applyToTag = true;
        private bool applyToLayer = true;
        private bool applyToTree = true;
        private bool applyToLines = true;
        private bool applyToFolderDefaultValues = true;
        private bool applyToSeparatorDefaultValues = true;
        private bool applyToLock = true;
        #endregion

        #region Preset Creator
        private Vector2 presetCreatorMainScroll;
        private Vector2 presetCreatorListScroll;
        private const int customPresetsSpacing = 10;
        private const float customPresetsLabelWidth = 185;
        private string customPresetName = string.Empty;
        private Color customPresetFolderTextColor = Color.white;
        private int customPresetFolderFontSize = 12;
        private FontStyle customPresetFolderFontStyle = FontStyle.Normal;
        private Color customPresetFolderColor = Color.white;
        private HD_Settings_Folders.FolderImageType customPresetFolderImageType = HD_Settings_Folders.FolderImageType.Default;
        private Color customPresetSeparatorTextColor = Color.white;
        private bool customPresetSeparatorIsGradientBackground = false;
        private Color customPresetSeparatorBackgroundColor = Color.gray;
        private Gradient customPresetSeparatorBackgroundGradient = new();
        private int customPresetSeparatorFontSize = 12;
        private FontStyle customPresetSeparatorFontStyle = FontStyle.Normal;
        private TextAnchor customPresetSeparatorTextAlignment = TextAnchor.MiddleCenter;
        private HD_Settings_Separators.SeparatorImageType customPresetSeparatorBackgroundImageType = HD_Settings_Separators.SeparatorImageType.Default;
        private Color customPresetTagTextColor = Color.gray;
        private FontStyle customPresetTagFontStyle = FontStyle.BoldAndItalic;
        private int customPresetTagFontSize = 10;
        private TextAnchor customPresetTagTextAnchor = TextAnchor.MiddleRight;
        private Color customPresetLayerTextColor = Color.gray;
        private FontStyle customPresetLayerFontStyle = FontStyle.Bold;
        private int customPresetLayerFontSize = 10;
        private TextAnchor customPresetLayerTextAnchor = TextAnchor.MiddleLeft;
        private Color customPresetTreeColor = Color.white;
        private Color customPresetHierarchyLineColor = HD_Common_Color.HexToColor("00000080");
        private Color customPresetLockColor = Color.white;
        private int customPresetLockFontSize = 11;
        private FontStyle customPresetLockFontStyle = FontStyle.BoldAndItalic;
        private TextAnchor customPresetLockTextAnchor = TextAnchor.MiddleCenter;
        private List<HD_Settings_Presets.HD_Preset> customPresets;
        #endregion

        #region General Settings
        private Vector2 generalSettingsMainScroll;
        private const float enumPopupLabelWidth = 190;
        private const float generalSettingsMainToggleLabelWidth = 360;
        private const float generalSettingsFilterToggleLabelWidth = 300;
        private const float maskFieldLabelWidth = 145;
        private HD_Settings_General.HierarchyLayoutMode tempLayoutMode;
        private HD_Settings_General.HierarchyTreeMode tempTreeMode;
        private bool tempEnableGameObjectMainIcon;
        private bool tempEnableGameObjectComponentIcons;
        private bool tempEnableHierarchyTree;
        private bool tempEnableGameObjectTag;
        private bool tempEnableGameObjectLayer;
        private bool tempEnableHierarchyRows;
        private bool tempEnableHierarchyLines;
        private bool tempEnableHierarchyButtons;
        private bool tempEnableMajorShortcuts;
        private bool tempDisableHierarchyDesignerDuringPlayMode;
        private bool tempExcludeFolderProperties;
        private List<string> tempExcludedComponents;
        private int tempMaximumComponentIconsAmount;
        private List<string> tempExcludedTags;
        private List<string> tempExcludedLayers;
        private static bool generalSettingsHasModifiedChanges = false;
        #endregion

        #region Design Settings
        private Vector2 designSettingsMainScroll;
        private const float designSettingslabelWidth = 260;
        private float tempComponentIconsSize;
        private int tempComponentIconsOffset;
        private float tempComponentIconsSpacing;
        private Color tempHierarchyTreeColor;
        private HD_Settings_Design.TreeBranchImageType tempTreeBranchImageType_I;
        private HD_Settings_Design.TreeBranchImageType tempTreeBranchImageType_L;
        private HD_Settings_Design.TreeBranchImageType tempTreeBranchImageType_T;
        private HD_Settings_Design.TreeBranchImageType tempTreeBranchImageType_TerminalBud;
        private Color tempTagColor;
        private TextAnchor tempTagTextAnchor;
        private FontStyle tempTagFontStyle;
        private int tempTagFontSize;
        private Color tempLayerColor;
        private TextAnchor tempLayerTextAnchor;
        private FontStyle tempLayerFontStyle;
        private int tempLayerFontSize;
        private int tempTagLayerOffset;
        private int tempTagLayerSpacing;
        private Color tempHierarchyLineColor;
        private int tempHierarchyLineThickness;
        private Color tempFolderDefaultTextColor;
        private int tempFolderDefaultFontSize;
        private FontStyle tempFolderDefaultFontStyle;
        private Color tempFolderDefaultImageColor;
        private HD_Settings_Folders.FolderImageType tempFolderDefaultImageType;
        private Color tempSeparatorDefaultTextColor;
        private bool tempSeparatorDefaultIsGradientBackground;
        private Color tempSeparatorDefaultBackgroundColor;
        private Gradient tempSeparatorDefaultBackgroundGradient;
        private int tempSeparatorDefaultFontSize;
        private FontStyle tempSeparatorDefaultFontStyle;
        private TextAnchor tempSeparatorDefaultTextAnchor;
        private HD_Settings_Separators.SeparatorImageType tempSeparatorDefaultImageType;
        private int tempSeparatorLeftSideTextAnchorOffset;
        private int tempSeparatorCenterTextAnchorOffset;
        private int tempSeparatorRightSideTextAnchorOffset;
        private Color tempLockColor;
        private TextAnchor tempLockTextAnchor;
        private FontStyle tempLockFontStyle;
        private int tempLockFontSize;
        private static bool designSettingsHasModifiedChanges = false;
        #endregion

        #region Shortcut Settings
        private Vector2 shortcutSettingsMainScroll;
        private Vector2 minorShortcutSettingsScroll;
        private const float majorShortcutEnumToggleLabelWidth = 340;
        private const float minorShortcutCommandLabelWidth = 200;
        private const float minorShortcutLabelWidth = 400;
        private readonly List<string> minorShortcutIdentifiers = new()
        {
            "Hierarchy Designer/Open Hierarchy Designer Window",
            "Hierarchy Designer/Open Folder Panel",
            "Hierarchy Designer/Open Separator Panel",
            "Hierarchy Designer/Open Tools Panel",
            "Hierarchy Designer/Open Presets Panel",
            "Hierarchy Designer/Open Preset Creator Panel",
            "Hierarchy Designer/Open General Settings Panel",
            "Hierarchy Designer/Open Design Settings Panel",
            "Hierarchy Designer/Open Shortcut Settings Panel",
            "Hierarchy Designer/Open Advanced Settings Panel",
            "Hierarchy Designer/Open Rename Tool Window",
            "Hierarchy Designer/Create All Folders",
            "Hierarchy Designer/Create Default Folder",
            "Hierarchy Designer/Create Missing Folders",
            "Hierarchy Designer/Create All Separators",
            "Hierarchy Designer/Create Default Separator",
            "Hierarchy Designer/Create Missing Separators",
            "Hierarchy Designer/Refresh All GameObjects' Data",
            "Hierarchy Designer/Refresh Selected GameObject's Data",
            "Hierarchy Designer/Refresh Selected Main Icon",
            "Hierarchy Designer/Refresh Selected Component Icons",
            "Hierarchy Designer/Refresh Selected Hierarchy Tree Icon",
            "Hierarchy Designer/Refresh Selected Tag",
            "Hierarchy Designer/Refresh Selected Layer",
            "Hierarchy Designer/Refresh Selected Layer",
            "Hierarchy Designer/Transform GameObject into a Folder",
            "Hierarchy Designer/Transform Folder into a GameObject",
            "Hierarchy Designer/Transform GameObject into a Separator",
            "Hierarchy Designer/Transform Separator into a GameObject",
            "Hierarchy Designer/Expand All GameObjects",
            "Hierarchy Designer/Collapse All GameObjects",
        };
        private readonly Dictionary<string, string> minorShortcutTooltips = new()
        {
            { "Hierarchy Designer/Open Hierarchy Designer Window", "Opens the Hierarchy Designer window on the last opened panel." },
            { "Hierarchy Designer/Open Folder Panel", "Opens the Hierarchy Designer window on the folder panel." },
            { "Hierarchy Designer/Open Separator Panel", "Opens the Hierarchy Designer window on the separator panel." },
            { "Hierarchy Designer/Open Tools Panel", "Opens the Hierarchy Designer window on the tools panel." },
            { "Hierarchy Designer/Open Presets Panel", "Opens the Hierarchy Designer window on the presets panel." },
            { "Hierarchy Designer/Open Preset Creator Panel", "Opens the Hierarchy Designer window on the preset creator panel." },
            { "Hierarchy Designer/Open General Settings Panel", "Opens the Hierarchy Designer window on the general settings panel." },
            { "Hierarchy Designer/Open Design Settings Panel", "Opens the Hierarchy Designer window on the design settings panel." },
            { "Hierarchy Designer/Open Shortcut Settings Panel", "Opens the Hierarchy Designer window on the shortcut settings panel." },
            { "Hierarchy Designer/Open Advanced Settings Panel", "Opens the Hierarchy Designer window on the advanced settings panel." },
            { "Hierarchy Designer/Open Rename Tool Window", "Opens the Rename Tool Window." },
            { "Hierarchy Designer/Create All Folders", "Creates all folders from your folder list." },
            { "Hierarchy Designer/Create Default Folder", "Creates a default folder." },
            { "Hierarchy Designer/Create Missing Folders", "Creates any folders defined in your folder list that are missing in the scene." },
            { "Hierarchy Designer/Create All Separators", "Creates all separators from your separator list." },
            { "Hierarchy Designer/Create Default Separator", "Creates a default separator." },
            { "Hierarchy Designer/Create Missing Separators", "Creates any separators defined in your separator list that are missing in the scene." },
            { "Hierarchy Designer/Refresh All GameObjects' Data", "Refreshes all GameObjects' data (e.g., main icon, component icon, tag, layer, etc.).\n\nNote: Only applicable if core features are in Smart Mode." },
            { "Hierarchy Designer/Refresh Selected GameObject's Data", "Refreshes all GameObjects' data (e.g., main icon, component icon, tag, layer, etc.) of the selected GameObjects.\n\nNote: Only applicable if core features are in Smart Mode." },
            { "Hierarchy Designer/Refresh Selected Main Icon", "Refreshes the main icon of the selected GameObjects.\n\nNote: Only applicable if the main icon is in Smart Mode." },
            { "Hierarchy Designer/Refresh Selected Component Icons", "Refreshes the component icon of the selected GameObjects.\n\nNote: Only applicable if the component icon is in Smart Mode." },
            { "Hierarchy Designer/Refresh Selected Hierarchy Tree Icon", "Refreshes the hierarchy tree icon of the selected GameObjects.\n\nNote: Only applicable if the hierarchy tree icon is in Smart Mode." },
            { "Hierarchy Designer/Refresh Selected Tag", "Refreshes the tag of the selected GameObjects.\n\nNote: Only applicable if the tag feature is in Smart Mode." },
            { "Hierarchy Designer/Refresh Selected Layer", "Refreshes the layer of the selected GameObjects.\n\nNote: Only applicable if the layer feature is in Smart Mode." },
            { "Hierarchy Designer/Transform GameObject into a Folder", "Transforms the selected GameObject into a folder and adds it to the folders list." },
            { "Hierarchy Designer/Transform Folder into a GameObject", "Transforms the selected folder into a GameObject and removes it from the folders list." },
            { "Hierarchy Designer/Transform GameObject into a Separator", "Transforms the selected GameObject into a separator and adds it to the separators list." },
            { "Hierarchy Designer/Transform Separator into a GameObject", "Transforms the selected separator into a GameObject and removes it from the separators list." },
            { "Hierarchy Designer/Expand All GameObjects", "Expands all GameObjects in the Hierarchy." },
            { "Hierarchy Designer/Collapse All GameObjects", "Collapses all GameObjects in the Hierarchy." },
        };
        private KeyCode tempToggleGameObjectActiveStateKeyCode;
        private KeyCode tempToggleLockStateKeyCode;
        private KeyCode tempChangeTagLayerKeyCode;
        private KeyCode tempRenameSelectedGameObjectsKeyCode;
        private static bool shortcutSettingsHasModifiedChanges = false;
        #endregion

        #region Advanced Settings
        private Vector2 advancedSettingsMainScroll;
        private const float advancedSettingsEnumPopupLabelWidth = 250;
        private const float advancedSettingsToggleLabelWidth = 460;
        private HD_Settings_Advanced.HierarchyDesignerLocation tempHierarchyLocation;
        private HD_Settings_Advanced.UpdateMode tempMainIconUpdateMode;
        private HD_Settings_Advanced.UpdateMode tempComponentsIconsUpdateMode;
        private HD_Settings_Advanced.UpdateMode tempHierarchyTreeUpdateMode;
        private HD_Settings_Advanced.UpdateMode tempTagUpdateMode;
        private HD_Settings_Advanced.UpdateMode tempLayerUpdateMode;
        private bool tempEnableDynamicBackgroundForGameObjectMainIcon;
        private bool tempEnablePreciseRectForDynamicBackgroundForGameObjectMainIcon;
        private bool tempEnableCustomizationForGameObjectComponentIcons;
        private bool tempEnableTooltipOnComponentIconHovered;
        private bool tempEnableActiveStateEffectForComponentIcons;
        private bool tempDisableComponentIconsForInactiveGameObjects;
        private bool tempEnableCustomInspectorUI;
        private bool tempEnableEditorUtilities;
        private bool tempIncludeBackgroundImageForGradientBackground;
        private bool tempExcludeFoldersFromCountSelectToolCalculations;
        private bool tempExcludeSeparatorsFromCountSelectToolCalculations;
        private static bool advancedSettingsHasModifiedChanges = false;
        #endregion
        #endregion

        #region Initialization
        public static void OpenWindow()
        {
            HD_Window_Main editorWindow = GetWindow<HD_Window_Main>(HD_Common_Constants.AssetName);
            editorWindow.minSize = new(500, 400);
            UpdateCurrentWindowLabel();
        }

        private void OnEnable()
        {
            InitializeMenus();
            InitializeFontSizeOptions();
            LoadSessionData();
            LoadFolderData();
            LoadSeparatorData();
            LoadTools();
            LoadPresets();
            LoadGeneralSettingsData();
            LoadDesignSettingsData();
            LoadShortcutSettingsData();
            LoadAdvancedSettingsData();
        }

        private void InitializeMenus()
        {
            if (utilitiesMenuItems == null)
            {
                utilitiesMenuItems = new()
                {
                    { "Tools", () => { SelectToolsWindow(); } },
                    { "Presets", () => { SelectPresetsWindow(); } },
                    { "Preset Creator", () => { SelectPresetCreatorWindow(); } }
                };
            }

            if (configurationsMenuItems == null)
            {
                configurationsMenuItems = new()
                {
                    { "General Settings", () => { SelectGeneralSettingsWindow(); } },
                    { "Design Settings", () => { SelectDesignSettingsWindow(); } },
                    { "Shortcut Settings", () => { SelectShortcutSettingsWindow(); } },
                    { "Advanced Settings", () => { SelectAdvancedSettingsWindow(); } }
                };
            }
        }

        private void InitializeFontSizeOptions()
        {
            for (int i = 0; i < fontSizeOptions.Length; i++)
            {
                fontSizeOptions[i] = 7 + i;
            }
        }

        private void LoadSessionData()
        {
            if (!HD_Manager_Session.instance.IsPatchNotesLoaded)
            {
                patchNotes = HD_Common_File.GetPatchNotesData();
                HD_Manager_Session.instance.PatchNotesContent = patchNotes;
                HD_Manager_Session.instance.IsPatchNotesLoaded = true;
            }
            else
            {
                patchNotes = HD_Manager_Session.instance.PatchNotesContent;
            }

            currentWindow = HD_Manager_Session.instance.currentWindow;
        }
        #endregion

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.PrimaryPanelStyle);

            if (currentWindow != CurrentWindow.Home)
            {
                EditorGUILayout.BeginVertical(HD_Common_GUI.HeaderPanelStyle);
                headerButtonsScroll = EditorGUILayout.BeginScrollView(headerButtonsScroll, GUI.skin.horizontalScrollbar, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Hierarchy <color=#{(HD_Manager_Editor.IsProSkin ? "67F758" : "50C044")}>Designer</color>", HD_Common_GUI.HeaderLabelLeftStyle, GUILayout.Width(220));
                GUILayout.FlexibleSpace();
                DrawHeaderButtons();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"▷ {cachedCurrentWindowLabel}", HD_Common_GUI.TabLabelStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{HD_Common_Constants.AssetVersion}", HD_Common_GUI.VersionLabelHeaderStyle, GUILayout.Height(20));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            #region Body
            switch (currentWindow)
            {
                case CurrentWindow.Home:
                    DrawHomeTab();
                    break;
                case CurrentWindow.About:
                    DrawAboutPanel();
                    break;
                case CurrentWindow.Folders:
                    DrawFoldersTab();
                    break;
                case CurrentWindow.Separators:
                    DrawSeparatorsTab();
                    break;
                case CurrentWindow.Tools:
                    DrawToolsTab();
                    break;
                case CurrentWindow.Presets:
                    DrawPresetsTab();
                    break;
                case CurrentWindow.PresetCreator:
                    DrawPresetCreatorTab();
                    break;
                case CurrentWindow.GeneralSettings:
                    DrawGeneralSettingsTab();
                    break;
                case CurrentWindow.DesignSettings:
                    DrawDesignSettingsTab();
                    break;
                case CurrentWindow.ShortcutSettings:
                    DrawShortcutSettingsTab();
                    break;
                case CurrentWindow.AdvancedSettings:
                    DrawAdvancedSettingsTab();
                    break;
            }
            #endregion

            EditorGUILayout.EndVertical();
        }

        #region Methods
        #region General
        private void DrawHeaderButtons()
        {
            if (GUILayout.Button("FOLDERS", HD_Common_GUI.HeaderButtonStyle, GUILayout.Height(primaryButtonsHeight)))
            {
                SelectFoldersWindow();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("SEPARATORS", HD_Common_GUI.HeaderButtonStyle, GUILayout.Height(primaryButtonsHeight)))
            {
                SelectSeparatorsWindow();
            }

            GUILayout.Label("│", HD_Common_GUI.DivisorLabelStyle, GUILayout.Width(15), GUILayout.Height(primaryButtonsHeight));

            if (GUILayout.Button("HOME", HD_Common_GUI.HeaderButtonStyle, GUILayout.Height(primaryButtonsHeight)))
            {
                SelectHomeWindow();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("ABOUT", HD_Common_GUI.HeaderButtonStyle, GUILayout.Height(primaryButtonsHeight)))
            {
                SelectAboutWindow();
            }

            GUILayout.Label("│", HD_Common_GUI.DivisorLabelStyle, GUILayout.Width(15), GUILayout.Height(primaryButtonsHeight));

            HD_Common_GUI.DrawPopupButton("UTILITIES ▾", HD_Common_GUI.HeaderButtonStyle, primaryButtonsHeight, utilitiesMenuItems);
            
            GUILayout.Space(8);

            HD_Common_GUI.DrawPopupButton("CONFIGURATIONS ▾", HD_Common_GUI.HeaderButtonStyle, primaryButtonsHeight, configurationsMenuItems);
        }

        private void SelectFoldersWindow()
        {
            SwitchWindow(CurrentWindow.Folders);
        }

        private void SelectSeparatorsWindow()
        {
            SwitchWindow(CurrentWindow.Separators);
        }

        private void SelectHomeWindow()
        {
            SwitchWindow(CurrentWindow.Home);
        }

        private void SelectAboutWindow()
        {
            SwitchWindow(CurrentWindow.About);
        }

        private void SelectToolsWindow()
        {
            SwitchWindow(CurrentWindow.Tools);
        }

        private void SelectPresetsWindow()
        {
            SwitchWindow(CurrentWindow.Presets);
        }

        private void SelectPresetCreatorWindow()
        {
            SwitchWindow(CurrentWindow.PresetCreator);
        }

        private void SelectGeneralSettingsWindow()
        {
            SwitchWindow(CurrentWindow.GeneralSettings);
        }

        private void SelectDesignSettingsWindow()
        {
            SwitchWindow(CurrentWindow.DesignSettings);
        }

        private void SelectShortcutSettingsWindow()
        {
            SwitchWindow(CurrentWindow.ShortcutSettings);
        }

        private void SelectAdvancedSettingsWindow()
        {
            SwitchWindow(CurrentWindow.AdvancedSettings);
        }

        public static void SwitchWindow(CurrentWindow newWindow, Action extraAction = null)
        {
            if (currentWindow == newWindow) return;

            extraAction?.Invoke();
            currentWindow = newWindow;
            HD_Manager_Session.instance.currentWindow = currentWindow;
            UpdateCurrentWindowLabel();
        }

        private static void UpdateCurrentWindowLabel()
        {
            string name = currentWindow.ToString();
            string correctedName = System.Text.RegularExpressions.Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
            cachedCurrentWindowLabel = correctedName.ToUpper();
        }
        #endregion

        #region Home
        private void DrawHomeTab()
        {
            homeScroll = EditorGUILayout.BeginScrollView(homeScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.FlexibleSpace();

            DrawTitle();
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            DrawButtons();
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Space(40);
            DrawAssetVersion();

            EditorGUILayout.EndScrollView();
        }

        private void DrawTitle()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            float labelWidth = position.width * titleWidthPercentage;
            float labelHeight = labelWidth * titleAspectRatio;

            labelWidth = Mathf.Clamp(labelWidth, titleMinWidth, titleMaxWidth);
            labelHeight = Mathf.Clamp(labelHeight, titleMinHeight, titleMaxHeight);

            GUILayout.Label(HD_Manager_Editor.IsProSkin ? HD_Common_Resources.Graphics.TitleDark : HD_Common_Resources.Graphics.TitleLight, HD_Common_GUI.TitleLabelStyle, GUILayout.Width(labelWidth), GUILayout.Height(labelHeight));

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("FOLDERS", HD_Common_GUI.PrimaryButtonStyle, GUILayout.Height(primaryButtonsHeight)))
            {
                SwitchWindow(CurrentWindow.Folders);
            }

            GUILayout.Space(15);

            if (GUILayout.Button("SEPARATORS", HD_Common_GUI.PrimaryButtonStyle, GUILayout.Height(primaryButtonsHeight)))
            {
                SwitchWindow(CurrentWindow.Separators);
            }

            GUILayout.Space(15);

            if (GUILayout.Button("HOME", HD_Common_GUI.PrimaryButtonStyle, GUILayout.Height(primaryButtonsHeight)))
            {
                SwitchWindow(CurrentWindow.Home);
            }

            GUILayout.Space(15);

            if (GUILayout.Button("ABOUT", HD_Common_GUI.PrimaryButtonStyle, GUILayout.Height(primaryButtonsHeight)))
            {
                SwitchWindow(CurrentWindow.About);
            }

            GUILayout.Space(13);

            HD_Common_GUI.DrawPopupButton("UTILITIES ▾", HD_Common_GUI.PrimaryButtonStyle, primaryButtonsHeight, utilitiesMenuItems);

            GUILayout.Space(9);

            HD_Common_GUI.DrawPopupButton("CONFIGURATIONS ▾", HD_Common_GUI.PrimaryButtonStyle, primaryButtonsHeight, configurationsMenuItems);

            GUILayout.FlexibleSpace();
        }

        private void DrawAssetVersion()
        {
            GUILayout.Label($"{HD_Common_Constants.AssetVersion}", HD_Common_GUI.FooterLabelStyle, GUILayout.Height(20));
        }
        #endregion

        #region About
        private void DrawAboutPanel()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            DrawSummary();
            EditorGUILayout.BeginVertical();
            DrawPatchNotes();
            DrawMyOtherAssets();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawSummary()
        {
            EditorGUILayout.BeginHorizontal(HD_Common_GUI.SecondaryPanelStyle);
            aboutSummaryScroll = EditorGUILayout.BeginScrollView(aboutSummaryScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.Label("Features Breakdown", HD_Common_GUI.FieldsCategoryLabelStyle);

            GUILayout.Label("Folders", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Label(folderText, HD_Common_GUI.RegularLabelStyle);

            GUILayout.Label("Separators", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Label(separatorText, HD_Common_GUI.RegularLabelStyle);

            GUILayout.Label("Saved Data", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Label(savedDataText, HD_Common_GUI.RegularLabelStyle);

            GUILayout.Label("Additional Notes", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Label(additionalNotesText, HD_Common_GUI.RegularLabelStyle);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPatchNotes()
        {
            EditorGUILayout.BeginHorizontal(HD_Common_GUI.SecondaryPanelStyle);
            aboutPatchNotesScroll = EditorGUILayout.BeginScrollView(aboutPatchNotesScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label("Patch Notes", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Label(patchNotes, HD_Common_GUI.RegularLabelStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMyOtherAssets()
        {
            EditorGUILayout.BeginHorizontal(HD_Common_GUI.SecondaryPanelStyle);
            aboutMyOtherAssetsScroll = EditorGUILayout.BeginScrollView(aboutMyOtherAssetsScroll, GUILayout.MinHeight(200), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label("My Other Assets", HD_Common_GUI.FieldsCategoryCenterLabelStyle);

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();

            #region PicEase
            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("PicEase", HD_Common_GUI.MiniBoldLabelCenterStyle);
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(string.Empty, HD_Common_GUI.PromotionalPicEaseStyle))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/picease-297051");
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("An image editor, map generator and screenshot tool.", HD_Common_GUI.RegularLabelCenterStyle);
            EditorGUILayout.EndVertical();
            #endregion

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Folders
        private void DrawFoldersTab()
        {
            #region Body
            folderMainScroll = EditorGUILayout.BeginScrollView(folderMainScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawFoldersCreationFields();
            if (tempFolders.Count > 0)
            {
                DrawFoldersGlobalFields();
                DrawFoldersList();
            }
            else
            {
                EditorGUILayout.LabelField("No folders found. Please create a new folder.", HD_Common_GUI.UnassignedLabelStyle);
            }
            EditorGUILayout.EndScrollView();
            #endregion

            #region Footer
            if (GUILayout.Button("Update and Save Folders", GUILayout.Height(primaryButtonsHeight)))
            {
                UpdateAndSaveFoldersData();
            }
            #endregion
        }

        private void DrawFoldersCreationFields()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Folder Creation", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Space(defaultMarginSpacing);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(folderCreationLabelWidth));
            newFolderName = EditorGUILayout.TextField(newFolderName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Color", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(folderCreationLabelWidth));
            newFolderTextColor = EditorGUILayout.ColorField(newFolderTextColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            string[] newFontSizeOptionsStrings = Array.ConvertAll(fontSizeOptions, x => x.ToString());
            int newFontSizeIndex = Array.IndexOf(fontSizeOptions, newFolderFontSize);
            EditorGUILayout.LabelField("Font Size", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(folderCreationLabelWidth));
            newFolderFontSize = fontSizeOptions[EditorGUILayout.Popup(newFontSizeIndex, newFontSizeOptionsStrings)];
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Font Style", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(folderCreationLabelWidth));
            newFolderFontStyle = (FontStyle)EditorGUILayout.EnumPopup(newFolderFontStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Image Color", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(folderCreationLabelWidth));
            newFolderIconColor = EditorGUILayout.ColorField(newFolderIconColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Image Type", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(folderCreationLabelWidth));
            if (GUILayout.Button(HD_Settings_Folders.GetFolderImageTypeDisplayName(newFolderImageType), EditorStyles.popup))
            {
                ShowFolderImageTypePopup();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
            if (GUILayout.Button("Create Folder", GUILayout.Height(secondaryButtonsHeight)))
            {
                if (IsFolderNameValid(newFolderName))
                {
                    CreateFolder(newFolderName, newFolderTextColor, newFolderFontSize, newFolderFontStyle, newFolderIconColor, newFolderImageType);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Folder Name", "Folder name is either duplicate or invalid.", "OK");
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawFoldersGlobalFields()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Folders' Global Fields", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Space(defaultMarginSpacing);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            tempFolderGlobalTextColor = EditorGUILayout.ColorField(tempFolderGlobalTextColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalFolderTextColor(tempFolderGlobalTextColor); }
            EditorGUI.BeginChangeCheck();
            string[] tempFontSizeOptionsStrings = Array.ConvertAll(fontSizeOptions, x => x.ToString());
            int tempFontSizeIndex = Array.IndexOf(fontSizeOptions, tempFolderGlobalFontSize);
            tempFolderGlobalFontSize = fontSizeOptions[EditorGUILayout.Popup(tempFontSizeIndex, tempFontSizeOptionsStrings, GUILayout.Width(50))];
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalFolderFontSize(tempFolderGlobalFontSize); }
            EditorGUI.BeginChangeCheck();
            tempFolderGlobalFontStyle = (FontStyle)EditorGUILayout.EnumPopup(tempFolderGlobalFontStyle, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalFolderFontStyle(tempFolderGlobalFontStyle); }
            EditorGUI.BeginChangeCheck();
            tempFolderGlobalIconColor = EditorGUILayout.ColorField(tempFolderGlobalIconColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalFolderIconColor(tempFolderGlobalIconColor); }
            if (GUILayout.Button(HD_Settings_Folders.GetFolderImageTypeDisplayName(tempGlobalFolderImageType), EditorStyles.popup, GUILayout.MinWidth(125), GUILayout.ExpandWidth(true))) { ShowFolderImageTypePopupGlobal(); }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawFoldersList()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Folders' List", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Space(defaultMarginSpacing);

            foldersListScroll = EditorGUILayout.BeginScrollView(foldersListScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            int index = 1;
            for (int i = 0; i < foldersOrder.Count; i++)
            {
                string key = foldersOrder[i];
                DrawFolders(index, key, tempFolders[key], i, foldersOrder.Count);
                index++;
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

        }

        private void UpdateAndSaveFoldersData()
        {
            HD_Settings_Folders.ApplyChangesToFolders(tempFolders, foldersOrder);
            HD_Settings_Folders.SaveSettings();
            HD_Manager_GameObject.ClearFolderCache();
            folderHasModifiedChanges = false;
        }

        private void LoadFolderData()
        {
            tempFolders = HD_Settings_Folders.GetAllFoldersData(true);
            foldersOrder = new List<string>(tempFolders.Keys);
        }

        private void LoadFolderCreationFields()
        {
            newFolderTextColor = HD_Settings_Design.FolderDefaultTextColor;
            newFolderFontSize = HD_Settings_Design.FolderDefaultFontSize;
            newFolderFontStyle = HD_Settings_Design.FolderDefaultFontStyle;
            newFolderIconColor = HD_Settings_Design.FolderDefaultImageColor;
            HD_Settings_Folders.FolderImageType newFolderImageType = HD_Settings_Design.FolderDefaultImageType;
        }

        private bool IsFolderNameValid(string folderName)
        {
            return !string.IsNullOrEmpty(folderName) && !tempFolders.TryGetValue(folderName, out _);
        }

        private void CreateFolder(string folderName, Color textColor, int fontSize, FontStyle fontStyle, Color ImageColor, HD_Settings_Folders.FolderImageType imageType)
        {
            HD_Settings_Folders.HD_FolderData newFolderData = new HD_Settings_Folders.HD_FolderData
            {
                Name = folderName,
                TextColor = textColor,
                FontSize = fontSize,
                FontStyle = fontStyle,
                ImageColor = ImageColor,
                ImageType = imageType
            };
            tempFolders[folderName] = newFolderData;
            foldersOrder.Add(folderName);
            newFolderName = "";
            newFolderTextColor = HD_Settings_Design.FolderDefaultTextColor;
            newFolderFontSize = HD_Settings_Design.FolderDefaultFontSize;
            newFolderFontStyle = HD_Settings_Design.FolderDefaultFontStyle;
            newFolderIconColor = HD_Settings_Design.FolderDefaultImageColor;
            newFolderImageType = HD_Settings_Design.FolderDefaultImageType;
            folderHasModifiedChanges = true;
            GUI.FocusControl(null);
        }

        private void DrawFolders(int index, string key, HD_Settings_Folders.HD_FolderData folderData, int position, int totalItems)
        {
            float folderLabelWidth = HD_Common_GUI.CalculateMaxLabelWidth(tempFolders.Keys);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{index}) {folderData.Name}", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(folderLabelWidth));
            EditorGUI.BeginChangeCheck();
            folderData.TextColor = EditorGUILayout.ColorField(folderData.TextColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            string[] fontSizeOptionsStrings = Array.ConvertAll(fontSizeOptions, x => x.ToString());
            int fontSizeIndex = Array.IndexOf(fontSizeOptions, folderData.FontSize);
            if (fontSizeIndex == -1) { fontSizeIndex = 5; }
            folderData.FontSize = fontSizeOptions[EditorGUILayout.Popup(fontSizeIndex, fontSizeOptionsStrings, GUILayout.Width(50))];
            folderData.FontStyle = (FontStyle)EditorGUILayout.EnumPopup(folderData.FontStyle, GUILayout.Width(110));
            folderData.ImageColor = EditorGUILayout.ColorField(folderData.ImageColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            if (GUILayout.Button(HD_Settings_Folders.GetFolderImageTypeDisplayName(folderData.ImageType), EditorStyles.popup, GUILayout.MinWidth(125), GUILayout.ExpandWidth(true))) { ShowFolderImageTypePopupForFolder(folderData); }
            if (EditorGUI.EndChangeCheck()) { folderHasModifiedChanges = true; }

            if (GUILayout.Button("↑", GUILayout.Width(moveItemInListButtonWidth)) && position > 0)
            {
                MoveFolder(position, position - 1);
            }
            if (GUILayout.Button("↓", GUILayout.Width(moveItemInListButtonWidth)) && position < totalItems - 1)
            {
                MoveFolder(position, position + 1);
            }
            if (GUILayout.Button("Create", GUILayout.Width(createButtonWidth)))
            {
                CreateFolderGameObject(folderData);
            }
            if (GUILayout.Button("Remove", GUILayout.Width(removeButtonWidth)))
            {
                RemoveFolder(key);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void MoveFolder(int indexA, int indexB)
        {
            string keyA = foldersOrder[indexA];
            string keyB = foldersOrder[indexB];

            foldersOrder[indexA] = keyB;
            foldersOrder[indexB] = keyA;
            folderHasModifiedChanges = true;
        }

        private void CreateFolderGameObject(HD_Settings_Folders.HD_FolderData folderData)
        {
            GameObject folder = new GameObject(folderData.Name);
            folder.AddComponent<HierarchyDesignerFolder>();
            Undo.RegisterCreatedObjectUndo(folder, $"Create {folderData.Name}");

            Texture2D inspectorIcon = HD_Common_Resources.FolderInspectorIcon;
            if (inspectorIcon != null)
            {
                EditorGUIUtility.SetIconForObject(folder, inspectorIcon);
            }
        }

        private void RemoveFolder(string folderName)
        {
            if (tempFolders.TryGetValue(folderName, out _))
            {
                tempFolders.Remove(folderName);
                foldersOrder.Remove(folderName);
                folderHasModifiedChanges = true;
                GUIUtility.ExitGUI();
            }
        }

        private void ShowFolderImageTypePopup()
        {
            GenericMenu menu = new GenericMenu();
            Dictionary<string, List<string>> groupedTypes = HD_Settings_Folders.GetFolderImageTypesGrouped();
            foreach (KeyValuePair<string, List<string>> group in groupedTypes)
            {
                foreach (string typeName in group.Value)
                {
                    menu.AddItem(new GUIContent($"{group.Key}/{typeName}"), typeName == HD_Settings_Folders.GetFolderImageTypeDisplayName(newFolderImageType), OnFolderImageTypeSelected, typeName);
                }
            }
            menu.ShowAsContext();
        }

        private void ShowFolderImageTypePopupGlobal()
        {
            GenericMenu menu = new GenericMenu();
            Dictionary<string, List<string>> groupedTypes = HD_Settings_Folders.GetFolderImageTypesGrouped();
            foreach (KeyValuePair<string, List<string>> group in groupedTypes)
            {
                foreach (string typeName in group.Value)
                {
                    menu.AddItem(new GUIContent($"{group.Key}/{typeName}"), typeName == HD_Settings_Folders.GetFolderImageTypeDisplayName(tempGlobalFolderImageType), OnFolderImageTypeGlobalSelected, typeName);
                }
            }
            menu.ShowAsContext();
        }

        private void ShowFolderImageTypePopupForFolder(HD_Settings_Folders.HD_FolderData folderData)
        {
            GenericMenu menu = new GenericMenu();
            Dictionary<string, List<string>> groupedTypes = HD_Settings_Folders.GetFolderImageTypesGrouped();
            foreach (KeyValuePair<string, List<string>> group in groupedTypes)
            {
                foreach (string typeName in group.Value)
                {
                    menu.AddItem(new GUIContent($"{group.Key}/{typeName}"), typeName == HD_Settings_Folders.GetFolderImageTypeDisplayName(folderData.ImageType), OnFolderImageTypeForFolderSelected, new KeyValuePair<HD_Settings_Folders.HD_FolderData, string>(folderData, typeName));
                }
            }
            menu.ShowAsContext();
        }

        private void OnFolderImageTypeSelected(object imageTypeObj)
        {
            string typeName = (string)imageTypeObj;
            newFolderImageType = HD_Settings_Folders.ParseFolderImageType(typeName);
        }

        private void OnFolderImageTypeGlobalSelected(object imageTypeObj)
        {
            string typeName = (string)imageTypeObj;
            tempGlobalFolderImageType = HD_Settings_Folders.ParseFolderImageType(typeName);
            UpdateGlobalFolderImageType(tempGlobalFolderImageType);
        }

        private void OnFolderImageTypeForFolderSelected(object folderDataAndTypeObj)
        {
            KeyValuePair<HD_Settings_Folders.HD_FolderData, string> folderDataAndType = (KeyValuePair<HD_Settings_Folders.HD_FolderData, string>)folderDataAndTypeObj;
            folderDataAndType.Key.ImageType = HD_Settings_Folders.ParseFolderImageType(folderDataAndType.Value);
        }

        private void UpdateGlobalFolderTextColor(Color color)
        {
            foreach (HD_Settings_Folders.HD_FolderData folder in tempFolders.Values)
            {
                folder.TextColor = color;
            }
            folderHasModifiedChanges = true;
        }

        private void UpdateGlobalFolderFontSize(int size)
        {
            foreach (HD_Settings_Folders.HD_FolderData folder in tempFolders.Values)
            {
                folder.FontSize = size;
            }
            folderHasModifiedChanges = true;
        }

        private void UpdateGlobalFolderFontStyle(FontStyle style)
        {
            foreach (HD_Settings_Folders.HD_FolderData folder in tempFolders.Values)
            {
                folder.FontStyle = style;
            }
            folderHasModifiedChanges = true;
        }

        private void UpdateGlobalFolderIconColor(Color color)
        {
            foreach (HD_Settings_Folders.HD_FolderData folder in tempFolders.Values)
            {
                folder.ImageColor = color;
            }
            folderHasModifiedChanges = true;
        }

        private void UpdateGlobalFolderImageType(HD_Settings_Folders.FolderImageType imageType)
        {
            foreach (HD_Settings_Folders.HD_FolderData folder in tempFolders.Values)
            {
                folder.ImageType = imageType;
            }
            folderHasModifiedChanges = true;
        }
        #endregion

        #region Separators
        private void DrawSeparatorsTab()
        {
            #region Body
            separatorMainScroll = EditorGUILayout.BeginScrollView(separatorMainScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawSeparatorsCreationFields();

            if (tempSeparators.Count > 0)
            {
                DrawSeparatorsGlobalFields();
                DrawSeparatorsList();
            }
            else
            {
                EditorGUILayout.LabelField("No separators found. Please create a new separator.", HD_Common_GUI.UnassignedLabelStyle);
            }
            EditorGUILayout.EndScrollView();
            #endregion

            #region Footer
            if (GUILayout.Button("Update and Save Separators", GUILayout.Height(primaryButtonsHeight)))
            {
                UpdateAndSaveSeparatorsData();
            }
            #endregion
        }

        private void DrawSeparatorsCreationFields()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Separator Creation", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Space(defaultMarginSpacing);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(separatorCreationLabelWidth));
            newSeparatorName = EditorGUILayout.TextField(newSeparatorName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Color", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(separatorCreationLabelWidth));
            newSeparatorTextColor = EditorGUILayout.ColorField(newSeparatorTextColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Is Gradient", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(separatorCreationLabelWidth));
            newSeparatorIsGradient = EditorGUILayout.Toggle(newSeparatorIsGradient);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (newSeparatorIsGradient)
            {
                EditorGUILayout.LabelField("Background Gradient", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(separatorCreationLabelWidth));
                newSeparatorBackgroundGradient = EditorGUILayout.GradientField(newSeparatorBackgroundGradient) != null ? newSeparatorBackgroundGradient : new Gradient();
            }
            else
            {
                EditorGUILayout.LabelField("Background Color", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(separatorCreationLabelWidth));
                newSeparatorBackgroundColor = EditorGUILayout.ColorField(newSeparatorBackgroundColor);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            string[] newFontSizeOptionsStrings = Array.ConvertAll(fontSizeOptions, x => x.ToString());
            int newFontSizeIndex = Array.IndexOf(fontSizeOptions, newSeparatorFontSize);
            EditorGUILayout.LabelField("Font Size", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(separatorCreationLabelWidth));
            newSeparatorFontSize = fontSizeOptions[EditorGUILayout.Popup(newFontSizeIndex, newFontSizeOptionsStrings)];
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Font Style", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(separatorCreationLabelWidth));
            newSeparatorFontStyle = (FontStyle)EditorGUILayout.EnumPopup(newSeparatorFontStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Text Anchor", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(separatorCreationLabelWidth));
            newSeparatorTextAnchor = (TextAnchor)EditorGUILayout.EnumPopup(newSeparatorTextAnchor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Background Type", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(separatorCreationLabelWidth));
            if (GUILayout.Button(HD_Settings_Separators.GetSeparatorImageTypeDisplayName(newSeparatorImageType), EditorStyles.popup))
            {
                ShowSeparatorImageTypePopup();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);
            if (GUILayout.Button("Create Separator", GUILayout.Height(secondaryButtonsHeight)))
            {
                if (IsSeparatorNameValid(newSeparatorName))
                {
                    CreateSeparator(newSeparatorName, newSeparatorTextColor, newSeparatorIsGradient, newSeparatorBackgroundColor, newSeparatorBackgroundGradient, newSeparatorFontSize, newSeparatorFontStyle, newSeparatorTextAnchor, newSeparatorImageType);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Separator Name", "Separator name is either duplicate or invalid.", "OK");
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSeparatorsGlobalFields()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Separators' Global Fields", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Space(defaultMarginSpacing);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            tempSeparatorGlobalTextColor = EditorGUILayout.ColorField(tempSeparatorGlobalTextColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalSeparatorTextColor(tempSeparatorGlobalTextColor); }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space(defaultMarginSpacing);
            tempSeparatorGlobalIsGradient = EditorGUILayout.Toggle(tempSeparatorGlobalIsGradient, GUILayout.Width(18));
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalSeparatorIsGradientBackground(tempSeparatorGlobalIsGradient); }
            EditorGUI.BeginChangeCheck();
            tempSeparatorGlobalBackgroundColor = EditorGUILayout.ColorField(tempSeparatorGlobalBackgroundColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalSeparatorBackgroundColor(tempSeparatorGlobalBackgroundColor); }
            EditorGUI.BeginChangeCheck();
            tempSeparatorGlobalBackgroundGradient = EditorGUILayout.GradientField(tempSeparatorGlobalBackgroundGradient, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalSeparatorGradientBackground(tempSeparatorGlobalBackgroundGradient); }
            EditorGUI.BeginChangeCheck();
            string[] tempFontSizeOptionsStrings = Array.ConvertAll(fontSizeOptions, x => x.ToString());
            int tempFontSizeIndex = Array.IndexOf(fontSizeOptions, tempSeparatorGlobalFontSize);
            tempSeparatorGlobalFontSize = fontSizeOptions[EditorGUILayout.Popup(tempFontSizeIndex, tempFontSizeOptionsStrings, GUILayout.Width(50))];
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalSeparatorFontSize(tempSeparatorGlobalFontSize); }
            EditorGUI.BeginChangeCheck();
            tempSeparatorGlobalFontStyle = (FontStyle)EditorGUILayout.EnumPopup(tempSeparatorGlobalFontStyle, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalSeparatorFontStyle(tempSeparatorGlobalFontStyle); }
            EditorGUI.BeginChangeCheck();
            tempSeparatorGlobalTextAnchor = (TextAnchor)EditorGUILayout.EnumPopup(tempSeparatorGlobalTextAnchor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck()) { UpdateGlobalSeparatorTextAnchor(tempSeparatorGlobalTextAnchor); }
            if (GUILayout.Button(HD_Settings_Separators.GetSeparatorImageTypeDisplayName(tempSeparatorGlobalImageType), EditorStyles.popup, GUILayout.MinWidth(150), GUILayout.ExpandWidth(true))) { ShowSeparatorImageTypePopupGlobal(); }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawSeparatorsList()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Separators' List", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Space(defaultMarginSpacing);

            separatorsListScroll = EditorGUILayout.BeginScrollView(separatorsListScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            int index = 1;
            for (int i = 0; i < separatorsOrder.Count; i++)
            {
                string key = separatorsOrder[i];
                DrawSeparators(index, key, tempSeparators[key], i, separatorsOrder.Count);
                index++;
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void UpdateAndSaveSeparatorsData()
        {
            HD_Settings_Separators.ApplyChangesToSeparators(tempSeparators, separatorsOrder);
            HD_Settings_Separators.SaveSettings();
            HD_Manager_GameObject.ClearSeparatorCache();
            separatorHasModifiedChanges = false;
        }

        private void LoadSeparatorData()
        {
            tempSeparators = HD_Settings_Separators.GetAllSeparatorsData(true);
            separatorsOrder = new List<string>(tempSeparators.Keys);
        }

        private void LoadSeparatorsCreationFields()
        {
            newSeparatorName = "";
            newSeparatorTextColor = HD_Settings_Design.SeparatorDefaultTextColor;
            newSeparatorIsGradient = HD_Settings_Design.SeparatorDefaultIsGradientBackground;
            newSeparatorBackgroundColor = HD_Settings_Design.SeparatorDefaultBackgroundColor;
            newSeparatorBackgroundGradient = HD_Common_Color.CopyGradient(HD_Settings_Design.SeparatorDefaultBackgroundGradient);
            newSeparatorFontSize = HD_Settings_Design.SeparatorDefaultFontSize;
            newSeparatorFontStyle = HD_Settings_Design.SeparatorDefaultFontStyle;
            newSeparatorTextAnchor = HD_Settings_Design.SeparatorDefaultTextAnchor;
            _ = HD_Settings_Design.SeparatorDefaultImageType;
        }

        private bool IsSeparatorNameValid(string separatorName)
        {
            return !string.IsNullOrEmpty(separatorName) && !tempSeparators.TryGetValue(separatorName, out _);
        }

        private void CreateSeparator(string separatorName, Color textColor, bool isGradient, Color backgroundColor, Gradient backgroundGradient, int fontSize, FontStyle fontStyle, TextAnchor textAnchor, HD_Settings_Separators.SeparatorImageType imageType)
        {
            HD_Settings_Separators.HD_SeparatorData newSeparatorData = new()
            {
                Name = separatorName,
                TextColor = textColor,
                IsGradientBackground = isGradient,
                BackgroundColor = backgroundColor,
                BackgroundGradient = HD_Common_Color.CopyGradient(backgroundGradient),
                FontSize = fontSize,
                FontStyle = fontStyle,
                TextAnchor = textAnchor,
                ImageType = imageType,

            };
            tempSeparators[separatorName] = newSeparatorData;
            separatorsOrder.Add(separatorName);
            LoadSeparatorsCreationFields();
            separatorHasModifiedChanges = true;
            GUI.FocusControl(null);
        }

        private void DrawSeparators(int index, string key, HD_Settings_Separators.HD_SeparatorData separatorData, int position, int totalItems)
        {
            float separatorLabelWidth = HD_Common_GUI.CalculateMaxLabelWidth(tempSeparators.Keys);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{index}) {separatorData.Name}", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(separatorLabelWidth));
            EditorGUI.BeginChangeCheck();
            separatorData.TextColor = EditorGUILayout.ColorField(separatorData.TextColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true));
            GUILayout.Space(defaultMarginSpacing);
            separatorData.IsGradientBackground = EditorGUILayout.Toggle(separatorData.IsGradientBackground, GUILayout.Width(18));
            if (separatorData.IsGradientBackground) { separatorData.BackgroundGradient = EditorGUILayout.GradientField(separatorData.BackgroundGradient, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true)) ?? new Gradient(); }
            else { separatorData.BackgroundColor = EditorGUILayout.ColorField(separatorData.BackgroundColor, GUILayout.MinWidth(100), GUILayout.ExpandWidth(true)); }
            string[] fontSizeOptionsStrings = Array.ConvertAll(fontSizeOptions, x => x.ToString());
            int fontSizeIndex = Array.IndexOf(fontSizeOptions, separatorData.FontSize);
            if (fontSizeIndex == -1) { fontSizeIndex = 5; }
            separatorData.FontSize = fontSizeOptions[EditorGUILayout.Popup(fontSizeIndex, fontSizeOptionsStrings, GUILayout.Width(50))];
            separatorData.FontStyle = (FontStyle)EditorGUILayout.EnumPopup(separatorData.FontStyle, GUILayout.Width(100));
            separatorData.TextAnchor = (TextAnchor)EditorGUILayout.EnumPopup(separatorData.TextAnchor, GUILayout.Width(115));
            if (GUILayout.Button(HD_Settings_Separators.GetSeparatorImageTypeDisplayName(separatorData.ImageType), EditorStyles.popup, GUILayout.Width(150))) { ShowSeparatorImageTypePopupForSeparator(separatorData); }
            if (EditorGUI.EndChangeCheck()) { separatorHasModifiedChanges = true; }

            if (GUILayout.Button("↑", GUILayout.Width(moveItemInListButtonWidth)) && position > 0)
            {
                MoveSeparator(position, position - 1);
            }
            if (GUILayout.Button("↓", GUILayout.Width(moveItemInListButtonWidth)) && position < totalItems - 1)
            {
                MoveSeparator(position, position + 1);
            }
            if (GUILayout.Button("Create", GUILayout.Width(createButtonWidth)))
            {
                CreateSeparatorGameObject(separatorData);
            }
            if (GUILayout.Button("Remove", GUILayout.Width(removeButtonWidth)))
            {
                RemoveSeparator(key);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void MoveSeparator(int indexA, int indexB)
        {
            string keyA = separatorsOrder[indexA];
            string keyB = separatorsOrder[indexB];

            separatorsOrder[indexA] = keyB;
            separatorsOrder[indexB] = keyA;
            separatorHasModifiedChanges = true;
        }

        private void CreateSeparatorGameObject(HD_Settings_Separators.HD_SeparatorData separatorData)
        {
            GameObject separator = new($"{HD_Common_Constants.SeparatorPrefix}{separatorData.Name}");
            separator.tag = HD_Common_Constants.SeparatorTag;
            HD_Common_Operations.SetSeparatorState(separator, false);
            separator.SetActive(false);
            Undo.RegisterCreatedObjectUndo(separator, $"Create {separatorData.Name}");

            Texture2D inspectorIcon = HD_Common_Resources.SeparatorInspectorIcon;
            if (inspectorIcon != null)
            {
                EditorGUIUtility.SetIconForObject(separator, inspectorIcon);
            }
        }

        private void RemoveSeparator(string separatorName)
        {
            if (tempSeparators.TryGetValue(separatorName, out _))
            {
                tempSeparators.Remove(separatorName);
                separatorsOrder.Remove(separatorName);
                separatorHasModifiedChanges = true;
                GUIUtility.ExitGUI();
            }
        }

        private void ShowSeparatorImageTypePopup()
        {
            GenericMenu menu = new();
            Dictionary<string, List<string>> groupedTypes = HD_Settings_Separators.GetSeparatorImageTypesGrouped();
            foreach (KeyValuePair<string, List<string>> group in groupedTypes)
            {
                foreach (string typeName in group.Value)
                {
                    menu.AddItem(new GUIContent($"{group.Key}/{typeName}"), typeName == HD_Settings_Separators.GetSeparatorImageTypeDisplayName(newSeparatorImageType), OnSeparatorImageTypeSelected, typeName);
                }
            }
            menu.ShowAsContext();
        }

        private void ShowSeparatorImageTypePopupGlobal()
        {
            GenericMenu menu = new();
            Dictionary<string, List<string>> groupedTypes = HD_Settings_Separators.GetSeparatorImageTypesGrouped();
            foreach (KeyValuePair<string, List<string>> group in groupedTypes)
            {
                foreach (string typeName in group.Value)
                {
                    menu.AddItem(new GUIContent($"{group.Key}/{typeName}"), typeName == HD_Settings_Separators.GetSeparatorImageTypeDisplayName(tempSeparatorGlobalImageType), OnSeparatorImageTypeGlobalSelected, typeName);
                }
            }
            menu.ShowAsContext();
        }

        private void ShowSeparatorImageTypePopupForSeparator(HD_Settings_Separators.HD_SeparatorData separatorData)
        {
            GenericMenu menu = new();
            Dictionary<string, List<string>> groupedTypes = HD_Settings_Separators.GetSeparatorImageTypesGrouped();
            foreach (KeyValuePair<string, List<string>> group in groupedTypes)
            {
                foreach (string typeName in group.Value)
                {
                    menu.AddItem(new GUIContent($"{group.Key}/{typeName}"), typeName == HD_Settings_Separators.GetSeparatorImageTypeDisplayName(separatorData.ImageType), OnSeparatorImageTypeForSeparatorSelected, new KeyValuePair<HD_Settings_Separators.HD_SeparatorData, string>(separatorData, typeName));
                }
            }
            menu.ShowAsContext();
        }

        private void OnSeparatorImageTypeSelected(object imageTypeObj)
        {
            string typeName = (string)imageTypeObj;
            newSeparatorImageType = HD_Settings_Separators.ParseSeparatorImageType(typeName);
        }

        private void OnSeparatorImageTypeGlobalSelected(object imageTypeObj)
        {
            string typeName = (string)imageTypeObj;
            tempSeparatorGlobalImageType = HD_Settings_Separators.ParseSeparatorImageType(typeName);
            UpdateGlobalSeparatorImageType(tempSeparatorGlobalImageType);
        }

        private void OnSeparatorImageTypeForSeparatorSelected(object separatorDataAndTypeObj)
        {
            KeyValuePair<HD_Settings_Separators.HD_SeparatorData, string> separatorDataAndType = (KeyValuePair<HD_Settings_Separators.HD_SeparatorData, string>)separatorDataAndTypeObj;
            separatorDataAndType.Key.ImageType = HD_Settings_Separators.ParseSeparatorImageType(separatorDataAndType.Value);
        }

        private void UpdateGlobalSeparatorTextColor(Color color)
        {
            foreach (HD_Settings_Separators.HD_SeparatorData separator in tempSeparators.Values)
            {
                separator.TextColor = color;
            }
            separatorHasModifiedChanges = true;
        }

        private void UpdateGlobalSeparatorIsGradientBackground(bool isGradientBackground)
        {
            foreach (HD_Settings_Separators.HD_SeparatorData separator in tempSeparators.Values)
            {
                separator.IsGradientBackground = isGradientBackground;
            }
            separatorHasModifiedChanges = true;
        }

        private void UpdateGlobalSeparatorBackgroundColor(Color color)
        {
            foreach (HD_Settings_Separators.HD_SeparatorData separator in tempSeparators.Values)
            {
                separator.BackgroundColor = color;
            }
            separatorHasModifiedChanges = true;
        }

        private void UpdateGlobalSeparatorGradientBackground(Gradient gradientBackground)
        {
            foreach (HD_Settings_Separators.HD_SeparatorData separator in tempSeparators.Values)
            {
                separator.BackgroundGradient = HD_Common_Color.CopyGradient(gradientBackground);
            }
            separatorHasModifiedChanges = true;
        }

        private void UpdateGlobalSeparatorFontSize(int size)
        {
            foreach (HD_Settings_Separators.HD_SeparatorData separator in tempSeparators.Values)
            {
                separator.FontSize = size;
            }
            separatorHasModifiedChanges = true;
        }

        private void UpdateGlobalSeparatorFontStyle(FontStyle style)
        {
            foreach (HD_Settings_Separators.HD_SeparatorData separator in tempSeparators.Values)
            {
                separator.FontStyle = style;
            }
            separatorHasModifiedChanges = true;
        }

        private void UpdateGlobalSeparatorTextAnchor(TextAnchor anchor)
        {
            foreach (HD_Settings_Separators.HD_SeparatorData separator in tempSeparators.Values)
            {
                separator.TextAnchor = anchor;
            }
            separatorHasModifiedChanges = true;
        }

        private void UpdateGlobalSeparatorImageType(HD_Settings_Separators.SeparatorImageType imageType)
        {
            foreach (HD_Settings_Separators.HD_SeparatorData separator in tempSeparators.Values)
            {
                separator.ImageType = imageType;
            }
            separatorHasModifiedChanges = true;
        }
        #endregion

        #region Tools
        private void DrawToolsTab()
        {
            toolsMainScroll = EditorGUILayout.BeginScrollView(toolsMainScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            DrawToolsCategory();

            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            DrawToolsActions();
            EditorGUILayout.Space(defaultMarginSpacing);

            if (GUILayout.Button("Apply Action", GUILayout.Height(primaryButtonsHeight)))
            {
                if (availableActionMethods.Count > selectedActionIndex && selectedActionIndex >= 0)
                {
                    MethodInfo methodToInvoke = availableActionMethods[selectedActionIndex];
                    methodToInvoke?.Invoke(null, null);
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolsCategory()
        {
            HierarchyDesigner_Attribute_Tools previousCategory = selectedCategory;

            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Category Selection", HD_Common_GUI.FieldsCategoryLabelStyle, GUILayout.Width(labelWidth));
            GUILayout.Space(2);
            selectedCategory = HD_Common_GUI.DrawEnumPopup("Selected Category", 145, selectedCategory, HierarchyDesigner_Attribute_Tools.Activate);
            if (previousCategory != selectedCategory) UpdateAvailableActions();
            EditorGUILayout.EndVertical();
        }

        private void DrawToolsActions()
        {
           
            if (availableActionNames.Count == 0) 
            {
                GUILayout.Label("No tools available for this category.", HD_Common_GUI.UnassignedLabelStyle); 
            }
            else
            {
                EditorGUILayout.LabelField("Action Selection", HD_Common_GUI.FieldsCategoryLabelStyle, GUILayout.Width(labelWidth));
                GUILayout.Space(defaultMarginSpacing);
                selectedActionIndex = EditorGUILayout.Popup(selectedActionIndex, availableActionNames.ToArray());
            }
        }

        private void LoadTools()
        {
            if (!cacheInitialized)
            {
                InitializeActionCache();
                cacheInitialized = true;
            }
            UpdateAvailableActions();
        }

        private void InitializeActionCache()
        {
            MethodInfo[] methods = typeof(HD_Common_Menu).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo method in methods)
            {
                object[] toolAttributes = method.GetCustomAttributes(typeof(HD_Common_Attributes), false);
                for (int i = 0; i < toolAttributes.Length; i++)
                {
                    if (toolAttributes[i] is HD_Common_Attributes toolAttribute)
                    {
                        object[] menuItemAttributes = method.GetCustomAttributes(typeof(MenuItem), true);
                        for (int j = 0; j < menuItemAttributes.Length; j++)
                        {
                            MenuItem menuItemAttribute = menuItemAttributes[j] as MenuItem;
                            if (menuItemAttribute != null)
                            {
                                string rawActionName = menuItemAttribute.menuItem;
                                string actionName = ExtractActionsFromCategories(rawActionName, toolAttribute.Category);
                                if (!string.IsNullOrEmpty(actionName))
                                {
                                    if (!cachedActions.TryGetValue(toolAttribute.Category, out List<(string Name, MethodInfo Method)> actionsList))
                                    {
                                        actionsList = new();
                                        cachedActions[toolAttribute.Category] = actionsList;
                                    }
                                    actionsList.Add((actionName, method));
                                }
                            }
                        }
                    }
                }
            }
        }

        private string ExtractActionsFromCategories(string menuItemPath, HierarchyDesigner_Attribute_Tools category)
        {
            if (string.IsNullOrEmpty(menuItemPath)) return null;

            string[] parts = menuItemPath.Split('/');
            if (parts.Length < 2) return null;

            return category switch
            {
                HierarchyDesigner_Attribute_Tools.Rename or HierarchyDesigner_Attribute_Tools.Sort => parts[^1],
                HierarchyDesigner_Attribute_Tools.Activate or HierarchyDesigner_Attribute_Tools.Deactivate or HierarchyDesigner_Attribute_Tools.Count or HierarchyDesigner_Attribute_Tools.Lock or HierarchyDesigner_Attribute_Tools.Unlock or HierarchyDesigner_Attribute_Tools.Select => (parts.Length > 4) ? string.Join("/", parts, 4, parts.Length - 4) : null,
                _ => (parts.Length >= 2) ? $"{parts[^2]}/{parts[^1]}" : null,
            };
        }

        private void UpdateAvailableActions()
        {
            availableActionNames.Clear();
            availableActionMethods.Clear();
            if (cachedActions.TryGetValue(selectedCategory, out List<(string Name, MethodInfo Method)> actions))
            {
                foreach ((string Name, MethodInfo Method) in actions)
                {
                    availableActionNames.Add(Name);
                    availableActionMethods.Add(Method);
                }
            }
            selectedActionIndex = 0;
        }
        #endregion

        #region Presets
        private void DrawPresetsTab()
        {
            #region Body
            presetsMainScroll = EditorGUILayout.BeginScrollView(presetsMainScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawPresetsList();
            DrawPresetsFeaturesFields();
            EditorGUILayout.EndScrollView();
            #endregion

            #region Footer
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable All Features", GUILayout.Height(secondaryButtonsHeight)))
            {
                EnableAllFeatures(true);
            }
            if (GUILayout.Button("Disable All Features", GUILayout.Height(secondaryButtonsHeight)))
            {
                EnableAllFeatures(false);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Confirm and Apply Preset", GUILayout.Height(primaryButtonsHeight)))
            {
                ApplySelectedPreset();
            }
            EditorGUILayout.EndVertical();
            #endregion
        }

        private void DrawPresetsList()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Preset Selection", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Space(defaultMarginSpacing);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selected Preset", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(presetslabelWidth));
            if (GUILayout.Button(presetNames[selectedPresetIndex], EditorStyles.popup))
            {
                ShowPresetPopup();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawPresetsFeaturesFields()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Apply Preset To:", HD_Common_GUI.FieldsCategoryLabelStyle, GUILayout.Width(presetslabelWidth));
            GUILayout.Space(defaultMarginSpacing);

            applyToFolders = HD_Common_GUI.DrawToggle("Folders", presetsToggleLabelWidth, applyToFolders, true, true, "Applies the folder values from the currently selected preset (i.e., Text Color, Font Size, Font Style, Image Color, and Image Type) to all folders in your folder list.");
            applyToSeparators = HD_Common_GUI.DrawToggle("Separators", presetsToggleLabelWidth, applyToSeparators, true, true, "Applies the separator values from the currently selected preset (i.e., Text Color, Is Gradient Background, Background Color, Background Gradient, Font Size, Font Style, Text Alignment and Background Image Type) to all separators in your separators list.");
            applyToTag = HD_Common_GUI.DrawToggle("GameObject's Tag", presetsToggleLabelWidth, applyToTag, true, true, "Applies the tag values from the currently selected preset (i.e., Color, Font Style, Font Size, and Text Anchor) to the GameObject's Tag.");
            applyToLayer = HD_Common_GUI.DrawToggle("GameObject's Layer", presetsToggleLabelWidth, applyToLayer, true, true, "Applies the layer values from the currently selected preset (i.e., Color, Font Style, Font Size, and Text Anchor) to the GameObject's Layer.");
            applyToTree = HD_Common_GUI.DrawToggle("Hierarchy Tree", presetsToggleLabelWidth, applyToTree, true, true, "Applies the tree values from the currently selected preset (i.e., Color) to the Hierarchy Tree.");
            applyToLines = HD_Common_GUI.DrawToggle("Hierarchy Lines", presetsToggleLabelWidth, applyToLines, true, true, "Applies the line values from the currently selected preset (i.e., Color) to the Hierarchy Lines.");
            applyToFolderDefaultValues = HD_Common_GUI.DrawToggle("Folder Default Values", presetsToggleLabelWidth, applyToFolderDefaultValues, true, true, "Applies the default folder values from the currently selected preset (i.e., Text Color, Font Size, Font Style, Image Color, and Image Type) to folders that are not present in your folders list, as well as to the folder creation fields.");
            applyToSeparatorDefaultValues = HD_Common_GUI.DrawToggle("Separator Default Values", presetsToggleLabelWidth, applyToSeparatorDefaultValues, true, true, "Applies the default separator values from the currently selected preset (i.e., Text Color, Is Gradient Background, Background Color, Background Gradient, Font Size, Font Style, Text Alignment and Background Image Type) to separators that are not present in your separators list, as well as to the separator creation fields.");
            applyToLock = HD_Common_GUI.DrawToggle("Lock Label", presetsToggleLabelWidth, applyToLock, true, true, "Applies the lock label values from the currently selected preset (i.e., Color, Font Size, Font Style and Text Anchor) to the Lock Label.");
            EditorGUILayout.EndVertical();
        }

        private void LoadPresets()
        {
            List<string> combinedPresetNames = new();
            combinedPresetNames.AddRange(HD_Settings_Presets.GetPresetNames());
            customPresets = HD_Settings_Presets.CustomPresets;
            combinedPresetNames.AddRange(HD_Settings_Presets.CustomPresets.ConvertAll(p => p.presetName));
            presetNames = combinedPresetNames.ToArray();

            if (selectedPresetIndex >= presetNames.Length)
            {
                selectedPresetIndex = 0;
            }
        }

        private void ShowPresetPopup()
        {
            GenericMenu menu = new();
            Dictionary<string, List<string>> groupedPresets = HD_Settings_Presets.GetPresetNamesGrouped();

            foreach (KeyValuePair<string, List<string>> group in groupedPresets)
            {
                foreach (string presetName in group.Value)
                {
                    menu.AddItem(new($"{group.Key}/{presetName}"), presetName == presetNames[selectedPresetIndex], OnPresetSelected, presetName);
                }
            }

            menu.ShowAsContext();
        }

        private void OnPresetSelected(object presetNameObj)
        {
            string presetName = (string)presetNameObj;
            int newIndex = Array.IndexOf(presetNames, presetName);

            if (newIndex >= 0 && newIndex < presetNames.Length)
            {
                selectedPresetIndex = newIndex;
            }
            else
            {
                selectedPresetIndex = 0;
            }
        }

        private void ApplySelectedPreset()
        {
            if (selectedPresetIndex < 0 || selectedPresetIndex >= presetNames.Length) return;

            HD_Settings_Presets.HD_Preset selectedPreset;
            if (selectedPresetIndex < HD_Settings_Presets.DefaultPresets.Count)
            {
                selectedPreset = HD_Settings_Presets.DefaultPresets[selectedPresetIndex];
            }
            else
            {
                int customIndex = selectedPresetIndex - HD_Settings_Presets.DefaultPresets.Count;
                selectedPreset = HD_Settings_Presets.CustomPresets[customIndex];
            }

            string message = "Are you sure you want to override your current values for: ";
            List<string> changesList = new();
            if (applyToFolders) changesList.Add("Folders");
            if (applyToSeparators) changesList.Add("Separators");
            if (applyToTag) changesList.Add("GameObject's Tag");
            if (applyToLayer) changesList.Add("GameObject's Layer");
            if (applyToTree) changesList.Add("Hierarchy Tree");
            if (applyToLines) changesList.Add("Hierarchy Lines");
            if (applyToFolderDefaultValues) changesList.Add("Folder Default Values");
            if (applyToSeparatorDefaultValues) changesList.Add("Separator Default Values");
            if (applyToLock) changesList.Add("Lock Label");
            message += string.Join(", ", changesList) + "?\n\n*If you select 'confirm' all values will be overridden and saved.*";

            if (EditorUtility.DisplayDialog("Confirm Preset Application", message, "Confirm", "Cancel"))
            {
                if (applyToFolders)
                {
                    HD_Common_Operations.ApplyPresetToFolders(selectedPreset);
                    HD_Manager_GameObject.ClearFolderCache();
                }
                if (applyToSeparators)
                {
                    HD_Common_Operations.ApplyPresetToSeparators(selectedPreset);
                    HD_Manager_GameObject.ClearSeparatorCache();
                }
                bool shouldSaveDesignSettings = false;
                if (applyToTag)
                {
                    HD_Common_Operations.ApplyPresetToTag(selectedPreset);
                    shouldSaveDesignSettings = true;
                }
                if (applyToLayer)
                {
                    HD_Common_Operations.ApplyPresetToLayer(selectedPreset);
                    shouldSaveDesignSettings = true;
                }
                if (applyToTree)
                {
                    HD_Common_Operations.ApplyPresetToTree(selectedPreset);
                    shouldSaveDesignSettings = true;
                }
                if (applyToLines)
                {
                    HD_Common_Operations.ApplyPresetToLines(selectedPreset);
                    shouldSaveDesignSettings = true;
                }
                if (applyToFolderDefaultValues)
                {
                    HD_Common_Operations.ApplyPresetToDefaultFolderValues(selectedPreset);
                    shouldSaveDesignSettings = true;
                }
                if (applyToSeparatorDefaultValues)
                {
                    HD_Common_Operations.ApplyPresetToDefaultSeparatorValues(selectedPreset);
                    shouldSaveDesignSettings = true;
                }
                if (applyToLock)
                {
                    HD_Common_Operations.ApplyPresetToLockLabel(selectedPreset);
                    shouldSaveDesignSettings = true;
                }
                if (shouldSaveDesignSettings)
                {
                    HD_Settings_Design.SaveSettings();
                    LoadDesignSettingsData();
                    LoadFolderCreationFields();
                    LoadSeparatorsCreationFields();
                }
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        private void EnableAllFeatures(bool enable)
        {
            applyToFolders = enable;
            applyToSeparators = enable;
            applyToTag = enable;
            applyToLayer = enable;
            applyToTree = enable;
            applyToLines = enable;
            applyToFolderDefaultValues = enable;
            applyToSeparatorDefaultValues = enable;
            applyToLock = enable;
        }
        #endregion

        #region Preset Creator
        private void DrawPresetCreatorTab()
        {
            CustomPresetList();
            DrawPresetCreatorFields();

            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Create Custom Preset", GUILayout.Height(primaryButtonsHeight)))
            {
                CreateCustomPreset();
            }
            if (GUILayout.Button("Reset All Fields", GUILayout.Height(primaryButtonsHeight)))
            {
                ResetCustomPresetFields();
            }
            EditorGUILayout.EndVertical();
        }
        
        private void DrawPresetCreatorFields() 
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Custom Preset Creation", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Space(defaultMarginSpacing);

            presetCreatorMainScroll = EditorGUILayout.BeginScrollView(presetCreatorMainScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            #region General
            GUILayout.Label("General", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);

            customPresetName = HD_Common_GUI.DrawTextField("Custom Preset Name", customPresetsLabelWidth, string.Empty, customPresetName, true, "The name of your custom preset.");
            #endregion

            GUILayout.Space(customPresetsSpacing);

            #region Folder
            GUILayout.Label("Folder", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);

            customPresetFolderTextColor = HD_Common_GUI.DrawColorField("Text Color", customPresetsLabelWidth, "#FFFFFF", customPresetFolderTextColor, true, "The folder text color value in your custom preset.");
            customPresetFolderFontSize = HD_Common_GUI.DrawIntSlider("Font Size", customPresetsLabelWidth, customPresetFolderFontSize, 12, 7, 21, true, "The folder font size value in your custom preset.");
            customPresetFolderFontStyle = HD_Common_GUI.DrawEnumPopup("Font Style", customPresetsLabelWidth, customPresetFolderFontStyle, FontStyle.Normal, true, "The folder font style value in your custom preset.");
            customPresetFolderColor = HD_Common_GUI.DrawColorField("Color", customPresetsLabelWidth, "#FFFFFF", customPresetFolderColor, true, "The folder color value in your custom preset.");
            customPresetFolderImageType = HD_Common_GUI.DrawEnumPopup("Image Type", customPresetsLabelWidth, customPresetFolderImageType, HD_Settings_Folders.FolderImageType.Default, true, "The folder image type value in your custom preset.");
            #endregion

            GUILayout.Space(customPresetsSpacing);

            #region Separator
            GUILayout.Label("Separator", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);

            customPresetSeparatorTextColor = HD_Common_GUI.DrawColorField("Text Color", customPresetsLabelWidth, "#FFFFFF", customPresetSeparatorTextColor, true, "The separator text color value in your custom preset.");
            customPresetSeparatorIsGradientBackground = HD_Common_GUI.DrawToggle("Is Gradient Background", customPresetsLabelWidth, customPresetSeparatorIsGradientBackground, false, true, "The separator is gradient value in your custom preset.");
            customPresetSeparatorBackgroundColor = HD_Common_GUI.DrawColorField("Background Color", customPresetsLabelWidth, "#808080", customPresetSeparatorBackgroundColor, true, "The separator background color value in your custom preset.");
            customPresetSeparatorBackgroundGradient = HD_Common_GUI.DrawGradientField("Background Gradient", customPresetsLabelWidth, new Gradient(), customPresetSeparatorBackgroundGradient, true, "The separator background gradient color value in your custom preset.");
            customPresetSeparatorFontStyle = HD_Common_GUI.DrawEnumPopup("Font Style", customPresetsLabelWidth, customPresetSeparatorFontStyle, FontStyle.Normal, true, "The separator font style value in your custom preset.");
            customPresetSeparatorFontSize = HD_Common_GUI.DrawIntSlider("Font Size", customPresetsLabelWidth, customPresetSeparatorFontSize, 12, 7, 21, true, "The separator font size value in your custom preset.");
            customPresetSeparatorTextAlignment = HD_Common_GUI.DrawEnumPopup("Text Alignment", customPresetsLabelWidth, customPresetSeparatorTextAlignment, TextAnchor.MiddleCenter, true, "The separator text alignment value in your custom preset.");
            customPresetSeparatorBackgroundImageType = HD_Common_GUI.DrawEnumPopup("Image Type", customPresetsLabelWidth, customPresetSeparatorBackgroundImageType, HD_Settings_Separators.SeparatorImageType.Default, true, "The separator background image type value in your custom preset.");
            #endregion

            GUILayout.Space(customPresetsSpacing);

            #region Tag
            GUILayout.Label("Tag", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);

            customPresetTagTextColor = HD_Common_GUI.DrawColorField("Text Color", customPresetsLabelWidth, "#808080", customPresetTagTextColor, true, "The tag text color value in your custom preset.");
            customPresetTagFontStyle = HD_Common_GUI.DrawEnumPopup("Font Style", customPresetsLabelWidth, customPresetTagFontStyle, FontStyle.Bold, true, "The tag font style value in your custom preset.");
            customPresetTagFontSize = HD_Common_GUI.DrawIntSlider("Font Size", customPresetsLabelWidth, customPresetTagFontSize, 10, 7, 21, true, "The tag font size value in your custom preset.");
            customPresetTagTextAnchor = HD_Common_GUI.DrawEnumPopup("Text Anchor", customPresetsLabelWidth, customPresetTagTextAnchor, TextAnchor.MiddleRight, true, "The tag text anchor value in your custom preset.");
            #endregion

            GUILayout.Space(customPresetsSpacing);

            #region Layer
            GUILayout.Label("Layer", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);

            customPresetLayerTextColor = HD_Common_GUI.DrawColorField("Text Color", customPresetsLabelWidth, "#808080", customPresetLayerTextColor, true, "The layer text color value in your custom preset.");
            customPresetLayerFontStyle = HD_Common_GUI.DrawEnumPopup("Font Style", customPresetsLabelWidth, customPresetLayerFontStyle, FontStyle.Bold, true, "The layer font style value in your custom preset.");
            customPresetLayerFontSize = HD_Common_GUI.DrawIntSlider("Font Size", customPresetsLabelWidth, customPresetLayerFontSize, 10, 7, 21, true, "The layer font size value in your custom preset.");
            customPresetLayerTextAnchor = HD_Common_GUI.DrawEnumPopup("Text Anchor", customPresetsLabelWidth, customPresetLayerTextAnchor, TextAnchor.MiddleLeft, true, "The layer text anchor value in your custom preset.");
            #endregion

            GUILayout.Space(customPresetsSpacing);

            #region Tree
            GUILayout.Label("Tree", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);

            customPresetTreeColor = HD_Common_GUI.DrawColorField("Tree Color", customPresetsLabelWidth, "#FFFFFF", customPresetTreeColor, true, "The hierarchy tree color value in your custom preset.");
            #endregion

            GUILayout.Space(customPresetsSpacing);

            #region Lines
            GUILayout.Label("Lines", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);

            customPresetHierarchyLineColor = HD_Common_GUI.DrawColorField("Hierarchy Line Color", customPresetsLabelWidth, "#808080", customPresetHierarchyLineColor, true, "The hierarchy lines color value in your custom preset.");
            #endregion

            GUILayout.Space(customPresetsSpacing);

            #region Lock Label
            GUILayout.Label("Lock Label", HD_Common_GUI.MiniBoldLabelStyle);
            GUILayout.Space(2);

            customPresetLockColor = HD_Common_GUI.DrawColorField("Color", customPresetsLabelWidth, "#FFFFFF", customPresetLockColor, true, "The lock label color value in your custom preset.");
            customPresetLockFontSize = HD_Common_GUI.DrawIntSlider("Font Size", customPresetsLabelWidth, customPresetLockFontSize, 12, 7, 21, true, "The lock label font size value in your custom preset.");
            customPresetLockFontStyle = HD_Common_GUI.DrawEnumPopup("Font Style", customPresetsLabelWidth, customPresetLockFontStyle, FontStyle.Normal, true, "The lock label font style value in your custom preset.");
            customPresetLockTextAnchor = HD_Common_GUI.DrawEnumPopup("Text Anchor", customPresetsLabelWidth, customPresetLockTextAnchor, TextAnchor.MiddleCenter, true, "The lock label text anchor value in your custom preset.");
            #endregion

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();        
        }

        private void CustomPresetList()
        {
            if (customPresets.Count < 1) return;

            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            presetCreatorListScroll = EditorGUILayout.BeginScrollView(presetCreatorListScroll, GUILayout.MinHeight(80), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.LabelField("Custom Presets' List", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Space(defaultMarginSpacing);

            if (customPresets.Count > 0)
            {
                List<string> presetNames = customPresets.ConvertAll(preset => preset.presetName);
                float customPresetsLabelWidth = HD_Common_GUI.CalculateMaxLabelWidth(presetNames);

                for (int i = 0; i < customPresets.Count; i++)
                {
                    HD_Settings_Presets.HD_Preset customPreset = customPresets[i];

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(customPreset.presetName, HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(customPresetsLabelWidth));
                    if (GUILayout.Button("Delete", GUILayout.Height(secondaryButtonsHeight)))
                    {
                        bool delete = EditorUtility.DisplayDialog("Delete Preset", $"Are you sure you want to delete the custom preset '{customPreset.presetName}'?", "Yes", "No");
                        if (delete)
                        {
                            HD_Settings_Presets.CustomPresets.RemoveAt(i);
                            HD_Settings_Presets.SaveCustomPresets();
                            LoadPresets();
                            Repaint();
                            EditorGUILayout.EndHorizontal();
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No custom presets found.", HD_Common_GUI.UnassignedLabelLeftStyle);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void CreateCustomPreset()
        {
            if (IsPresetNameValid(customPresetName))
            {
                HD_Settings_Presets.HD_Preset newPreset = new(
                    customPresetName,
                    customPresetFolderTextColor, customPresetFolderFontSize, customPresetFolderFontStyle, customPresetFolderColor, customPresetFolderImageType,
                    customPresetSeparatorTextColor, customPresetSeparatorIsGradientBackground, customPresetSeparatorBackgroundColor, customPresetSeparatorBackgroundGradient,
                    customPresetSeparatorFontStyle, customPresetSeparatorFontSize, customPresetSeparatorTextAlignment, customPresetSeparatorBackgroundImageType,
                    customPresetTagTextColor, customPresetTagFontStyle, customPresetTagFontSize, customPresetTagTextAnchor,
                    customPresetLayerTextColor, customPresetLayerFontStyle, customPresetLayerFontSize, customPresetLayerTextAnchor,
                    customPresetTreeColor, customPresetHierarchyLineColor,
                    customPresetLockColor, customPresetLockFontSize, customPresetLockFontStyle, customPresetLockTextAnchor
                );

                HD_Settings_Presets.CustomPresets.Add(newPreset);
                HD_Settings_Presets.SaveCustomPresets();
                LoadPresets();

                EditorUtility.DisplayDialog("Success", $"Preset '{customPresetName}' created successfully.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid Preset Name", "Preset name is either duplicate or invalid.", "OK");
            }
        }

        private void ResetCustomPresetFields()
        {
            customPresetName = string.Empty;

            customPresetFolderTextColor = Color.white;
            customPresetFolderFontSize = 12;
            customPresetFolderFontStyle = FontStyle.Normal;
            customPresetFolderColor = Color.white;
            customPresetFolderImageType = HD_Settings_Folders.FolderImageType.Default;

            customPresetSeparatorTextColor = Color.white;
            customPresetSeparatorIsGradientBackground = false;
            customPresetSeparatorBackgroundColor = Color.gray;
            customPresetSeparatorBackgroundGradient = new Gradient();
            customPresetSeparatorFontSize = 12;
            customPresetSeparatorFontStyle = FontStyle.Normal;
            customPresetSeparatorTextAlignment = TextAnchor.MiddleCenter;
            customPresetSeparatorBackgroundImageType = HD_Settings_Separators.SeparatorImageType.Default;

            customPresetTagTextColor = Color.gray;
            customPresetTagFontStyle = FontStyle.BoldAndItalic;
            customPresetTagFontSize = 10;
            customPresetTagTextAnchor = TextAnchor.MiddleRight;

            customPresetLayerTextColor = Color.gray;
            customPresetLayerFontStyle = FontStyle.Bold;
            customPresetLayerFontSize = 10;
            customPresetLayerTextAnchor = TextAnchor.MiddleLeft;

            customPresetTreeColor = Color.white;
            customPresetHierarchyLineColor = HD_Common_Color.HexToColor("00000080");

            customPresetLockColor = Color.white;
            customPresetLockFontSize = 11;
            customPresetLockFontStyle = FontStyle.BoldAndItalic;
            customPresetLockTextAnchor = TextAnchor.MiddleCenter;

            Repaint();
        }

        private bool IsPresetNameValid(string presetName)
        {
            if (string.IsNullOrWhiteSpace(presetName)) return false;

            foreach (HD_Settings_Presets.HD_Preset preset in HD_Settings_Presets.DefaultPresets)
            {
                if (preset.presetName == presetName) return false;
            }

            foreach (HD_Settings_Presets.HD_Preset preset in HD_Settings_Presets.CustomPresets)
            {
                if (preset.presetName == presetName) return false;
            }

            return true;
        }
        #endregion

        #region General Settings
        private void DrawGeneralSettingsTab()
        {
            #region Body
            generalSettingsMainScroll = EditorGUILayout.BeginScrollView(generalSettingsMainScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawGeneralSettingsCoreFeatures();
            DrawGeneralSettingsMainFeatures();
            DrawGeneralSettingsFilteringFeatures();
            EditorGUILayout.EndScrollView();
            #endregion

            #region Footer
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable All Features", GUILayout.Height(secondaryButtonsHeight)))
            {
                EnableAllGeneralSettingsFeatures(true);
            }
            if (GUILayout.Button("Disable All Features", GUILayout.Height(secondaryButtonsHeight)))
            {
                EnableAllGeneralSettingsFeatures(false);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Update and Save General Settings", GUILayout.Height(primaryButtonsHeight)))
            {
                UpdateAndSaveGeneralSettingsData();
            }
            #endregion
        }

        private void DrawGeneralSettingsCoreFeatures()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Core Features", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempLayoutMode = HD_Common_GUI.DrawEnumPopup("Hierarchy Layout Mode", enumPopupLabelWidth, tempLayoutMode, HD_Settings_General.HierarchyLayoutMode.Split, true, "The layout of your Hierarchy window:\n\n• Consecutive: Elements are displayed after the GameObject's name and are drawn one after the other.\n\n• Docked: Elements are docked to the right side of your Hierarchy window.\n\n• Split: Elements are divided into two parts (consecutive and docked).");
            tempTreeMode = HD_Common_GUI.DrawEnumPopup("Hierarchy Tree Mode", enumPopupLabelWidth, tempTreeMode, HD_Settings_General.HierarchyTreeMode.Default, true, "The mode of the Hierarchy Tree feature:\n\n• Minimal: Uses the default tree branch and tree branch Type I for all parent-child relationships.\n\n• Default: Uses all four branch types (i.e., Type I, Type L, Type T, and Type T-Bud) for parent-child relationships.");
            if (EditorGUI.EndChangeCheck()) { generalSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawGeneralSettingsMainFeatures()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Main Features", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempEnableGameObjectMainIcon = HD_Common_GUI.DrawToggle("Enable GameObject's Main Icon", generalSettingsMainToggleLabelWidth, tempEnableGameObjectMainIcon, true, true, "Displays the main icon for GameObjects. Main icons are determined based on the order of components in a GameObject (i.e., default Unity behavior; usually, the second or third component becomes the main icon).\n\nNote: You can modify the main icon of a GameObject by moving the desired component to the second or third position.");
            tempEnableGameObjectComponentIcons = HD_Common_GUI.DrawToggle("Enable GameObject's Component Icons", generalSettingsMainToggleLabelWidth, tempEnableGameObjectComponentIcons, true, true, "Displays all components of the GameObject in the Hierarchy window.");
            tempEnableGameObjectTag = HD_Common_GUI.DrawToggle("Enable GameObject's Tag", generalSettingsMainToggleLabelWidth, tempEnableGameObjectTag, true, true, "Displays the tag of the GameObject in the Hierarchy window.");
            tempEnableGameObjectLayer = HD_Common_GUI.DrawToggle("Enable GameObject's Layer", generalSettingsMainToggleLabelWidth, tempEnableGameObjectLayer, true, true, "Displays the layer of the GameObject in the Hierarchy window.");
            tempEnableHierarchyTree = HD_Common_GUI.DrawToggle("Enable Hierarchy Tree", generalSettingsMainToggleLabelWidth, tempEnableHierarchyTree, true, true, "Displays the parent-child relationship of GameObjects in the Hierarchy window through branch icons.");
            tempEnableHierarchyRows = HD_Common_GUI.DrawToggle("Enable Hierarchy Rows", generalSettingsMainToggleLabelWidth, tempEnableHierarchyRows, true, true, "Draws background rows for alternating GameObjects in the Hierarchy window.");
            tempEnableHierarchyLines = HD_Common_GUI.DrawToggle("Enable Hierarchy Lines", generalSettingsMainToggleLabelWidth, tempEnableHierarchyLines, true, true, "Draws horizontal lines under each GameObject in the Hierarchy window.");
            tempEnableHierarchyButtons = HD_Common_GUI.DrawToggle("Enable Hierarchy Buttons", generalSettingsMainToggleLabelWidth, tempEnableHierarchyButtons, true, true, "Displays utility buttons (i.e., Active State and Lock State) for each GameObject in the Hierarchy window.");
            tempEnableMajorShortcuts = HD_Common_GUI.DrawToggle("Enable Major Shortcuts", generalSettingsMainToggleLabelWidth, tempEnableMajorShortcuts, true, true, "Allows major shortcuts (i.e., Toggle GameObject Active State and Lock State, Chage Selected Tag and Layer; and Rename Selected GameObjects) to be executed.\n\nNote: Disabling this feature improves performance as it will not check for input while interacting with the Hierarchy window.");
            tempDisableHierarchyDesignerDuringPlayMode = HD_Common_GUI.DrawToggle("Disable Hierarchy Designer During PlayMode", generalSettingsMainToggleLabelWidth, tempDisableHierarchyDesignerDuringPlayMode, true, true, "Disables the majority of Hierarchy Designer's features while in Play mode.\n\nNote: It is recommended to disable this feature only when debugging specific aspects of your game where performance is not a concern.");
            if (EditorGUI.EndChangeCheck()) { generalSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawGeneralSettingsFilteringFeatures()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Filtering Features", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempExcludeFolderProperties = HD_Common_GUI.DrawToggle("Exclude Folder Properties", generalSettingsFilterToggleLabelWidth, tempExcludeFolderProperties, true, true, "Excludes certain main features (i.e., Component Icons, Tag and Layer) for folder GameObjects.");
            string tempExcludedComponentsString = string.Join(", ", tempExcludedComponents);
            tempExcludedComponentsString = HD_Common_GUI.DrawTextField("Excluded Components Icons", generalSettingsFilterToggleLabelWidth, "Transform, RectTransform, CanvasRenderer", tempExcludedComponentsString, true, "Excludes a list of components from being displayed by the Component Icon main feature.\n\nUsage Example: Light, BoxCollider, Image\n\nHow to use: ComponentName (no space) + ,");
            tempExcludedComponents = tempExcludedComponentsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
            tempMaximumComponentIconsAmount = HD_Common_GUI.DrawIntSlider("Maximum Component Icons Amount", generalSettingsFilterToggleLabelWidth, tempMaximumComponentIconsAmount, 10 , 1, 25, true, "Limits how many Component Icons are allowed to be displayed for each GameObject.");
            if (EditorGUI.EndChangeCheck()) { generalSettingsHasModifiedChanges = true; }
            GUILayout.Space(defaultMarginSpacing);

            #region Tag
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            int tagMask = GetMaskFromList(tempExcludedTags, tags);
            EditorGUI.BeginChangeCheck();
            tagMask = HD_Common_GUI.DrawMaskField("Excluded Tags", maskFieldLabelWidth, tagMask, 1, tags, true, "Excludes selected tags from being displayed in the GameObject's Tag\nfeature.");
            if (EditorGUI.EndChangeCheck()) { generalSettingsHasModifiedChanges = true; }
            tempExcludedTags = GetListFromMask(tagMask, tags);
            #endregion

            #region Layer
            string[] layers = UnityEditorInternal.InternalEditorUtility.layers;
            int layerMask = GetMaskFromList(tempExcludedLayers, layers);
            layerMask = HD_Common_GUI.DrawMaskField("Excluded Layers", maskFieldLabelWidth, layerMask, 1, layers, true, "Excludes selected layers from being displayed in the GameObject's Layer feature.");
            EditorGUI.BeginChangeCheck();
            tempExcludedLayers = GetListFromMask(layerMask, layers);
            if (EditorGUI.EndChangeCheck()) { generalSettingsHasModifiedChanges = true; }
            #endregion
            EditorGUILayout.EndVertical();
        }

        private void UpdateAndSaveGeneralSettingsData()
        {
            HD_Settings_General.LayoutMode = tempLayoutMode;
            HD_Settings_General.TreeMode = tempTreeMode;
            HD_Settings_General.EnableGameObjectMainIcon = tempEnableGameObjectMainIcon;
            HD_Settings_General.EnableGameObjectComponentIcons = tempEnableGameObjectComponentIcons;
            HD_Settings_General.EnableHierarchyTree = tempEnableHierarchyTree;
            HD_Settings_General.EnableGameObjectTag = tempEnableGameObjectTag;
            HD_Settings_General.EnableGameObjectLayer = tempEnableGameObjectLayer;
            HD_Settings_General.EnableHierarchyRows = tempEnableHierarchyRows;
            HD_Settings_General.EnableHierarchyLines = tempEnableHierarchyLines;
            HD_Settings_General.EnableHierarchyButtons = tempEnableHierarchyButtons;
            HD_Settings_General.EnableMajorShortcuts = tempEnableMajorShortcuts;
            HD_Settings_General.DisableHierarchyDesignerDuringPlayMode = tempDisableHierarchyDesignerDuringPlayMode;
            HD_Settings_General.ExcludeFolderProperties = tempExcludeFolderProperties;
            HD_Settings_General.ExcludedComponents = tempExcludedComponents;
            HD_Settings_General.MaximumComponentIconsAmount = tempMaximumComponentIconsAmount;
            HD_Settings_General.ExcludedTags = tempExcludedTags;
            HD_Settings_General.ExcludedLayers = tempExcludedLayers;
            HD_Settings_General.SaveSettings();
            generalSettingsHasModifiedChanges = false;
        }

        private void LoadGeneralSettingsData()
        {
            tempLayoutMode = HD_Settings_General.LayoutMode;
            tempTreeMode = HD_Settings_General.TreeMode;
            tempEnableGameObjectMainIcon = HD_Settings_General.EnableGameObjectMainIcon;
            tempEnableGameObjectComponentIcons = HD_Settings_General.EnableGameObjectComponentIcons;
            tempEnableGameObjectTag = HD_Settings_General.EnableGameObjectTag;
            tempEnableGameObjectLayer = HD_Settings_General.EnableGameObjectLayer;
            tempEnableHierarchyTree = HD_Settings_General.EnableHierarchyTree;
            tempEnableHierarchyRows = HD_Settings_General.EnableHierarchyRows;
            tempEnableHierarchyLines = HD_Settings_General.EnableHierarchyLines;
            tempEnableHierarchyButtons = HD_Settings_General.EnableHierarchyButtons;
            tempEnableMajorShortcuts = HD_Settings_General.EnableMajorShortcuts;
            tempDisableHierarchyDesignerDuringPlayMode = HD_Settings_General.DisableHierarchyDesignerDuringPlayMode;
            tempExcludeFolderProperties = HD_Settings_General.ExcludeFolderProperties;
            tempExcludedComponents = HD_Settings_General.ExcludedComponents;
            tempMaximumComponentIconsAmount = HD_Settings_General.MaximumComponentIconsAmount;
            tempExcludedTags = HD_Settings_General.ExcludedTags;
            tempExcludedLayers = HD_Settings_General.ExcludedLayers;
        }

        private void EnableAllGeneralSettingsFeatures(bool enable)
        {
            tempEnableGameObjectMainIcon = enable;
            tempEnableGameObjectComponentIcons = enable;
            tempEnableGameObjectTag = enable;
            tempEnableGameObjectLayer = enable;
            tempEnableHierarchyTree = enable;
            tempEnableHierarchyRows = enable;
            tempEnableHierarchyLines = enable;
            tempEnableHierarchyButtons = enable;
            tempEnableMajorShortcuts = enable;
            tempDisableHierarchyDesignerDuringPlayMode = enable;
            tempExcludeFolderProperties = enable;
            generalSettingsHasModifiedChanges = true;
        }

        private int GetMaskFromList(List<string> list, string[] allItems)
        {
            int mask = 0;
            for (int i = 0; i < allItems.Length; i++)
            {
                if (list.Contains(allItems[i]))
                {
                    mask |= 1 << i;
                }
            }
            return mask;
        }

        private List<string> GetListFromMask(int mask, string[] allItems)
        {
            List<string> list = new();
            for (int i = 0; i < allItems.Length; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    list.Add(allItems[i]);
                }
            }
            return list;
        }
        #endregion

        #region Design Settings
        private void DrawDesignSettingsTab()
        {
            #region Body
            designSettingsMainScroll = EditorGUILayout.BeginScrollView(designSettingsMainScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawDesignSettingsComponentIcons();
            DrawDesignSettingsTag();
            DrawDesignSettingsLayer();
            DrawDesignSettingsTagAndLayer();
            DrawDesignSettingsHierarchyTree();
            DrawDesignSettingsHierarchyLines();
            DrawDesignSettingsFolder();
            DrawDesignSettingsSeparator();
            DrawDesignSettingsLock();
            EditorGUILayout.EndScrollView();
            #endregion

            #region Footer
            if (GUILayout.Button("Update and Save Design Settings", GUILayout.Height(primaryButtonsHeight)))
            {
                UpdateAndSaveDesignSettingsData();
            }
            #endregion
        }

        private void DrawDesignSettingsComponentIcons()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Component Icons", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempComponentIconsSize = HD_Common_GUI.DrawFloatSlider("Component Icons Size", designSettingslabelWidth, tempComponentIconsSize, 1.0f, 0.5f, 1.0f, true, "The size of component icons, where a value of 1 represents 100%.");
            tempComponentIconsOffset = HD_Common_GUI.DrawIntSlider("Component Icons Offset", designSettingslabelWidth, tempComponentIconsOffset, 21, 15, 30, true, "The horizontal offset position of component icons relative to their initial position, based on the Hierarchy Layout Mode.");
            tempComponentIconsSpacing = HD_Common_GUI.DrawFloatSlider("Component Icons Spacing", designSettingslabelWidth, tempComponentIconsSpacing, 2f, 0.0f, 10.0f, true, "The spacing between each component icon.");
            if (EditorGUI.EndChangeCheck()) { designSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawDesignSettingsTag()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Tag", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempTagColor = HD_Common_GUI.DrawColorField("Tag Color", designSettingslabelWidth, "#808080", tempTagColor, true, "The color of the GameObject's tag label.");
            tempTagFontSize = HD_Common_GUI.DrawIntSlider("Tag Font Size", designSettingslabelWidth, tempTagFontSize, 10, 7, 21, true, "The font size of the GameObject's tag label.");
            tempTagFontStyle = HD_Common_GUI.DrawEnumPopup("Tag Font Style", designSettingslabelWidth, tempTagFontStyle, FontStyle.BoldAndItalic, true, "The font style of the GameObject's tag label.");
            tempTagTextAnchor = HD_Common_GUI.DrawEnumPopup("Tag Text Anchor", designSettingslabelWidth, tempTagTextAnchor, TextAnchor.MiddleRight, true, "The text anchor of the GameObject's tag label.");
            if (EditorGUI.EndChangeCheck()) { designSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawDesignSettingsLayer()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Layer", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempLayerColor = HD_Common_GUI.DrawColorField("Layer Color", designSettingslabelWidth, "#808080", tempLayerColor, true, "The color of the GameObject's layer.");
            tempLayerFontSize = HD_Common_GUI.DrawIntSlider("Layer Font Size", designSettingslabelWidth, tempLayerFontSize, 10, 7, 21, true, "The font size of the GameObject's layer.");
            tempLayerFontStyle = HD_Common_GUI.DrawEnumPopup("Layer Font Style", designSettingslabelWidth, tempLayerFontStyle, FontStyle.BoldAndItalic, true, "The font style of the GameObject's layer.");
            tempLayerTextAnchor = HD_Common_GUI.DrawEnumPopup("Layer Text Anchor", designSettingslabelWidth, tempLayerTextAnchor, TextAnchor.MiddleLeft, true, "The text anchor of the GameObject's\nlayer.");
            if (EditorGUI.EndChangeCheck()) { designSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawDesignSettingsTagAndLayer()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Tag and Layer", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempTagLayerOffset = HD_Common_GUI.DrawIntSlider("Tag and Layer Offset", designSettingslabelWidth, tempTagLayerOffset, 5, 0, 20, true, "The horizontal offset position of the tag and layer labels relative to their initial position, based on the Hierarchy Layout Mode.");
            tempTagLayerSpacing = HD_Common_GUI.DrawIntSlider("Tag and Layer Spacing", designSettingslabelWidth, tempTagLayerSpacing, 5, 0, 20, true, "The spacing between the tag and layer labels.");
            if (EditorGUI.EndChangeCheck()) { designSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawDesignSettingsHierarchyTree()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Hierarchy Tree", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempHierarchyTreeColor = HD_Common_GUI.DrawColorField("Tree Color", designSettingslabelWidth, "#FFFFFF", tempHierarchyTreeColor, true, "The color of the Hierarchy Tree branches.");
            tempTreeBranchImageType_I = HD_Common_GUI.DrawEnumPopup("Tree Branch Image Type I", designSettingslabelWidth, tempTreeBranchImageType_I, HD_Settings_Design.TreeBranchImageType.Default, true, "The branch icon of the Hierarchy Tree's Branch Type I.");
            tempTreeBranchImageType_L = HD_Common_GUI.DrawEnumPopup("Tree Branch Image Type L", designSettingslabelWidth, tempTreeBranchImageType_L, HD_Settings_Design.TreeBranchImageType.Default, true, "The branch icon of the Hierarchy Tree's Branch Type L.");
            tempTreeBranchImageType_T = HD_Common_GUI.DrawEnumPopup("Tree Branch Image Type T", designSettingslabelWidth, tempTreeBranchImageType_T, HD_Settings_Design.TreeBranchImageType.Default, true, "The branch icon of the Hierarchy Tree's Branch Type T.");
            tempTreeBranchImageType_TerminalBud = HD_Common_GUI.DrawEnumPopup("Tree Branch Image Type T-Bud", designSettingslabelWidth, tempTreeBranchImageType_TerminalBud, HD_Settings_Design.TreeBranchImageType.Default, true, "The branch icon of the Hierarchy Tree's Branch Type T-Bud.");
            if (EditorGUI.EndChangeCheck()) { designSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawDesignSettingsHierarchyLines()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Hierarchy Lines", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempHierarchyLineColor = HD_Common_GUI.DrawColorField("Lines Color", designSettingslabelWidth, "#00000080", tempHierarchyLineColor, true, "The color of the Hierarchy Lines.");
            tempHierarchyLineThickness = HD_Common_GUI.DrawIntSlider("Lines Thickness", designSettingslabelWidth, tempHierarchyLineThickness, 1, 1, 3, true, "The thickness of the Hierarchy Lines.");
            if (EditorGUI.EndChangeCheck()) { designSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawDesignSettingsFolder()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Folder", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempFolderDefaultTextColor = HD_Common_GUI.DrawColorField("Default Text Color", designSettingslabelWidth, "#FFFFFF", tempFolderDefaultTextColor, true, "The text color for folders that are not present in your folder list, as well as the default text color value in the folder creation field.");
            tempFolderDefaultFontSize = HD_Common_GUI.DrawIntSlider("Default Font Size", designSettingslabelWidth, tempFolderDefaultFontSize, 12, 7, 21, true, "The font size for folders that are not present in your folder list, as well as the default font size value in the folder creation field.");
            tempFolderDefaultFontStyle = HD_Common_GUI.DrawEnumPopup("Default Font Style", designSettingslabelWidth, tempFolderDefaultFontStyle, FontStyle.Normal, true, "The font style for folders that are not present in your folder list, as well as the default font style value in the folder creation field.");
            tempFolderDefaultImageColor = HD_Common_GUI.DrawColorField("Default Image Color", designSettingslabelWidth, "#FFFFFF", tempFolderDefaultImageColor, true, "The image color for folders that are not present in your folder list, as well as the default image color value in the folder creation field.");
            tempFolderDefaultImageType = HD_Common_GUI.DrawEnumPopup("Default Image Type", designSettingslabelWidth, tempFolderDefaultImageType, HD_Settings_Folders.FolderImageType.Default, true, "The image type for folders that are not present in your folder list, as well as the default image type value in the folder creation field.");
            if (EditorGUI.EndChangeCheck()) { designSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawDesignSettingsSeparator()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Separator", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempSeparatorDefaultTextColor = HD_Common_GUI.DrawColorField("Default Text Color", designSettingslabelWidth, "#FFFFFF", tempSeparatorDefaultTextColor, true, "The text color for separators that are not present in your separators list, as well as the default text color value in the separator creation field.");
            tempSeparatorDefaultIsGradientBackground = HD_Common_GUI.DrawToggle("Default Is Gradient Background", designSettingslabelWidth, tempSeparatorDefaultIsGradientBackground, false, true, "The is gradient background for separators that are not present in your separators list, as well as the default is gradient background value in the separator creation field.");
            tempSeparatorDefaultBackgroundColor = HD_Common_GUI.DrawColorField("Default Background Color", designSettingslabelWidth, "#808080", tempSeparatorDefaultBackgroundColor, true, "The background color for separators that are not present in your separators list, as well as the default background color value in the separator creation field.");
            tempSeparatorDefaultBackgroundGradient = HD_Common_GUI.DrawGradientField("Default Background Gradient", designSettingslabelWidth, new(), tempSeparatorDefaultBackgroundGradient ?? new(), true, "The background gradient for separators that are not present in your separators list, as well as the default background gradient value in the separator creation field.");
            tempSeparatorDefaultFontSize = HD_Common_GUI.DrawIntSlider("Default Font Size", designSettingslabelWidth, tempSeparatorDefaultFontSize, 12, 7, 21, true, "The font size for separators that are not present in your separators list, as well as the default font size value in the separator creation field.");
            tempSeparatorDefaultFontStyle = HD_Common_GUI.DrawEnumPopup("Default Font Style", designSettingslabelWidth, tempSeparatorDefaultFontStyle, FontStyle.Normal, true, "The font style for separators that are not present in your separators list, as well as the default font style value in the separator creation field.");
            tempSeparatorDefaultTextAnchor = HD_Common_GUI.DrawEnumPopup("Default Text Anchor", designSettingslabelWidth, tempSeparatorDefaultTextAnchor, TextAnchor.MiddleCenter, true, "The text anchor for separators that are not present in your separators list, as well as the default text anchor value in the separator creation field.");
            tempSeparatorDefaultImageType = HD_Common_GUI.DrawEnumPopup("Default Image Type", designSettingslabelWidth, tempSeparatorDefaultImageType, HD_Settings_Separators.SeparatorImageType.Default, true, "The image type for separators that are not present in your separators list, as well as the default image type value in the separator creation field.");
            tempSeparatorLeftSideTextAnchorOffset = HD_Common_GUI.DrawIntSlider("Left Side Text Anchor Offset", designSettingslabelWidth, tempSeparatorLeftSideTextAnchorOffset, 3, 0, 33, true, "The horizontal left-side offset for separators with the following text anchor values: Upper Left, Middle Left, and Lower Left.");
            tempSeparatorCenterTextAnchorOffset = HD_Common_GUI.DrawIntSlider("Center Text Anchor Offset", designSettingslabelWidth, tempSeparatorCenterTextAnchorOffset, -15, -66, 66, true, "The horizontal center offset for separators with the following text anchor values: Middle Center, Upper Center, and Lower Center.");
            tempSeparatorRightSideTextAnchorOffset = HD_Common_GUI.DrawIntSlider("Right Side Text Anchor Offset", designSettingslabelWidth, tempSeparatorRightSideTextAnchorOffset, 36, 33, 66, true, "The horizontal right-side offset for separators with the following text anchor values: Upper Right, Middle Right, and Lower Right.");
            if (EditorGUI.EndChangeCheck()) { designSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawDesignSettingsLock()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Lock Label", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempLockColor = HD_Common_GUI.DrawColorField("Text Color", designSettingslabelWidth, "#FFFFFF", tempLockColor, true, "The color of the Lock label.");
            tempLockFontSize = HD_Common_GUI.DrawIntSlider("Font Size", designSettingslabelWidth, tempLockFontSize, 11, 7, 21, true, "The font size of the Lock label.");
            tempLockFontStyle = HD_Common_GUI.DrawEnumPopup("Font Style", designSettingslabelWidth, tempLockFontStyle, FontStyle.BoldAndItalic, true, "The font style of the Lock label.");
            tempLockTextAnchor = HD_Common_GUI.DrawEnumPopup("Text Anchor", designSettingslabelWidth, tempLockTextAnchor, TextAnchor.MiddleCenter, true, "The text anchor of the Lock label.");
            if (EditorGUI.EndChangeCheck()) { designSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void UpdateAndSaveDesignSettingsData()
        {
            HD_Settings_Design.ComponentIconsSize = tempComponentIconsSize;
            HD_Settings_Design.ComponentIconsOffset = tempComponentIconsOffset;
            HD_Settings_Design.ComponentIconsSpacing = tempComponentIconsSpacing;
            HD_Settings_Design.HierarchyTreeColor = tempHierarchyTreeColor;
            HD_Settings_Design.TreeBranchImageType_I = tempTreeBranchImageType_I;
            HD_Settings_Design.TreeBranchImageType_L = tempTreeBranchImageType_L;
            HD_Settings_Design.TreeBranchImageType_T = tempTreeBranchImageType_T;
            HD_Settings_Design.TreeBranchImageType_TerminalBud = tempTreeBranchImageType_TerminalBud;
            HD_Settings_Design.TagColor = tempTagColor;
            HD_Settings_Design.TagTextAnchor = tempTagTextAnchor;
            HD_Settings_Design.TagFontStyle = tempTagFontStyle;
            HD_Settings_Design.TagFontSize = tempTagFontSize;
            HD_Settings_Design.LayerColor = tempLayerColor;
            HD_Settings_Design.LayerTextAnchor = tempLayerTextAnchor;
            HD_Settings_Design.LayerFontStyle = tempLayerFontStyle;
            HD_Settings_Design.LayerFontSize = tempLayerFontSize;
            HD_Settings_Design.TagLayerOffset = tempTagLayerOffset;
            HD_Settings_Design.TagLayerSpacing = tempTagLayerSpacing;
            HD_Settings_Design.HierarchyLineColor = tempHierarchyLineColor;
            HD_Settings_Design.HierarchyLineThickness = tempHierarchyLineThickness;
            HD_Settings_Design.FolderDefaultTextColor = tempFolderDefaultTextColor;
            HD_Settings_Design.FolderDefaultFontSize = tempFolderDefaultFontSize;
            HD_Settings_Design.FolderDefaultFontStyle = tempFolderDefaultFontStyle;
            HD_Settings_Design.FolderDefaultImageColor = tempFolderDefaultImageColor;
            HD_Settings_Design.FolderDefaultImageType = tempFolderDefaultImageType;
            HD_Settings_Design.SeparatorDefaultTextColor = tempSeparatorDefaultTextColor;
            HD_Settings_Design.SeparatorDefaultIsGradientBackground = tempSeparatorDefaultIsGradientBackground;
            HD_Settings_Design.SeparatorDefaultBackgroundColor = tempSeparatorDefaultBackgroundColor;
            HD_Settings_Design.SeparatorDefaultBackgroundGradient = tempSeparatorDefaultBackgroundGradient;
            HD_Settings_Design.SeparatorDefaultFontSize = tempSeparatorDefaultFontSize;
            HD_Settings_Design.SeparatorDefaultFontStyle = tempSeparatorDefaultFontStyle;
            HD_Settings_Design.SeparatorDefaultTextAnchor = tempSeparatorDefaultTextAnchor;
            HD_Settings_Design.SeparatorDefaultImageType = tempSeparatorDefaultImageType;
            HD_Settings_Design.SeparatorLeftSideTextAnchorOffset = tempSeparatorLeftSideTextAnchorOffset;
            HD_Settings_Design.SeparatorCenterTextAnchorOffset = tempSeparatorCenterTextAnchorOffset;
            HD_Settings_Design.SeparatorRightSideTextAnchorOffset = tempSeparatorRightSideTextAnchorOffset;
            HD_Settings_Design.LockColor = tempLockColor;
            HD_Settings_Design.LockTextAnchor = tempLockTextAnchor;
            HD_Settings_Design.LockFontStyle = tempLockFontStyle;
            HD_Settings_Design.LockFontSize = tempLockFontSize;

            HD_Settings_Design.SaveSettings();
            HD_Manager_GameObject.ClearFolderCache();
            HD_Manager_GameObject.ClearSeparatorCache();
            designSettingsHasModifiedChanges = false;
        }

        private void LoadDesignSettingsData()
        {
            tempComponentIconsSize = HD_Settings_Design.ComponentIconsSize;
            tempComponentIconsOffset = HD_Settings_Design.ComponentIconsOffset;
            tempComponentIconsSpacing = HD_Settings_Design.ComponentIconsSpacing;
            tempHierarchyTreeColor = HD_Settings_Design.HierarchyTreeColor;
            tempTreeBranchImageType_I = HD_Settings_Design.TreeBranchImageType_I;
            tempTreeBranchImageType_L = HD_Settings_Design.TreeBranchImageType_L;
            tempTreeBranchImageType_T = HD_Settings_Design.TreeBranchImageType_T;
            tempTreeBranchImageType_TerminalBud = HD_Settings_Design.TreeBranchImageType_TerminalBud;
            tempTagColor = HD_Settings_Design.TagColor;
            tempTagTextAnchor = HD_Settings_Design.TagTextAnchor;
            tempTagFontStyle = HD_Settings_Design.TagFontStyle;
            tempTagFontSize = HD_Settings_Design.TagFontSize;
            tempLayerColor = HD_Settings_Design.LayerColor;
            tempLayerTextAnchor = HD_Settings_Design.LayerTextAnchor;
            tempLayerFontStyle = HD_Settings_Design.LayerFontStyle;
            tempLayerFontSize = HD_Settings_Design.LayerFontSize;
            tempTagLayerOffset = HD_Settings_Design.TagLayerOffset;
            tempTagLayerSpacing = HD_Settings_Design.TagLayerSpacing;
            tempHierarchyLineColor = HD_Settings_Design.HierarchyLineColor;
            tempHierarchyLineThickness = HD_Settings_Design.HierarchyLineThickness;
            tempFolderDefaultTextColor = HD_Settings_Design.FolderDefaultTextColor;
            tempFolderDefaultFontSize = HD_Settings_Design.FolderDefaultFontSize;
            tempFolderDefaultFontStyle = HD_Settings_Design.FolderDefaultFontStyle;
            tempFolderDefaultImageColor = HD_Settings_Design.FolderDefaultImageColor;
            tempFolderDefaultImageType = HD_Settings_Design.FolderDefaultImageType;
            tempSeparatorDefaultTextColor = HD_Settings_Design.SeparatorDefaultTextColor;
            tempSeparatorDefaultIsGradientBackground = HD_Settings_Design.SeparatorDefaultIsGradientBackground;
            tempSeparatorDefaultBackgroundColor = HD_Settings_Design.SeparatorDefaultBackgroundColor;
            tempSeparatorDefaultBackgroundGradient = HD_Settings_Design.SeparatorDefaultBackgroundGradient;
            tempSeparatorDefaultFontSize = HD_Settings_Design.SeparatorDefaultFontSize;
            tempSeparatorDefaultFontStyle = HD_Settings_Design.SeparatorDefaultFontStyle;
            tempSeparatorDefaultTextAnchor = HD_Settings_Design.SeparatorDefaultTextAnchor;
            tempSeparatorDefaultImageType = HD_Settings_Design.SeparatorDefaultImageType;
            tempSeparatorLeftSideTextAnchorOffset = HD_Settings_Design.SeparatorLeftSideTextAnchorOffset;
            tempSeparatorCenterTextAnchorOffset = HD_Settings_Design.SeparatorCenterTextAnchorOffset;
            tempSeparatorRightSideTextAnchorOffset = HD_Settings_Design.SeparatorRightSideTextAnchorOffset;
            tempLockColor = HD_Settings_Design.LockColor;
            tempLockTextAnchor = HD_Settings_Design.LockTextAnchor;
            tempLockFontStyle = HD_Settings_Design.LockFontStyle;
            tempLockFontSize = HD_Settings_Design.LockFontSize;
        }
        #endregion

        #region Shortcut Settings
        private void DrawShortcutSettingsTab()
        {
            #region Body
            shortcutSettingsMainScroll = EditorGUILayout.BeginScrollView(shortcutSettingsMainScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawMajorShortcuts();
            DrawMinorShortcuts();
            EditorGUILayout.EndScrollView();
            #endregion

            #region Footer
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Open Shortcut Manager", GUILayout.Height(primaryButtonsHeight)))
            {
                EditorApplication.ExecuteMenuItem("Edit/Shortcuts...");
            }
            if (GUILayout.Button("Update and Save Major Shortcut Settings", GUILayout.Height(primaryButtonsHeight)))
            {
                UpdateAndSaveShortcutSettingsData();
            }
            EditorGUILayout.EndVertical();
            #endregion
        }

        private void DrawMajorShortcuts()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Major Shortcuts", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempToggleGameObjectActiveStateKeyCode = HD_Common_GUI.DrawEnumPopup("Toggle GameObject Active State Key Code", majorShortcutEnumToggleLabelWidth, tempToggleGameObjectActiveStateKeyCode, KeyCode.Mouse2, true, "The key code to toggle the active state of the hovered GameObject or selected GameObjects. This input must be entered within the Hierarchy window, as it is only detected while interacting with the Hierarchy.");
            tempToggleLockStateKeyCode = HD_Common_GUI.DrawEnumPopup("Toggle GameObject Lock State Key Code", majorShortcutEnumToggleLabelWidth, tempToggleLockStateKeyCode, KeyCode.F1, true, "The key code to toggle the lock state of the hovered GameObject or selected GameObjects.\n\nNote: The Hierarchy window must be focused for this to work.");
            tempChangeTagLayerKeyCode = HD_Common_GUI.DrawEnumPopup("Change Selected Tag, Layer Key Code", majorShortcutEnumToggleLabelWidth, tempChangeTagLayerKeyCode, KeyCode.F2, true, "The key code to change the current tag or layer of a GameObject. Hover over the tag or layer and press the key code to apply\n\nNote: The Hierarchy window must be focused for this to work.");
            tempRenameSelectedGameObjectsKeyCode = HD_Common_GUI.DrawEnumPopup("Rename Selected GameObjects Key Code", majorShortcutEnumToggleLabelWidth, tempRenameSelectedGameObjectsKeyCode, KeyCode.F3, true, "The key code to rename the selected GameObject(s).\n\nNote: The Hierarchy window must be focused for this to work.");
            if (EditorGUI.EndChangeCheck()) { shortcutSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawMinorShortcuts()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Minor Shortcuts", HD_Common_GUI.FieldsCategoryLabelStyle);
            GUILayout.Space(defaultMarginSpacing);

            minorShortcutSettingsScroll = EditorGUILayout.BeginScrollView(minorShortcutSettingsScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            foreach (string shortcutId in minorShortcutIdentifiers)
            {
                ShortcutBinding currentBinding = ShortcutManager.instance.GetShortcutBinding(shortcutId);
                string[] parts = shortcutId.Split('/');
                string commandName = parts[^1];
                string tooltipText = minorShortcutTooltips.TryGetValue(shortcutId, out string tip) ? tip : string.Empty;

                EditorGUILayout.BeginHorizontal();

                if (!string.IsNullOrEmpty(tooltipText)) HD_Common_GUI.DrawTooltip(tooltipText);
                GUILayout.Space(4);
                GUILayout.Label(commandName + ":", HD_Common_GUI.LayoutLabelStyle, GUILayout.Width(minorShortcutCommandLabelWidth));

                bool hasKeyCombination = false;
                foreach (KeyCombination kc in currentBinding.keyCombinationSequence)
                {
                    if (!hasKeyCombination)
                    {
                        hasKeyCombination = true;
                        GUILayout.Label(kc.ToString(), HD_Common_GUI.AssignedLabelStyle, GUILayout.MinWidth(minorShortcutLabelWidth));
                    }
                    else
                    {
                        GUILayout.Label(" + " + kc.ToString(), HD_Common_GUI.AssignedLabelStyle, GUILayout.MinWidth(minorShortcutLabelWidth));
                    }
                }
                if (!hasKeyCombination)
                {
                    GUILayout.Label("unassigned shortcut", HD_Common_GUI.UnassignedLabelStyle, GUILayout.MinWidth(minorShortcutLabelWidth));
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(4);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.HelpBox("To modify minor shortcuts, please go to: Edit/Shortcuts.../Hierarchy Designer.\nYou can click the button below for quick access, then in the category section, search for Hierarchy Designer.", MessageType.Info);
        }

        private void UpdateAndSaveShortcutSettingsData()
        {
            HD_Settings_Shortcuts.ToggleGameObjectActiveStateKeyCode = tempToggleGameObjectActiveStateKeyCode;
            HD_Settings_Shortcuts.ToggleLockStateKeyCode = tempToggleLockStateKeyCode;
            HD_Settings_Shortcuts.ChangeTagLayerKeyCode = tempChangeTagLayerKeyCode;
            HD_Settings_Shortcuts.RenameSelectedGameObjectsKeyCode = tempRenameSelectedGameObjectsKeyCode;
            HD_Settings_Shortcuts.SaveSettings();
            shortcutSettingsHasModifiedChanges = false;
        }

        private void LoadShortcutSettingsData()
        {
            tempToggleGameObjectActiveStateKeyCode = HD_Settings_Shortcuts.ToggleGameObjectActiveStateKeyCode;
            tempToggleLockStateKeyCode = HD_Settings_Shortcuts.ToggleLockStateKeyCode;
            tempChangeTagLayerKeyCode = HD_Settings_Shortcuts.ChangeTagLayerKeyCode;
            tempRenameSelectedGameObjectsKeyCode = HD_Settings_Shortcuts.RenameSelectedGameObjectsKeyCode;
        }
        #endregion

        #region Advanced Settings
        private void DrawAdvancedSettingsTab()
        {
            #region Body
            advancedSettingsMainScroll = EditorGUILayout.BeginScrollView(advancedSettingsMainScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawAdvancedCoreFeatures();
            DrawAdvancedMainIconFeatures();
            DrawAdvancedComponentIconsFeatures();
            DrawAdvancedFolderFeatures();
            DrawAdvancedSeparatorFeatures();
            DrawAdvancedHierarchyToolsFeatures();
            EditorGUILayout.EndScrollView();
            #endregion

            #region Footer
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable All Features", GUILayout.Height(secondaryButtonsHeight)))
            {
                EnableAllAdvancedSettingsFeatures(true);
            }
            if (GUILayout.Button("Disable All Features", GUILayout.Height(secondaryButtonsHeight)))
            {
                EnableAllAdvancedSettingsFeatures(false);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Update and Save Advanced Settings", GUILayout.Height(primaryButtonsHeight)))
            {
                UpdateAndSaveAdvancedSettingsData();
            }
            #endregion
        }

        private void DrawAdvancedCoreFeatures()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Core Features", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempHierarchyLocation = HD_Common_GUI.DrawEnumPopup("Hierarchy Designer Location", advancedSettingsEnumPopupLabelWidth, tempHierarchyLocation, HD_Settings_Advanced.HierarchyDesignerLocation.Tools, true, "The location of Hierarchy Designer in the top menu bar (e.g., Tools/Hierarchy Designer, Plugins/Hierarchy Designer, etc.).\n\nNote: Modifying this setting will trigger a script recompilation.");
            tempMainIconUpdateMode = HD_Common_GUI.DrawEnumPopup("Main Icon Update Mode", advancedSettingsEnumPopupLabelWidth, tempMainIconUpdateMode, HD_Settings_Advanced.UpdateMode.Dynamic, true, "The update mode for the Main Icon feature:\n\nDynamic: Checks for changes dynamically during Hierarchy events.\n\nSmart: Checks periodically, such as during scene open/reload or script recompilation.\n\nNote: In Smart mode, you can manually check for changes by refreshing through the context menus or using minor shortcuts.");
            tempComponentsIconsUpdateMode = HD_Common_GUI.DrawEnumPopup("Component Icons Update Mode", advancedSettingsEnumPopupLabelWidth, tempComponentsIconsUpdateMode, HD_Settings_Advanced.UpdateMode.Dynamic, true, "The update mode for the Component Icons feature:\n\nDynamic: Checks for changes dynamically during Hierarchy events.\n\nSmart: Checks periodically, such as during scene open/reload or script recompilation.\n\nNote: In Smart mode, you can manually check for changes by refreshing through the context menus or using minor shortcuts.");
            tempHierarchyTreeUpdateMode = HD_Common_GUI.DrawEnumPopup("Hierarchy Tree Update Mode", advancedSettingsEnumPopupLabelWidth, tempHierarchyTreeUpdateMode, HD_Settings_Advanced.UpdateMode.Dynamic, true, "The update mode for the Hierarchy Tree feature:\n\nDynamic: Checks for changes dynamically during Hierarchy events.\n\nSmart: Checks periodically, such as during scene open/reload or script recompilation.\n\nNote: In Smart mode, you can manually check for changes by refreshing through the context menus or using minor shortcuts.");
            tempTagUpdateMode = HD_Common_GUI.DrawEnumPopup("Tag Update Mode", advancedSettingsEnumPopupLabelWidth, tempTagUpdateMode, HD_Settings_Advanced.UpdateMode.Dynamic, true, "The update mode for the Tag feature:\n\nDynamic: Checks for changes dynamically during Hierarchy events.\n\nSmart: Checks periodically, such as during scene open/reload or script recompilation.\n\nNote: In Smart mode, you can manually check for changes by refreshing through the context menus or using minor shortcuts.");
            tempLayerUpdateMode = HD_Common_GUI.DrawEnumPopup("Layer Update Mode", advancedSettingsEnumPopupLabelWidth, tempLayerUpdateMode, HD_Settings_Advanced.UpdateMode.Dynamic, true, "The update mode for the Layer feature:\n\nDynamic: Checks for changes dynamically during Hierarchy events.\n\nSmart: Checks periodically, such as during scene open/reload or script recompilation.\n\nNote: In Smart mode, you can manually check for changes by refreshing through the context menus or using minor shortcuts.");
            if (EditorGUI.EndChangeCheck()) { advancedSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawAdvancedMainIconFeatures()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Main Icon", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempEnableDynamicBackgroundForGameObjectMainIcon = HD_Common_GUI.DrawToggle("Enable Dynamic Background", advancedSettingsToggleLabelWidth, tempEnableDynamicBackgroundForGameObjectMainIcon, true, true, "The background of the main icon will match the background color of the Hierarchy window (i.e., Editor Light, Dark Mode, GameObject Selected, Focused, Unfocused).");
            tempEnablePreciseRectForDynamicBackgroundForGameObjectMainIcon = HD_Common_GUI.DrawToggle("Enable Precise Rect For Dynamic Background", advancedSettingsToggleLabelWidth, tempEnablePreciseRectForDynamicBackgroundForGameObjectMainIcon, true, true, "Uses precise rect calculations for pointer/mouse detection utilized by the Dynamic Background feature.");
            if (EditorGUI.EndChangeCheck()) { advancedSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawAdvancedComponentIconsFeatures()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Component Icons", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempEnableCustomizationForGameObjectComponentIcons = HD_Common_GUI.DrawToggle("Enable Design Customization For Component Icons", advancedSettingsToggleLabelWidth, tempEnableCustomizationForGameObjectComponentIcons, true, true, "Enables calculation of component icon design settings (e.g., Component Icon Size, Offset, and Spacing).\n\nNote: If you are using the default values, you may turn this off to reduce extra calculations in the component icon logic.");
            tempEnableTooltipOnComponentIconHovered = HD_Common_GUI.DrawToggle("Enable Tooltip For Component Icons", advancedSettingsToggleLabelWidth, tempEnableTooltipOnComponentIconHovered, true, true, "Displays the component name when hovering over the component icon.");
            tempEnableActiveStateEffectForComponentIcons = HD_Common_GUI.DrawToggle("Enable Active State Effect For Component Icons", advancedSettingsToggleLabelWidth, tempEnableActiveStateEffectForComponentIcons, true, true, "Displays which components are disabled for a given object.");
            tempDisableComponentIconsForInactiveGameObjects = HD_Common_GUI.DrawToggle("Disable Component Icons For Inactive GameObjects", advancedSettingsToggleLabelWidth, tempDisableComponentIconsForInactiveGameObjects, true, true, "Hides component icons for inactive GameObjects.");
            if (EditorGUI.EndChangeCheck()) { advancedSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawAdvancedFolderFeatures()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Folders", HD_Common_GUI.FieldsCategoryLabelStyle);

            #region Runtime Folder
            EditorGUILayout.LabelField("Runtime Folder", HD_Common_GUI.MiniBoldLabelStyle);
            EditorGUILayout.Space(2);

            EditorGUI.BeginChangeCheck();
            tempEnableCustomInspectorUI = HD_Common_GUI.DrawToggle("Enable Custom Inspector UI", advancedSettingsToggleLabelWidth, tempEnableCustomInspectorUI, true, true, "Enables a custom inspector UI for Folder GameObjects.");
            tempEnableEditorUtilities = HD_Common_GUI.DrawToggle("Enable Editor Utilities", advancedSettingsToggleLabelWidth, tempEnableEditorUtilities, true, true, "Enables editor-only utilities (e.g., Toggle Active State, Delete, Children List, etc.) in the inspector window for Folder GameObjects.");
            if (EditorGUI.EndChangeCheck()) { advancedSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
            #endregion
        }

        private void DrawAdvancedSeparatorFeatures()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Separators", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempIncludeBackgroundImageForGradientBackground = HD_Common_GUI.DrawToggle("Include Background Image For Gradient Background", advancedSettingsToggleLabelWidth, tempIncludeBackgroundImageForGradientBackground, true, true, "Includes the Background Image Type for Separators that uses a gradient background. The background image type will be used first, followed by the gradient placed on top.");
            if (EditorGUI.EndChangeCheck()) { advancedSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void DrawAdvancedHierarchyToolsFeatures()
        {
            EditorGUILayout.BeginVertical(HD_Common_GUI.SecondaryPanelStyle);
            EditorGUILayout.LabelField("Hierarchy Tools", HD_Common_GUI.FieldsCategoryLabelStyle);

            EditorGUI.BeginChangeCheck();
            tempExcludeFoldersFromCountSelectToolCalculations = HD_Common_GUI.DrawToggle("Exclude Folders From Count-Select Tool Calculations", advancedSettingsToggleLabelWidth, tempExcludeFoldersFromCountSelectToolCalculations, true, true, "Excludes Folder GameObjects from Count and Select tool calculations.");
            tempExcludeSeparatorsFromCountSelectToolCalculations = HD_Common_GUI.DrawToggle("Exclude Separators From Count-Select Tool Calculations", advancedSettingsToggleLabelWidth, tempExcludeSeparatorsFromCountSelectToolCalculations, true, true, "Excludes Separator GameObjects from Count and Select tool calculations.");
            if (EditorGUI.EndChangeCheck()) { advancedSettingsHasModifiedChanges = true; }
            EditorGUILayout.EndVertical();
        }

        private void UpdateAndSaveAdvancedSettingsData()
        {
            bool hierarchyLocationChanged = tempHierarchyLocation != HD_Settings_Advanced.HierarchyLocation;

            HD_Settings_Advanced.HierarchyLocation = tempHierarchyLocation;
            HD_Settings_Advanced.MainIconUpdateMode = tempMainIconUpdateMode;
            HD_Settings_Advanced.ComponentsIconsUpdateMode = tempComponentsIconsUpdateMode;
            HD_Settings_Advanced.HierarchyTreeUpdateMode = tempHierarchyTreeUpdateMode;
            HD_Settings_Advanced.TagUpdateMode = tempTagUpdateMode;
            HD_Settings_Advanced.LayerUpdateMode = tempLayerUpdateMode;
            HD_Settings_Advanced.EnableDynamicBackgroundForGameObjectMainIcon = tempEnableDynamicBackgroundForGameObjectMainIcon;
            HD_Settings_Advanced.EnablePreciseRectForDynamicBackgroundForGameObjectMainIcon = tempEnablePreciseRectForDynamicBackgroundForGameObjectMainIcon;
            HD_Settings_Advanced.EnableCustomizationForGameObjectComponentIcons = tempEnableCustomizationForGameObjectComponentIcons;
            HD_Settings_Advanced.EnableTooltipOnComponentIconHovered = tempEnableTooltipOnComponentIconHovered;
            HD_Settings_Advanced.EnableActiveStateEffectForComponentIcons = tempEnableActiveStateEffectForComponentIcons;
            HD_Settings_Advanced.DisableComponentIconsForInactiveGameObjects = tempDisableComponentIconsForInactiveGameObjects;
            HD_Settings_Advanced.EnableCustomInspectorGUI = tempEnableCustomInspectorUI;
            HD_Settings_Advanced.IncludeEditorUtilitiesForHierarchyDesignerRuntimeFolder = tempEnableEditorUtilities;
            HD_Settings_Advanced.IncludeBackgroundImageForGradientBackground = tempIncludeBackgroundImageForGradientBackground;
            HD_Settings_Advanced.ExcludeFoldersFromCountSelectToolCalculations = tempExcludeFoldersFromCountSelectToolCalculations;
            HD_Settings_Advanced.ExcludeSeparatorsFromCountSelectToolCalculations = tempExcludeSeparatorsFromCountSelectToolCalculations;
            HD_Settings_Advanced.SaveSettings();
            advancedSettingsHasModifiedChanges = false;

            if (hierarchyLocationChanged)
            {
                HD_Settings_Advanced.GenerateConstantsFile(tempHierarchyLocation);
            }
        }

        private void LoadAdvancedSettingsData()
        {
            tempHierarchyLocation = HD_Settings_Advanced.HierarchyLocation;
            tempMainIconUpdateMode = HD_Settings_Advanced.MainIconUpdateMode;
            tempComponentsIconsUpdateMode = HD_Settings_Advanced.ComponentsIconsUpdateMode;
            tempHierarchyTreeUpdateMode = HD_Settings_Advanced.HierarchyTreeUpdateMode;
            tempTagUpdateMode = HD_Settings_Advanced.TagUpdateMode;
            tempLayerUpdateMode = HD_Settings_Advanced.LayerUpdateMode;
            tempEnableDynamicBackgroundForGameObjectMainIcon = HD_Settings_Advanced.EnableDynamicBackgroundForGameObjectMainIcon;
            tempEnablePreciseRectForDynamicBackgroundForGameObjectMainIcon = HD_Settings_Advanced.EnablePreciseRectForDynamicBackgroundForGameObjectMainIcon;
            tempEnableCustomizationForGameObjectComponentIcons = HD_Settings_Advanced.EnableCustomizationForGameObjectComponentIcons;
            tempEnableTooltipOnComponentIconHovered = HD_Settings_Advanced.EnableTooltipOnComponentIconHovered;
            tempEnableActiveStateEffectForComponentIcons = HD_Settings_Advanced.EnableActiveStateEffectForComponentIcons;
            tempDisableComponentIconsForInactiveGameObjects = HD_Settings_Advanced.DisableComponentIconsForInactiveGameObjects;
            tempEnableCustomInspectorUI = HD_Settings_Advanced.EnableCustomInspectorGUI;
            tempEnableEditorUtilities = HD_Settings_Advanced.IncludeEditorUtilitiesForHierarchyDesignerRuntimeFolder;
            tempIncludeBackgroundImageForGradientBackground = HD_Settings_Advanced.IncludeBackgroundImageForGradientBackground;
            tempExcludeFoldersFromCountSelectToolCalculations = HD_Settings_Advanced.ExcludeFoldersFromCountSelectToolCalculations;
            tempExcludeSeparatorsFromCountSelectToolCalculations = HD_Settings_Advanced.ExcludeSeparatorsFromCountSelectToolCalculations;
        }

        private void EnableAllAdvancedSettingsFeatures(bool enable)
        {
            tempEnableDynamicBackgroundForGameObjectMainIcon = enable;
            tempEnablePreciseRectForDynamicBackgroundForGameObjectMainIcon = enable;
            tempEnableCustomizationForGameObjectComponentIcons = enable;
            tempEnableTooltipOnComponentIconHovered = enable;
            tempEnableActiveStateEffectForComponentIcons = enable;
            tempDisableComponentIconsForInactiveGameObjects = enable;
            tempEnableCustomInspectorUI = enable;
            tempEnableEditorUtilities = enable;
            tempIncludeBackgroundImageForGradientBackground = enable;
            tempExcludeFoldersFromCountSelectToolCalculations = enable;
            tempExcludeSeparatorsFromCountSelectToolCalculations = enable;
            advancedSettingsHasModifiedChanges = true;
        }
        #endregion
        #endregion

        private void OnDestroy()
        {
            string message = "The following settings have been modified: ";
            List<string> modifiedSettingsList = new();

            if (folderHasModifiedChanges) modifiedSettingsList.Add("Folders");
            if (separatorHasModifiedChanges) modifiedSettingsList.Add("Separators");
            if (generalSettingsHasModifiedChanges) modifiedSettingsList.Add("General Settings");
            if (designSettingsHasModifiedChanges) modifiedSettingsList.Add("Design Settings");
            if (shortcutSettingsHasModifiedChanges) modifiedSettingsList.Add("Shortcut Settings");
            if (advancedSettingsHasModifiedChanges) modifiedSettingsList.Add("Advanced Settings");

            if (modifiedSettingsList.Count > 0)
            {
                message += string.Join(", ", modifiedSettingsList) + ".\n\nWould you like to save the changes?";
                bool shouldSave = EditorUtility.DisplayDialog("Data Has Been Modified!", message, "Save", "Don't Save");

                if (shouldSave)
                {
                    if (folderHasModifiedChanges) UpdateAndSaveFoldersData();
                    if (separatorHasModifiedChanges) UpdateAndSaveSeparatorsData();
                    if (generalSettingsHasModifiedChanges) UpdateAndSaveGeneralSettingsData();
                    if (designSettingsHasModifiedChanges) UpdateAndSaveDesignSettingsData();
                    if (shortcutSettingsHasModifiedChanges) UpdateAndSaveShortcutSettingsData();
                    if (advancedSettingsHasModifiedChanges) UpdateAndSaveAdvancedSettingsData();
                }
            }

            folderHasModifiedChanges = false;
            separatorHasModifiedChanges = false;
            generalSettingsHasModifiedChanges = false;
            designSettingsHasModifiedChanges = false;
            shortcutSettingsHasModifiedChanges = false;
            advancedSettingsHasModifiedChanges = false;

            HD_Manager_Session.instance.currentWindow = currentWindow;
        }
    }
}
#endif