using System.Collections;
using UnityEngine;

namespace Plugins.BerserkPixel.Avesta_Tilemap_Generator.Samples.Scripts
{
    public class CameraRotate : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] [Range(0f, 1f)] private float rotationDuration = .8f;
        [SerializeField] private float degrees = 90f;

        [Header("Key Mapping")] [SerializeField]
        private KeyCode _rotateLeftKey = KeyCode.Q;

        [SerializeField] private KeyCode _rotateRightKey = KeyCode.E;

        private void Update()
        {
            // depending on the input rotate + - 90 degrees on the y axis
            var toRotateDegrees = target.rotation.eulerAngles.y;

            if (Input.GetKeyDown(_rotateLeftKey)) StartCoroutine(RotateMe(Vector3.up * degrees, rotationDuration));

            if (Input.GetKeyDown(_rotateRightKey)) StartCoroutine(RotateMe(Vector3.up * -degrees, rotationDuration));
        }

        private IEnumerator RotateMe(Vector3 byAngles, float inTime)
        {
            var fromAngle = target.rotation;
            var toAngle = Quaternion.Euler(target.eulerAngles + byAngles);
            for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
            {
                target.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
                yield return null;
            }
        }
    }
}