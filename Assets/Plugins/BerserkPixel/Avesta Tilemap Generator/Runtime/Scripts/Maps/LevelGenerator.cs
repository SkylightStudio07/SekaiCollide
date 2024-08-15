using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Destructible;
using BerserkPixel.Tilemap_Generator.SO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator {
    public abstract class LevelGenerator : MonoBehaviour {
        public delegate void OnMapChangeDelegate(MapArray map);

        [Space]
        [Header("Tilemap")]
        [SerializeField]
        protected Tilemap tilemap;

        [Tooltip("Check to clear the map before putting new tiles on it")]
        [SerializeField]
        private bool _clearOnGenerate = true;

        [Tooltip("Check this if you have an extra Tilemap as shadows")]
        [SerializeField]
        private bool _generateShadows;

        [Note("Be aware that the priority of the layers goes from top to bottom.\n" +
              "Every map algorithm must have the same dimensions")]
        [Tooltip("The list of layers that will be used to generate the tilemap")]
        [SerializeField]
        protected List<MapLayer> layers;

        private Dictionary<int, MapLayer> _cachedAllActiveDictionary;

        private MapArray _generatedMap;

        private void OnDestroy() {
            foreach (var mapLayer in _cachedAllActiveDictionary) {
                mapLayer.Value.MapConfig.OnMapChange -= HandleMapConfigChange;
            }
        }

        protected virtual void OnValidate() {
            var totalLayers = GetActiveLayers();
            if (totalLayers is not {Count: > 0}) {
                return;
            }

            try {
                _cachedAllActiveDictionary = totalLayers.ToDictionary(layer => layer.MapConfig.GetHashCode());
            }
            catch (ArgumentException) {
                return;
            }

            if (totalLayers.Count > 1) {
                for (var i = 0; i < totalLayers.Count; i++) {
                    if (i == 0) {
                        totalLayers[i].IsTheOnlyActive = true;
                        totalLayers[i].IsAdditive = false;
                    }
                    else {
                        totalLayers[i].IsTheOnlyActive = false;
                    }
                }
            }
            else {
                totalLayers[0].IsTheOnlyActive = true;
                totalLayers[0].IsAdditive = false;
            }

            foreach (var mapLayer in _cachedAllActiveDictionary) {
                mapLayer.Value.MapConfig.OnMapChange += HandleMapConfigChange;
            }

            _generatedMap = _cachedAllActiveDictionary.Values.ToList().GetTotalMap();
            OnMapChange?.Invoke(_generatedMap);
        }

        public event OnMapChangeDelegate OnMapChange;

        private void HandleMapConfigChange(MapConfigSO changedMap) {
            _generatedMap = _cachedAllActiveDictionary.Values.ToList().GetTotalMap();
            OnMapChange?.Invoke(_generatedMap);
        }

        public abstract List<MapLayer> GetActiveLayers();

        /// <summary>
        ///     Renders the MapArray into a Tilemap.
        ///     <param name="terrainMap">The MapArray to use for rendering</param>
        /// </summary>
        protected abstract Task RenderMap(MapArray terrainMap);

        /// <summary>
        ///     If selected in the inspector, this method attaches a TilemapCollider2D and a
        ///     Rigidbody2D properly to the Tilemap
        /// </summary>
        protected abstract void GenerateColliders();

        /// <summary>
        ///     Clears the tilemap and it's colliders
        /// </summary>
        protected abstract void ClearMap();

        public async void GenerateLayers() {
            var totalLayers = GetActiveLayers().GetTotalMap();

            if (_clearOnGenerate) {
                ClearMap();
            }

            await RenderMap(totalLayers);

            GenerateColliders();

            if (_generateShadows) {
                var allShadows = FindObjectsOfType<TilemapShadow>();
                foreach (var shadow in allShadows) {
                    if (shadow.gameObject.activeInHierarchy) {
                        shadow.UpdateShadows();
                    }
                }
            }

            var mapAdditives = FindObjectsOfType<MapAdditive>();
            foreach (var additive in mapAdditives) {
                if (additive.gameObject.activeInHierarchy) {
                    additive.AddTiles();
                }
            }

            var destructibleTilemapBackgrounds = FindObjectsOfType<DestructibleTilemapBackground>();
            foreach (var destructibleBg in destructibleTilemapBackgrounds) {
                if (destructibleBg.gameObject.activeInHierarchy) {
                    destructibleBg.UpdateBackground();
                }
            }
        }

        public void DoClearMap() {
            ClearMap();

            var tilemapShadows = FindObjectsOfType<TilemapShadow>();
            foreach (var shadow in tilemapShadows) {
                shadow.ClearShadows();
            }

            var mapAdditives = FindObjectsOfType<MapAdditive>();
            foreach (var additive in mapAdditives) {
                additive.Clear();
            }

            var destructibleTilemapBackgrounds = FindObjectsOfType<DestructibleTilemapBackground>();
            foreach (var destructibleBg in destructibleTilemapBackgrounds) {
                destructibleBg.ClearBackground();
            }
        }
    }
}