using System;
using BerserkPixel.Tilemap_Generator.Factory;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.SO {
    public abstract class MapConfigSO : ScriptableObject, IEquatable<MapConfigSO> {
        protected const float _compareThreshold = 0.1f;

        [Delayed]
        [Min(1)]
        public int width;

        [Delayed]
        [Min(1)]
        public int height;

        public Action<MapConfigSO> OnMapChange;

        public abstract bool Equals(MapConfigSO map);

        public abstract MapFactory GetFactory();

        public void MapChange() {
            OnMapChange?.Invoke(this);
        }

        public override bool Equals(object other) {
            //Sequence of checks should be exactly the following.
            //If you don't check "other" on null, then "other.GetType()" further can 
            //throw NullReferenceException
            if (other == null) {
                return false;
            }

            // Comparing by references here is not necessary.
            // If you're sure that many compares will end up be references comparing 
            // then you can implement it
            if (ReferenceEquals(this, other)) {
                return true;
            }

            //If parent and inheritor instances can possibly be treated as equal then 
            //you can immediately move to comparing their fields.
            return GetType() == other.GetType() && Equals(other as MapConfigSO);
        }

        public abstract override int GetHashCode();
    }
}