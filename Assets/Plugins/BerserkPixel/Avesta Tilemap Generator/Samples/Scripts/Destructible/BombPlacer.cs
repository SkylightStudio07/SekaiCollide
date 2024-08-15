using BerserkPixel.Tilemap_Generator.Destructible;
using UnityEngine;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts.Destructible
{
    public class BombPlacer : MonoBehaviour
    {
        [SerializeField] private Bomb bombPrefab;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
                // spawn bomb
                Instantiate(bombPrefab, transform.position, Quaternion.identity);
        }

        private void OnEnable()
        {
            DestructibleTilemap.OnTileHit += HandleTileHit;
        }

        private void OnDisable()
        {
            DestructibleTilemap.OnTileHit -= HandleTileHit;
        }

        private void HandleTileHit(DestructibleTilemap tilemap, TileHitInfo<DestructibleTile> obj)
        {
            obj.Tile.UpdateNeighborPositions();
        }
    }
}