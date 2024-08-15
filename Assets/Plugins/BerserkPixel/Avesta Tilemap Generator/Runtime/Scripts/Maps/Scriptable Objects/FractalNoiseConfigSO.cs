using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Fractal Noise")]
    public class FractalNoiseConfigSO : MapConfigSO, IEquatable<FractalNoiseConfigSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float fillPercent = .5f;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        // The base frequency of the noise
        [DelayedCallback(nameof(MapChange))]
        [Range(1, 10)]
        public float baseFrequency = 1.0f;

        // The number of octaves to use in the noise
        [DelayedCallback(nameof(MapChange))]
        [Range(1, 10)]
        public int numOctaves = 1;

        [DelayedCallback(nameof(MapChange))]
        [Range(0.0001f, 10)]
        public float scale = 1.0f;

        public bool Equals(FractalNoiseConfigSO other) {
            return other != null &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   Math.Abs(baseFrequency - other.baseFrequency) < _compareThreshold &&
                   numOctaves == other.numOctaves &&
                   Math.Abs(scale - other.scale) < _compareThreshold;
        }

        public override MapFactory GetFactory() {
            return new FractalNoiseFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as FractalNoiseConfigSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + seed.GetHashCode();
                hash = hash * 16777619 + fillPercent.GetHashCode();
                hash = hash * 16777619 + baseFrequency.GetHashCode();
                hash = hash * 16777619 + numOctaves.GetHashCode();
                hash = hash * 16777619 + scale.GetHashCode();
                return hash;
            }
        }
    }
}