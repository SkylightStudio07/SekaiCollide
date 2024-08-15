using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator {
    [AddComponentMenu("Avesta/Tilemaps/Map Tile Additive")]
    public class MapTileAdditive : MapAdditive {
        [Tooltip("Add your Tile here")]
        [SerializeField]
        private TileBase _tile;

        private bool IsTileWithinMargins(Tilemap source, Vector3Int position) {
            var up = position;
            up.y += _padding;

            var down = position;
            down.y -= _padding;

            var left = position;
            left.x -= _padding;

            var right = position;
            right.x += _padding;

            return source.HasTile(up) && source.HasTile(down) && source.HasTile(left) && source.HasTile(right);
        }

        protected override void Copy(Tilemap source, Tilemap destination) {
            source.RefreshAllTiles();
            destination.RefreshAllTiles();

            var referenceTilemapPositions = new List<Vector3Int>();

            // we grab all filled positions from the ref tilemap
            foreach (var pos in source.cellBounds.allPositionsWithin) {
                var localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (source.HasTile(localPlace) && IsTileWithinMargins(source, localPlace)) {
                    referenceTilemapPositions.Add(localPlace);
                }
            }

            // we turn our list into an array
            var positions = new Vector3Int[referenceTilemapPositions.Count];
            var allTiles = new TileBase[referenceTilemapPositions.Count];
            var i = 0;

            foreach (var tilePosition in referenceTilemapPositions) {
                positions[i] = tilePosition;
                allTiles[i] = _tile;
                i++;
            }

            // we clear our tilemap and resize it
            destination.ClearAllTiles();
            destination.RefreshAllTiles();
            destination.size = source.size;
            destination.origin = source.origin;
            destination.ResizeBounds();

            // we feed it our positions
            destination.SetTiles(positions, allTiles);
        }

        protected override void SetSortingLayer(Tilemap source, Tilemap destination) {
            var sourceRenderer = source.GetComponent<TilemapRenderer>();
            var destinationRenderer = destination.GetComponent<TilemapRenderer>();

            if (sourceRenderer == null || destinationRenderer == null) {
                return;
            }

            destinationRenderer.sortingLayerID = sourceRenderer.sortingLayerID;

            var sourceOrder = sourceRenderer.sortingOrder;
            destinationRenderer.sortingOrder = sourceOrder + 1;
        }

        public override void Clear() {
            if (_tilemap == null) {
                return;
            }

            _tilemap.ClearAllTiles();
            _tilemap.RefreshAllTiles();
        }
    }
}