using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class CellularAutomataJob : MapGenerationJob<CellularMapConfigSO> {
        public override MapArray GenerateNoiseMap(CellularMapConfigSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);
            var fillPercent = mapConfig.fillPercent;
            var seed = mapConfig.seed;
            var invert = mapConfig.invert;
            var iterations = mapConfig.smoothSteps;
            var numberOfNeighbours = mapConfig.numberOfNeighbours;

            var width = dimensions.x;
            var height = dimensions.y;

            var keyLength = width * height;

            // var initialMap = new NativeArray<int>(keyLength, Allocator.TempJob);
            // MapArrayExt.GetInitialRandomMap(
            //     seed, 100 - fillPercent, ref initialMap, true
            // );

            var initialMap = MapArrayExt.GetInitialRandomMap(
                seed, width, height, fillPercent, invert
            );

            var newGrid = new NativeArray<int>(keyLength, Allocator.TempJob);

            // Apply the cellular automata rules over multiple iterations
            for (var i = 1; i <= iterations; i++) {
                var job = new JCellularAutomata
                {
                    Dimensions = dimensions,
                    NumberOfNeighbours = numberOfNeighbours,
                    Existing = initialMap,
                    Result = newGrid
                };

                job.Schedule(newGrid.Length, 64)
                    .Complete();

                initialMap.Dispose();
                initialMap = newGrid;

                if (i != iterations) {
                    newGrid = new NativeArray<int>(keyLength, Allocator.TempJob);
                }
            }

            var terrainMap = newGrid.GetMap(width, height);

            newGrid.Dispose();

            return terrainMap;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    internal struct JCellularAutomata : IJobParallelFor {
        public int2 Dimensions;

        public int NumberOfNeighbours;

        [ReadOnly]
        public NativeArray<int> Existing;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var width = Dimensions.x;
            var height = Dimensions.y;

            var x = index % width;
            var y = index / height;

            var aliveNeighbors = GetNumberOfNeighbors(x, y);

            var currentIndex = y * width + x;

            if (aliveNeighbors >= NumberOfNeighbours) {
                Result[currentIndex] = MapGeneratorConst.TERRAIN_TILE;
            }
            else {
                Result[currentIndex] = MapGeneratorConst.DEFAULT_TILE;
            }
        }

        private int GetNumberOfNeighbors(int x, int y) {
            var wallCount = 0;

            for (var neighbourY = y - 1; neighbourY <= y + 1; neighbourY++) {
                for (var neighbourX = x - 1; neighbourX <= x + 1; neighbourX++) {
                    if (neighbourX >= 0 && neighbourX < Dimensions.x && neighbourY >= 0 &&
                        neighbourY < Dimensions.y) {
                        if (neighbourX == x && neighbourY == y) {
                            continue;
                        }

                        var key = neighbourY * Dimensions.x + neighbourX;
                        wallCount += Existing[key];
                    }
                    else {
                        wallCount++;
                    }
                }
            }

            return wallCount;
        }
    }
}