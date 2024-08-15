namespace BerserkPixel.Tilemap_Generator {
    public static class AlgorithmExt {
        private static int InvertTile(this int tile) {
            if (tile == MapGeneratorConst.DEFAULT_TILE) {
                return MapGeneratorConst.TERRAIN_TILE;
            }

            if (tile == MapGeneratorConst.TERRAIN_TILE) {
                return MapGeneratorConst.DEFAULT_TILE;
            }

            return MapGeneratorConst.DEFAULT_TILE;
        }

        public static int GetTile(this float noise, float percentage, bool invert = false) {
            var tile = noise > percentage ? MapGeneratorConst.TERRAIN_TILE : MapGeneratorConst.DEFAULT_TILE;

            return invert ? tile.InvertTile() : tile;
        }

        public static int GetTile(this int noise, float percentage, bool invert = false) {
            var tile = noise > percentage ? MapGeneratorConst.TERRAIN_TILE : MapGeneratorConst.DEFAULT_TILE;

            return invert ? tile.InvertTile() : tile;
        }
    }
}