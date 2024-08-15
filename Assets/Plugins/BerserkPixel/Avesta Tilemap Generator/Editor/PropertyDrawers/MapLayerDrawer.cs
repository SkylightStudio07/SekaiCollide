using BerserkPixel.Tilemap_Generator.SO;
using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator
{
    [CustomPropertyDrawer(typeof(MapLayer), true)]
    public class MapLayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // default implementation
            EditorGUI.PropertyField(position, property, label, true);
            
            var mapConfig = property.FindPropertyRelative("mapConfig");
            
            if (mapConfig == null || !property.isExpanded || mapConfig.objectReferenceValue == null) return;

            if (!(mapConfig.objectReferenceValue is MapConfigSO mapConfigSO)) return;
            
            var serializedObject = new SerializedObject(mapConfigSO);

            if (!ShouldDisplayPreview(property) || !mapConfigSO.ShouldBeRandom()) return;
            
            const int buttonWidth = 120;
            
            var buttonPosition = new Rect(position)
            {
                x = position.width - buttonWidth * .75f,
                y = position.y + 20,
                width = buttonWidth,
                height = 24
            };
            
            if (GUI.Button(buttonPosition, "Randomize"))
            {
                EditorGUI.BeginChangeCheck();
                mapConfigSO.SetRandomSeed();
                mapConfigSO.MapChange();
            
                if (EditorGUI.EndChangeCheck()) {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private static bool ShouldDisplayPreview(SerializedProperty property)
        {
            return property.isExpanded &&
                   property.displayName.Contains("[ACTIVE]") &&
                   !property.displayName.StartsWith("Full Grid");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUI.GetPropertyHeight(property);
    }
}