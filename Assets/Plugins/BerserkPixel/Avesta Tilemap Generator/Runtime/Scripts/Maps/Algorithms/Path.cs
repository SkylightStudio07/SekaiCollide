using System;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class Path : IMapAlgorithm {
        private readonly PathSO _mapConfig;

        public Path(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as PathSO;
        }

        public string GetMapName() {
            return "Path_GeneratedMap_[" + _mapConfig.seed + "][" + _mapConfig.direction + "]";
        }

        public MapArray RandomFillMap() {
            var terrainMap = MapArrayExt.FullDefaultMap(_mapConfig);

            var pseudoRandom = new Random(_mapConfig.seed.GetHashCode());

            var pathWidth = 1;
            var initialX = _mapConfig.startingPoint.x;
            var initialY = _mapConfig.startingPoint.y;

            Carve(terrainMap, initialX, initialY);

            switch (_mapConfig.direction) {
                case PathSO.Directions.TopToBottom:
                    var x1 = initialX;
                    for (var i = -pathWidth; i <= pathWidth; i++) {
                        Carve(terrainMap, x1 + i, initialY);
                    }

                    for (var y = initialY; y > 0; y--) {
                        pathWidth = ComputeWidth(
                            pseudoRandom,
                            pathWidth
                        );

                        x1 = DetermineNextStep(pseudoRandom, x1);

                        for (var i = -pathWidth; i <= pathWidth; i++) {
                            Carve(terrainMap, x1 + i, y);
                        }
                    }

                    break;
                case PathSO.Directions.BottomToTop:
                    var x2 = initialX;
                    for (var i = -pathWidth; i <= pathWidth; i++) {
                        Carve(terrainMap, x2 + i, initialY);
                    }

                    for (var y = initialY; y < _mapConfig.height; y++) {
                        pathWidth = ComputeWidth(
                            pseudoRandom,
                            pathWidth
                        );

                        x2 = DetermineNextStep(pseudoRandom, x2);

                        for (var i = -pathWidth; i <= pathWidth; i++) {
                            Carve(terrainMap, x2 + i, y);
                        }
                    }

                    break;
                case PathSO.Directions.LeftToRight:
                    var y1 = initialY;
                    for (var i = -pathWidth; i <= pathWidth; i++) {
                        Carve(terrainMap, initialX, y1 + i);
                    }

                    for (var x = initialX; x < _mapConfig.width; x++) {
                        pathWidth = ComputeWidth(
                            pseudoRandom,
                            pathWidth
                        );

                        y1 = DetermineNextStep(pseudoRandom, y1);

                        for (var i = -pathWidth; i <= pathWidth; i++) {
                            Carve(terrainMap, x, y1 + i);
                        }
                    }

                    break;
                case PathSO.Directions.RightToLeft:
                    var y2 = initialY;
                    for (var i = -pathWidth; i <= pathWidth; i++) {
                        Carve(terrainMap, initialX, y2 + i);
                    }

                    for (var x = initialX; x > 0; x--) {
                        pathWidth = ComputeWidth(pseudoRandom, pathWidth);

                        y2 = DetermineNextStep(pseudoRandom, y2);

                        for (var i = -pathWidth; i <= pathWidth; i++) {
                            Carve(terrainMap, x, y2 + i);
                        }
                    }

                    break;
            }

            return terrainMap;
        }

        private void Carve(MapArray terrainMap, int x, int y) {
            if (
                x >= 0
                && x <= terrainMap.Width
                && y >= 0
                && y <= terrainMap.Height
            ) {
                terrainMap[x, y] = _mapConfig.invert ? MapGeneratorConst.DEFAULT_TILE : MapGeneratorConst.TERRAIN_TILE;
            }
        }

        /// <summary>
        ///     Determines the new width of the path
        /// </summary>
        /// <param name="random"></param>
        /// <param name="pathWidth"></param>
        /// <returns></returns>
        private int ComputeWidth(Random random, int pathWidth) {
            if (random.Next(0, 101) > _mapConfig.widthChangePercentage) {
                var widthChange = random.Next(-_mapConfig.pathMaxWidth, _mapConfig.pathMaxWidth);
                pathWidth += widthChange;
                if (pathWidth < _mapConfig.pathMinWidth) {
                    pathWidth = _mapConfig.pathMinWidth;
                }

                if (pathWidth > _mapConfig.pathMaxWidth) {
                    pathWidth = _mapConfig.pathMaxWidth;
                }
            }

            return pathWidth;
        }

        /// <summary>
        ///     Determines in what direction to move the path
        /// </summary>
        /// <param name="random"></param>
        /// <param name="x"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        private int DetermineNextStep(Random random, int x) {
            if (random.Next(0, 101) <= _mapConfig.directionChangePercentage) {
                return x;
            }

            var xChange = random.Next(-_mapConfig.directionChangeDistance, _mapConfig.directionChangeDistance);
            x += xChange;

            if (x < _mapConfig.pathMaxWidth) {
                x = _mapConfig.pathMaxWidth;
            }

            if (x > _mapConfig.width - _mapConfig.pathMaxWidth) {
                x = _mapConfig.width - _mapConfig.pathMaxWidth;
            }

            return x;
        }
    }
}