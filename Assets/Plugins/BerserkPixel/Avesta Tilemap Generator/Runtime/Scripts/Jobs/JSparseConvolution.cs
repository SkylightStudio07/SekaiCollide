using BerserkPixel.Tilemap_Generator.SO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public class SparseConvolutionJob : MapGenerationJob<SparseConvolutionSO> {
        public override MapArray GenerateNoiseMap(SparseConvolutionSO mapConfig) {
            var dimensions = new int2(mapConfig.width, mapConfig.height);

            var seed = mapConfig.seed;
            var kernelSize = mapConfig.kernelSize;

            var mapSize = dimensions.x * dimensions.y;
            const int batchCount = 64;

            using var points = new NativeArray<SparsePoint>(mapConfig.numberOfPoints, Allocator.TempJob);

            var pseudoRandom = new Random((uint) seed.GetHashCode());

            var pointsJob = new JSparsePoints
            {
                Random = pseudoRandom,
                Dimensions = dimensions,
                Points = points
            };

            pointsJob.Schedule(points.Length, batchCount).Complete();

            using var kernels = new NativeArray<float>(kernelSize * kernelSize, Allocator.TempJob);

            var kernelJob = new JSparseKernels
            {
                KernelSize = mapConfig.kernelSize,
                Sigma = mapConfig.sigma,
                Type = mapConfig.type,
                Kernels = kernels
            };

            kernelJob.Schedule(kernels.Length, batchCount).Complete();

            using var jobResult = new NativeArray<int>(mapSize, Allocator.TempJob);

            var job = new JSparseConvolution
            {
                Dimensions = dimensions,
                Kernels = kernels,
                FillPercent = mapConfig.fillPercent,
                Invert = mapConfig.invert,
                Points = points,
                Result = jobResult
            };

            job.Schedule(jobResult.Length, batchCount)
                .Complete();

            var terrainMap = jobResult.GetMap(dimensions.x, dimensions.y);

            return terrainMap;
        }
    }

    public struct SparsePoint {
        public int2 Position;
        public float Value; // Optional: You can store additional data if needed.
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct JSparsePoints : IJobParallelFor {
        public Random Random;
        public int2 Dimensions;

        [WriteOnly]
        public NativeArray<SparsePoint> Points;

        public void Execute(int index) {
            var x = Random.NextInt(Dimensions.x + 1);
            var y = Random.NextInt(Dimensions.y + 1);

            var sparsePoint = new SparsePoint
            {
                Position = new int2(x, y),
                Value = Random.NextFloat(0f, 1f) // Optional: You can use different data for each sparse point.
            };

            Points[index] = sparsePoint;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct JSparseKernels : IJobParallelFor {
        public int KernelSize;
        public float Sigma;
        public SparseType Type;

        [WriteOnly]
        public NativeArray<float> Kernels;

        public void Execute(int index) {
            var x = index % KernelSize;
            var y = index / KernelSize;

            // Ensure the kernel size is odd.
            if (KernelSize % 2 == 0) {
                KernelSize++;
            }

            var halfSize = KernelSize / 2;

            x -= halfSize;
            y -= halfSize;

            switch (Type) {
                case SparseType.Gaussian:
                    DoGaussian(index, x, y);
                    break;
                case SparseType.Radial:
                    DoRadial(index, x, y);
                    break;
                case SparseType.Circles:
                    DoCircles(index, x, y);
                    break;
                case SparseType.StarShape:
                    DoStar(index, x, y);
                    break;
                case SparseType.Spiral:
                    DoSpiral(index, x, y);
                    break;
                case SparseType.Hexagonal:
                    DoHexagon(index, x, y);
                    break;
                case SparseType.Waves:
                    DoWaves(index, x, y);
                    break;
            }
        }

        private void DoGaussian(int index, int x, int y) {
            var twoSigmaSquared = 2.0f * Sigma * Sigma;
            var exponent = -(x * x + y * y) / twoSigmaSquared;
            Kernels[index] = math.exp(exponent) / (math.PI * twoSigmaSquared);
        }

        private void DoRadial(int index, int x, int y) {
            var halfSize = KernelSize / 2;
            var distance = math.sqrt(x * x + y * y);
            var weight = math.clamp(1.0f - distance / halfSize, 0f, 1f);
            Kernels[index] = weight;
        }

        private void DoCircles(int index, int x, int y) {
            var halfSize = KernelSize / 2;
            var distance = math.sqrt(x * x + y * y);
            var weight = distance <= halfSize ? 1.0f : 0.0f;
            Kernels[index] = weight;
        }

        private void DoStar(int index, int x, int y) {
            var halfSize = KernelSize / 2;
            // Calculate the star-shaped pattern using a combination of X and Y distances.
            var weight = 1.0f - (float) math.abs(x) / halfSize - (float) math.abs(y) / halfSize;
            Kernels[index] = math.clamp(weight, 0f, 1f);
        }

        private void DoSpiral(int index, int x, int y) {
            var halfSize = KernelSize / 2;
            var radius = math.sqrt(x * x + y * y);
            var angle = math.atan2(y, x);
            // Adjust the weight based on radius and angle.
            var weight = math.clamp(1.0f - radius / halfSize, 0f, 1f) * math.abs(math.cos(angle));
            Kernels[index] = weight;

            // diamond pattern KEEP but similar to above (spiral)
            // float weight = 1.0f - (math.abs(x) + math.abs(y)) / (2.0f * halfSize);
            // Kernels[index] = math.clamp(weight, 0f, 1f);
        }

        private void DoHexagon(int index, int x, int y) {
            var halfSize = KernelSize / 2;
            var sideLength = halfSize * 0.75f;
            float horizontalDistance = math.abs(x);
            float verticalDistance = math.abs(y);
            var weight = 0.0f;

            if (horizontalDistance <= sideLength && verticalDistance <= halfSize) {
                if (math.abs(horizontalDistance - verticalDistance / 2.0f) <= sideLength / 2.0f) {
                    weight = 1.0f;
                }
            }

            Kernels[index] = weight;
        }

        private void DoWaves(int index, int x, int y) {
            var weight = math.sin(x * math.PI * Sigma / KernelSize) * math.sin(y * math.PI * Sigma / KernelSize);
            Kernels[index] = weight;
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct JSparseConvolution : IJobParallelFor {
        public int2 Dimensions;
        public float FillPercent;
        public bool Invert;

        [ReadOnly]
        public NativeArray<SparsePoint> Points;

        [ReadOnly]
        public NativeArray<float> Kernels;

        [WriteOnly]
        public NativeArray<int> Result;

        public void Execute(int index) {
            var width = Dimensions.x;
            var x = index % width;
            var y = index / width;

            var position = new int2(x, y);

            // Initialize noise value.
            var noiseValue = 0f;

            // Loop through sparse points and apply kernel.
            foreach (var point in Points) {
                var offset = position - point.Position;

                var kernelIndex = offset.y * width + offset.x;

                if (kernelIndex >= 0 && kernelIndex < Kernels.Length) {
                    noiseValue += Kernels[kernelIndex] / point.Value;
                }
            }

            Result[index] = noiseValue.GetTile(1 - FillPercent, Invert);
        }
    }
}