using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Basic Random")]
    public class BasicRandomSO : MapConfigSO, IEquatable<BasicRandomSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0, 100)]
        public float fillPercent = 50;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        public bool Equals(BasicRandomSO other) {
            return other != null &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   invert == other.invert;
        }

        public override MapFactory GetFactory() {
            return new BasicRandomFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as BasicRandomSO;

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
                return hash;
            }
        }
    }
}