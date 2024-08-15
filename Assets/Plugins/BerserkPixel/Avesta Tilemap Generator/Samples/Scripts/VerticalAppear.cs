using UnityEngine;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts
{
    public class VerticalAppear : MonoBehaviour
    {
        [SerializeField] private float _timeToAppear = .3f;
        [SerializeField] private float _startingDelta = .3f;

        private Vector3Int _finalPosition;
        private Vector3Int _initialPosition;

        private bool _shouldMove;

        private void Start()
        {
            _finalPosition = Vector3Int.FloorToInt(transform.position);
            _initialPosition = _finalPosition;
            _initialPosition.y -= 2;

            transform.position = _initialPosition;

            var initalTime = _startingDelta + Random.Range(-.5f, .5f);

            Invoke(nameof(StartMoving), initalTime);
        }

        private void Update()
        {
            if (!_shouldMove) return;

            transform.position = Vector3.Lerp(transform.position, _finalPosition, _timeToAppear);
        }

        private void StartMoving()
        {
            _shouldMove = true;
        }
    }
}