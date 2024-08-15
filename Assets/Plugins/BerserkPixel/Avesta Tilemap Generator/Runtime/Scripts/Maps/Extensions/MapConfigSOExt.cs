using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace BerserkPixel.Tilemap_Generator.SO {
    public static class MapConfigSOExt {
        private static readonly Type[] notRandomMaps =
        {
            typeof(FullGridSO)
        };

        public static bool ShouldBeRandom(this MapConfigSO mapConfig) {
            return !notRandomMaps.Contains(mapConfig.GetType());
        }

        public static void SetRandomSeed(this MapConfigSO mapConfig) {
            var random = (float) (Time.time / new Random().NextDouble());

            switch (mapConfig) {
                case CellularMapConfigSO cellularMap:
                    cellularMap.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case PerlinNoiseMapConfigSO perlinMap:
                    perlinMap.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case BasicRandomSO basicMap:
                    basicMap.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case RandomWalkSO randomMap:
                    randomMap.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case PathSO pathMap:
                    pathMap.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case IslandConfigSO islandMap:
                    islandMap.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case FractalNoiseConfigSO fractalMap:
                    fractalMap.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case GameOfLifeMapConfigSO gameMap:
                    gameMap.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case RidgedMultifractalSO ridged:
                    ridged.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case FourierNoiseSO fourier:
                    fourier.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case DomainWarpingSO domainWarping:
                    domainWarping.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case VoronoiSO voronoi:
                    voronoi.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case WaveletNoiseSO wavelet:
                    wavelet.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
                case SparseConvolutionSO sparse:
                    sparse.seed = random.ToString(CultureInfo.CurrentCulture);
                    break;
            }
        }
    }
}