using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class BasicRandom : IMapAlgorithm {
        private readonly BasicRandomJob _job;
        private readonly BasicRandomSO _mapConfig;

        public BasicRandom(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as BasicRandomSO;
            _job = new BasicRandomJob();
        }

        public string GetMapName() {
            return $"Random_GeneratedMap_[{_mapConfig.seed}][{_mapConfig.fillPercent}]";
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }
    }
}