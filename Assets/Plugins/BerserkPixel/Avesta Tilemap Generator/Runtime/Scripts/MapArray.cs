using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator {
    public struct MapArray {
        private int[] Array { get; }
        public int Width { get; }
        public int Height { get; }
        private readonly Vector3Int[] _positions;
        private int _index;

        public MapArray(int width, int height) {
            Width = width;
            Height = height;
            Array = new int[Width * Height];
            _positions = new Vector3Int[Array.Length];
            _index = 0;
        }

        /// <summary>
        ///     Access array by 2D coordinates.
        /// </summary>
        /// <param name="x">Horizontal (width) axis.</param>
        /// <param name="y">Vertical (height) axis.</param>
        /// <returns></returns>
        public int this[int x, int y] {
            get => Array[Key(x, y)];
            set
            {
                SetPosition(x, y);
                Array[Key(x, y)] = value;
            }
        }

        public int Key(int x, int y) {
            return y * Width + x;
        }

        private void SetPosition(int x, int y) {
            if (_index >= Array.Length) {
                return;
            }

            _positions[_index] = new Vector3Int(-x + Width / 2, -y + Height / 2, 0);
            _index++;
        }

        public HashSet<Vector3Int> GetPositions() {
            var allTerrainPositions = new HashSet<Vector3Int>();
            var i = 0;
            foreach (var item in Array) {
                if (item == MapGeneratorConst.TERRAIN_TILE) {
                    var position = _positions[i];
                    allTerrainPositions.Add(position);
                }

                i++;
            }

            return allTerrainPositions;
        }

        public Vector3Int[] GetPositions3D(int currentY) {
            var threeDPositions = new HashSet<Vector3Int>();
            var i = 0;
            foreach (var item in Array) {
                if (item == MapGeneratorConst.TERRAIN_TILE) {
                    var position = To3D(_positions[i], currentY);
                    threeDPositions.Add(position);
                }

                i++;
            }

            return threeDPositions.ToArray();
        }

        private static Vector3Int To3D(Vector3Int twoD, int currentY) {
            return new Vector3Int(twoD.x, currentY, twoD.y);
        }

        public TileBase[] GetTiles(TileBase tile) {
            var tileArray = new TileBase[Array.Length];

            var i = 0;
            foreach (var item in Array) {
                if (item == MapGeneratorConst.TERRAIN_TILE) {
                    tileArray[i] = tile;
                }

                i++;
            }

            return tileArray;
        }

        public TileBase GetTile(TileBase tile, int x, int y) {
            return this[x, y] == MapGeneratorConst.TERRAIN_TILE ? tile : null;
        }

        public override bool Equals(object obj) {
            if (!(obj is MapArray other)) {
                return false;
            }

            return Width == other.Width &&
                   Height == other.Height &&
                   _positions.Length == other._positions.Length;
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + Width.GetHashCode();
                hash = hash * 16777619 + Height.GetHashCode();
                hash = hash * 16777619 + _positions.GetHashCode();
                hash = hash * 16777619 + Array.GetHashCode();
                return hash;
            }
        }
    }
}