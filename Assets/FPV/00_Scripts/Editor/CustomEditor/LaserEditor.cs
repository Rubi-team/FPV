using FPV.Runtime;
using UnityEditor;
using UnityEngine;

namespace FPV.Editor
{
    [CustomEditor(typeof(Laser))]
    public class LaserEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // Utilise l’inspecteur standard

            var laser = (Laser)target;

            // Vérifie les changements de valeur
            if (laser.editorLaserCount != previousCount)
            {
                Undo.RegisterCompleteObjectUndo(laser.gameObject, "Modifier lasers");
                UpdateLaserBeams(laser, laser.editorLaserCount);
                previousCount = laser.editorLaserCount;
            }
        }

        private int previousCount = -1;


        private void UpdateLaserBeams(Laser laser, int count)
        {
            if (laser.beamPrefab == null)
            {
                Debug.LogWarning("Aucun prefab de laser assigné !");
                return;
            }

            var currentCount = laser.transform.childCount;

            // Supprime les beams en trop
            for (var i = currentCount - 1; i >= count; i--) DestroyImmediate(laser.transform.GetChild(i).gameObject);

            // Ajoute les beams manquants
            for (var i = currentCount; i < count; i++)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(laser.beamPrefab, laser.transform);
                instance.name = $"Beam_{i + 1}";

                // Applique le décalage si spacing activé
                var offset = laser.useSpacing ? i * laser.beamSpacing * laser.spacingAxis.normalized : Vector3.zero;
                instance.transform.localPosition = offset;

                instance.transform.localRotation = Quaternion.identity;
            }

            // Optionnel : Met à jour le nom des beams existants si besoin (par exemple après suppression)
            for (var i = 0; i < count; i++) laser.transform.GetChild(i).name = $"Beam_{i + 1}";
        }
    }
}