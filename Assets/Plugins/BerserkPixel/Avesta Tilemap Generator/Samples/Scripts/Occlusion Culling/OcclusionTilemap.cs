using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BerserkPixel.Tilemap_Generator.OcclussionCulling
{
    public class OcclusionTilemap : MonoBehaviour
    {
        private TilemapRenderer[] _tilemapRenderers;

        private void OnValidate()
        {
            _tilemapRenderers = GetComponentsInChildren<TilemapRenderer>();
        }

        private void Awake()
        {
            foreach (var tilemapRenderer in _tilemapRenderers) tilemapRenderer.enabled = false;
        }

        private void OnShown()
        {
            if (_tilemapRenderers == null || _tilemapRenderers.Length <= 0) return;

            foreach (var tilemapRenderer in _tilemapRenderers) tilemapRenderer.enabled = true;
        }

        private void OnDisappear()
        {
            if (_tilemapRenderers == null || _tilemapRenderers.Length <= 0) return;

            foreach (var tilemapRenderer in _tilemapRenderers) tilemapRenderer.enabled = false;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                OnShown();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                OnDisappear();
            }
        }
    }
}