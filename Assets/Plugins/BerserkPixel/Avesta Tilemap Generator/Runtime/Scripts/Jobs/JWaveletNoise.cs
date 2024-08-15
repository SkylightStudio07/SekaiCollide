using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class WaveletNoiseJob : MapGenerationJob<WaveletNoiseSO> {
        public override MapArray GenerateNoiseMap(WaveletNoiseSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);
            var seed = mapConfig.seed;
            var levels = mapConfig.levels;
            var persistence = mapConfig.persistence;
            var fillPercent = mapConfig.fillPercent;
            var invert = mapConfig.invert;

            var width = dimensions.x;
            var height = dimensions.y;

            var keyLength = width * height;

            using var jobResult = new NativeArray<int>(keyLength, Allocator.TempJob);

            var pseudoRandom = new Random(seed.GetHashCode());

            float offsetX = pseudoRandom.Next(-100000, 100000);
            float offsetY = pseudoRandom.Next(-100000, 100000);

            var job = new JWaveletNoise
            {
                Dimensions = dimensions,
                Levels = levels,
                Persistence = persistence,
                OffsetX = offsetX,
                OffsetY = offsetY,
                FillPercentage = fillPercent,
                Invert = invert,
                Result = jobResult
            };

            job.Schedule(jobResult.Length, 32)
                .Complete();

            var terrainMap = jobResult.GetMap(width, height);

            return terrainMap;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    internal struct JWaveletNoise : IJobParallelFor {
        public int2 Dimensions;
        public int Levels;
        public float Persistence;
        public float OffsetX;
        public float OffsetY;

        public float FillPercentage;
        public bool Invert;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var x = index % Dimensions.x;
            var y = index / Dimensions.y;

            var noiseValue = 0.0f;

            for (var level = 1; level <= Levels; level++) {
                var sampleX = (float) x / Dimensions.x * (1 << level) + OffsetX;
                var sampleY = (float) y / Dimensions.y * (1 << level) + OffsetY;

                var weight = 1.0f / math.pow(2.0f, level * Persistence);

                var a = new float2(sampleX, sampleY);
                var noise = Unity.Mathematics.noise.snoise(a);

                noiseValue += weight * noise;
            }

            Result[index] = noiseValue.GetTile(1 - FillPercentage, Invert);
        }
    }
}