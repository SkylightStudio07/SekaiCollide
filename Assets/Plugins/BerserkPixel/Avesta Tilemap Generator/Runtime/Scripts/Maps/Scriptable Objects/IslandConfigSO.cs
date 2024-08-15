using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Island")]
    public class IslandConfigSO : MapConfigSO, IEquatable<IslandConfigSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float fillPercent = .4f;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        [DelayedCallback(nameof(MapChange))]
        public IslandDistance distanceAlgorithm = IslandDistance.Eucledian;

        // The number of octaves to use in the noise
        [DelayedCallback(nameof(MapChange))]
        [Range(1, 10)]
        public int numOctaves = 1;

        [DelayedCallback(nameof(MapChange))]
        [Range(0.0001f, 10)]
        public float scale = 5f;

        public bool Equals(IslandConfigSO other) {
            return other != null &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   invert == other.invert &&
                   distanceAlgorithm.Equals(other.distanceAlgorithm) &&
                   numOctaves == other.numOctaves &&
                   Math.Abs(scale - other.scale) < _compareThreshold;
        }

        public override MapFactory GetFactory() {
            return new IslandFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as IslandConfigSO;

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
                hash = hash * 16777619 + distanceAlgorithm.GetHashCode();
                hash = hash * 16777619 + numOctaves.GetHashCode();
                hash = hash * 16777619 + scale.GetHashCode();
                return hash;
            }
        }
    }

    [Serializable]
    public enum IslandDistance {
        None,
        Basic,
        Eucledian,
        SquareBumb
    }
}