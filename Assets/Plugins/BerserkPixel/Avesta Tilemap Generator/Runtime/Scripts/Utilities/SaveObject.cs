using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator {
    public class SaveObject : MonoBehaviour {
        [SerializeField]
        private string mapName;

        [SerializeField]
        private GameObject objectToSave;

#if UNITY_EDITOR

        /// <summary>
        ///     Saves a map on a Prefab using the DirectoryHelpers script
        /// </summary>
        public void SaveAssetMap() {
            // choose a random name if there's nothing in the inspector 
            var saveName = mapName.Equals("")
                ? MapGeneratorConst.WorldNames[Random.Range(0, MapGeneratorConst.WorldNames.Length)]
                : mapName;

            // we need to wrap the saved game object with a grid component if it's not there

            DirectoryHelpers.SaveMap(
                saveName,
                objectToSave,
                error => { EditorUtility.DisplayDialog("Tilemap NOT saved", error, "Continue"); },
                message =>
                {
                    EditorUtility.DisplayDialog("Tilemap saved", message, "Continue");
                    mapName = "";
                }
            );
        }

#endif
    }
}