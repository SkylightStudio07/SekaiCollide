using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class DomainWarpingJob : MapGenerationJob<DomainWarpingSO> {
        public override MapArray GenerateNoiseMap(DomainWarpingSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);
            var seed = mapConfig.seed;
            var warpAmount = mapConfig.warpAmount;
            var scale = mapConfig.scale;
            var sampleScale = mapConfig.sampleScale;
            var fillPercent = mapConfig.fillPercent;
            var invert = mapConfig.invert;

            var pseudoRandom = new Random(seed.GetHashCode());

            float offsetX = pseudoRandom.Next(-100000, 100000);
            float offsetY = pseudoRandom.Next(-100000, 100000);

            using var jobResult = new NativeArray<int>(dimensions.x * dimensions.y, Allocator.TempJob);

            var job = new JDomainWarping
            {
                Dimensions = dimensions,
                Scale = scale,
                WarpAmount = warpAmount,
                OffsetX = offsetX,
                OffsetY = offsetY,
                SampleScale = sampleScale,
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
    internal struct JDomainWarping : IJobParallelFor {
        public int2 Dimensions;
        public float WarpAmount;
        public float Scale;
        public float OffsetX;
        public float OffsetY;
        public float SampleScale;
        public float FillPercent;
        public bool Invert;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var x = index % Dimensions.x;
            var y = index / Dimensions.y;

            var xCoord = (float) x / Dimensions.x * Scale;
            var yCoord = (float) y / Dimensions.y * Scale;

            var a = new float2(xCoord, yCoord);
            var noiseA = noise.snoise(a);

            var warpedX = xCoord + noiseA * WarpAmount;

            a = new float2(xCoord + OffsetX, yCoord + OffsetY);
            noiseA = noise.snoise(a);

            var warpedY = yCoord + noiseA * WarpAmount;

            a = new float2(warpedX, warpedY);
            noiseA = noise.snoise(a);
            var sample = noiseA;

            Result[index] = (sample * SampleScale).GetTile(1 - FillPercent, Invert);
        }
    }
}