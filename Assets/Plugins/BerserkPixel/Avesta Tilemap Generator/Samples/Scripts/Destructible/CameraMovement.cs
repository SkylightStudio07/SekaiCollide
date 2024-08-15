using UnityEngine;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts.Destructible
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 14.0f;

        [Tooltip("Used for diagonal movement")] [SerializeField]
        private float moveLimiter = 0.6f;

        private Vector3 _movement;

        private void Update()
        {
            // movement
            _movement = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.up * Input.GetAxisRaw("Vertical");
            _movement.z = 0;

            if (_movement.magnitude != 0) // Check for diagonal movement
                // limit movement speed diagonally, so you move at 70% speed
                _movement *= moveLimiter;

            _movement *= speed * Time.deltaTime;

            transform.Translate(_movement);
        }
    }
}