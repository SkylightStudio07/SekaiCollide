using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class FractalNoise : IMapAlgorithm {
        private readonly FractalNoiseJob _job;
        private readonly FractalNoiseConfigSO _mapConfig;

        public FractalNoise(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as FractalNoiseConfigSO;
            _job = new FractalNoiseJob();
        }

        public string GetMapName() {
            return $"Fractal_GeneratedMap_[{_mapConfig.seed}]";
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }
    }
}