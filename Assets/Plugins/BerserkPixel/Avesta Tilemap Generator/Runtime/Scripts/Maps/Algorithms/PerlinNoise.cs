using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class PerlinNoise : IMapAlgorithm {
        private readonly PerlinNoiseJob _job;
        private readonly PerlinNoiseMapConfigSO _mapConfig;

        public PerlinNoise(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as PerlinNoiseMapConfigSO;
            _job = new PerlinNoiseJob();
        }

        public string GetMapName() {
            return "PN_GeneratedMap_[" + _mapConfig.scale + "][" + _mapConfig.fillPercent + "]";
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }
    }
}