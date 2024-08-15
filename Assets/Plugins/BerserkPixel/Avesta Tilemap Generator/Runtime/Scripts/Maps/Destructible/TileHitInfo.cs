using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator.Destructible {
    public struct TileHitInfo<T> where T : TileBase {
        public T Tile;
        public Vector3Int CellPosition;
    }
}