using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class RidgedMultifractal : IMapAlgorithm {
        private readonly RidgedMultifractalJob _job;
        private readonly RidgedMultifractalSO _mapConfig;

        public RidgedMultifractal(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as RidgedMultifractalSO;
            _job = new RidgedMultifractalJob();
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }

        public string GetMapName() {
            return $"RidgedMultifractal_GeneratedMap_[{_mapConfig.fillPercent}][{_mapConfig.octaves}]";
        }
    }
}