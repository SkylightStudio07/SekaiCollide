using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator.Destructible {
    [CreateAssetMenuAttribute(menuName = "Avesta/Tiles/Destructible Tile")]
    public class DestructibleTile : RuleTile {
        public int earnedPoints = 1;
        public int durability = 3;

        private int _durability;

        public bool IsTileDestructed => _durability <= 0;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            var tile = tilemap.GetTile(position);
            if (tile == null || tile != this) {
                return;
            }

            if (IsTileDestructed) {
                tileData.sprite = null;
                tileData.colliderType = Tile.ColliderType.None;
                tileData.flags = TileFlags.None;
            }
            else {
                base.GetTileData(position, tilemap, ref tileData);
            }
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject) {
            _durability = durability;
            return base.StartUp(position, tilemap, instantiatedGameObject);
        }

        public void Damage(int amount) {
            _durability -= amount;
        }
    }
}