using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class GameOfLifeJob : MapGenerationJob<GameOfLifeMapConfigSO> {
        public override MapArray GenerateNoiseMap(GameOfLifeMapConfigSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);
            float iniChance = mapConfig.iniChance;
            var invert = mapConfig.invert;
            var seed = mapConfig.seed;
            var iterations = mapConfig.numR;
            var birthLimit = mapConfig.birthLimit;
            var deathLimit = mapConfig.deathLimit;

            var width = dimensions.x;
            var height = dimensions.y;

            var keyLength = width * height;

            var existing = MapArrayExt.GetInitialRandomMap(
                seed, width, height, iniChance, invert
            );

            var jobResult = new NativeArray<int>(keyLength, Allocator.TempJob);

            for (var i = 1; i <= iterations; i++) {
                var job = new JGameOfLife
                {
                    Dimensions = dimensions,
                    BirthLimit = birthLimit,
                    DeathLimit = deathLimit,
                    Invert = invert,
                    Existing = existing,
                    Result = jobResult
                };

                job.Schedule(jobResult.Length, 32)
                    .Complete();

                existing.Dispose();
                existing = jobResult;

                if (i != iterations) {
                    jobResult = new NativeArray<int>(keyLength, Allocator.TempJob);
                }
            }

            var terrainMap = jobResult.GetMap(width, height);

            jobResult.Dispose();

            return terrainMap;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    internal struct JGameOfLife : IJobParallelFor {
        public int2 Dimensions;
        public int BirthLimit;
        public int DeathLimit;
        public bool Invert;

        [ReadOnly]
        public NativeArray<int> Existing;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var x = index % Dimensions.x;
            var y = index / Dimensions.y;

            var neighbors = GetNumberOfNeighbors(x, y);

            if (Existing[index] == MapGeneratorConst.DEFAULT_TILE) {
                if (neighbors < DeathLimit) {
                    Result[index] = 1.GetTile(0, Invert);
                }
                else {
                    Result[index] = .1f.GetTile(1, Invert);
                }
            }

            if (Existing[index] == MapGeneratorConst.TERRAIN_TILE) {
                if (neighbors > BirthLimit) {
                    Result[index] = .1f.GetTile(1, Invert);
                }
                else {
                    Result[index] = 1.GetTile(0, Invert);
                }
            }
        }

        private int GetNumberOfNeighbors(int x, int y) {
            var wallCount = 0;

            for (var neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            for (var neighbourX = x - 1; neighbourX <= x + 1; neighbourX++) {
                if (neighbourX >= 0 && neighbourX < Dimensions.x && neighbourY >= 0 &&
                    neighbourY < Dimensions.y) {
                    if (neighbourX != x || neighbourY != y) {
                        var key = neighbourY * Dimensions.x + neighbourX;
                        wallCount += Existing[key];
                    }
                }
                else {
                    wallCount++;
                }
            }

            return wallCount;
        }
    }
}