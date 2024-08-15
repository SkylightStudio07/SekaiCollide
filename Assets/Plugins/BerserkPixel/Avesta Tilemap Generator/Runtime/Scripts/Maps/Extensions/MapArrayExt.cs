using System.Collections.Generic;
using BerserkPixel.Tilemap_Generator.SO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator {
    public static class MapArrayExt {
        public static MapArray GetMap(this NativeArray<int> jobResult, int width, int height) {
            var terrainMap = new MapArray(width, height);

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++) {
                var key = terrainMap.Key(x, y);

                terrainMap[x, y] = jobResult[key];
            }

            return terrainMap;
        }

        public static NativeArray<int> GetInitialRandomMap(string seed, int width, int height, float fillPercent,
            bool invert = false) {
            var terrainMap = new NativeArray<int>(width * height, Allocator.TempJob);
            var pseudoRandom = new Random(seed.GetHashCode());

            // we fill with random variables
            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    var key = y * width + x;
                    terrainMap[key] = pseudoRandom.Next(0, 100).GetTile(100 - fillPercent, invert);
                }
            }

            return terrainMap;
        }

        public static void GetInitialRandomMap(string seed, float fillPercent,
            ref NativeArray<int> terrainMap, bool invert = false) {
            var pseudoRandom = new Random(seed.GetHashCode());

            // we fill with random variables
            var length = terrainMap.Length;
            for (var i = 0; i < length; i++) {
                terrainMap[i] = pseudoRandom.Next(0, 100).GetTile(100 - fillPercent, invert);
            }
        }

        public static void GetInitialRandomMap(float chance, float fillPercent,
            ref NativeArray<int> terrainMap, bool invert = false) {
            // we fill with random variables
            var length = terrainMap.Length;
            for (var i = 0; i < length; i++) {
                terrainMap[i] = chance.GetTile(100 - fillPercent, invert);
            }
        }

        public static MapArray FullDefaultMap(RandomWalkSO mapConfig) {
            var terrainMap = new MapArray(mapConfig.width, mapConfig.height);

            for (var y = 0; y < mapConfig.height; y++)
            for (var x = 0; x < mapConfig.width; x++) {
                terrainMap[x, y] = mapConfig.invert ? MapGeneratorConst.TERRAIN_TILE : MapGeneratorConst.DEFAULT_TILE;
            }

            return terrainMap;
        }

        public static MapArray FullDefaultMap(PathSO mapConfig) {
            var terrainMap = new MapArray(mapConfig.width, mapConfig.height);

            for (var y = 0; y < mapConfig.height; y++)
            for (var x = 0; x < mapConfig.width; x++) {
                terrainMap[x, y] = mapConfig.invert ? MapGeneratorConst.TERRAIN_TILE : MapGeneratorConst.DEFAULT_TILE;
            }

            return terrainMap;
        }

        public static TileChangeData[] GetTilesData(TileBase tile, IEnumerable<Vector3Int> positions) {
            var tileArray = new List<TileChangeData>();

            var tileData = new TileChangeData
            {
                tile = tile
            };

            foreach (var position in positions) {
                tileData.position = position;
                tileArray.Add(tileData);
            }

            return tileArray.ToArray();
        }
    }
}