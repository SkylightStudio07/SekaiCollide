using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator {
    public static class MapTypeExt {
        public static MapType GetFromSO(this MapConfigSO mapConfig) {
            var selectedType = mapConfig.GetType();

            if (selectedType == typeof(CellularMapConfigSO)) {
                return MapType.CellularAutomata;
            }

            if (selectedType == typeof(PerlinNoiseMapConfigSO)) {
                return MapType.PerlinNoise;
            }

            if (selectedType == typeof(GameOfLifeMapConfigSO)) {
                return MapType.GameOfLife;
            }

            if (selectedType == typeof(BasicRandomSO)) {
                return MapType.BasicRandom;
            }

            if (selectedType == typeof(RandomWalkSO)) {
                return MapType.RandomWalk;
            }

            if (selectedType == typeof(PathSO)) {
                return MapType.Path;
            }

            if (selectedType == typeof(IslandConfigSO)) {
                return MapType.Island;
            }

            if (selectedType == typeof(FractalNoiseConfigSO)) {
                return MapType.FractalNoise;
            }

            if (selectedType == typeof(DomainWarpingSO)) {
                return MapType.DomainWarping;
            }

            if (selectedType == typeof(RidgedMultifractalSO)) {
                return MapType.RidgedMultifractal;
            }

            if (selectedType == typeof(WaveletNoiseSO)) {
                return MapType.WaveletNoise;
            }

            if (selectedType == typeof(FourierNoiseSO)) {
                return MapType.FourierNoise;
            }

            if (selectedType == typeof(VoronoiSO)) {
                return MapType.Voronoi;
            }

            if (selectedType == typeof(SparseConvolutionSO)) {
                return MapType.SparseConvolution;
            }

            // by default it's a full grid type
            return MapType.FullGrid;
        }
    }
}