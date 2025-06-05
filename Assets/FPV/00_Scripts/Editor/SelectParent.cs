using UnityEditor;

namespace FPV.Editor

{
    public class SelectParent : EditorWindow
    {
        [MenuItem("Edit/Select parent &c")]
        private static void SelectParentOfObject()
        {
            Selection.activeGameObject = Selection.activeGameObject.transform.parent.gameObject;
        }
    }
}