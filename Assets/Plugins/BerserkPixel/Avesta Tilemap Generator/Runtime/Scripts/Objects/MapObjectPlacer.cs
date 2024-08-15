using System.Collections.Generic;
using System.Linq;
using BerserkPixel.Tilemap_Generator.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator {
    public class MapObjectPlacer : MonoBehaviour {
        [SerializeField]
        private Tilemap targetTilemap;

        [SerializeField]
        private Tilemap[] obstaclesTilemaps;

        [Tooltip("True will update inside the Editor whenever a new change is made on this script")]
        public bool autoUpdate = true;

        [Header("Objects To Place")]
        [SerializeField]
        private TileObjects[] objects;

        [SerializeField]
        [Range(0, 100)]
        [Delayed]
        private int objectFillPercent = 3;

        [Header("Clustering")]
        [SerializeField]
        private bool useClusters = true;

        [SerializeField]
        [Delayed]
        private int maxObjectsPerCluster = 4;

        [SerializeField]
        [Range(0, 10)]
        [Delayed]
        private int spacing = 2;

        private readonly Dictionary<Vector3Int, GameObject> _terrainObjects = new();

        private List<Vector3Int> _bannedPlaces = new();

        private Transform _mapObjects;
        private WeightedRandomPicker<GameObject> _weightedRandomPicker;

        private void OnValidate() {
            if (objects == null || objects.Length <= 0) {
                objects = new TileObjects[1];
            }
        }

        private void Init() {
            if (targetTilemap == null) {
                Debug.LogError("You need to set a Tilemap on the inspector");
            }
        }

        #region Editor

        public void PlaceObjects() {
            Init();

            if (targetTilemap == null) {
                return;
            }

            if (_bannedPlaces.Count <= 0) {
                _bannedPlaces = obstaclesTilemaps.GetUsedTilesInAllTilemaps();
            }

            var terrainTiles = targetTilemap.GetWorldPositionsWithTiles();

            if (terrainTiles.Count <= 0) {
                var positions = new HashSet<Vector3Int>();
                foreach (Transform t in GetMapTilesTransform()) {
                    var position = Vector3Int.CeilToInt(t.position);
                    positions.Add(position);
                }

                terrainTiles = positions.ToList();
            }

            var hasAnyPrefab = objects.Any(o => o.prefab != null);

            if (!hasAnyPrefab || terrainTiles.Count <= 0) {
                Debug.LogError("There are no objects with prefabs or the Tilemap is empty. " +
                               "Check your objects array and the tile you are searching for.");
                return;
            }

            var candidates = objects.Select(o => o.prefab).ToList();
            var weights = objects.Select(o => o.weight).ToList();
            _weightedRandomPicker = new WeightedRandomPicker<GameObject>(candidates, weights);

            _mapObjects = GetMapObjectsTransform();

            foreach (var worldPosition in terrainTiles) {
                var shouldPlaceObject = Random.Range(0f, 100f) < objectFillPercent;

                // if the chance is not enough we continue
                if (!shouldPlaceObject) {
                    continue;
                }

                var isBannedPlace = _bannedPlaces.Contains(worldPosition);
                // if we are not allowed to place a tile there (can be a wall or used
                if (isBannedPlace) {
                    continue;
                }

                if (useClusters) {
                    PlaceClusterObjects(_weightedRandomPicker.Pick(), worldPosition);
                }
                else {
                    PlaceUniqueObject(_weightedRandomPicker.Pick(), worldPosition);
                }

                // we update the new placed position object as banned so we don't overlap
                _bannedPlaces.Add(worldPosition);
            }
        }

        private void PlaceUniqueObject(GameObject prefab, Vector3Int position) {
            // if we already have an object on that position
            if (_terrainObjects.ContainsKey(position)) {
                return;
            }

            var tileObject = GetTileObject(prefab);

            if (tileObject.prefab != null) {
                var objectInTilemap = InstantiateObject(prefab, position + tileObject.offset);
                _terrainObjects.Add(position, objectInTilemap);
            }
        }

        private void PlaceClusterObjects(GameObject prefab, Vector3Int position) {
            var tileObject = GetTileObject(prefab);

            if (tileObject.prefab != null) {
                switch (tileObject.objectType) {
                    case ObjectType.None:
                    default:
                        // basically do nothing
                        break;
                    case ObjectType.Object2D:
                        InstantiateCluster2D(prefab, position);
                        break;
                    case ObjectType.Object3D:
                        InstantiateCluster3D(prefab, position, tileObject.offset);
                        break;
                }
            }
        }

        private void InstantiateCluster2D(GameObject prefab, Vector3Int position) {
            // we have already decided there's at least one object here
            var amount = Random.Range(0, maxObjectsPerCluster) + 1;

            var initialPosition = new Vector2Int(position.x, position.y);

            for (var i = 0; i < amount; i++) {
                var delta = Vector2Int.one * spacing;
                var point = (Vector3Int) (initialPosition + delta);

                var isBannedPlace = _bannedPlaces.Contains(point);
                // if we are not allowed to place a tile there (can be a wall or used
                if (isBannedPlace) {
                    continue;
                }

                // if we already have an object on that position
                if (_terrainObjects.ContainsKey(point)) {
                    continue;
                }

                // check if tilemap has ground there
                if (!targetTilemap.HasTile(point)) {
                    continue;
                }

                var objectInTilemap = InstantiateObject(prefab, point);
                _terrainObjects.Add(point, objectInTilemap);
            }
        }

        private void InstantiateCluster3D(GameObject prefab, Vector3Int position, Vector3 offset) {
            // we have already decided there's at least one object here
            var amount = Random.Range(0, maxObjectsPerCluster) + 1;

            var initialPosition = position;

            for (var i = 0; i < amount; i++) {
                var delta = new Vector3Int(1, 0, 1) * spacing;
                var point = initialPosition + delta;

                var isBannedPlace = _bannedPlaces.Contains(point);
                // if we are not allowed to place a tile there (can be a wall or used)
                if (isBannedPlace) {
                    continue;
                }

                // if we already have an object on that position
                if (_terrainObjects.ContainsKey(point)) {
                    continue;
                }

                var objectInTilemap = InstantiateObject(prefab, point + prefab.transform.position + offset);
                _terrainObjects.Add(point, objectInTilemap);
            }
        }

        private GameObject InstantiateObject(GameObject prefab, Vector3 position) {
            var objectInTilemap = Instantiate(prefab, _mapObjects.transform);
            var objectTransform = objectInTilemap.transform;
            objectTransform.position = position;

            var tileObject = GetTileObject(prefab);

            if (tileObject.prefab == null) {
                return objectInTilemap;
            }

            tileObject.SetRotation(objectTransform);
            tileObject.SetScale(objectTransform);

            return objectInTilemap;
        }

        private TileObjects GetTileObject(GameObject toSearch) {
            foreach (var obj in objects) {
                if (obj.prefab == toSearch) {
                    return obj;
                }
            }

            return default;
        }

        private Transform GetMapObjectsTransform() {
            var currentObjects = targetTilemap.transform.Find("Objects");
            if (currentObjects != null) {
                return currentObjects;
            }

            return new GameObject("Objects")
            {
                transform =
                {
                    parent = targetTilemap.transform
                }
            }.transform;
        }

        private Transform GetMapTilesTransform() {
            var currentObjects = targetTilemap.transform.Find("Tiles");
            if (currentObjects != null) {
                return currentObjects;
            }

            return new GameObject("Tiles")
            {
                transform =
                {
                    parent = targetTilemap.transform
                }
            }.transform;
        }

        public void ClearObjects() {
            Init();

            _terrainObjects.Clear();
            _bannedPlaces.Clear();

            if (targetTilemap == null) {
                return;
            }

            var currentMapObjects = GetMapObjectsTransform();
            if (currentMapObjects != null) {
                DestroyImmediate(currentMapObjects.gameObject);
            }
        }

        #endregion
    }
}