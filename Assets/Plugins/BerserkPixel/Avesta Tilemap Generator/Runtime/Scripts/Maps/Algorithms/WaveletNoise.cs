using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class WaveletNoise : IMapAlgorithm {
        private readonly WaveletNoiseJob _job;
        private readonly WaveletNoiseSO _mapConfig;

        public WaveletNoise(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as WaveletNoiseSO;
            _job = new WaveletNoiseJob();
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }

        public string GetMapName() {
            return $"WaveletNoise_GeneratedMap_[{_mapConfig.levels}][{_mapConfig.persistence}]";
        }
    }
}