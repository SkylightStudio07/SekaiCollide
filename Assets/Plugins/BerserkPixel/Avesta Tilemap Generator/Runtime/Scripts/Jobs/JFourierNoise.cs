using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class FourierNoiseJob : MapGenerationJob<FourierNoiseSO> {
        public override MapArray GenerateNoiseMap(FourierNoiseSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);
            var seed = mapConfig.seed;
            var frequency = mapConfig.frequency;
            var amplitude = mapConfig.amplitude;
            var scale = mapConfig.scale;
            var octaves = mapConfig.octaves;
            var lacunarity = mapConfig.lacunarity;
            var persistence = mapConfig.persistence;

            var fillPercent = mapConfig.fillPercent;
            var invert = mapConfig.invert;

            using var jobResult = new NativeArray<int>(dimensions.x * dimensions.y, Allocator.TempJob);

            var pseudoRandom = new Random(seed.GetHashCode());

            float offsetX = pseudoRandom.Next(-100000, 100000);
            float offsetY = pseudoRandom.Next(-100000, 100000);

            var job = new JFourierNoise
            {
                Dimensions = dimensions,
                Frequency = frequency,
                Amplitude = amplitude,
                Scale = scale,
                Octaves = octaves,
                Lacunarity = lacunarity,
                Persistence = persistence,
                OffsetX = offsetX,
                OffsetY = offsetY,
                FillPercent = fillPercent,
                Invert = invert,
                Result = jobResult
            };

            job.Schedule(jobResult.Length, 32)
                .Complete();

            var terrainMap = jobResult.GetMap(dimensions.x, dimensions.y);

            return terrainMap;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    internal struct JFourierNoise : IJobParallelFor {
        public int2 Dimensions;
        public float Frequency;
        public float Amplitude;
        public float Scale;
        public int Octaves;
        public float Lacunarity;
        public float Persistence;
        public float OffsetX;
        public float OffsetY;

        public float FillPercent;
        public bool Invert;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var x = index % Dimensions.x;
            var y = index / Dimensions.y;

            var sampleX = x / Scale * Frequency + OffsetX;
            var sampleY = y / Scale * Frequency + OffsetY;

            var noiseValue = 0f;
            var amplitudeSum = 0f;
            var frequencySum = 1f;

            for (var i = 0; i < Octaves; i++) {
                var a = new float2(sampleX * frequencySum, sampleY * frequencySum);
                var perlinValue = Unity.Mathematics.noise.snoise(a);

                noiseValue += perlinValue * amplitudeSum;

                amplitudeSum += Amplitude;
                frequencySum *= Lacunarity;
            }

            var noise = noiseValue * Persistence;
            Result[index] = noise.GetTile(1 - FillPercent, Invert);
        }
    }
}