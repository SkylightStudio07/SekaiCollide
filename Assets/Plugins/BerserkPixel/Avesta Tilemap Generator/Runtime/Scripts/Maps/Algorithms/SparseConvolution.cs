using BerserkPixel.Tilemap_Generator.Jobs;
using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Algorithms {
    public class SparseConvolution : IMapAlgorithm {
        private readonly SparseConvolutionJob _job;
        private readonly SparseConvolutionSO _mapConfig;

        public SparseConvolution(MapConfigSO mapConfig) {
            _mapConfig = mapConfig as SparseConvolutionSO;
            _job = new SparseConvolutionJob();
        }

        public MapArray RandomFillMap() {
            return _job.GenerateNoiseMap(_mapConfig);
        }

        public string GetMapName() {
            return $"SparseConvolution_GeneratedMap_[{_mapConfig.seed}][{_mapConfig.kernelSize}]";
        }
    }
}