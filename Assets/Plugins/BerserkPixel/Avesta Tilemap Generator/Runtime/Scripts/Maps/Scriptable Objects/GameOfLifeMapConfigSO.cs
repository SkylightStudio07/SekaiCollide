using System;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    [CreateAssetMenu(fileName = "New Map Configuration", menuName = "Avesta/Maps/Game of Life")]
    public class GameOfLifeMapConfigSO : MapConfigSO, IEquatable<GameOfLifeMapConfigSO> {
        [DelayedCallback(nameof(MapChange))]
        [Range(0, 100)]
        public int iniChance = 50;

        [DelayedCallback(nameof(MapChange))]
        public string seed;

        [DelayedCallback(nameof(MapChange))]
        public bool invert;

        [DelayedCallback(nameof(MapChange))]
        [Range(1, 8)]
        public int birthLimit = 5;

        [DelayedCallback(nameof(MapChange))]
        [Range(1, 8)]
        public int deathLimit = 4;

        [DelayedCallback(nameof(MapChange))]
        [Range(1, 10)]
        public int numR = 3;

        public bool Equals(GameOfLifeMapConfigSO other) {
            return other != null &&
                   iniChance == other.iniChance &&
                   string.Compare(seed, other.seed, StringComparison.CurrentCulture) == 0 &&
                   invert == other.invert &&
                   birthLimit == other.birthLimit &&
                   deathLimit == other.deathLimit &&
                   numR == other.numR;
        }

        public override MapFactory GetFactory() {
            return new GameOfLifeFactory(this);
        }

        public override bool Equals(MapConfigSO map) {
            var other = map as GameOfLifeMapConfigSO;

            return Equals(other);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + seed.GetHashCode();
                hash = hash * 16777619 + iniChance.GetHashCode();
                hash = hash * 16777619 + invert.GetHashCode();
                hash = hash * 16777619 + birthLimit.GetHashCode();
                hash = hash * 16777619 + deathLimit.GetHashCode();
                hash = hash * 16777619 + numR.GetHashCode();
                return hash;
            }
        }
    }
}