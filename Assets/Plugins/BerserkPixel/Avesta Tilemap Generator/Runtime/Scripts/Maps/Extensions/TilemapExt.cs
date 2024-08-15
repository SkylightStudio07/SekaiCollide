using System.Collections.Generic;
using BerserkPixel.Tilemap_Generator.Destructible;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator.Extensions {
    public static class TilemapExt {
        public static List<Vector3Int> GetWorldPositionsWithTiles(this Tilemap tilemap) {
            var tileWorldLocations = new List<Vector3Int>();
            foreach (var localPlace in tilemap.cellBounds.allPositionsWithin) {
                if (tilemap.HasTile(localPlace)) {
                    var place = Vector3Int.FloorToInt(tilemap.GetCellCenterWorld(localPlace));
                    tileWorldLocations.Add(place);
                }
            }

            return tileWorldLocations;
        }

        public static List<Vector3Int> GetUsedTilesInAllTilemaps(this Tilemap[] tilemaps) {
            var allUsedTiles = new List<Vector3Int>();

            foreach (var tilemap in tilemaps) {
                allUsedTiles.AddRange(tilemap.GetWorldPositionsWithTiles());
            }

            return allUsedTiles;
        }

        public static List<Vector3Int> GetWorldPositionsForTile(this Tilemap tilemap, TileBase tile) {
            var tileWorldLocations = new List<Vector3Int>();
            foreach (var localPlace in tilemap.cellBounds.allPositionsWithin) {
                if (tilemap.HasTile(localPlace) && tilemap.GetTile(localPlace) == tile) {
                    var place = Vector3Int.FloorToInt(tilemap.GetCellCenterWorld(localPlace));
                    tileWorldLocations.Add(place);
                }
            }

            return tileWorldLocations;
        }

        public static List<Vector3Int> GetLocalPositionsForTile(this Tilemap tilemap, TileBase tile) {
            var tileWorldLocations = new List<Vector3Int>();
            foreach (var localPlace in tilemap.cellBounds.allPositionsWithin) {
                if (tilemap.HasTile(localPlace) && tilemap.GetTile(localPlace) == tile) {
                    tileWorldLocations.Add(localPlace);
                }
            }

            return tileWorldLocations;
        }

        public static bool IsPositionTypeOfTile(this Tilemap tilemap, TileBase tile, Vector3 worldPosition) {
            var localPlace = tilemap.WorldToCell(worldPosition);
            return tilemap.HasTile(localPlace) && tilemap.GetTile(localPlace) == tile;
        }

        public static TileHitInfo<T> GetTileHit<T>(this Tilemap tilemap, Vector3 worldPosition) where T : TileBase {
            var cell = Vector3Int.FloorToInt(worldPosition);
            var value = new TileHitInfo<T>
            {
                Tile = tilemap.GetTile(cell) as T,
                CellPosition = cell
            };
            return value;
        }

        public static void RefreshTileFromWorldPosition(this Tilemap tilemap, Vector3Int cellPosition) {
            DebugDrawRect(cellPosition, 1, Color.red, 2);

            // this will call the Tile.GetTileData
            tilemap.RefreshTile(cellPosition);
            // this makes sure they are rendered properly
            tilemap.SetTile(cellPosition, null);
        }

        public static void DebugDrawRect(Vector3 position, float size, Color color, float duration) {
            Debug.DrawLine(position, new Vector3(position.x + size, position.y, position.z), color, duration);
            Debug.DrawLine(position, new Vector3(position.x, position.y - size, position.z), color, duration);
            Debug.DrawLine(new Vector3(position.x, position.y - size, position.z),
                new Vector3(position.x + size, position.y - size, position.z), color, duration);
            Debug.DrawLine(new Vector3(position.x + size, position.y - size, position.z),
                new Vector3(position.x + size, position.y, position.z), color, duration);
        }
    }
}