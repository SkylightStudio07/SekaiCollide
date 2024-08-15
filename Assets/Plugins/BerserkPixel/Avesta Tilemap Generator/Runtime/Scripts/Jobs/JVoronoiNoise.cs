using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class VoronoiNoiseJob : MapGenerationJob<VoronoiSO> {
        public override MapArray GenerateNoiseMap(VoronoiSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);
            var frequency = mapConfig.frequency;
            var seed = mapConfig.seed;
            var amplitude = mapConfig.amplitude;
            var numPoints = mapConfig.numPoints;

            var fillPercent = mapConfig.fillPercent;
            var invert = mapConfig.invert;

            using var jobResult = new NativeArray<int>(dimensions.x * dimensions.y, Allocator.TempJob);

            var points = new NativeArray<Vector2>(dimensions.x * dimensions.y, Allocator.TempJob);

            var pseudoRandom = new Random(seed.GetHashCode());

            var width = dimensions.x;
            var height = dimensions.y;

            for (var i = 0; i < numPoints; i++) {
                points[i] = new Vector2(pseudoRandom.Next(0, width), pseudoRandom.Next(0, height));
            }

            var job = new JVoronoiNoise
            {
                Dimensions = dimensions,
                Frequency = frequency,
                Amplitude = amplitude,
                NumPoints = numPoints,
                Points = points,
                FillPercent = fillPercent,
                Invert = invert,
                Result = jobResult
            };

            job.Schedule(jobResult.Length, 32)
                .Complete();

            var terrainMap = jobResult.GetMap(dimensions.x, dimensions.y);

            points.Dispose();

            return terrainMap;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    internal struct JVoronoiNoise : IJobParallelFor {
        public int2 Dimensions;
        public float Frequency;
        public float Amplitude;
        public float NumPoints;

        public float FillPercent;
        public bool Invert;

        [ReadOnly]
        public NativeArray<Vector2> Points;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var x = index % Dimensions.x;
            var y = index / Dimensions.y;

            var point = new Vector2(x, y);
            var minDist = float.MaxValue;

            // find nearest point
            for (var i = 0; i < NumPoints; i++) {
                var dist = Vector2.Distance(point, Points[i]);
                if (dist < minDist) {
                    minDist = dist;
                }
            }

            // apply amplitude and frequency to distance
            var noiseValue = minDist * Frequency * Amplitude;

            // clamp noise value between 0 and 1
            noiseValue = math.clamp(noiseValue, 0f, 1f);

            Result[index] = noiseValue.GetTile(1 - FillPercent, Invert);
        }
    }
}