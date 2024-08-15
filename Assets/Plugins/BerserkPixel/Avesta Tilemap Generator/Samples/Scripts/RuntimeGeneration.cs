using System.Collections;
using BerserkPixel.Tilemap_Generator;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts
{
    public class RuntimeGeneration : MonoBehaviour
    {
        [SerializeField] private KeyCode _levelKey;
        [SerializeField] private KeyCode _objectKey;
        [SerializeField] private KeyCode _colorKey;

        [Header("Colors")] [SerializeField] private Color[] _bgColors;

        [SerializeField] private Color[] _fgColors;
        [SerializeField] private Tilemap _bgTilemap;
        [SerializeField] private Tilemap _fgTilemap;

        [Header("Camera Zoom")] [SerializeField]
        private KeyCode _minusZoomKey;

        [SerializeField] private KeyCode _plusZoomKey;
        [SerializeField] private float _minZoom = 5;
        [SerializeField] private float _maxZoom = 12;
        private LevelObjectGenerator[] _levelObjectGenerators;

        private LevelTileGenerator[] _levelTileGenerators;

        private Camera _mainCamera;

        private MapObjectPlacer _mapObjectPlacer;
        private float _targetZoom;

        private void Awake()
        {
            _levelTileGenerators = FindObjectsOfType<LevelTileGenerator>();
            _levelObjectGenerators = FindObjectsOfType<LevelObjectGenerator>();
            _mapObjectPlacer = FindObjectOfType<MapObjectPlacer>();

            _mainCamera = Camera.main;
            _targetZoom = _mainCamera.orthographicSize;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_levelKey)) StartCoroutine(GenerateLevel());

            if (Input.GetKeyDown(_objectKey)) StartCoroutine(GenerateObjects());

            if (Input.GetKeyDown(_minusZoomKey)) _targetZoom = Mathf.Max(_mainCamera.orthographicSize - 2, _minZoom);

            if (Input.GetKeyDown(_plusZoomKey)) _targetZoom = Mathf.Min(_mainCamera.orthographicSize + 2, _maxZoom);

            if (Input.GetKeyDown(_colorKey)) ChangeTilemapColors();
        }

        private void LateUpdate()
        {
            _mainCamera.orthographicSize = Mathf.Lerp(_mainCamera.orthographicSize, _targetZoom, .2f);
        }

        private void ChangeTilemapColors()
        {
            if (_bgTilemap != null)
            {
                var randomBgColor = _bgColors[Random.Range(0, _bgColors.Length)];
                _bgTilemap.color = randomBgColor;
            }

            if (_fgTilemap != null)
            {
                var randomFgColor = _fgColors[Random.Range(0, _fgColors.Length)];
                _fgTilemap.color = randomFgColor;
            }
        }

        private IEnumerator GenerateObjects()
        {
            yield return new WaitForEndOfFrame();

            if (_mapObjectPlacer != null) _mapObjectPlacer.PlaceObjects();
        }

        private IEnumerator GenerateLevel()
        {
            yield return new WaitForEndOfFrame();

            if (_mapObjectPlacer != null) _mapObjectPlacer.ClearObjects();

            if (_levelTileGenerators != null && _levelTileGenerators.Length > 0)
                foreach (var levelTileGenerator in _levelTileGenerators)
                {
                    foreach (var layer in levelTileGenerator.GetActiveLayers()) layer.SetRandomSeed();
                    levelTileGenerator.GenerateLayers();
                }
            else if (_levelObjectGenerators != null)
                foreach (var levelObjectGenerator in _levelObjectGenerators)
                {
                    foreach (var layer in levelObjectGenerator.GetActiveLayers()) layer.SetRandomSeed();
                    levelObjectGenerator.GenerateLayers();
                }

            ChangeTilemapColors();

            if (_mapObjectPlacer != null) _mapObjectPlacer.PlaceObjects();
        }
    }
}