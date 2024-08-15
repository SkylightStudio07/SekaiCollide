using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator
{
    public abstract class LevelGeneratorEditor<T> : Editor where T : LevelGenerator
    {
        private T _generator;

        private void OnEnable()
        {
            _generator = (T) target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Place one of this scripts for each Tilemap you want to generate",
                MessageType.None);

            base.OnInspectorGUI();
            
            serializedObject.ApplyModifiedProperties();

            AddButtons(_generator);
        }

        private static void AddButtons(T generator)
        {
#if UNITY_EDITOR && !UNITY_2021_2_OR_NEWER
            GUILayout.Space(8);

            if (GUILayout.Button("Show Preview"))
            {
                OldPreviewOverlay.ShowWindow();
            }
#endif
            GUILayout.Space(8);

            if (GUILayout.Button("Generate Map"))
            {
                Debug.Log($"Generating Map");
                generator.GenerateLayers();
            }

            GUILayout.Space(8);

            if (!GUILayout.Button("Clear Map")) return;

            Debug.Log("Clearing Map");
            generator.DoClearMap();
        }
    }
}