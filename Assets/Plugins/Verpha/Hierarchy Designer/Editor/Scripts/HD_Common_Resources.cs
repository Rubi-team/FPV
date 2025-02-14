#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    internal static class HD_Common_Resources
    {
        private static class ResourceNames
        {
            #region Fonts
            internal const string FontBold = "Hierarchy Designer Font Bold";
            internal const string FontRegular = "Hierarchy Designer Font Regular";
            #endregion

            #region Icons
            internal const string IconResetDark = "Hierarchy Designer Icon Reset Dark";
            internal const string IconResetLight = "Hierarchy Designer Icon Reset Light";
            internal const string IconTooltipDark = "Hierarchy Designer Icon Tooltip Dark";
            internal const string IconTooltipLight = "Hierarchy Designer Icon Tooltip Light";
            internal const string IconLockDark = "Hierarchy Designer Icon Lock Dark";
            internal const string IconLockLight = "Hierarchy Designer Icon Lock Light";
            internal const string IconUnlockDark = "Hierarchy Designer Icon Unlock Dark";
            internal const string IconUnlockLight = "Hierarchy Designer Icon Unlock Light";
            internal const string IconVisibilityOnDark = "Hierarchy Designer Icon Visibility On Dark";
            internal const string IconVisibilityOnLight = "Hierarchy Designer Icon Visibility On Light";
            internal const string IconVisibilityOffDark = "Hierarchy Designer Icon Visibility Off Dark";
            internal const string IconVisibilityOffLight = "Hierarchy Designer Icon Visibility Off Light";
            #endregion

            #region Graphics
            internal const string GraphicsTitleDark = "Hierarchy Designer Graphics Title Dark";
            internal const string GraphicsTitleLight = "Hierarchy Designer Graphics Title Light";
            #endregion

            #region Promotional
            internal const string PromotionalPicEase = "Hierarchy Designer Promotional PicEase";
            #endregion
        }

        #region Classes
        internal static class Fonts
        {
            private static readonly Lazy<Font> _bold = new(() => HD_Common_Texture.LoadFont(ResourceNames.FontBold));
            public static Font Bold => _bold.Value;

            private static readonly Lazy<Font> _regular = new(() => HD_Common_Texture.LoadFont(ResourceNames.FontRegular));
            public static Font Regular => _regular.Value;
        }

        internal static class Icons
        {
            private static readonly Lazy<Texture2D> _resetIcon = new(() => HD_Manager_Editor.IsProSkin ? HD_Common_Texture.LoadTexture(ResourceNames.IconResetDark) : HD_Common_Texture.LoadTexture(ResourceNames.IconResetLight));
            public static Texture2D Reset => _resetIcon.Value;

            private static readonly Lazy<Texture2D> _tooltipIcon = new(() => HD_Manager_Editor.IsProSkin ? HD_Common_Texture.LoadTexture(ResourceNames.IconTooltipDark) : HD_Common_Texture.LoadTexture(ResourceNames.IconTooltipLight));
            public static Texture2D Tooltip => _tooltipIcon.Value;

            private static readonly Lazy<Texture2D> _lockIcon = new(() => HD_Manager_Editor.IsProSkin ? HD_Common_Texture.LoadTexture(ResourceNames.IconLockDark) : HD_Common_Texture.LoadTexture(ResourceNames.IconLockLight));
            public static Texture2D Lock => _lockIcon.Value;

            private static readonly Lazy<Texture2D> _unlockIcon = new(() => HD_Manager_Editor.IsProSkin ? HD_Common_Texture.LoadTexture(ResourceNames.IconUnlockDark) : HD_Common_Texture.LoadTexture(ResourceNames.IconUnlockLight));
            public static Texture2D Unlock => _unlockIcon.Value;

            private static readonly Lazy<Texture2D> _visibilityOnIcon = new(() => HD_Manager_Editor.IsProSkin ? HD_Common_Texture.LoadTexture(ResourceNames.IconVisibilityOnDark) : HD_Common_Texture.LoadTexture(ResourceNames.IconVisibilityOnLight));
            public static Texture2D VisibilityOn => _visibilityOnIcon.Value;

            private static readonly Lazy<Texture2D> _visibilityOffIcon = new(() => HD_Manager_Editor.IsProSkin ? HD_Common_Texture.LoadTexture(ResourceNames.IconVisibilityOffDark) : HD_Common_Texture.LoadTexture(ResourceNames.IconVisibilityOffLight));
            public static Texture2D VisibilityOff => _visibilityOffIcon.Value;
        }

        internal static class Graphics
        {
            private static readonly Lazy<Texture2D> _titleDark = new(() => HD_Common_Texture.LoadTexture(ResourceNames.GraphicsTitleDark));
            public static Texture2D TitleDark => _titleDark.Value;

            private static readonly Lazy<Texture2D> _titleLight = new(() => HD_Common_Texture.LoadTexture(ResourceNames.GraphicsTitleLight));
            public static Texture2D TitleLight => _titleLight.Value;
        }

        internal static class Promotional
        {
            private static readonly Lazy<Texture2D> _picEasePromotionalIcon = new(() => HD_Common_Texture.LoadTexture(ResourceNames.PromotionalPicEase));
            public static Texture2D PicEasePromotionalIcon => _picEasePromotionalIcon.Value;
        }
        #endregion

        #region Old
        #region General
        private static readonly string defaultFontBoldName = "Hierarchy Designer Font Bold";
        private static readonly string defaultFontRegularName = "Hierarchy Designer Font Regular";
        private static readonly string defaultTextureName = "Hierarchy Designer Default Texture";

        public static readonly Font DefaultFontBold = HD_Common_Texture.LoadFont(defaultFontBoldName);
        public static readonly Font DefaultFont = HD_Common_Texture.LoadFont(defaultFontRegularName);
        public static readonly Texture2D DefaultTexture = HD_Common_Texture.LoadTexture(defaultTextureName);
        #endregion

        #region Tree Branches
        private static readonly string treeBranchIconDefaultIName = "Hierarchy Designer Tree Branch Icon Default I";
        private static readonly string treeBranchIconDefaultLName = "Hierarchy Designer Tree Branch Icon Default L";
        private static readonly string treeBranchIconDefaultTName = "Hierarchy Designer Tree Branch Icon Default T";
        private static readonly string treeBranchIconDefaultTerminalBudName = "Hierarchy Designer Tree Branch Icon Default Terminal Bud";
        private static readonly string treeBranchIconCurvedIName = "Hierarchy Designer Tree Branch Icon Curved I";
        private static readonly string treeBranchIconCurvedLName = "Hierarchy Designer Tree Branch Icon Curved L";
        private static readonly string treeBranchIconCurvedTName = "Hierarchy Designer Tree Branch Icon Curved T";
        private static readonly string treeBranchIconCurvedTerminalBudName = "Hierarchy Designer Tree Branch Icon Curved Terminal Bud";
        private static readonly string treeBranchIconDottedIName = "Hierarchy Designer Tree Branch Icon Dotted I";
        private static readonly string treeBranchIconDottedLName = "Hierarchy Designer Tree Branch Icon Dotted L";
        private static readonly string treeBranchIconDottedTName = "Hierarchy Designer Tree Branch Icon Dotted T";
        private static readonly string treeBranchIconDottedTerminalBudName = "Hierarchy Designer Tree Branch Icon Dotted Terminal Bud";
        private static readonly string treeBranchIconSegmentedIName = "Hierarchy Designer Tree Branch Icon Segmented I";
        private static readonly string treeBranchIconSegmentedLName = "Hierarchy Designer Tree Branch Icon Segmented L";
        private static readonly string treeBranchIconSegmentedTName = "Hierarchy Designer Tree Branch Icon Segmented T";
        private static readonly string treeBranchIconSegmentedTerminalBudName = "Hierarchy Designer Tree Branch Icon Segmented Terminal Bud";

        public static readonly Texture2D TreeBranchIconDefault_I = HD_Common_Texture.LoadTexture(treeBranchIconDefaultIName);
        public static readonly Texture2D TreeBranchIconDefault_L = HD_Common_Texture.LoadTexture(treeBranchIconDefaultLName);
        public static readonly Texture2D TreeBranchIconDefault_T = HD_Common_Texture.LoadTexture(treeBranchIconDefaultTName);
        public static readonly Texture2D TreeBranchIconDefault_TerminalBud = HD_Common_Texture.LoadTexture(treeBranchIconDefaultTerminalBudName);
        public static readonly Texture2D TreeBranchIconCurved_I = HD_Common_Texture.LoadTexture(treeBranchIconCurvedIName);
        public static readonly Texture2D TreeBranchIconCurved_L = HD_Common_Texture.LoadTexture(treeBranchIconCurvedLName);
        public static readonly Texture2D TreeBranchIconCurved_T = HD_Common_Texture.LoadTexture(treeBranchIconCurvedTName);
        public static readonly Texture2D TreeBranchIconCurved_TerminalBud = HD_Common_Texture.LoadTexture(treeBranchIconCurvedTerminalBudName);
        public static readonly Texture2D TreeBranchIconDotted_I = HD_Common_Texture.LoadTexture(treeBranchIconDottedIName);
        public static readonly Texture2D TreeBranchIconDotted_L = HD_Common_Texture.LoadTexture(treeBranchIconDottedLName);
        public static readonly Texture2D TreeBranchIconDotted_T = HD_Common_Texture.LoadTexture(treeBranchIconDottedTName);
        public static readonly Texture2D TreeBranchIconDotted_TerminalBud = HD_Common_Texture.LoadTexture(treeBranchIconDottedTerminalBudName);
        public static readonly Texture2D TreeBranchIconSegmented_I = HD_Common_Texture.LoadTexture(treeBranchIconSegmentedIName);
        public static readonly Texture2D TreeBranchIconSegmented_L = HD_Common_Texture.LoadTexture(treeBranchIconSegmentedLName);
        public static readonly Texture2D TreeBranchIconSegmented_T = HD_Common_Texture.LoadTexture(treeBranchIconSegmentedTName);
        public static readonly Texture2D TreeBranchIconSegmented_TerminalBud = HD_Common_Texture.LoadTexture(treeBranchIconSegmentedTerminalBudName);

        public static Texture2D GetTreeBranchIconI(HD_Settings_Design.TreeBranchImageType imageType)
        {
            return imageType switch
            {
                HD_Settings_Design.TreeBranchImageType.Curved => TreeBranchIconCurved_I,
                HD_Settings_Design.TreeBranchImageType.Dotted => TreeBranchIconDotted_I,
                HD_Settings_Design.TreeBranchImageType.Segmented => TreeBranchIconSegmented_I,
                _ => TreeBranchIconDefault_I,
            };
        }

        public static Texture2D GetTreeBranchIconL(HD_Settings_Design.TreeBranchImageType imageType)
        {
            return imageType switch
            {
                HD_Settings_Design.TreeBranchImageType.Curved => TreeBranchIconCurved_L,
                HD_Settings_Design.TreeBranchImageType.Dotted => TreeBranchIconDotted_L,
                HD_Settings_Design.TreeBranchImageType.Segmented => TreeBranchIconSegmented_L,
                _ => TreeBranchIconDefault_L,
            };
        }

        public static Texture2D GetTreeBranchIconT(HD_Settings_Design.TreeBranchImageType imageType)
        {
            return imageType switch
            {
                HD_Settings_Design.TreeBranchImageType.Curved => TreeBranchIconCurved_T,
                HD_Settings_Design.TreeBranchImageType.Dotted => TreeBranchIconDotted_T,
                HD_Settings_Design.TreeBranchImageType.Segmented => TreeBranchIconSegmented_T,
                _ => TreeBranchIconDefault_T,
            };
        }

        public static Texture2D GetTreeBranchIconTerminalBud(HD_Settings_Design.TreeBranchImageType imageType)
        {
            return imageType switch
            {
                HD_Settings_Design.TreeBranchImageType.Curved => TreeBranchIconCurved_TerminalBud,
                HD_Settings_Design.TreeBranchImageType.Dotted => TreeBranchIconDotted_TerminalBud,
                HD_Settings_Design.TreeBranchImageType.Segmented => TreeBranchIconSegmented_TerminalBud,
                _ => TreeBranchIconDefault_TerminalBud,
            };
        }
        #endregion

        #region Folder Images
        private static readonly string folderDefaultIconName = "Hierarchy Designer Folder Icon Default";
        private static readonly string folderDefaultOutlineIconName = "Hierarchy Designer Folder Icon Default Outline";
        private static readonly string folderModernIIconName = "Hierarchy Designer Folder Icon Modern I";
        private static readonly string folderModernIIIconName = "Hierarchy Designer Folder Icon Modern II";
        private static readonly string folderModernIIIIconName = "Hierarchy Designer Folder Icon Modern III";
        private static readonly string folderModernOutlineIconName = "Hierarchy Designer Folder Icon Modern Outline";
        private static readonly string folderNeoIIconName = "Hierarchy Designer Folder Icon Neo I";
        private static readonly string folderNeoIIIconName = "Hierarchy Designer Folder Icon Neo II";
        private static readonly string folderNeoOutlineconName = "Hierarchy Designer Folder Icon Neo Outline";
        private static readonly string folderInspectorIconName = "Hierarchy Designer Folder Icon Scene";

        public static readonly Texture2D FolderDefaultIcon = HD_Common_Texture.LoadTexture(folderDefaultIconName);
        public static readonly Texture2D FolderDefaultOutlineIcon = HD_Common_Texture.LoadTexture(folderDefaultOutlineIconName);
        public static readonly Texture2D FolderModernIIcon = HD_Common_Texture.LoadTexture(folderModernIIconName);
        public static readonly Texture2D FolderModernIIIcon = HD_Common_Texture.LoadTexture(folderModernIIIconName);
        public static readonly Texture2D FolderModernIIIIcon = HD_Common_Texture.LoadTexture(folderModernIIIIconName);
        public static readonly Texture2D FolderModernOutlineIcon = HD_Common_Texture.LoadTexture(folderModernOutlineIconName);
        public static readonly Texture2D FolderNeoIIconName = HD_Common_Texture.LoadTexture(folderNeoIIconName);
        public static readonly Texture2D FolderNeoIIIconName = HD_Common_Texture.LoadTexture(folderNeoIIIconName);
        public static readonly Texture2D FolderNeoOutlineconName = HD_Common_Texture.LoadTexture(folderNeoOutlineconName);
        public static readonly Texture2D FolderInspectorIcon = HD_Common_Texture.LoadTexture(folderInspectorIconName);

        public static Texture2D FolderImageType(HD_Settings_Folders.FolderImageType folderImageType)
        {
            return folderImageType switch
            {
                HD_Settings_Folders.FolderImageType.DefaultOutline => FolderDefaultOutlineIcon,
                HD_Settings_Folders.FolderImageType.ModernI => FolderModernIIcon,
                HD_Settings_Folders.FolderImageType.ModernII => FolderModernIIIcon,
                HD_Settings_Folders.FolderImageType.ModernIII => FolderModernIIIIcon,
                HD_Settings_Folders.FolderImageType.ModernOutline => FolderModernOutlineIcon,
                HD_Settings_Folders.FolderImageType.NeoI => FolderNeoIIconName,
                HD_Settings_Folders.FolderImageType.NeoII => FolderNeoIIIconName,
                HD_Settings_Folders.FolderImageType.NeoOutline => FolderNeoOutlineconName,
                _ => FolderDefaultIcon,
            };
        }
        #endregion

        #region Separator Images
        private static readonly string separatorBackgroundImageDefaultName = "Hierarchy Designer Separator Background Image Default";
        private static readonly string separatorBackgroundImageDefaultFadedBottomName = "Hierarchy Designer Separator Background Image Default Faded Bottom";
        private static readonly string separatorBackgroundImageDefaultFadedLeftName = "Hierarchy Designer Separator Background Image Default Faded Left";
        private static readonly string separatorBackgroundImageDefaultFadedSidewaysName = "Hierarchy Designer Separator Background Image Default Faded Sideways";
        private static readonly string separatorBackgroundImageDefaultFadedRightName = "Hierarchy Designer Separator Background Image Default Faded Right";
        private static readonly string separatorBackgroundImageDefaultFadedTopName = "Hierarchy Designer Separator Background Image Default Faded Top";
        private static readonly string separatorBackgroundImageClassicIName = "Hierarchy Designer Separator Background Image Classic I";
        private static readonly string separatorBackgroundImageClassicIIName = "Hierarchy Designer Separator Background Image Classic II";
        private static readonly string separatorBackgroundImageModernIName = "Hierarchy Designer Separator Background Image Modern I";
        private static readonly string separatorBackgroundImageModernIIName = "Hierarchy Designer Separator Background Image Modern II";
        private static readonly string separatorBackgroundImageModernIIIName = "Hierarchy Designer Separator Background Image Modern III";
        private static readonly string separatorBackgroundImageNeoIName = "Hierarchy Designer Separator Background Image Neo I";
        private static readonly string separatorBackgroundImageNeoIIName = "Hierarchy Designer Separator Background Image Neo II";
        private static readonly string separatorBackgroundImageNextGenIName = "Hierarchy Designer Separator Background Image Next-Gen I";
        private static readonly string separatorBackgroundImageNextGenIIName = "Hierarchy Designer Separator Background Image Next-Gen II";
        private static readonly string separatorBackgroundImagePostmodernIName = "Hierarchy Designer Separator Background Image Postmodern I";
        private static readonly string separatorBackgroundImagePostmodernIIName = "Hierarchy Designer Separator Background Image Postmodern II";
        private static readonly string separatorInspectorIconName = "Hierarchy Designer Separator Icon Inspector";

        public static readonly Texture2D SeparatorBackgroundImageDefault = HD_Common_Texture.LoadTexture(separatorBackgroundImageDefaultName);
        public static readonly Texture2D SeparatorBackgroundImageDefaultFadedBottom = HD_Common_Texture.LoadTexture(separatorBackgroundImageDefaultFadedBottomName);
        public static readonly Texture2D SeparatorBackgroundImageDefaultFadedLeft = HD_Common_Texture.LoadTexture(separatorBackgroundImageDefaultFadedLeftName);
        public static readonly Texture2D SeparatorBackgroundImageDefaultFadedSideways = HD_Common_Texture.LoadTexture(separatorBackgroundImageDefaultFadedSidewaysName);
        public static readonly Texture2D SeparatorBackgroundImageDefaultFadedRight = HD_Common_Texture.LoadTexture(separatorBackgroundImageDefaultFadedRightName);
        public static readonly Texture2D SeparatorBackgroundImageDefaultFadedTop = HD_Common_Texture.LoadTexture(separatorBackgroundImageDefaultFadedTopName);
        public static readonly Texture2D SeparatorBackgroundImageClassicI = HD_Common_Texture.LoadTexture(separatorBackgroundImageClassicIName);
        public static readonly Texture2D SeparatorBackgroundImageClassicII = HD_Common_Texture.LoadTexture(separatorBackgroundImageClassicIIName);
        public static readonly Texture2D SeparatorBackgroundImageModernI = HD_Common_Texture.LoadTexture(separatorBackgroundImageModernIName);
        public static readonly Texture2D SeparatorBackgroundImageModernII = HD_Common_Texture.LoadTexture(separatorBackgroundImageModernIIName);
        public static readonly Texture2D SeparatorBackgroundImageModernIII = HD_Common_Texture.LoadTexture(separatorBackgroundImageModernIIIName);
        public static readonly Texture2D SeparatorBackgroundImageNeoI = HD_Common_Texture.LoadTexture(separatorBackgroundImageNeoIName);
        public static readonly Texture2D SeparatorBackgroundImageNeoII = HD_Common_Texture.LoadTexture(separatorBackgroundImageNeoIIName);
        public static readonly Texture2D SeparatorBackgroundImageNextGenI = HD_Common_Texture.LoadTexture(separatorBackgroundImageNextGenIName);
        public static readonly Texture2D SeparatorBackgroundImageNextGenII = HD_Common_Texture.LoadTexture(separatorBackgroundImageNextGenIIName);
        public static readonly Texture2D SeparatorBackgroundImagePostmodernI = HD_Common_Texture.LoadTexture(separatorBackgroundImagePostmodernIName);
        public static readonly Texture2D SeparatorBackgroundImagePostmodernII = HD_Common_Texture.LoadTexture(separatorBackgroundImagePostmodernIIName);
        public static readonly Texture2D SeparatorInspectorIcon = HD_Common_Texture.LoadTexture(separatorInspectorIconName);

        public static Texture2D SeparatorImageType(HD_Settings_Separators.SeparatorImageType separatorImageType)
        {
            return separatorImageType switch
            {
                HD_Settings_Separators.SeparatorImageType.DefaultFadedBottom => SeparatorBackgroundImageDefaultFadedBottom,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedLeft => SeparatorBackgroundImageDefaultFadedLeft,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedSideways => SeparatorBackgroundImageDefaultFadedSideways,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedRight => SeparatorBackgroundImageDefaultFadedRight,
                HD_Settings_Separators.SeparatorImageType.DefaultFadedTop => SeparatorBackgroundImageDefaultFadedTop,
                HD_Settings_Separators.SeparatorImageType.ClassicI => SeparatorBackgroundImageClassicI,
                HD_Settings_Separators.SeparatorImageType.ClassicII => SeparatorBackgroundImageClassicII,
                HD_Settings_Separators.SeparatorImageType.ModernI => SeparatorBackgroundImageModernI,
                HD_Settings_Separators.SeparatorImageType.ModernII => SeparatorBackgroundImageModernII,
                HD_Settings_Separators.SeparatorImageType.ModernIII => SeparatorBackgroundImageModernIII,
                HD_Settings_Separators.SeparatorImageType.NeoI => SeparatorBackgroundImageNeoI,
                HD_Settings_Separators.SeparatorImageType.NeoII => SeparatorBackgroundImageNeoII,
                HD_Settings_Separators.SeparatorImageType.NextGenI => SeparatorBackgroundImageNextGenI,
                HD_Settings_Separators.SeparatorImageType.NextGenII => SeparatorBackgroundImageNextGenII,
                HD_Settings_Separators.SeparatorImageType.PostmodernI => SeparatorBackgroundImagePostmodernI,
                HD_Settings_Separators.SeparatorImageType.PostmodernII => SeparatorBackgroundImagePostmodernII,
                _ => SeparatorBackgroundImageDefault,
            };
        }
        #endregion
        #endregion
    }
}
#endif