using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class CellularAutomata : IMapAlgorithm {
        private readonly CellularAutomataJob _job;
        private readonly CellularMapConfigSO _mapConfig;

        public CellularAutomata(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as CellularMapConfigSO;
            _job = new CellularAutomataJob();
        }

        public string GetMapName() {
            return "CA_GeneratedMap_[" + _mapConfig.seed + "][" + _mapConfig.fillPercent + "]";
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }
    }
}