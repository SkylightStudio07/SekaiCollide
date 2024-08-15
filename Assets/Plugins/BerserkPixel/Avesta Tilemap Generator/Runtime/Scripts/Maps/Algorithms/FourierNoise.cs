using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class FourierNoise : IMapAlgorithm {
        private readonly FourierNoiseJob _job;
        private readonly FourierNoiseSO _mapConfig;

        public FourierNoise(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as FourierNoiseSO;
            _job = new FourierNoiseJob();
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }

        public string GetMapName() {
            return $"FourierNoise_GeneratedMap_[{_mapConfig.fillPercent}][{_mapConfig.frequency}]";
        }
    }
}