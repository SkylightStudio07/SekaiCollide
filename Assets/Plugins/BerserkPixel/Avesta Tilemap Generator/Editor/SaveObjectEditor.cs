using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator
{
    [CustomEditor(typeof(SaveObject))]
    public class SaveObjectEditor : Editor
    {
        private SaveObject _generator;

        private void OnEnable()
        {
            _generator = (SaveObject) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!GUILayout.Button("Save Map")) return;

            Debug.Log("Saving Map");
            _generator.SaveAssetMap();
        }
    }
}