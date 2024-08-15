using BerserkPixel.Tilemap_Generator.Destructible;
using UnityEngine;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts.Destructible
{
    public class ClickAndDestroy : MonoBehaviour
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private float checkRadius = .4f;

        [Tooltip(
            "Depending on the type of game you could have different tools/weapons with different amount of damages.")]
        [SerializeField]
        private int tileDamage = 3;

        [Header("FX")] [SerializeField] private Transform tileVisuals;

        [SerializeField] private ParticleSystem destroyParticles;

        private Camera _mainCamera;
        private Vector2 _mousePosition;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            _mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var overlapCircle = Physics2D.OverlapCircle(_mousePosition, checkRadius, targetMask);

            DestructibleTilemap destructibleTilemap = null;

            if (overlapCircle != null && overlapCircle.TryGetComponent(out destructibleTilemap))
            {
                if (!tileVisuals.gameObject.activeSelf) tileVisuals.gameObject.SetActive(true);

                tileVisuals.position = destructibleTilemap.GetTileCenter(_mousePosition);
            }
            else
            {
                tileVisuals.gameObject.SetActive(false);
            }

            if (!Input.GetMouseButton(0) || overlapCircle == null || destructibleTilemap == null) return;

            // if we clicked on a proper destructible tile we perform the visuals and destroy the tile

            destroyParticles.transform.position = _mousePosition;
            destroyParticles.Play();
            // here we send the hit position where we clicked and the amount of damage to that tile.
            // depending on the type of game you could have different tools/weapons with different 
            // amount of damages.
            destructibleTilemap.PerformContact(_mousePosition, tileDamage);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_mousePosition, checkRadius);
        }
    }
}