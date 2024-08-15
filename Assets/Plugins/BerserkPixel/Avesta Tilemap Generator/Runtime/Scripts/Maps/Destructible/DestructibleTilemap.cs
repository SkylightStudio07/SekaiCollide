using System;
using BerserkPixel.Tilemap_Generator.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator.Destructible {
    [RequireComponent(typeof(Tilemap))]
    public class DestructibleTilemap : MonoBehaviour {
        public static Action<DestructibleTilemap, TileHitInfo<DestructibleTile>> OnTileHit = delegate { };
        private Tilemap _tilemap;

        private void Awake() {
            _tilemap = GetComponent<Tilemap>();
        }

        /// <summary>
        ///     Performs the destruction of a tile, returning the earned points set on the <see cref="DestructibleTile" />.
        /// </summary>
        /// <param name="worldPosition">The world position where to check</param>
        /// <param name="damageAmount">The amount of damage to that tile</param>
        /// <returns>The earned points of that <see cref="DestructibleTile" /></returns>
        public int PerformContact(Vector2 worldPosition, int damageAmount) {
            return PerformContact((Vector3) worldPosition, damageAmount);
        }

        /// <summary>
        ///     Performs the destruction of a tile, returning the earned points set on the <see cref="DestructibleTile" />.
        /// </summary>
        /// <param name="worldPosition">The world position where to check</param>
        /// <param name="damageAmount">The amount of damage to that tile</param>
        /// <returns>The earned points of that <see cref="DestructibleTile" /></returns>
        public int PerformContact(Vector3 worldPosition, int damageAmount) {
            var tileHitInfo = _tilemap.GetTileHit<DestructibleTile>(worldPosition);

            if (tileHitInfo.Tile == null) {
                return 0;
            }

            OnTileHit?.Invoke(this, tileHitInfo);

            tileHitInfo.Tile.Damage(damageAmount);

            if (!tileHitInfo.Tile.IsTileDestructed) {
                return 0;
            }

            _tilemap.RefreshTileFromWorldPosition(tileHitInfo.CellPosition);

            return tileHitInfo.Tile.earnedPoints;
        }

        /// <summary>
        ///     Transforms a world position (ex. a mouse position) to the center of a tile
        /// </summary>
        /// <param name="worldPosition">The <see cref="Vector3" /> reference to check</param>
        /// <returns>The center in world coordinates of a tile</returns>
        public Vector3 GetTileCenter(Vector3 worldPosition) {
            var tilePos = _tilemap.WorldToCell(worldPosition);
            var tileCenter = _tilemap.GetCellCenterWorld(tilePos);
            return tileCenter;
        }
    }
}