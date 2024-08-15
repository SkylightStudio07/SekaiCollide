using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class VoronoiNoise : IMapAlgorithm {
        private readonly VoronoiNoiseJob _job;
        private readonly VoronoiSO _mapConfig;

        public VoronoiNoise(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as VoronoiSO;
            _job = new VoronoiNoiseJob();
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }

        public string GetMapName() {
            return $"VoronoiNoise_GeneratedMap_[{_mapConfig.fillPercent}][{_mapConfig.frequency}]";
        }
    }
}