using BerserkPixel.Tilemap_Generator.SO;
using UnityEngine;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class RandomWalk : IMapAlgorithm {
        private readonly RandomWalkSO _mapConfig;

        public RandomWalk(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as RandomWalkSO;
        }

        public string GetMapName() {
            return $"RW_GeneratedMap_[{_mapConfig.seed}][{_mapConfig.fillPercent}]";
        }

        public MapArray RandomFillMap() {
            var terrainMap = MapArrayExt.FullDefaultMap(_mapConfig);

            var pseudoRandom = new Random(_mapConfig.seed.GetHashCode());

            var requiredFillQuantity = (int) (_mapConfig.width * _mapConfig.height * _mapConfig.fillPercent / 100);
            var fillCounter = 0;

            _mapConfig.startingPoint = new Vector2Int(pseudoRandom.Next(0, _mapConfig.width),
                pseudoRandom.Next(0, _mapConfig.height));

            var currentX = _mapConfig.startingPoint.x;
            var currentY = _mapConfig.startingPoint.y;
            terrainMap[currentX, currentY] = 0;
            fillCounter++;
            var iterationsCounter = 0;

            while (fillCounter < requiredFillQuantity && iterationsCounter < _mapConfig.maxIterations) {
                var direction = pseudoRandom.Next(4);

                switch (direction) {
                    case 0:
                        if (currentY + 1 < _mapConfig.height) {
                            currentY++;
                            terrainMap = Carve(terrainMap, currentX, currentY, ref fillCounter);
                        }

                        break;
                    case 1:
                        if (currentY - 1 > 1) {
                            currentY--;
                            terrainMap = Carve(terrainMap, currentX, currentY, ref fillCounter);
                        }

                        break;
                    case 2:
                        if (currentX - 1 > 1) {
                            currentX--;
                            terrainMap = Carve(terrainMap, currentX, currentY, ref fillCounter);
                        }

                        break;
                    case 3:
                        if (currentX + 1 < _mapConfig.width) {
                            currentX++;
                            terrainMap = Carve(terrainMap, currentX, currentY, ref fillCounter);
                        }

                        break;
                }

                iterationsCounter++;
            }

            return terrainMap;
        }

        private MapArray Carve(MapArray terrainMap, int x, int y, ref int fillCounter) {
            var tile = _mapConfig.invert ? MapGeneratorConst.DEFAULT_TILE : MapGeneratorConst.TERRAIN_TILE;
            var checkTile = _mapConfig.invert ? MapGeneratorConst.TERRAIN_TILE : MapGeneratorConst.DEFAULT_TILE;

            if (terrainMap[x, y] != checkTile) {
                return terrainMap;
            }

            terrainMap[x, y] = tile;
            fillCounter++;
            return terrainMap;
        }
    }
}