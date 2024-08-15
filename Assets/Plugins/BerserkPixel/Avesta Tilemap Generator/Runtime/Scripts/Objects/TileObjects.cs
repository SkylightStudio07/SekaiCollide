using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BerserkPixel.Tilemap_Generator {
    [Serializable]
    public struct TileObjects {
        // [SerializeField]
        // [HideInInspector]
        // private string name;

        public GameObject prefab;
        public ObjectType objectType;

        [Range(0f, 1f)]
        public float weight;

        public Vector3 offset;

        [Header("Randomize")]
        [SerializeField]
        private float minScaleVariation;

        [SerializeField]
        private float maxScaleVariation;

        // public void OnBeforeSerialize() {
        //     if (prefab == null) return;
        //
        //     name = GenerateName();
        // }
        //
        // public void OnAfterDeserialize() { }

        public void SetScale(Transform objectTransform) {
            var localScale = objectTransform.localScale;
            var randomY = Random.Range(localScale.y - minScaleVariation, localScale.y + maxScaleVariation);
            var randomZ = Random.Range(localScale.z - minScaleVariation, localScale.z + maxScaleVariation);
            var randomX = Random.Range(localScale.x - minScaleVariation, localScale.x + maxScaleVariation);

            objectTransform.localScale = new Vector3(randomX, randomY, randomZ);
        }

        public void SetRotation(Transform objectTransform) {
            switch (objectType) {
                case ObjectType.None:
                default:
                    // basically do nothing
                    return;
                case ObjectType.Object2D:
                    objectTransform.eulerAngles = GetRotation2D(objectTransform.eulerAngles);
                    return;
                case ObjectType.Object3D:
                    objectTransform.eulerAngles = GetRotation3D(objectTransform.eulerAngles);
                    return;
            }
        }

        private Vector3 GetRotation3D(Vector3 currentRotation) {
            var randomY = Random.Range(-360f, 360f);
            return new Vector3(currentRotation.x, randomY, currentRotation.z);
        }

        private Vector3 GetRotation2D(Vector3 currentRotation) {
            var randomZ = Random.Range(-360f, 360f);
            return new Vector3(currentRotation.x, currentRotation.y, randomZ);
        }

        // private string GenerateName() {
        //     return prefab.name;
        // }
    }
}