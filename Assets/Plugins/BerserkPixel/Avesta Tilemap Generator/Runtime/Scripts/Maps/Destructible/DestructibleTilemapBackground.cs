using System.Collections.Generic;
using BerserkPixel.Tilemap_Generator.Attributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator.Destructible {
    public class DestructibleTilemapBackground : MonoBehaviour {
        [SerializeField]
        private Tilemap destructibleTilemap;

        [SerializeField]
        private TileBase tile;

        [InspectorButton(nameof(UpdateBackground))]
        public bool UpdateBackgroundButton;

        private Tilemap _tilemap;

        public void UpdateBackground() {
            if (destructibleTilemap == null) {
                return;
            }

            _tilemap = gameObject.GetComponent<Tilemap>();

            Copy(destructibleTilemap, _tilemap, tile);
        }

        /// <summary>
        ///     Copies the source tilemap on the destination tilemap
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="tileBase"></param>
        private void Copy(Tilemap source, Tilemap destination, TileBase tileBase) {
            source.RefreshAllTiles();
            destination.RefreshAllTiles();

            var referenceTilemapPositions = new List<Vector3Int>();

            // we grab all filled positions from the ref tilemap
            foreach (var pos in source.cellBounds.allPositionsWithin) {
                var localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (source.HasTile(localPlace)) {
                    referenceTilemapPositions.Add(localPlace);
                }
            }

            // we turn our list into an array
            var positions = new Vector3Int[referenceTilemapPositions.Count];
            var allTiles = new TileBase[referenceTilemapPositions.Count];
            var i = 0;

            foreach (var tilePosition in referenceTilemapPositions) {
                positions[i] = tilePosition;
                allTiles[i] = tileBase;
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

        public void ClearBackground() {
            if (_tilemap == null) {
                return;
            }

            _tilemap.ClearAllTiles();
            _tilemap.RefreshAllTiles();
        }
    }
}