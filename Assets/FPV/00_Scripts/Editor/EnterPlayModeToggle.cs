using UnityEditor;
using UnityEngine;

namespace FPV.Editor
{
    public class EnterPlayModeToggle
    {
        private const string MenuName = "Tools/Fast Play Mode (No Reload)";

        [MenuItem(MenuName, false, 10)] // Ajouter un raccourci Ctrl+Alt+P
        private static void TogglePlayModeSettings()

        {
            bool enabled = EditorSettings.enterPlayModeOptionsEnabled;
            EditorSettings.enterPlayModeOptionsEnabled = !enabled;
        
            if (!enabled)
            {
                EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
                Console.Log("Editor","Fast Play Mode ACTIVÉ (Reload désactivé)");
            }
            else
            {
                EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.None;
                Console.Log("Editor","Fast Play Mode DÉSACTIVÉ (Reload activé)");
            }

            Menu.SetChecked(MenuName, EditorSettings.enterPlayModeOptionsEnabled);
        }

        [MenuItem(MenuName, true)]
        private static bool ValidateMenu()
        {
            Menu.SetChecked(MenuName, EditorSettings.enterPlayModeOptionsEnabled);
            return true;
        }
    }
}