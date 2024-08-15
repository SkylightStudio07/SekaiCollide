using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Cellular")]
    public class CellularMapConfigSO : MapConfigSO, IEquatable<CellularMapConfigSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0, 100)]
        public float fillPercent = 50;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        [DelayedCallback(nameof(MapChange))]
        [Range(0, 10)]
        public int smoothSteps = 5;

        [DelayedCallback(nameof(MapChange))]
        [Range(1, 8)]
        public int numberOfNeighbours = 4;

        public bool Equals(CellularMapConfigSO other) {
            return other != null &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   invert == other.invert &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   smoothSteps == other.smoothSteps &&
                   numberOfNeighbours == other.numberOfNeighbours;
        }

        public override MapFactory GetFactory() {
            return new CellularAutomataFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as CellularMapConfigSO;

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
                hash = hash * 16777619 + smoothSteps.GetHashCode();
                hash = hash * 16777619 + numberOfNeighbours.GetHashCode();
                return hash;
            }
        }
    }
}