#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    internal static class HD_Settings_Folders
    {
        #region Properties
        [System.Serializable]
        public class HD_FolderData
        {
            public string Name = "Folder";
            public Color TextColor = HD_Settings_Design.FolderDefaultTextColor;
            public int FontSize = HD_Settings_Design.FolderDefaultFontSize;
            public FontStyle FontStyle = HD_Settings_Design.FolderDefaultFontStyle;
            public Color ImageColor = HD_Settings_Design.FolderDefaultImageColor;
            public FolderImageType ImageType = HD_Settings_Design.FolderDefaultImageType;
        }
        public enum FolderImageType
        { 
            Default, 
            DefaultOutline,
            ModernI,
            ModernII,
            ModernIII,
            ModernOutline,
            NeoI,
            NeoII,
            NeoOutline,
        }
        private static Dictionary<string, HD_FolderData> folders = new();
        #endregion

        #region Initialization
        public static void Initialize()
        {
            LoadSettings();
            LoadHierarchyDesignerManagerGameObjectCaches();
        }

        private static void LoadHierarchyDesignerManagerGameObjectCaches()
        {
            Dictionary<int, (Color textColor, int fontSize, FontStyle fontStyle, Color folderColor, FolderImageType folderImageType)> folderCache = new();
            foreach (KeyValuePair<string, HD_FolderData> folder in folders)
            {
                int instanceID = folder.Key.GetHashCode();
                folderCache[instanceID] = (folder.Value.TextColor, folder.Value.FontSize, folder.Value.FontStyle, folder.Value.ImageColor, folder.Value.ImageType);
            }
            HD_Manager_GameObject.FolderCache = folderCache;
        }
        #endregion

        #region Methods
        public static void SetFolderData(string folderName, Color textColor, int fontSize, FontStyle fontStyle, Color imageColor, FolderImageType imageType)
        {
            if (folders.TryGetValue(folderName, out HD_FolderData folderData))
            {
                folderData.TextColor = textColor;
                folderData.FontSize = fontSize;
                folderData.FontStyle = fontStyle;
                folderData.ImageColor = imageColor;
                folderData.ImageType = imageType;
            }
            else
            {
                folders[folderName] = new()
                {
                    Name = folderName,
                    TextColor = textColor,
                    FontSize = fontSize,
                    FontStyle = fontStyle,
                    ImageColor = imageColor,
                    ImageType = imageType
                };
            }
            SaveSettings();
            HD_Manager_GameObject.ClearFolderCache();
        }

        public static void ApplyChangesToFolders(Dictionary<string, HD_FolderData> tempFolders, List<string> foldersOrder)
        {
            Dictionary<string, HD_FolderData> orderedFolders = new();
            foreach (string key in foldersOrder)
            {
                if (tempFolders.TryGetValue(key, out HD_FolderData folderData))
                {
                    orderedFolders[key] = folderData;
                }
            }
            folders = orderedFolders;
        }

        public static HD_FolderData GetFolderData(string folderName)
        {
            if (folders.TryGetValue(folderName, out HD_FolderData folderData))
            {
                return folderData;
            }
            return null;
        }

        public static Dictionary<string, HD_FolderData> GetAllFoldersData(bool updateData)
        {
            if (updateData) LoadSettings();
            return new(folders);
        }

        public static bool RemoveFolderData(string folderName)
        {
            if (folders.TryGetValue(folderName, out _))
            {
                folders.Remove(folderName);
                SaveSettings();
                HD_Manager_GameObject.ClearFolderCache();
                return true;
            }
            return false;
        }

        public static Dictionary<string, List<string>> GetFolderImageTypesGrouped()
        {
            return new()
            {
                {
                    "Default", new()
                    {
                        "Default",
                        "Default Outline"
                    }
                },
                {
                    "Modern", new()
                    {
                        "Modern I",
                        "Modern II",
                        "Modern III",
                        "Modern Outline"
                    }
                },
                {
                    "Neo",
                    new()
                    {
                        "Neo I",
                        "Neo II",
                        "Neo Outline",
                    }
                }
            };
        }

        public static FolderImageType ParseFolderImageType(string displayName)
        {
            return displayName switch
            {
                "Default" => FolderImageType.Default,
                "Default Outline" => FolderImageType.DefaultOutline,
                "Modern I" => FolderImageType.ModernI,
                "Modern II" => FolderImageType.ModernII,
                "Modern III" => FolderImageType.ModernIII,
                "Modern Outline" => FolderImageType.ModernOutline,
                "Neo I" => FolderImageType.NeoI,
                "Neo II" => FolderImageType.NeoII,
                "Neo Outline" => FolderImageType.NeoOutline,
                _ => FolderImageType.Default,
            };
        }

        public static string GetFolderImageTypeDisplayName(FolderImageType imageType)
        {
            return imageType switch
            {
                FolderImageType.Default => "Default",
                FolderImageType.DefaultOutline => "Default Outline",
                FolderImageType.ModernI => "Modern I",
                FolderImageType.ModernII => "Modern II",
                FolderImageType.ModernIII => "Modern III",
                FolderImageType.ModernOutline => "Modern Outline",
                FolderImageType.NeoI => "Neo I",
                FolderImageType.NeoII => "Neo II",
                FolderImageType.NeoOutline => "Neo Outline",
                _ => imageType.ToString(),
            };
        }
        #endregion

        #region Save and Load
        public static void SaveSettings()
        {
            string dataFilePath = HD_Common_File.GetSavedDataFilePath(HD_Common_Constants.FolderSettingsTextFileName);
            HD_Common_Serializable<HD_FolderData> serializableList = new(new(folders.Values));
            string json = JsonUtility.ToJson(serializableList, true);
            File.WriteAllText(dataFilePath, json);
            AssetDatabase.Refresh();
        }

        public static void LoadSettings()
        {
            string dataFilePath = HD_Common_File.GetSavedDataFilePath(HD_Common_Constants.FolderSettingsTextFileName);
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                HD_Common_Serializable<HD_FolderData> loadedFolders = JsonUtility.FromJson<HD_Common_Serializable<HD_FolderData>>(json);
                folders.Clear();
                foreach (HD_FolderData folder in loadedFolders.items)
                {
                    folder.ImageType = HD_Common_Checker.ParseEnum(folder.ImageType.ToString(), FolderImageType.Default);
                    folders[folder.Name] = folder;
                }
            }
            else
            {
                SetDefaultSettings();
            }
        }

        private static void SetDefaultSettings()
        {
            folders = new();
        }
        #endregion
    }
}
#endif