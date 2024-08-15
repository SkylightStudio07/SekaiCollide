using UnityEngine;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts
{
    public class Player3DController : MonoBehaviour
    {
        [SerializeField] private float speed = 20.0f;

        [Tooltip("Used for diagonal movement")] [SerializeField]
        private float moveLimiter = 0.7f;

        [SerializeField] private Transform cameraTransform;

        private Vector3 _movement;

        private void Update()
        {
            // movement
            _movement = cameraTransform.right * Input.GetAxisRaw("Horizontal") +
                        cameraTransform.forward * Input.GetAxisRaw("Vertical");
            _movement.y = 0;

            if (_movement.magnitude != 0) // Check for diagonal movement
                // limit movement speed diagonally, so you move at 'moveLimiter' % speed
                _movement *= moveLimiter;

            _movement *= speed * Time.deltaTime;

            transform.Translate(_movement);
        }
    }
}