#if UNITY_EDITOR && UNITY_2021_2_OR_NEWER

using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace BerserkPixel.Tilemap_Generator
{
    [Overlay(typeof(SceneView), "Preview Overlay")]
    [Icon(_assetPath + "world_icon.png")]
    public class PreviewOverlay : Overlay, ITransientOverlay
    {
        private const string _assetPath = "Assets/Plugins/BerserkPixel/Avesta Tilemap Generator/Icons/";

        private LevelGenerator _generator;
        private VisualElement _root;

        public bool visible
        {
            get
            {
                if (Application.isPlaying) return false;
                return Selection.activeGameObject != null &&
                       Selection.activeGameObject.GetComponent<LevelGenerator>() != null;
            }
        }

        public override void OnCreated()
        {
            base.OnCreated();
            displayedChanged += HandleDisplayChanged;
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();
            displayedChanged -= HandleDisplayChanged;
        }

        private void HandleDisplayChanged(bool isDisplayed)
        {
            if (_generator == null) return;

            if (isDisplayed)
            {
                _generator.OnMapChange += HandleMapChange;
            }
            else
            {
                _generator.OnMapChange -= HandleMapChange;
            }
        }

        private void HandleMapChange(MapArray map)
        {
            if (map.Width <= 0 || map.Height <= 0 || _root == null) return;

            var image = UpdateTextureMap(map);

            if (image == null) return;

            _root.Clear();
            _root.Add(image);
        }

        public override VisualElement CreatePanelContent()
        {
            _root = new VisualElement {name = "Preview"};
            
            if (Selection.activeGameObject == null) return _root;
            
            _generator = Selection.activeGameObject.GetComponent<LevelGenerator>();

            if (_generator == null) return _root;

            var image = GenerateTextureMap();

            if (image == null) return _root;

            _root.Clear();
            _root.Add(image);

            return _root;
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