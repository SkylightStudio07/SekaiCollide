using System;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Full Grid")]
    public class FullGridSO : MapConfigSO, IEquatable<FullGridSO> {
        public bool Equals(FullGridSO other) {
            return other != null &&
                   width == other.width &&
                   height == other.height;
        }

        public override MapFactory GetFactory() {
            return new FullGridFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as FullGridSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + width.GetHashCode();
                hash = hash * 16777619 + height.GetHashCode();
                return hash;
            }
        }
    }
}