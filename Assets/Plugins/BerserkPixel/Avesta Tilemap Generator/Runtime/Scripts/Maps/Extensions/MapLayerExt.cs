using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator {
    public static class MapLayerExt {
        private static MapArray _cachedMapArray;

        public static MapArray GetTotalMap(this List<MapLayer> allActiveLayers) {
            var maxWidth = allActiveLayers.Max(layer => layer.Width);
            var maxHeight = allActiveLayers.Max(layer => layer.Height);

            _cachedMapArray = allActiveLayers.Aggregate(_cachedMapArray,
                (current, layer) => layer.IsAdditive
                    ? GetAdditiveLayer(current, layer.GetMapData(), maxWidth, maxHeight)
                    : layer.GetMapData());

            return _cachedMapArray;
        }

        private static MapArray GetAdditiveLayer(MapArray currentLayer, MapArray activeLayer, int maxWidth,
            int maxHeight) {
            var combined = new MapArray(maxWidth, maxHeight);

            for (var y = 0; y < combined.Height; y++)
            for (var x = 0; x < combined.Width; x++) {
                var current = currentLayer[x, y];
                var active = activeLayer[x, y];

                // we are using Mathf.Min since we have this 2 options:
                // MapGeneratorConst.TERRAIN_TILE = 0;
                // MapGeneratorConst.DEFAULT_TILE = 1;
                combined[x, y] = Mathf.Min(current, active);
            }

            return combined;
        }
    }
}