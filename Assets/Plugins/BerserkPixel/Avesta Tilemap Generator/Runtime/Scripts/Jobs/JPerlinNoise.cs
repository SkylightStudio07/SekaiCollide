using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class PerlinNoiseJob : MapGenerationJob<PerlinNoiseMapConfigSO> {
        public override MapArray GenerateNoiseMap(PerlinNoiseMapConfigSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);

            var fillPercent = mapConfig.fillPercent;
            var scale = mapConfig.scale;
            var isIsland = mapConfig.isIsland;
            var islandSizeFactor = mapConfig.islandSizeFactor;
            var seed = mapConfig.seed;
            var invert = mapConfig.invert;

            var width = dimensions.x;
            var height = dimensions.y;

            var keyLength = width * height;

            using var jobResult = new NativeArray<int>(keyLength, Allocator.TempJob);

            var pseudoRandom = new Random(seed.GetHashCode());

            float offsetX = pseudoRandom.Next(-100000, 100000);
            float offsetY = pseudoRandom.Next(-100000, 100000);

            var job = new JPerlinNoise
            {
                Dimensions = dimensions,
                FillPercent = fillPercent,
                Invert = invert,
                Scale = scale,
                IsIsland = isIsland,
                IslandSizeFactor = islandSizeFactor,
                OffsetX = offsetX,
                OffsetY = offsetY,
                Result = jobResult
            };

            job.Schedule(jobResult.Length, 32)
                .Complete();

            var terrainMap = jobResult.GetMap(width, height);

            return terrainMap;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    internal struct JPerlinNoise : IJobParallelFor {
        public int2 Dimensions;
        public float FillPercent;
        public bool Invert;
        public float Scale;
        public bool IsIsland;
        public float IslandSizeFactor;
        public float OffsetX;
        public float OffsetY;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var x = index % Dimensions.x;
            var y = index / Dimensions.y;

            var noise = GetPerlinNoise(x, y);

            Result[index] = noise.GetTile(1 - FillPercent, Invert);
        }

        private float GetPerlinNoise(float x, float y) {
            var halfWidth = Dimensions.x / 2;
            var halfHeight = Dimensions.y / 2;

            var xCoord = (x - halfWidth) / Dimensions.x * Scale + OffsetX;
            var yCoord = (y - halfHeight) / Dimensions.y * Scale + OffsetY;

            var a = new float2(xCoord, yCoord);
            var noiseValue = noise.snoise(a);

            if (!IsIsland) {
                return noiseValue;
            }

            var xv = x / Dimensions.x * 2 - 1;
            var yv = y / Dimensions.y * 2 - 1;
            var v = math.max(math.abs(xv), math.abs(yv));

            var tripleV = v * v * v;
            var tripleFallFactor = (IslandSizeFactor - IslandSizeFactor * v) *
                                   (IslandSizeFactor - IslandSizeFactor * v) *
                                   (IslandSizeFactor - IslandSizeFactor * v);

            noiseValue -= tripleV / (tripleV + tripleFallFactor);

            return noiseValue;
        }
    }
}