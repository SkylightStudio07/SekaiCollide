using System;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    public class InspectorButtonAttribute : PropertyAttribute {
        public readonly string MethodName;
        public readonly float verticalPadding = 8f;

        public InspectorButtonAttribute(string MethodName) {
            this.MethodName = MethodName;
            verticalPadding = 8f;
        }

        public InspectorButtonAttribute(string MethodName, float VerticalPadding) {
            this.MethodName = MethodName;
            verticalPadding = VerticalPadding;
        }
    }
}