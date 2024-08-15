using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class DomainWarping : IMapAlgorithm {
        private readonly DomainWarpingJob _job;
        private readonly DomainWarpingSO _mapConfig;

        public DomainWarping(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as DomainWarpingSO;
            _job = new DomainWarpingJob();
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }

        public string GetMapName() {
            return $"DomainWarping_GeneratedMap_[{_mapConfig.warpAmount}][{_mapConfig.scale}]";
        }
    }
}