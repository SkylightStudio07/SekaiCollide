using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class FullGrid : IMapAlgorithm {
        private readonly FullGridJob _job;
        private readonly FullGridSO _mapConfig;

        public FullGrid(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as FullGridSO;
            _job = new FullGridJob();
        }

        public string GetMapName() {
            return $"Full Grid_[{_mapConfig.width}x{_mapConfig.height}]";
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }
    }
}