using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    // [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Sparse Convolution")]
    public class SparseConvolutionSO : MapConfigSO, IEquatable<SparseConvolutionSO> {
        [DelayedCallback(nameof(MapChange))]
        public SparseType type;

        [DelayedCallback(nameof(MapChange))]
        [Range(0f, 1f)]
        public float fillPercent = .5f;

        [DelayedCallback(nameof(MapChange))]
        [Range(0.1f, 6f)]
        public float sigma = 1.0f;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        [Range(1, 40)]
        public int kernelSize = 2;

        [DelayedCallback(nameof(MapChange))]
        [Range(1, 1000)]
        public int numberOfPoints = 8;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        public bool Equals(SparseConvolutionSO other) {
            return other != null &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   type == other.type &&
                   Math.Abs(fillPercent - other.fillPercent) < _compareThreshold &&
                   Math.Abs(sigma - other.sigma) < _compareThreshold &&
                   kernelSize == other.kernelSize &&
                   numberOfPoints == other.numberOfPoints &&
                   width == other.width &&
                   height == other.height &&
                   invert == other.invert;
        }

        public override MapFactory GetFactory() {
            return new SparseConvolutionFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as SparseConvolutionSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + width.GetHashCode();
                hash = hash * 16777619 + height.GetHashCode();
                hash = hash * 16777619 + type.GetHashCode();
                hash = hash * 16777619 + seed.GetHashCode();
                hash = hash * 16777619 + kernelSize.GetHashCode();
                hash = hash * 16777619 + numberOfPoints.GetHashCode();
                hash = hash * 16777619 + fillPercent.GetHashCode();
                hash = hash * 16777619 + sigma.GetHashCode();
                hash = hash * 16777619 + invert.GetHashCode();
                return hash;
            }
        }
    }

    [Serializable]
    public enum SparseType {
        Gaussian = 0,
        Radial = 1,
        Circles = 2,
        StarShape = 3,
        Spiral = 4,
        Hexagonal = 5,
        Waves = 6
    }
}