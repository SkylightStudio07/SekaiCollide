using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Domain Warping")]
    public class DomainWarpingSO : MapConfigSO, IEquatable<DomainWarpingSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float fillPercent = .5f;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        [DelayedCallback(nameof(MapChange))]
        [Range(1f, 10f)]
        public float scale = 5f;

        [DelayedCallback(nameof(MapChange))]
        [Range(1f, 10f)]
        public float warpAmount = 1;

        [DelayedCallback(nameof(MapChange))]
        [Range(1f, 10f)]
        public float sampleScale = 2f;

        public bool Equals(DomainWarpingSO other) {
            return other != null &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   Math.Abs(warpAmount - other.warpAmount) < _compareThreshold &&
                   Math.Abs(scale - other.scale) < _compareThreshold &&
                   Math.Abs(sampleScale - other.sampleScale) < _compareThreshold &&
                   invert == other.invert;
        }

        public override MapFactory GetFactory() {
            return new DomainWarpFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as DomainWarpingSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + seed.GetHashCode();
                hash = hash * 16777619 + warpAmount.GetHashCode();
                hash = hash * 16777619 + scale.GetHashCode();
                hash = hash * 16777619 + fillPercent.GetHashCode();
                hash = hash * 16777619 + invert.GetHashCode();
                return hash;
            }
        }
    }
}