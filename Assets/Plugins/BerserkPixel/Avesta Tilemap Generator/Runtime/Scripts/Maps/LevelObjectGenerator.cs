using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator {
    [AddComponentMenu("Avesta/Tilemaps/Level Object Generator")]
    public class LevelObjectGenerator : LevelGenerator {
        [Space]
        [Header("Type of Tile to use")]
        [Tooltip("Add your GameObject here")]
        [SerializeField]
        private Transform tile;

        private HashSet<Transform> _allTiles;

        public override List<MapLayer> GetActiveLayers() {
            if (layers == null || layers.Count <= 0) {
                return new List<MapLayer>(1);
            }

            return layers.FindAll(layer => layer.active);
        }

        protected override void GenerateColliders() {
            /* do nothing */
        }

        protected override Task RenderMap(MapArray terrainMap) {
            if (_allTiles == null) {
                _allTiles = new HashSet<Transform>();
            }
            else {
                _allTiles.Clear();
            }

            var positions = terrainMap.GetPositions3D(Mathf.FloorToInt(tilemap.transform.position.y));

            var currentMapObjects = GetMapObjectsTransform();

            foreach (var position in positions) {
                var t = Instantiate(tile, currentMapObjects);
                t.position = position;
                _allTiles.Add(t);
            }

            return Task.CompletedTask;
        }

        protected override void ClearMap() {
            if (tilemap != null) {
                tilemap.ClearAllTiles();
            }

            var currentMapObjects = GetMapObjectsTransform();
            if (currentMapObjects != null) {
                DestroyImmediate(currentMapObjects.gameObject);
            }

            if (_allTiles != null) {
                _allTiles.Clear();
            }
        }

        private Transform GetMapObjectsTransform() {
            var currentObjects = tilemap.transform.Find("Tiles");
            if (currentObjects != null) {
                return currentObjects;
            }

            return new GameObject("Tiles")
            {
                transform =
                {
                    parent = tilemap.transform,
                    localPosition = Vector3.zero
                }
            }.transform;
        }
    }
}