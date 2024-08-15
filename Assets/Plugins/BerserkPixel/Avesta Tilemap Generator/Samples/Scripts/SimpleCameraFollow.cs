using UnityEngine;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts
{
    public class SimpleCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = Vector3.back;

        private void LateUpdate()
        {
            transform.position = target.position + offset;
        }
    }
}