using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Perlin")]
    public class PerlinNoiseMapConfigSO : MapConfigSO, IEquatable<PerlinNoiseMapConfigSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float fillPercent = .5f;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        [DelayedCallback(nameof(MapChange))]
        public bool isIsland = true;

        [DelayedCallback(nameof(MapChange))]
        public float islandSizeFactor = 4.2f;

        [DelayedCallback(nameof(MapChange))]
        public float scale = 20f;

        public bool Equals(PerlinNoiseMapConfigSO other) {
            return other != null &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   invert == other.invert &&
                   isIsland == other.isIsland &&
                   Math.Abs(islandSizeFactor - other.islandSizeFactor) < _compareThreshold &&
                   Math.Abs(scale - other.scale) < _compareThreshold;
        }

        public override MapFactory GetFactory() {
            return new PerlinNoiseFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as PerlinNoiseMapConfigSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + seed.GetHashCode();
                hash = hash * 16777619 + fillPercent.GetHashCode();
                hash = hash * 16777619 + invert.GetHashCode();
                hash = hash * 16777619 + isIsland.GetHashCode();
                hash = hash * 16777619 + islandSizeFactor.GetHashCode();
                hash = hash * 16777619 + scale.GetHashCode();
                return hash;
            }
        }
    }
}