using BerserkPixel.Tilemap_Generator.Attributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator {
    [RequireComponent(typeof(Tilemap))]
    public abstract class MapAdditive : MonoBehaviour {
        /// the tilemap to copy
        [SerializeField]
        protected Tilemap _referenceTilemap;

        [SerializeField]
        protected int _padding;

        [InspectorButton("AddTiles", 8)]
        public bool AddTilesButton;

        [InspectorButton("Clear", 8)]
        public bool ClearButton;

        protected Tilemap _tilemap;

        private void OnValidate() {
            if (_tilemap == null) {
                _tilemap = GetComponent<Tilemap>();
            }
        }

        public void AddTiles() {
            if (_referenceTilemap == null) {
                return;
            }

            Clear();

            SetSortingLayer(_referenceTilemap, _tilemap);

            Copy(_referenceTilemap, _tilemap);
        }

        protected abstract void SetSortingLayer(Tilemap source, Tilemap destination);

        /// <summary>
        ///     Copies the source tilemap on the destination tilemap
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        protected abstract void Copy(Tilemap source, Tilemap destination);

        public abstract void Clear();
    }
}