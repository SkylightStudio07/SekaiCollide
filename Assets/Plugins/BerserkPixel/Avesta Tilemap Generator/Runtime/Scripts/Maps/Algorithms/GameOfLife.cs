using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class GameOfLife : IMapAlgorithm {
        private readonly GameOfLifeJob _job;
        private readonly GameOfLifeMapConfigSO _mapConfig;

        public GameOfLife(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as GameOfLifeMapConfigSO;
            _job = new GameOfLifeJob();
        }

        public string GetMapName() {
            return $"GoL_GeneratedMap_[{_mapConfig.numR}]";
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }
    }
}