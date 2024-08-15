using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class RidgedMultifractalJob : MapGenerationJob<RidgedMultifractalSO> {
        public override MapArray GenerateNoiseMap(RidgedMultifractalSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);
            var seed = mapConfig.seed;
            var iterations = mapConfig.octaves;
            var fillPercent = mapConfig.fillPercent;
            var persistence = mapConfig.persistence;
            var lacunarity = mapConfig.lacunarity;
            var threshold = mapConfig.threshold;
            var invert = mapConfig.invert;

            var width = dimensions.x;
            var height = dimensions.y;

            var keyLength = width * height;

            using var jobResult = new NativeArray<int>(keyLength, Allocator.TempJob);

            var pseudoRandom = new Random(seed.GetHashCode());

            float offsetX = pseudoRandom.Next(-100000, 100000);
            float offsetY = pseudoRandom.Next(-100000, 100000);

            var job = new JRidgedMultifractal
            {
                Dimensions = dimensions,
                Octaves = iterations,
                Persistance = persistence,
                Lacunarity = lacunarity,
                Threshold = threshold,
                FillPercent = fillPercent,
                Invert = invert,
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
    internal struct JRidgedMultifractal : IJobParallelFor {
        public int2 Dimensions;
        public float Octaves;
        public float Persistance;
        public float Lacunarity;
        public float Threshold;
        public float FillPercent;
        public bool Invert;
        public float OffsetX;
        public float OffsetY;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var x = index % Dimensions.x;
            var y = index / Dimensions.y;

            var frequency = 1.0f;
            var amplitude = 1.0f;
            var noiseValue = 0.0f;

            for (var o = 0; o < Octaves; o++) {
                var sampleX = (float) x / Dimensions.x * frequency + OffsetX;
                var sampleY = (float) y / Dimensions.y * frequency + OffsetY;

                var a = new float2(sampleX, sampleY);
                var noiseSample = noise.snoise(a);

                var ridgedValue = math.abs(noiseSample * 2 - 1);
                ridgedValue = 1 - ridgedValue; // Invert the ridges

                noiseValue += ridgedValue * amplitude;
                amplitude *= Persistance;
                frequency *= Lacunarity;
            }

            noiseValue = (noiseValue - Threshold) * 2; // Amplify the noise

            Result[index] = noiseValue.GetTile(1 - FillPercent, Invert);
        }
    }
}