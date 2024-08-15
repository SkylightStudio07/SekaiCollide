using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BerserkPixel.Tilemap_Generator.Attributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator {
    [AddComponentMenu("Avesta/Tilemaps/Level Tile Generator")]
    public class LevelTileGenerator : LevelGenerator {
        [Space]
        [Header("Type of Tile to use")]
        [Tooltip("Add your Tile here")]
        [SerializeField]
        private TileBase tile;

        [Header("Collider Generation")]
        [SerializeField]
        private bool generateColliders = true;
        
        [Note("Experimental features", verticalPadding: 4)]
        
        [Header("Chunks")]
        [SerializeField]
        [Min(1)]
        [DelayedCallback(nameof(NumberOfChunksChange), .5f)]
        private int numberOfChunks = 4;

        public override List<MapLayer> GetActiveLayers() {
            if (layers == null || layers.Count <= 0) {
                return new List<MapLayer>(1);
            }

            return layers.FindAll(layer => layer.active);
        }

        protected override void GenerateColliders() {
            if (!generateColliders) {
                return;
            }

            if (tilemap.TryGetComponent(out TilemapCollider2D currentCollider)) {
                currentCollider.ProcessTilemapChanges();
            }
            else {
                var tilemapCollider = tilemap.gameObject.AddComponent<TilemapCollider2D>();
                tilemap.gameObject.AddComponent<CompositeCollider2D>();
                tilemapCollider.usedByComposite = true;

                var rb = tilemap.gameObject.GetComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
            }
        }

        protected override async Task RenderMap(MapArray terrainMap) {
            await RenderChunk(terrainMap);
        }

        protected override void OnValidate() {
            base.OnValidate();
            numberOfChunks = ToNearest(numberOfChunks);
        }

        private void NumberOfChunksChange() {
            numberOfChunks = ToNearest(numberOfChunks);
        }

        private int ToNearest(int x) {
            return (int) Mathf.Pow(2, Mathf.Round(Mathf.Log(x) / Mathf.Log(2)));
        }

        private Task RenderChunk(MapArray terrainMap) {
            var allTerrainPositions = terrainMap.GetPositions();

            var width = terrainMap.Width;
            var height = terrainMap.Height;

            var chunksPerRow = (int) math.sqrt(numberOfChunks);
            var chunksPerColumn = numberOfChunks / chunksPerRow;

            var chunkSizeX = width / chunksPerRow;
            var chunkSizeY = height / chunksPerColumn;
            
            var offsetX = 0; // X offset for the first chunk
            var offsetY = 0; // Y offset for the first chunk

            var tasks = new Task[numberOfChunks];

            for (var i = 0; i < numberOfChunks; i++) {
                var row = i % chunksPerRow;
                var col = i / chunksPerRow;

                var chunkX = row * chunkSizeX - width / 2;
                var chunkY = col * chunkSizeY - height / 2;

                var chunkXEnd = chunkX + chunkSizeX;
                var chunkYEnd = chunkY + chunkSizeY;

                tasks[i] = GenerateChunkDelayed(
                    ref allTerrainPositions,
                    offsetX + chunkX,
                    offsetX + chunkXEnd,
                    offsetY + chunkY,
                    offsetY + chunkYEnd
                );
            }

            return Task.WhenAll(tasks);
        }

        private Task GenerateChunkDelayed(
            ref HashSet<Vector3Int> allValidPositions,
            int fromX,
            int toX,
            int fromY,
            int toY) {
            var positions = new List<Vector3Int>();

            for (var x = fromX; x < toX; x++) {
                for (var y = fromY; y < toY; y++) {
                    var toFind = new Vector3Int(x, y);
                    if (allValidPositions.Contains(toFind)) {
                        positions.Add(toFind);
                    }
                }
            }

            var tiles = new TileBase[positions.Count];
            Array.Fill(tiles, tile);
            tilemap.SetTiles(positions.ToArray(), tiles);

            return Task.CompletedTask;
        }

        protected override void ClearMap() {
            if (tilemap == null) {
                return;
            }

            tilemap.ClearAllTiles();
            tilemap.RefreshAllTiles();
        }
    }
}