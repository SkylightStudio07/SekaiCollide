using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator {
    [AddComponentMenu("Avesta/Tilemaps/Map Object Additive")]
    public class MapObjectAdditive : MapAdditive {
        [Tooltip("Add your GameObject here")]
        [SerializeField]
        private Transform _tile;

        [SerializeField]
        private float _yOffset = .25f;

        private bool IsTileWithinMargins(Transform[] allPositions, Vector3Int position) {
            var up = position;
            up.z += _padding;

            var down = position;
            down.z -= _padding;

            var left = position;
            left.x -= _padding;

            var right = position;
            right.x += _padding;

            return allPositions.Any(t => t.position == up) &&
                   allPositions.Any(t => t.position == down) &&
                   allPositions.Any(t => t.position == left) &&
                   allPositions.Any(t => t.position == right);
        }

        protected override void Copy(Tilemap source, Tilemap destination) {
            var currentMapObjects =
                GetMapObjectsTransform(_referenceTilemap.transform).GetComponentsInChildren<Transform>();

            var currentObjects = _tilemap.transform.Find("Tiles");
            if (currentObjects == null) {
                currentObjects = new GameObject("Tiles")
                {
                    transform =
                    {
                        parent = _tilemap.transform,
                        position = Vector3.zero,
                        localPosition = Vector3.zero
                    }
                }.transform;
            }

            foreach (var child in currentMapObjects) {
                var localPlace = Vector3Int.FloorToInt(child.position);

                if (!IsTileWithinMargins(currentMapObjects, localPlace)) {
                    continue;
                }

                localPlace.y = Mathf.FloorToInt(_tilemap.transform.position.y);

                var t = Instantiate(_tile, currentObjects);
                t.position = localPlace;
            }
        }

        protected override void SetSortingLayer(Tilemap source, Tilemap destination) {
            var position = source.transform.position;
            position.y += _yOffset;
            destination.transform.position = position;
        }

        private Transform GetMapObjectsTransform(Transform tilemap) {
            var currentObjects = tilemap.Find("Tiles");
            if (currentObjects != null) {
                return currentObjects;
            }

            return new GameObject("Tiles")
            {
                transform =
                {
                    parent = _referenceTilemap.transform
                }
            }.transform;
        }

        public override void Clear() {
            if (_tilemap != null) {
                _tilemap.ClearAllTiles();
            }

            var currentMapObjects = GetMapObjectsTransform(_tilemap.transform);
            if (currentMapObjects != null) {
                DestroyImmediate(currentMapObjects.gameObject);
            }
        }
    }
}