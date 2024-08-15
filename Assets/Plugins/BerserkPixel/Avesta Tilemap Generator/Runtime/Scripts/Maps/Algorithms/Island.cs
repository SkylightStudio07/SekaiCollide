using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class Island : IMapAlgorithm {
        private readonly IslandJob _job;
        private readonly IslandConfigSO _mapConfig;

        public Island(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as IslandConfigSO;
            _job = new IslandJob();
        }

        public string GetMapName() {
            return $"Island_GeneratedMap_[{_mapConfig.seed}]";
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }
    }
}