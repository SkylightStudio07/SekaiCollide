using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Fourier Noise")]
    public class FourierNoiseSO : MapConfigSO, IEquatable<FourierNoiseSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float fillPercent = .5f;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        [Tooltip("Control the strength of the noise")]
        public float frequency = .1f;

        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 10f)]
        [Tooltip("Control the strength of the noise")]
        public float amplitude = 5f;

        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        [Tooltip("Controls the overall size of the noise features")]
        public float scale;

        [DelayedCallback(nameof(MapChange))]
        [Range(1, 8)]
        [Tooltip("Controls the number of noise layers added together")]
        public int octaves;

        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        [Tooltip("Controls the increase in frequency between successive noise layers")]
        public float lacunarity = .5f;

        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        [Tooltip("Controls the decrease in amplitude between successive noise layers")]
        public float persistence = .5f;

        public bool Equals(FourierNoiseSO other) {
            return other != null &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   Math.Abs(frequency - other.frequency) < _compareThreshold &&
                   Math.Abs(amplitude - other.amplitude) < _compareThreshold &&
                   Math.Abs(lacunarity - other.lacunarity) < _compareThreshold &&
                   Math.Abs(persistence - other.persistence) < _compareThreshold &&
                   Math.Abs(scale - other.scale) < _compareThreshold &&
                   octaves == other.octaves &&
                   invert == other.invert;
        }

        public override MapFactory GetFactory() {
            return new FourierNoiseFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as FourierNoiseSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + seed.GetHashCode();
                hash = hash * 16777619 + fillPercent.GetHashCode();
                hash = hash * 16777619 + frequency.GetHashCode();
                hash = hash * 16777619 + amplitude.GetHashCode();
                hash = hash * 16777619 + lacunarity.GetHashCode();
                hash = hash * 16777619 + persistence.GetHashCode();
                hash = hash * 16777619 + scale.GetHashCode();
                hash = hash * 16777619 + octaves.GetHashCode();
                hash = hash * 16777619 + invert.GetHashCode();
                return hash;
            }
        }
    }
}