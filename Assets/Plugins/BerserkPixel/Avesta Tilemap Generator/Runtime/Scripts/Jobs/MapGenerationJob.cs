using BerserkPixel.Tilemap_Generator.SO;

namespace BerserkPixel.Tilemap_Generator.Jobs {
    public abstract class MapGenerationJob<T> where T : MapConfigSO {
        public abstract MapArray GenerateNoiseMap(T mapConfig);
    }
}