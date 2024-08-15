using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Path")]
    public class PathSO : MapConfigSO, IEquatable<PathSO> {
        [Serializable]
        public enum Directions {
            TopToBottom,
            BottomToTop,
            LeftToRight,
            RightToLeft
        }

        [DelayedCallback(nameof(MapChange))]
        public Directions direction;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        [DelayedCallback(nameof(MapChange))]
        public Vector2Int startingPoint = Vector2Int.zero;

        [DelayedCallback(nameof(MapChange))]
        public int pathMinWidth = 2;

        [DelayedCallback(nameof(MapChange))]
        public int pathMaxWidth = 4;

        [DelayedCallback(nameof(MapChange))]
        public int directionChangeDistance = 2;

        [DelayedCallback(nameof(MapChange))]
        [Range(0, 100)]
        public int widthChangePercentage = 50;

        [DelayedCallback(nameof(MapChange))]
        [Range(0, 100)]
        public int directionChangePercentage = 50;

        private void OnValidate() {
            startingPoint.x = Mathf.Clamp(startingPoint.x, pathMinWidth, width - pathMinWidth);
            startingPoint.y = Mathf.Clamp(startingPoint.y, pathMinWidth, height - (pathMaxWidth + pathMinWidth));
        }

        public bool Equals(PathSO other) {
            return other != null &&
                   direction == other.direction &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   invert == other.invert &&
                   startingPoint.Equals(other.startingPoint) &&
                   pathMinWidth == other.pathMinWidth &&
                   pathMaxWidth == other.pathMaxWidth &&
                   directionChangeDistance == other.directionChangeDistance &&
                   widthChangePercentage == other.widthChangePercentage &&
                   directionChangePercentage == other.directionChangePercentage;
        }

        public override MapFactory GetFactory() {
            return new PathFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as PathSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + seed.GetHashCode();
                hash = hash * 16777619 + direction.GetHashCode();
                hash = hash * 16777619 + invert.GetHashCode();
                hash = hash * 16777619 + startingPoint.GetHashCode();
                hash = hash * 16777619 + pathMinWidth.GetHashCode();
                hash = hash * 16777619 + pathMaxWidth.GetHashCode();
                hash = hash * 16777619 + directionChangeDistance.GetHashCode();
                hash = hash * 16777619 + directionChangePercentage.GetHashCode();
                hash = hash * 16777619 + widthChangePercentage.GetHashCode();
                return hash;
            }
        }
    }
}