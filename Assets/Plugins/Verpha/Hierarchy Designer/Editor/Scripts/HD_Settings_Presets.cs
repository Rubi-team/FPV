#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Verpha.HierarchyDesigner
{
    internal class HD_Settings_Presets
    {
        #region Properties
        [System.Serializable]
        public class HD_Preset
        {
            public string presetName;
            public Color folderTextColor;
            public int folderFontSize;
            public FontStyle folderFontStyle;
            public Color folderColor;
            public HD_Settings_Folders.FolderImageType folderImageType;
            public Color separatorTextColor;
            public bool separatorIsGradientBackground;
            public Color separatorBackgroundColor;
            public Gradient separatorBackgroundGradient;
            public FontStyle separatorFontStyle;
            public int separatorFontSize;
            public TextAnchor separatorTextAlignment;
            public HD_Settings_Separators.SeparatorImageType separatorBackgroundImageType;
            public Color tagTextColor;
            public FontStyle tagFontStyle;
            public int tagFontSize;
            public TextAnchor tagTextAnchor;
            public Color layerTextColor;
            public FontStyle layerFontStyle;
            public int layerFontSize;
            public TextAnchor layerTextAnchor;
            public Color treeColor;
            public Color hierarchyLineColor;
            public Color lockColor;
            public int lockFontSize;
            public FontStyle lockFontStyle;
            public TextAnchor lockTextAnchor;

            public HD_Preset(
                string name,
                Color folderTextColor, 
                int folderFontSize, 
                FontStyle folderFontStyle,
                Color folderColor,
                HD_Settings_Folders.FolderImageType folderImageType,
                Color separatorTextColor,
                bool separatorIsGradientBackground,
                Color separatorBackgroundColor,
                Gradient separatorBackgroundGradient,
                FontStyle separatorFontStyle,
                int separatorFontSize,
                TextAnchor separatorTextAlignment,
                HD_Settings_Separators.SeparatorImageType separatorBackgroundImageType,
                Color tagTextColor,
                FontStyle tagFontStyle,
                int tagFontSize,
                TextAnchor tagTextAnchor,
                Color layerTextColor,
                FontStyle layerFontStyle,
                int layerFontSize,
                TextAnchor layerTextAnchor,
                Color treeColor,
                Color hierarchyLineColor,
                Color lockColor,
                int lockFontSize,
                FontStyle lockFontStyle,
                TextAnchor lockTextAnchor)
            {
                this.presetName = name;
                this.folderTextColor = folderTextColor;
                this.folderFontSize = folderFontSize;
                this.folderFontStyle = folderFontStyle;
                this.folderColor = folderColor;
                this.folderImageType = folderImageType;
                this.separatorTextColor = separatorTextColor;
                this.separatorIsGradientBackground = separatorIsGradientBackground;
                this.separatorBackgroundColor = separatorBackgroundColor;
                this.separatorBackgroundGradient = separatorBackgroundGradient;
                this.separatorFontStyle = separatorFontStyle;
                this.separatorFontSize = separatorFontSize;
                this.separatorTextAlignment = separatorTextAlignment;
                this.separatorBackgroundImageType = separatorBackgroundImageType;
                this.tagTextColor = tagTextColor;
                this.tagFontStyle = tagFontStyle;
                this.tagFontSize = tagFontSize;
                this.tagTextAnchor = tagTextAnchor;
                this.layerTextColor = layerTextColor;
                this.layerFontStyle = layerFontStyle;
                this.layerFontSize = layerFontSize;
                this.layerTextAnchor = layerTextAnchor;
                this.treeColor = treeColor;
                this.hierarchyLineColor = hierarchyLineColor;
                this.lockColor = lockColor;
                this.lockFontSize = lockFontSize;
                this.lockFontStyle = lockFontStyle;
                this.lockTextAnchor = lockTextAnchor;
            }
        }

        private static List<HD_Preset> defaultPresets;
        private static List<HD_Preset> customPresets;

        [System.Serializable]
        private class PresetListWrapper
        {
            public List<HD_Preset> presets;
        }
        #endregion

        #region Initialization
        public static void Initialize()
        {
            LoadDefaultPresets();
            LoadCustomPresets();
        }
        #endregion

        #region Accessors
        public static List<HD_Preset> DefaultPresets
        {
            get
            {
                if (defaultPresets == null) LoadDefaultPresets();
                return defaultPresets;
            }
        }

        public static List<HD_Preset> CustomPresets
        {
            get
            {
                if (customPresets == null) LoadCustomPresets();
                return customPresets;
            }
        }
        #endregion

        #region Default Presets
        private static HD_Preset AgeOfEnlightenmentPreset()
        {
            return new(
                "Age of Enlightenment",
                HD_Common_Color.HexToColor("#FFF9F4"),
                11,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#E2DAC1"),
                HD_Settings_Folders.FolderImageType.Default,
                HD_Common_Color.HexToColor("#464646"),
                false,
                HD_Common_Color.HexToColor("#FFF9F4"),
                new Gradient(),
                FontStyle.Normal,
                11,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.ClassicI,
                HD_Common_Color.HexToColor("#6C6C6C"),
                FontStyle.Italic,
                10,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#FAF1EA"),
                FontStyle.Italic,
                10,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#6C6C6C"),
                HD_Common_Color.HexToColor("#FAF1EA80"),
                HD_Common_Color.HexToColor("#FAF1EA"),
                10,
                FontStyle.Normal,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset AzureDreamscapePreset()
        {
            return new(
                "Azure Dreamscape",
                HD_Common_Color.HexToColor("#8E9FD5"),
                11,
                FontStyle.BoldAndItalic,
                HD_Common_Color.HexToColor("#318DCB"),
                HD_Settings_Folders.FolderImageType.ModernOutline,
                HD_Common_Color.HexToColor("#7EBCEF"),
                false,
                HD_Common_Color.HexToColor("#3C5A81"),
                new Gradient(),
                FontStyle.BoldAndItalic,
                13,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedSideways,
                HD_Common_Color.HexToColor("#8E9FD5"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#8E9FD5"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#5A5485"),
                HD_Common_Color.HexToColor("#8E9FD580"),
                HD_Common_Color.HexToColor("#8E9FD5"),
                11,
                FontStyle.BoldAndItalic,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset BlackAndGoldPreset()
        {
            return new(
                "Black and Gold",
                HD_Common_Color.HexToColor("#FFD102"),
                12,
                FontStyle.Bold,
                HD_Common_Color.HexToColor("#1C1C1C"),
                HD_Settings_Folders.FolderImageType.ModernI,
                HD_Common_Color.HexToColor("#FFD102"),
                false,
                HD_Common_Color.HexToColor("#1C1C1C"),
                new Gradient(),
                FontStyle.BoldAndItalic,
                13,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.ModernI,
                HD_Common_Color.HexToColor("#1C1C1C"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#1C1C1C"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#FFC402"),
                HD_Common_Color.HexToColor("#00000080"),
                HD_Common_Color.HexToColor("#FFC402"),
                11,
                FontStyle.BoldAndItalic,
                TextAnchor.MiddleRight
            );
        }

        private static HD_Preset BlackAndWhitePreset()
        {
            return new(
                "Black and White",
                HD_Common_Color.HexToColor("#FFFFFF"),
                12,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#000000"),
                HD_Settings_Folders.FolderImageType.Default,
                HD_Common_Color.HexToColor("#ffffff"),
                false,
                HD_Common_Color.HexToColor("#000000"),
                new Gradient(),
                FontStyle.Bold,
                12,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.Default,
                HD_Common_Color.HexToColor("#ffffff80"),
                FontStyle.Italic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#ffffff80"),
                FontStyle.Italic,
                9,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#FFFFFF"),
                HD_Common_Color.HexToColor("#FFFFFF80"),
                HD_Common_Color.HexToColor("#FFFFFF"),
                11,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset BloodyMaryPreset()
        {
            return new(
                "Bloody Mary",
                HD_Common_Color.HexToColor("#FFEEAAF0"),
                11,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#C50515E6"),
                HD_Settings_Folders.FolderImageType.ModernIII,
                HD_Common_Color.HexToColor("#FFFFFFE1"),
                false,
                HD_Common_Color.HexToColor("#CF1625F0"),
                new Gradient(),
                FontStyle.Bold,
                12,
                TextAnchor.UpperCenter,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedBottom,
                HD_Common_Color.HexToColor("#FFEEAA9C"),
                FontStyle.Italic,
                8,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#FFEEAA9C"),
                FontStyle.Italic,
                8,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#FFFFFFC8"),
                HD_Common_Color.HexToColor("#00000080"),
                HD_Common_Color.HexToColor("#FFFFFFC8"),
                11,
                FontStyle.Normal,
                TextAnchor.UpperCenter
            );
        }

        private static HD_Preset BlueHarmonyPreset()
        {
            return new(
                "Blue Harmony",
                HD_Common_Color.HexToColor("#A5D2FF"),
                11,
                FontStyle.Bold,
                HD_Common_Color.HexToColor("#6AB1F8"),
                HD_Settings_Folders.FolderImageType.ModernII,
                HD_Common_Color.HexToColor("#A5D2FF"),
                false,
                HD_Common_Color.HexToColor("#277DEC"),
                new Gradient(),
                FontStyle.Bold,
                12,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.ModernII,
                HD_Common_Color.HexToColor("#6AB1F8F0"),
                FontStyle.Bold,
                8,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#A5D2FF"),
                FontStyle.Bold,
                9,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#A5D2FF"),
                HD_Common_Color.HexToColor("#A5D2FF80"),
                HD_Common_Color.HexToColor("#A5D2FF"),
                11,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset DeepOceanPreset()
        {
            return new(
                "Deep Ocean",
                HD_Common_Color.HexToColor("#1E4E8A"),
                12,
                FontStyle.BoldAndItalic,
                HD_Common_Color.HexToColor("#1E4E8A"),
                HD_Settings_Folders.FolderImageType.ModernIII,
                HD_Common_Color.HexToColor("#041F54C8"),
                false,
                HD_Common_Color.HexToColor("#041F54"),
                new Gradient(),
                FontStyle.Bold,
                12,
                TextAnchor.LowerRight,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedRight,
                HD_Common_Color.HexToColor("#213864"),
                FontStyle.Bold,
                8,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#213864"),
                FontStyle.Bold,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#213864"),
                HD_Common_Color.HexToColor("#21386480"),
                HD_Common_Color.HexToColor("#213864"),
                10,
                FontStyle.BoldAndItalic,
                TextAnchor.MiddleRight
            );
        }

        private static HD_Preset DunesPreset()
        {
            return new(
                "Dunes",
                HD_Common_Color.HexToColor("#E7D7C7"),
                12,
                FontStyle.Italic,
                HD_Common_Color.HexToColor("#D6AC84"),
                HD_Settings_Folders.FolderImageType.NeoI,
                HD_Common_Color.HexToColor("#E4C6AB"),
                false,
                HD_Common_Color.HexToColor("#AB673F"),
                new Gradient(),
                FontStyle.Italic,
                13,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedRight,
                HD_Common_Color.HexToColor("#DDC0A4E1"),
                FontStyle.Italic,
                8,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#DDC0A4E1"),
                FontStyle.Italic,
                8,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#DDC0A4E1"),
                HD_Common_Color.HexToColor("#DDC0A480"),
                HD_Common_Color.HexToColor("#DDC0A4E1"),
                11,
                FontStyle.Italic,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset FreshSwampPreset()
        {
            return new(
                "Fresh Swamp",
                HD_Common_Color.HexToColor("#E7DECD"),
                12,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#CAC599"),
                HD_Settings_Folders.FolderImageType.ModernIII,
                HD_Common_Color.HexToColor("#FBFAF8"),
                true,
                HD_Common_Color.HexToColor("#698F3F"),
#if UNITY_2022_3_OR_NEWER
                HD_Common_Color.CreateGradient(GradientMode.PerceptualBlend, ("#698F3F", 255, 0f), ("#804E49", 255, 100f)),
#else
                HD_Common_Color.CreateGradient(GradientMode.Blend, ("#698F3F", 255, 0f), ("#804E49", 255, 100f)),
#endif
                FontStyle.Normal,
                13,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.ModernII,
                HD_Common_Color.HexToColor("#698F3F"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#E7DECD"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#698F3F"),
                HD_Common_Color.HexToColor("#698F3F80"),
                HD_Common_Color.HexToColor("#FBFAF8"),
                11,
                FontStyle.BoldAndItalic,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset FrostyFogPreset()
        {
            return new(
                "Frosty Fog",
                HD_Common_Color.HexToColor("#DBEAEE"),
                12,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#C4E7F3DC"),
                HD_Settings_Folders.FolderImageType.Default,
                HD_Common_Color.HexToColor("#E2F0F5"),
                true,
                HD_Common_Color.HexToColor("#C7E6F1"),
                HD_Common_Color.CreateGradient(GradientMode.Blend, ("#A9DDEF", 255, 20f), ("#BDE7F5", 200, 50f), ("#DCF6FF", 120, 90f), ("DBEFF5", 100, 100f)),
                FontStyle.Italic,
                13,
                TextAnchor.MiddleRight,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedRight,
                HD_Common_Color.HexToColor("#ACDEEF"),
                FontStyle.BoldAndItalic,
                10,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#9FA8AB"),
                FontStyle.Italic,
                11,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#CADCE2"),
                HD_Common_Color.HexToColor("#C4E5F180"),
                HD_Common_Color.HexToColor("#C4E5F1"),
                11,
                FontStyle.BoldAndItalic,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset HoHoHoPreset()
        {
            return new(
                "Ho Ho Ho",
                HD_Common_Color.HexToColor("#FFFFFF"),
                11,
                FontStyle.Bold,
                HD_Common_Color.HexToColor("#E02D3C"),
                HD_Settings_Folders.FolderImageType.ModernIII,
                HD_Common_Color.HexToColor("#FFFFFF"),
                false,
                HD_Common_Color.HexToColor("#E02D3CF0"),
                new Gradient(),
                FontStyle.Bold,
                12,
                TextAnchor.UpperCenter,
                HD_Settings_Separators.SeparatorImageType.ModernII,
                HD_Common_Color.HexToColor("#FFFFFF"),
                FontStyle.Bold,
                10,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#FF5564"),
                FontStyle.Bold,
                10,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#FFFFFF"),
                HD_Common_Color.HexToColor("#00000080"),
                HD_Common_Color.HexToColor("#E7E7E7"),
                11,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset IronCinderPreset()
        {
            return new(
                "Iron Cinder",
                HD_Common_Color.HexToColor("#C8C8C8"),
                11,
                FontStyle.BoldAndItalic,
                HD_Common_Color.HexToColor("#969696"),
                HD_Settings_Folders.FolderImageType.ModernIII,
                HD_Common_Color.HexToColor("#C8C8C8"),
                true,
                HD_Common_Color.HexToColor("#646464"),
#if UNITY_2022_3_OR_NEWER
                HD_Common_Color.CreateGradient(GradientMode.PerceptualBlend, ("#191919", 255, 0f), ("#323232", 250, 25f), ("#646464", 250, 50f), ("323232", 250, 75f), ("191919", 250, 100f)),
#else
                HD_Common_Color.CreateGradient(GradientMode.Blend, ("#191919", 255, 0f), ("#323232", 250, 25f), ("#646464", 250, 50f), ("323232", 250, 75f), ("191919", 250, 100f)),
#endif
                FontStyle.BoldAndItalic,
                12,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.ModernIII,
                HD_Common_Color.HexToColor("#C8C8C8"),
                FontStyle.Italic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#969696"),
                FontStyle.Italic,
                9,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#646464"),
                HD_Common_Color.HexToColor("#19191980"),
                HD_Common_Color.HexToColor("#C8C8C8"),
                11,
                FontStyle.BoldAndItalic,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset JadeLakePreset()
        {
            return new(
                "Jade Lake",
                HD_Common_Color.HexToColor("#DBD3D8"),
                11,
                FontStyle.Bold,
                HD_Common_Color.HexToColor("#6E8E83"),
                HD_Settings_Folders.FolderImageType.ModernIII,
                HD_Common_Color.HexToColor("#DBD3D8"),
                false,
                HD_Common_Color.HexToColor("#2E5E4E"),
                new Gradient(),
                FontStyle.Bold,
                12,
                TextAnchor.UpperCenter,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedBottom,
                HD_Common_Color.HexToColor("#93A7AA"),
                FontStyle.Italic,
                8,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#DBD3D8"),
                FontStyle.Italic,
                8,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#93A7AA"),
                HD_Common_Color.HexToColor("#2E5E4E80"),
                HD_Common_Color.HexToColor("#A7B5B9"),
                11,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset LittleRedPreset()
        {
            return new(
                "Little Red",
                HD_Common_Color.HexToColor("#FFFFFF"),
                11,
                FontStyle.Bold,
                HD_Common_Color.HexToColor("#E02D3C"),
                HD_Settings_Folders.FolderImageType.Default,
                HD_Common_Color.HexToColor("#FFFFFF"),
                false,
                HD_Common_Color.HexToColor("#E02D3CF0"),
                new Gradient(),
                FontStyle.Bold,
                11,
                TextAnchor.MiddleLeft,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedRight,
                HD_Common_Color.HexToColor("#FFFFFF"),
                FontStyle.Bold,
                10,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#D62E3C"),
                FontStyle.Bold,
                10,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#FFFFFF"),
                HD_Common_Color.HexToColor("#291C1C80"),
                HD_Common_Color.HexToColor("#FFFFFF"),
                11,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset MinimalBlackPreset()
        {
            return new(
                "Minimal Black",
                HD_Common_Color.HexToColor("#000000"),
                11,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#000000"),
                HD_Settings_Folders.FolderImageType.DefaultOutline,
                HD_Common_Color.HexToColor("#646464"),
                false,
                HD_Common_Color.HexToColor("#000000"),
                new Gradient(),
                FontStyle.Bold,
                10,
                TextAnchor.MiddleLeft,
                HD_Settings_Separators.SeparatorImageType.Default,
                HD_Common_Color.HexToColor("#000000C8"),
                FontStyle.Italic,
                8,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#000000C8"),
                FontStyle.Italic,
                8,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#000000F0"),
                HD_Common_Color.HexToColor("#00000080"),
                HD_Common_Color.HexToColor("#000000F0"),
                10,
                FontStyle.Normal,
                TextAnchor.UpperCenter
            );
        }

        private static HD_Preset MinimalWhitePreset()
        {
            return new(
                "Minimal White",
                HD_Common_Color.HexToColor("#FFFFFF"),
                11,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#FFFFFF"),
                HD_Settings_Folders.FolderImageType.DefaultOutline,
                HD_Common_Color.HexToColor("#9B9B9B"),
                false,
                HD_Common_Color.HexToColor("#FFFFFF"),
                new Gradient(),
                FontStyle.Bold,
                10,
                TextAnchor.MiddleLeft,
                HD_Settings_Separators.SeparatorImageType.Default,
                HD_Common_Color.HexToColor("#FFFFFFC8"),
                FontStyle.Italic,
                8,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#FFFFFFC8"),
                FontStyle.Italic,
                8,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#FFFFFFF0"),
                HD_Common_Color.HexToColor("#FFFFFF80"),
                HD_Common_Color.HexToColor("#FFFFFFF0"),
                10,
                FontStyle.Normal,
                TextAnchor.UpperCenter
            );
        }

        private static HD_Preset NaturePreset()
        {
            return new(
                "Nature",
                HD_Common_Color.HexToColor("#AAD9A5"),
                12,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#DFEAF0"),
                HD_Settings_Folders.FolderImageType.Default,
                HD_Common_Color.HexToColor("#DFF6CA"),
                false,
                HD_Common_Color.HexToColor("#70B879"),
                new Gradient(),
                FontStyle.Normal,
                13,
                TextAnchor.MiddleLeft,
                HD_Settings_Separators.SeparatorImageType.ModernII,
                HD_Common_Color.HexToColor("#AAD9A5C8"),
                FontStyle.Normal,
                9,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#AAD9A5C8"),
                FontStyle.Normal,
                9,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#BCD8E3"),
                HD_Common_Color.HexToColor("#BFDFB180"),
                HD_Common_Color.HexToColor("#BFDFB1"),
                11,
                FontStyle.Italic,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset NavyBlueLightPreset()
        {
            return new(
                "Navy Blue Light",
                HD_Common_Color.HexToColor("#AAD6EC"),
                11,
                FontStyle.Bold,
                HD_Common_Color.HexToColor("#AAD6EC"),
                HD_Settings_Folders.FolderImageType.Default,
                HD_Common_Color.HexToColor("#AAD6EC"),
                false,
                HD_Common_Color.HexToColor("#113065"),
                new Gradient(),
                FontStyle.Bold,
                12,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.ModernII,
                HD_Common_Color.HexToColor("#AAD6ECC8"),
                FontStyle.Bold,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#AAD6ECC8"),
                FontStyle.Bold,
                9,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#AAD6EC"),
                HD_Common_Color.HexToColor("#AAD6EC80"),
                HD_Common_Color.HexToColor("#AAD6EC"),
                11,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset OldSchoolPreset()
        {
            return new(
                "Old School",
                HD_Common_Color.HexToColor("#1FC742"),
                11,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#686868"),
                HD_Settings_Folders.FolderImageType.Default,
                HD_Common_Color.HexToColor("#00FF34"),
                false,
                HD_Common_Color.HexToColor("#010101"),
                new Gradient(),
                FontStyle.Normal,
                12,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.Default,
                HD_Common_Color.HexToColor("#1FC742F0"),
                FontStyle.Normal,
                9,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#1FC742F0"),
                FontStyle.Normal,
                9,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#686868"),
                HD_Common_Color.HexToColor("#7D7D7D80"),
                HD_Common_Color.HexToColor("#7D7D7D"),
                11,
                FontStyle.Normal,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset PrettyPinkPreset()
        {
            return new(
                "Pretty Pink",
                HD_Common_Color.HexToColor("#FFFFFF"),
                11,
                FontStyle.Bold,
                HD_Common_Color.HexToColor("#FF4071"),
                HD_Settings_Folders.FolderImageType.ModernIII,
                HD_Common_Color.HexToColor("#EFEBE0"),
                false,
                HD_Common_Color.HexToColor("#FB4570"),
                new Gradient(),
                FontStyle.Bold,
                12,
                TextAnchor.MiddleLeft,
                HD_Settings_Separators.SeparatorImageType.ModernII,
                HD_Common_Color.HexToColor("#FB4570FA"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#FB4570FA"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#FB4570"),
                HD_Common_Color.HexToColor("#FB457080"),
                HD_Common_Color.HexToColor("#FB4570"),
                11,
                FontStyle.BoldAndItalic,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset PrismaticPreset()
        {
            return new(
                "Prismatic",
                HD_Common_Color.HexToColor("#E5CCE5"),
                11,
                FontStyle.BoldAndItalic,
                HD_Common_Color.HexToColor("#A2D5FF"),
                HD_Settings_Folders.FolderImageType.ModernIII,
                HD_Common_Color.HexToColor("#FFFFFF"),
                true,
                HD_Common_Color.HexToColor("#FFFFFF"),
#if UNITY_2022_3_OR_NEWER
                HD_Common_Color.CreateGradient(GradientMode.PerceptualBlend, ("#2F7FFF", 155, 0f), ("#72BFAF", 158, 35f), ("E8CEE8", 162, 70f), ("#FFFFFF", 165, 100f)),
#else
                HD_Common_Color.CreateGradient(GradientMode.Blend, ("#2F7FFF", 155, 0f), ("#72BFAF", 158, 35f), ("E8CEE8", 162, 70f), ("#FFFFFF", 165, 100f)),
#endif
                FontStyle.BoldAndItalic,
                12,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.NeoI,
                HD_Common_Color.HexToColor("#9FD3E0"),
                FontStyle.BoldAndItalic,
                10,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#E09FAD"),
                FontStyle.BoldAndItalic,
                10,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#FFFFFF"),
                HD_Common_Color.HexToColor("#E09FAD80"),
                HD_Common_Color.HexToColor("#FFFFFF"),
                11,
                FontStyle.BoldAndItalic,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset RedDawnPreset()
        {
            return new(
                "Red Dawn",
                HD_Common_Color.HexToColor("#FE5E2A"),
                11,
                FontStyle.BoldAndItalic,
                HD_Common_Color.HexToColor("#DF4148"),
                HD_Settings_Folders.FolderImageType.ModernI,
                HD_Common_Color.HexToColor("#FF5F2A"),
                false,
                HD_Common_Color.HexToColor("#C00531"),
                new Gradient(),
                FontStyle.BoldAndItalic,
                13,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedSideways,
                HD_Common_Color.HexToColor("#DF4148F0"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#DF4148F0"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#DF4148"),
                HD_Common_Color.HexToColor("#DF4148B4"),
                HD_Common_Color.HexToColor("#DF4148"),
                11,
                FontStyle.Italic,
                TextAnchor.MiddleRight
            );
        }

        private static HD_Preset SnowPreset()
        {
            return new(
                "Snow",
                HD_Common_Color.HexToColor("#FFFFFF"),
                12,
                FontStyle.BoldAndItalic,
                HD_Common_Color.HexToColor("#E9FCFE"),
                HD_Settings_Folders.FolderImageType.Default,
                HD_Common_Color.HexToColor("#9CD4DB"),
                true,
                HD_Common_Color.HexToColor("#FFFFFF"),
                HD_Common_Color.CreateGradient(GradientMode.Blend, ("#FFFFFF", 240, 0f), ("#E1FDFF", 255, 50f), ("#FFFFFF", 240, 100f)),
                FontStyle.BoldAndItalic,
                12,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.NeoI,
                HD_Common_Color.HexToColor("#FFFFFF"),
                FontStyle.BoldAndItalic,
                10,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#E6FDFE"),
                FontStyle.BoldAndItalic,
                10,
                TextAnchor.MiddleCenter,
                HD_Common_Color.HexToColor("#FFFFFF"),
                HD_Common_Color.HexToColor("#E6FDFE80"),
                HD_Common_Color.HexToColor("#FFFFFF"),
                11,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset StrawberrySalmonPreset()
        {
            return new HD_Preset(
                "Strawberry Salmon",
                HD_Common_Color.HexToColor("#FFC6C6"),
                11,
                FontStyle.Bold,
                HD_Common_Color.HexToColor("#FF5574"),
                HD_Settings_Folders.FolderImageType.ModernI,
                HD_Common_Color.HexToColor("#FAD8D8"),
                false,
                HD_Common_Color.HexToColor("#F87474"),
                new Gradient(),
                FontStyle.Bold,
                11,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.NeoI,
                HD_Common_Color.HexToColor("#D85E74"),
                FontStyle.Italic,
                10,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#FAD8D8"),
                FontStyle.Italic,
                10,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#FAD8D8"),
                HD_Common_Color.HexToColor("#FAD8D880"),
                HD_Common_Color.HexToColor("#FAD8D8"),
                11,
                FontStyle.Italic,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset SunflowerPreset()
        {
            return new(
                "Sunflower",
                HD_Common_Color.HexToColor("#F8B701"),
                12,
                FontStyle.Bold,
                HD_Common_Color.HexToColor("#298AEC"),
                HD_Settings_Folders.FolderImageType.ModernI,
                HD_Common_Color.HexToColor("#FFC80A"),
                false,
                HD_Common_Color.HexToColor("#2A8FF3"),
                new Gradient(),
                FontStyle.Bold,
                13,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.ModernI,
                HD_Common_Color.HexToColor("#F8B701"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#F8B701"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#F8B701"),
                HD_Common_Color.HexToColor("#F8B70180"),
                HD_Common_Color.HexToColor("#F8B701"),
                10,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset TheTwoRealmsPreset()
        {
            return new(
                "The Two Realms",
                HD_Common_Color.HexToColor("#01BAEF"),
                12,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#0CBABA"),
                HD_Settings_Folders.FolderImageType.ModernI,
                HD_Common_Color.HexToColor("#150811"),
                true,
                HD_Common_Color.HexToColor("#26081C"),
                HD_Common_Color.CreateGradient(GradientMode.Blend, ("#150811", 255, 0f), ("#26081C", 255, 20f), ("#380036", 150, 50f), ("0CBABA", 200, 75f), ("01BAEF", 255, 100f)),
                FontStyle.Bold,
                12,
                TextAnchor.UpperCenter,
                HD_Settings_Separators.SeparatorImageType.NeoI,
                HD_Common_Color.HexToColor("#380036"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#01BAEF"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#150811"),
                HD_Common_Color.HexToColor("#15081180"),
                HD_Common_Color.HexToColor("#01BAEF"),
                11,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
        }

        private static HD_Preset WildcatsPreset()
        {
            return new(
                "Wildcats",
                HD_Common_Color.HexToColor("#FFFFFF"),
                11,
                FontStyle.Bold,
                HD_Common_Color.HexToColor("#FFCF28"),
                HD_Settings_Folders.FolderImageType.Default,
                HD_Common_Color.HexToColor("#FFCF28"),
                false,
                HD_Common_Color.HexToColor("#1D5098"),
                new Gradient(),
                FontStyle.Bold,
                13,
                TextAnchor.MiddleCenter,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedSideways,
                HD_Common_Color.HexToColor("#FFFFFF"),
                FontStyle.Bold,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#FFCF28"),
                FontStyle.BoldAndItalic,
                10,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#FFFFFF"),
                HD_Common_Color.HexToColor("#1D509880"),
                HD_Common_Color.HexToColor("#F8B701"),
                11,
                FontStyle.BoldAndItalic,
                TextAnchor.UpperCenter
            );
        }

        private static HD_Preset YoungMonarchPreset()
        {
            return new(
                "Young Monarch",
                HD_Common_Color.HexToColor("#EAEAEA"),
                12,
                FontStyle.Normal,
                HD_Common_Color.HexToColor("#4F6D7A"),
                HD_Settings_Folders.FolderImageType.Default,
                HD_Common_Color.HexToColor("#EAEAEA"),
                false,
                HD_Common_Color.HexToColor("#DD6E42"),
                new Gradient(),
                FontStyle.Bold,
                12,
                TextAnchor.UpperCenter,
                HD_Settings_Separators.SeparatorImageType.NeoII,
                HD_Common_Color.HexToColor("#E8DAB2"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleRight,
                HD_Common_Color.HexToColor("#C0D6DF"),
                FontStyle.BoldAndItalic,
                9,
                TextAnchor.MiddleLeft,
                HD_Common_Color.HexToColor("#4F6D7A"),
                HD_Common_Color.HexToColor("#EAEAEA80"),
                HD_Common_Color.HexToColor("#E8DAB2"),
                11,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );
        }
        #endregion

        #region Save and Load
        public static void SaveCustomPresets()
        {
            string filePath = HD_Common_File.GetSavedDataFilePath(HD_Common_Constants.CustomPresetsTextFileName);
            PresetListWrapper wrapper = new() { presets = customPresets };
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(filePath, json);
            AssetDatabase.Refresh();
        }

        private static void LoadDefaultPresets()
        {
            defaultPresets = new()
            {
                AgeOfEnlightenmentPreset(),
                AzureDreamscapePreset(),
                BlackAndGoldPreset(),
                BlackAndWhitePreset(),
                BloodyMaryPreset(),
                BlueHarmonyPreset(),
                DeepOceanPreset(),
                DunesPreset(),
                FreshSwampPreset(),
                FrostyFogPreset(),
                HoHoHoPreset(),
                IronCinderPreset(),
                JadeLakePreset(),
                LittleRedPreset(),
                MinimalBlackPreset(),
                MinimalWhitePreset(),
                NaturePreset(),
                NavyBlueLightPreset(),
                OldSchoolPreset(),
                PrettyPinkPreset(),
                PrismaticPreset(),
                RedDawnPreset(),
                SnowPreset(),
                StrawberrySalmonPreset(),
                SunflowerPreset(),
                TheTwoRealmsPreset(),
                WildcatsPreset(),
                YoungMonarchPreset(),
            };
        }

        private static void LoadCustomPresets()
        {
            string filePath = HD_Common_File.GetSavedDataFilePath(HD_Common_Constants.CustomPresetsTextFileName);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                customPresets = JsonUtility.FromJson<PresetListWrapper>(json).presets;
            }
            else
            {
                SetCustomPresetsSettings();
            }
        }

        private static void SetCustomPresetsSettings()
        {
            customPresets = new();
        }
        #endregion

        #region Operations
        public static string[] GetPresetNames()
        {
            List<HD_Preset> presetList = DefaultPresets;
            string[] presetNames = new string[presetList.Count];
            for (int i = 0; i < presetList.Count; i++)
            {
                presetNames[i] = presetList[i].presetName;
            }
            return presetNames;
        }

        public static Dictionary<string, List<string>> GetPresetNamesGrouped()
        {
            List<HD_Preset> defaultPresetList = DefaultPresets;
            List<HD_Preset> customPresetList = CustomPresets;

            Dictionary<string, List<string>> groupedPresets = new()
            {
                { "A-E", new() },
                { "F-J", new() },
                { "K-O", new() },
                { "P-T", new() },
                { "U-Z", new() },
                { "Custom", new() }
            };

            foreach (HD_Preset preset in defaultPresetList)
            {
                char firstChar = preset.presetName.ToUpper()[0];
                if (firstChar >= 'A' && firstChar <= 'E') { groupedPresets["A-E"].Add(preset.presetName); }
                else if (firstChar >= 'F' && firstChar <= 'J') { groupedPresets["F-J"].Add(preset.presetName); }
                else if (firstChar >= 'K' && firstChar <= 'O') { groupedPresets["K-O"].Add(preset.presetName); }
                else if (firstChar >= 'P' && firstChar <= 'T') { groupedPresets["P-T"].Add(preset.presetName); }
                else if (firstChar >= 'U' && firstChar <= 'Z') { groupedPresets["U-Z"].Add(preset.presetName); }
            }

            List<string> customPresetNames = new();
            foreach (HD_Preset customPreset in customPresetList)
            {
                customPresetNames.Add(customPreset.presetName);
            }
            customPresetNames.Sort();
            groupedPresets["Custom"].AddRange(customPresetNames);

            return groupedPresets;
        }
        #endregion
    }
}
#endif