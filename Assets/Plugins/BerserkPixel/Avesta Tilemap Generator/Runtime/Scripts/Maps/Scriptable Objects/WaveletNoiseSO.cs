using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Wavelet")]
    public class WaveletNoiseSO : MapConfigSO, IEquatable<WaveletNoiseSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float fillPercent = .5f;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        [DelayedCallback(nameof(MapChange))]
        [Range(1, 10)]
        public int levels = 1;

        [DelayedCallback(nameof(MapChange))]
        [Range(0.1f, 1f)]
        public float persistence = .7f;

        public bool Equals(WaveletNoiseSO other) {
            return other != null &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   levels == other.levels &&
                   invert == other.invert &&
                   Math.Abs(persistence - other.persistence) < _compareThreshold;
        }

        public override MapFactory GetFactory() {
            return new WaveletNoiseFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as WaveletNoiseSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + seed.GetHashCode();
                hash = hash * 16777619 + levels.GetHashCode();
                hash = hash * 16777619 + fillPercent.GetHashCode();
                hash = hash * 16777619 + invert.GetHashCode();
                hash = hash * 16777619 + persistence.GetHashCode();
                return hash;
            }
        }
    }
}