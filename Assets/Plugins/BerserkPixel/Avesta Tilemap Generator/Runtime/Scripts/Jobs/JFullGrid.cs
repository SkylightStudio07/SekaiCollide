using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class FullGridJob : MapGenerationJob<FullGridSO> {
        public override MapArray GenerateNoiseMap(FullGridSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);

            var width = dimensions.x;
            var height = dimensions.y;

            var keyLength = width * height;
            using var jobResult = new NativeArray<int>(keyLength, Allocator.TempJob);

            var job = new JFullGrid
            {
                Result = jobResult
            };

            job.Schedule(jobResult.Length, 32)
                .Complete();

            var terrainMap = jobResult.GetMap(width, height);

            return terrainMap;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    internal struct JFullGrid : IJobParallelFor {
        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            Result[index] = MapGeneratorConst.TERRAIN_TILE;
        }
    }
}