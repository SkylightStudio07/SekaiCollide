using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class BasicRandomJob : MapGenerationJob<BasicRandomSO> {
        public override MapArray GenerateNoiseMap(BasicRandomSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);

            var fillPercent = mapConfig.fillPercent;
            var seed = mapConfig.seed;
            var invert = mapConfig.invert;

            using var jobResult = new NativeArray<int>(dimensions.x * dimensions.y, Allocator.TempJob);
            using var chances = new NativeArray<float>(dimensions.x * dimensions.y, Allocator.TempJob);

            var pseudoRandom = new Random(seed.GetHashCode());

            for (var i = 0; i < jobResult.Length; i++) {
                var nativeChances = chances;
                nativeChances[i] = pseudoRandom.Next(0, 101);
            }

            var job = new JBasicRandom
            {
                FillPercent = fillPercent,
                Invert = invert,
                Chances = chances,
                Result = jobResult
            };

            job.Schedule(jobResult.Length, 64).Complete();

            var terrainMap = jobResult.GetMap(dimensions.x, dimensions.y);

            return terrainMap;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    internal struct JBasicRandom : IJobParallelFor {
        public float FillPercent;
        public bool Invert;

        [ReadOnly]
        public NativeArray<float> Chances;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            Result[index] = Chances[index].GetTile(100 - FillPercent, Invert);
        }
    }
}