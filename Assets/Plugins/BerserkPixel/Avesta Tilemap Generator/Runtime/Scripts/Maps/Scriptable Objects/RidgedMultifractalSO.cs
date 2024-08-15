using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Ridged Multifractal")]
    public class RidgedMultifractalSO : MapConfigSO, IEquatable<RidgedMultifractalSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float fillPercent = .5f;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        [DelayedCallback(nameof(MapChange))]
        [Range(1, 8)]
        public int octaves = 4;

        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 4f)]
        public float persistence = 0.5f;

        [DelayedCallback(nameof(MapChange))]
        [Range(1f, 10f)]
        public float lacunarity = 2f;

        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 10f)]
        public float threshold = 0.5f;

        public bool Equals(RidgedMultifractalSO other) {
            return other != null &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   Math.Abs(lacunarity - other.lacunarity) < _compareThreshold &&
                   Math.Abs(persistence - other.persistence) < _compareThreshold &&
                   octaves == other.octaves &&
                   invert == other.invert;
        }

        public override MapFactory GetFactory() {
            return new RidgedMultifractalFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as RidgedMultifractalSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + seed.GetHashCode();
                hash = hash * 16777619 + fillPercent.GetHashCode();
                hash = hash * 16777619 + lacunarity.GetHashCode();
                hash = hash * 16777619 + persistence.GetHashCode();
                hash = hash * 16777619 + octaves.GetHashCode();
                hash = hash * 16777619 + invert.GetHashCode();
                return hash;
            }
        }
    }
}