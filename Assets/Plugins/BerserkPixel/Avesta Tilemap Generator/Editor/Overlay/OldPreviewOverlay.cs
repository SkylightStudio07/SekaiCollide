#if UNITY_EDITOR && !UNITY_2021_2_OR_NEWER

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BerserkPixel.Tilemap_Generator
{
    public class OldPreviewOverlay : EditorWindow
    {
        private LevelGenerator _generator;
        private VisualElement _root;

        public static void ShowWindow()
        {
            var window = GetWindow<OldPreviewOverlay>();
            window.maxSize = new Vector2(Texture2DExt.PREVIEW_SIZE, Texture2DExt.PREVIEW_SIZE);
            window.minSize = window.maxSize;
            window.titleContent = new GUIContent("Preview Overlay");
            window.Show();
        }

        private void CreateGUI()
        {
            _root = new VisualElement
            {
                name = "Preview",
                style =
                {
                    width = new StyleLength(Texture2DExt.PREVIEW_SIZE),
                    height = new StyleLength(Texture2DExt.PREVIEW_SIZE)
                }
            };

            _generator = Selection.activeGameObject.GetComponent<LevelGenerator>();

            if (_generator == null) return;
            
            var image = GenerateTextureMap();

            if (image == null) return;

            _root.Clear();
            _root.Add(image);

            rootVisualElement.Add(_root);
        }

        private void OnEnable()
        {
            _generator = Selection.activeGameObject.GetComponent<LevelGenerator>();
            
            if (_generator == null) return;
            
            _generator.OnMapChange += HandleMapChange;
        }

        private void OnDisable()
        {
            if (_generator == null) return;
            
            _generator.OnMapChange -= HandleMapChange;
        }
        
        private void HandleMapChange(MapArray map)
        {
            if (map.Width <= 0 || map.Height <= 0 || _generator == null) return;

            var image = UpdateTextureMap(map);

            if (image == null) return;

            _root.Clear();
            _root.Add(image);
        }

        private Image GenerateTextureMap()
        {
            if (_generator == null) return null;

            var allActiveLayers = _generator.GetActiveLayers();

            if (allActiveLayers == null || allActiveLayers.Count <= 0) return null;

            var currentMap = allActiveLayers.GetTotalMap();

            return UpdateTextureMap(currentMap);
        }

        private Image UpdateTextureMap(MapArray mapArray)
        {
            if (_generator == null) return null;

            var allActiveLayers = _generator.GetActiveLayers();

            if (allActiveLayers == null || allActiveLayers.Count <= 0) return null;

            var algorithmsUsed = string.Empty;

            foreach (var layer in allActiveLayers)
                if (layer.IsAdditive)
                    algorithmsUsed += $"\n{layer.name}";
                else
                    algorithmsUsed = layer.name;

            return new Image
            {
                tooltip = algorithmsUsed,
                image = mapArray.GetMapTexture(),
                scaleMode = ScaleMode.StretchToFill
            };
        }
    }
}

#endif