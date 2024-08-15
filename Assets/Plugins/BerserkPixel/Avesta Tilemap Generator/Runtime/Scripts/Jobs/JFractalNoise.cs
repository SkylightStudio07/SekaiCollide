using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class FractalNoiseJob : MapGenerationJob<FractalNoiseConfigSO> {
        public override MapArray GenerateNoiseMap(FractalNoiseConfigSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);
            var fillPercent = mapConfig.fillPercent;
            var numOctaves = mapConfig.numOctaves;
            var frequency = mapConfig.baseFrequency;
            var scale = mapConfig.scale;
            var seed = mapConfig.seed;
            var invert = mapConfig.invert;

            var width = dimensions.x;
            var height = dimensions.y;

            var keyLength = width * height;

            using var jobResult = new NativeArray<int>(keyLength, Allocator.TempJob);

            var pseudoRandom = new Random(seed.GetHashCode());

            float offsetX = pseudoRandom.Next(-100000, 100000);
            float offsetY = pseudoRandom.Next(-100000, 100000);

            var job = new JFractalNoise
            {
                Dimensions = dimensions,
                FillPercent = fillPercent,
                Invert = invert,
                Frequency = frequency,
                Scale = scale,
                Octaves = numOctaves,
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
    internal struct JFractalNoise : IJobParallelFor {
        public int2 Dimensions;
        public float FillPercent;
        public bool Invert;
        public float Frequency;
        public float Scale;
        public int Octaves;
        public float OffsetX;
        public float OffsetY;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var x = index % Dimensions.x;
            var y = index / Dimensions.y;

            var noise = GetPerlinNoise(x, y, OffsetX, OffsetY);

            Result[index] = noise.GetTile(1 - FillPercent, Invert);
        }

        private float GetPerlinNoise(float x, float y, float offsetX, float offsetY) {
            var halfWidth = Dimensions.x / 2;
            var halfHeight = Dimensions.y / 2;

            var xCoord = (x - halfWidth) / Dimensions.x / Scale * Frequency + offsetX;
            var yCoord = (y - halfHeight) / Dimensions.y / Scale * Frequency + offsetY;

            var a = new float2(xCoord, yCoord);
            var noise = Unity.Mathematics.noise.snoise(a);

            // Add additional octaves to the noise
            for (var k = 1; k < Octaves; k++) {
                var pow = math.pow(2, k);

                var b = new float2(
                    xCoord * pow,
                    yCoord * pow
                );

                noise += Unity.Mathematics.noise.snoise(b);
            }

            return noise;
        }
    }
}