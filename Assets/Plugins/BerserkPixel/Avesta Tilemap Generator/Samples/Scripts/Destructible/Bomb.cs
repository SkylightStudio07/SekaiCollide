using System.Collections;
using System.Collections.Generic;
using BerserkPixel.Tilemap_Generator.Destructible;
using UnityEngine;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts.Destructible
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private float checkRadius = .4f;

        [Tooltip(
            "Depending on the type of game you could have different tools/weapons with different amount of damages.")]
        [SerializeField]
        private int tileDamage = 3;

        [SerializeField] private float timeForExplosion = 1f;

        [Header("FX")] [SerializeField] private GameObject bombParticlesPrefab;
        [SerializeField] private Transform pivotTransform;
        [SerializeField] [Range(-360f, 360f)] private float wiggleAngle = 30.0f;
        [SerializeField] private float wiggleSpeed = 1.0f;
        private float _speed;

        private float _time;

        private void Start()
        {
            StartCoroutine(StartExplosion());
        }

        private void Update()
        {
            // wiggle
            _time += Time.deltaTime;

            // just a "Sinerp" instead of just doing a linear interpolation (Mathf.Lerp)
            var theta = _time / timeForExplosion;
            theta = Mathf.Sin(theta * Mathf.PI * 0.5f);

            _speed = Mathf.Lerp(wiggleSpeed, 50, theta);

            var angle = wiggleAngle * Mathf.Sin(_time * _speed);
            pivotTransform.localRotation = Quaternion.Lerp(
                pivotTransform.localRotation,
                Quaternion.Euler(0, 0, angle),
                .1f
            );
        }

        private IEnumerator StartExplosion()
        {
            yield return new WaitForSeconds(timeForExplosion);

            var position = transform.position;

            Instantiate(bombParticlesPrefab, position, Quaternion.identity);

            // check the colliders
            var raycasts = GetRaysFromPosition(position);
            foreach (var hit in raycasts)
                if (hit.transform.TryGetComponent(out DestructibleTilemap destructibleTilemap))
                    // here we send the hit position where we clicked and the amount of damage to that tile.
                    // depending on the type of game you could have different tools/weapons with different 
                    // amount of damages.
                    destructibleTilemap.PerformContact(hit.point, tileDamage);

            Destroy(gameObject);
        }

        private RaycastHit2D[] GetRaysFromPosition(Vector3 position)
        {
            var allHits = new List<RaycastHit2D>();
            var newHitPos = Vector2Int.FloorToInt(position);

            var center = Physics2D.CircleCast(newHitPos, checkRadius, Vector2.right, targetMask);

            var west = Physics2D.CircleCast(newHitPos + Vector2Int.left, checkRadius, Vector2.right, targetMask);
            var east = Physics2D.CircleCast(newHitPos + Vector2Int.right, checkRadius, Vector2.right, targetMask);
            var north = Physics2D.CircleCast(newHitPos + Vector2Int.up, checkRadius, Vector2.right, targetMask);
            var south = Physics2D.CircleCast(newHitPos + Vector2Int.down, checkRadius, Vector2.right, targetMask);

            if (center) allHits.Add(center);
            if (west) allHits.Add(west);
            if (east) allHits.Add(east);
            if (north) allHits.Add(north);
            if (south) allHits.Add(south);

            return allHits.ToArray();
        }
    }
}