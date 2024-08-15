using System;
using UnityEngine;

namespace BerserkPixel.Readme
{
    [CreateAssetMenu(menuName = "Readme/Readme", fileName = "Readme")]
    public class Readme : ScriptableObject
    {
        [HideInInspector] public bool ShowInInspector;

        [NonSerialized] public string AssetName = "Avesta";
        [NonSerialized] public string AssetVersion = "2.9.0";
        [NonSerialized] public bool checkUpdate = true;
        [NonSerialized] public bool contact = true;
        [NonSerialized] public bool debug = true;
        [NonSerialized] public bool documentation = true;
        [NonSerialized] public string FileType = "BerserkPixel.Avesta";
        [NonSerialized] public string PackageName = "Avesta: Procedural Tilemap Generator";
        [NonSerialized] public bool specialThanks = true;

        [NonSerialized] public string UnityVersion = Application.unityVersion;

        [HideInInspector] public BerserkURL urlsConfig = new BerserkURL();

        private void OnValidate()
        {
            ShowInInspector = urlsConfig == null;
        }
    }
}