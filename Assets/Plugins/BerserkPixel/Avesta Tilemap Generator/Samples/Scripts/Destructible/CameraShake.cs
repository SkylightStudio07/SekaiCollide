using System.Collections;
using BerserkPixel.Tilemap_Generator.Destructible;
using UnityEngine;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts.Destructible
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private float cameraShakeDuration = 0.25f;
        [SerializeField] private float cameraShakeDecreaseFactor = 3f;
        [SerializeField] private float cameraShakeAmount = 2f;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            DestructibleTilemap.OnTileHit += ShakeCamera;
        }

        private void OnDisable()
        {
            DestructibleTilemap.OnTileHit -= ShakeCamera;
        }

        private void ShakeCamera(DestructibleTilemap tilemap, TileHitInfo<DestructibleTile> obj)
        {
            StopAllCoroutines();
            StartCoroutine(EnumeratorShakeCamera());
        }

        // coroutine
        private IEnumerator EnumeratorShakeCamera()
        {
            var originalPos = _mainCamera.transform.localPosition;
            var duration = cameraShakeDuration;
            while (duration > 0)
            {
                _mainCamera.transform.localPosition = originalPos + Random.insideUnitSphere * cameraShakeAmount;
                duration -= Time.deltaTime * cameraShakeDecreaseFactor;
                yield return null;
            }

            _mainCamera.transform.localPosition = originalPos;
        }
    }
}