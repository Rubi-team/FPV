using UnityEditor;
using UnityEngine;

namespace FPV.Editor
{
    public class DebugCategoryWindow : EditorWindow
    {
        /*private string newCategory = "";

        public static void ShowWindow()
        {
            DebugCategoryWindow window = GetWindow<DebugCategoryWindow>("Add Debug Category");
            window.minSize = new Vector2(300, 100);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Enter a new debug category:", EditorStyles.boldLabel);
            newCategory = EditorGUILayout.TextField("Category Name:", newCategory);

            GUILayout.Space(10);

            if (GUILayout.Button("Add Category"))
            {
                if (!string.IsNullOrWhiteSpace(newCategory))
                {
                    DebugConsoleButton.AddCategory(newCategory.Trim());
                    Close();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Category name cannot be empty!", "OK");
                }
            }
        }*/
    }
}