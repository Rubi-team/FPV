#if UNITY_EDITOR
using UnityEditor;

namespace Verpha.HierarchyDesigner
{
    [InitializeOnLoad]
    internal static class HD_Manager_Initializer
    {
        static HD_Manager_Initializer()
        {
            HD_Manager_Editor.LoadCache();
            HD_Settings_General.Initialize();
            HD_Settings_Advanced.Initialize();
            HD_Settings_Shortcuts.Initialize();
            HD_Manager_Editor.Initialize();
            HD_Settings_Design.Initialize();
            HD_Settings_Folders.Initialize();
            HD_Settings_Separators.Initialize();
            HD_Manager_GameObject.Initialize();
            HD_Settings_Presets.Initialize();
        }
    }
}
#endif