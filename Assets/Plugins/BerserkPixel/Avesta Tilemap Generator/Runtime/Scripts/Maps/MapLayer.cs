using System;
using System.Text.RegularExpressions;
using BerserkPixel.Tilemap_Generator.Algorithms;
using BerserkPixel.Tilemap_Generator.Attributes;
using BerserkPixel.Tilemap_Generator.SO;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator {
    [Serializable]
    public class MapLayer : ISerializationCallbackReceiver, IEquatable<MapLayer> {
        [HideInInspector]
        public string name;

        [Tooltip("Whether this layer should be taken into account when generating the final grid")]
        public bool active = true;

        [DrawIf(nameof(IsTheOnlyActive), false)]
        [Tooltip("Whether this layer should be added to the previous one. Used to combine algorithms")]
        [SerializeField]
        private bool isAdditive;

        [Header("Algorithm")]
        [Expandable(BackgroundStyles.Darken)]
        [SerializeField]
        protected MapConfigSO mapConfig;

        [HideInInspector]
        public bool IsTheOnlyActive = true;

        protected IMapAlgorithm _mapAlgorithm;
        protected MapType _mapType;

        public bool IsAdditive {
            get => isAdditive;
            set => isAdditive = value;
        }

        public MapConfigSO MapConfig => mapConfig;

        public int Width => mapConfig.width;
        public int Height => mapConfig.height;

        public bool Equals(MapLayer other) {
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
            if (GetType() != other.GetType()) {
                return false;
            }

            Debug.Log("Layer equals 2");

            return string.Compare(name, other.name, StringComparison.CurrentCulture) == 0 &&
                   mapConfig.Equals(other.mapConfig) &&
                   isAdditive == other.isAdditive &&
                   Width == other.Width &&
                   Height == other.Height;
        }

        public void OnBeforeSerialize() {
            if (mapConfig == null) {
                return;
            }

            _mapType = mapConfig.GetFromSO();

            name = GenerateName();
        }

        public void OnAfterDeserialize() {
            if (mapConfig == null) {
                return;
            }

            _mapType = mapConfig.GetFromSO();

            name = GenerateName();
        }

        /// Gets a preview of the Map. Used from the editor
        public MapArray GetMapData() {
            _mapAlgorithm = mapConfig.GetFactory().CreateMap();
            return _mapAlgorithm.RandomFillMap();
        }

        public void SetRandomSeed() {
            if (mapConfig == null) {
                return;
            }

            mapConfig.SetRandomSeed();
        }

        private string GenerateName() {
            var activeStatus = active ? "[ACTIVE]" : "[INACTIVE]";
            var additiveStatus = active && isAdditive ? "++ " : "";
            return $"{additiveStatus}{SplitCamelCase(_mapType.ToString())} {activeStatus}";
        }

        private string SplitCamelCase(string input) {
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }

        public override bool Equals(object other) {
            //Sequence of checks should be exactly the following.
            //If you don't check "other" on null, then "other.GetType()" further can 
            //throw NullReferenceException
            if (other == null) {
                return false;
            }

            //If references point to the same address, then objects identity is
            //guaranteed.
            if (ReferenceEquals(this, other)) {
                return true;
            }

            //If this type is on top of a class hierarchy, or just doesn't have any
            //inheritors, then you just can do the following:        
            //Vehicle tmp = other as Vehicle; if(tmp==null) return false;
            //After that you can immediately call this.Equals(tmp)
            if (GetType() != other.GetType()) {
                return false;
            }

            return Equals(other as MapLayer);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int) 2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + name.GetHashCode();
                hash = hash * 16777619 + mapConfig.GetHashCode();
                hash = hash * 16777619 + isAdditive.GetHashCode();
                hash = hash * 16777619 + Width.GetHashCode();
                hash = hash * 16777619 + Height.GetHashCode();
                return hash;
            }
        }
    }
}