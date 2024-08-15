using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Voronoi")]
    public class VoronoiSO : MapConfigSO, IEquatable<VoronoiSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float fillPercent = .5f;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        [DelayedCallback(nameof(MapChange))]
        [Range(1, 100)]
        public int numPoints = 5;

        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float frequency = .5f;

        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float amplitude = .5f;

        public bool Equals(VoronoiSO other) {
            return other != null &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   Math.Abs(frequency - other.frequency) < _compareThreshold &&
                   Math.Abs(amplitude - other.amplitude) < _compareThreshold &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   numPoints == other.numPoints &&
                   invert == other.invert;
        }

        public override MapFactory GetFactory() {
            return new VoronoiFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as VoronoiSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + fillPercent.GetHashCode();
                hash = hash * 16777619 + frequency.GetHashCode();
                hash = hash * 16777619 + seed.GetHashCode();
                hash = hash * 16777619 + amplitude.GetHashCode();
                hash = hash * 16777619 + numPoints.GetHashCode();
                hash = hash * 16777619 + invert.GetHashCode();
                return hash;
            }
        }
    }
}