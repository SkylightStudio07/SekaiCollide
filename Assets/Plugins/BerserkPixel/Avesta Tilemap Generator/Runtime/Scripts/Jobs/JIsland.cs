using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class IslandJob : MapGenerationJob<IslandConfigSO> {
        public override MapArray GenerateNoiseMap(IslandConfigSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);
            var fillPercent = mapConfig.fillPercent;
            var seed = mapConfig.seed;
            var distanceAlgorithm = mapConfig.distanceAlgorithm;
            var invert = mapConfig.invert;
            var scale = mapConfig.scale;
            var numOctaves = mapConfig.numOctaves;

            var width = dimensions.x;
            var height = dimensions.y;

            using var jobResult = new NativeArray<int>(width * height, Allocator.TempJob);

            var pseudoRandom = new Random(seed.GetHashCode());

            float offsetX = pseudoRandom.Next(-100000, 100000);
            float offsetY = pseudoRandom.Next(-100000, 100000);

            var job = new JIsland
            {
                Dimensions = dimensions,
                FillPercent = fillPercent,
                DistanceAlgorithm = distanceAlgorithm,
                Invert = invert,
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
    internal struct JIsland : IJobParallelFor {
        public int2 Dimensions;
        public float FillPercent;
        public IslandDistance DistanceAlgorithm;
        public bool Invert;
        public float Scale;
        public int Octaves;
        public float OffsetX;
        public float OffsetY;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var x = index % Dimensions.x;
            var y = index / Dimensions.y;

            var (noise, distance) = GetPerlinNoise(x, y);

            var isPartOfIsland = noise > .3f + (1f - FillPercent) * distance;

            var terrainTile = Invert ? MapGeneratorConst.DEFAULT_TILE : MapGeneratorConst.TERRAIN_TILE;
            var defaultTile = Invert ? MapGeneratorConst.TERRAIN_TILE : MapGeneratorConst.DEFAULT_TILE;

            var tile = isPartOfIsland ? terrainTile : defaultTile;

            Result[index] = tile;
        }

        private (float, float) GetPerlinNoise(float x, float y) {
            var halfWidth = Dimensions.x / 2;
            var halfHeight = Dimensions.y / 2;

            var xCoord = (x - halfWidth) / Dimensions.x / Scale + OffsetX;
            var yCoord = (y - halfHeight) / Dimensions.y / Scale + OffsetY;

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

            var d = DistanceToCenter(x, y, ref noise);

            return (noise, d);
        }

        private float DistanceToCenter(float x, float y, ref float noise) {
            var width = Dimensions.x;
            var height = Dimensions.y;

            var dx = 2 * x / width - 1;
            var dy = 2 * y / height - 1;

            if (DistanceAlgorithm == IslandDistance.Eucledian)
                // d = min(1, (nx² + ny²) / sqrt(2))
            {
                return math.min(1, (dx * dx + dy * dy) / math.SQRT2);
            }

            if (DistanceAlgorithm == IslandDistance.SquareBumb)
                // d = 1 - (1-nx²) * (1-ny²)
            {
                return 1 - (1 - dx * dx) * (1 - dy * dy);
            }

            if (DistanceAlgorithm == IslandDistance.Basic) {
                var halfWidth = width / 2f;
                var halfHeight = height / 2f;

                // Simple squaring, you can use whatever math libraries are available to you to make this more readable
                var distanceX = (halfWidth - x) * (halfWidth - x);
                var distanceY = (halfHeight - y) * (halfHeight - y);

                var distanceToCenter = math.sqrt(distanceX + distanceY);

                // Make sure this value ends up as a float and not an integer
                // If you're not outputting this to an image, get the correct 1.0 white on the furthest edges by dividing by half the map size, in this case 64. You will get higher than 1.0 values, so clamp them!
                distanceToCenter /= math.max(halfWidth, halfHeight);
                return distanceToCenter;
            }

            var xv = x / width * 2 - 1;
            var yv = y / height * 2 - 1;
            var v = math.max(math.abs(xv), math.abs(yv));

            var tripleV = v * v * v;
            var islandSizeFactor = FillPercent * 10;
            var tripleFallFactor = (islandSizeFactor - islandSizeFactor * v) *
                                   (islandSizeFactor - islandSizeFactor * v) *
                                   (islandSizeFactor - islandSizeFactor * v);

            var result = tripleV / (tripleV + tripleFallFactor);

            noise -= tripleV / (tripleV + tripleFallFactor);

            return result;
        }
    }
}