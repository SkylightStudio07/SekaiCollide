using System.Collections.Generic;
using BerserkPixel.Tilemap_Generator.Attributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator {
    /// <summary>
    ///     A class to put on a tilemap so it acts as a shadow/copy of another reference tilemap.
    ///     Useful for wall shadows for example.
    ///     Offsetting the tilemap and changing its sorting order etc is done via the regular components
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("Avesta/Tilemaps/Tilemap Shadow")]
    [RequireComponent(typeof(Tilemap))]
    public class TilemapShadow : MonoBehaviour {
        /// the tilemap to copy
        [SerializeField]
        private Tilemap ReferenceTilemap;

        [InspectorButton("UpdateShadows")]
        public bool UpdateShadowButton;

        private Tilemap _tilemap;

        private void Init() {
            if (_tilemap == null) {
                _tilemap = gameObject.GetComponent<Tilemap>();
            }
        }

        /// <summary>
        ///     This method will copy the reference tilemap into the one on this gameobject
        /// </summary>
        public void UpdateShadows() {
            if (ReferenceTilemap == null) {
                return;
            }

            Init();

            Copy(ReferenceTilemap, _tilemap);

            SetSortingLayer(ReferenceTilemap, _tilemap);

            if (transform.position.y == 0) {
                var tempPos = transform.position;
                tempPos.y = -.25f;
                transform.position = tempPos;
            }
        }

        private static void SetSortingLayer(Tilemap source, Tilemap destination) {
            var sourceRenderer = source.GetComponent<TilemapRenderer>();
            var destinationRenderer = destination.GetComponent<TilemapRenderer>();

            if (sourceRenderer == null || destinationRenderer == null) {
                return;
            }

            destinationRenderer.sortingLayerID = sourceRenderer.sortingLayerID;

            var sourceOrder = sourceRenderer.sortingOrder;
            destinationRenderer.sortingOrder = sourceOrder - 1;
        }

        /// <summary>
        ///     Copies the source tilemap on the destination tilemap
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private static void Copy(Tilemap source, Tilemap destination) {
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
                allTiles[i] = source.GetTile(tilePosition);
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

        public void ClearShadows() {
            if (_tilemap == null) {
                return;
            }

            _tilemap.ClearAllTiles();
            _tilemap.RefreshAllTiles();
        }
    }
}