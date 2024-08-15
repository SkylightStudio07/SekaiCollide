using System;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.Attributes {
    /// <summary>
    ///     <para>Attribute used to make a variable in a script be delayed and afterwards invoke a callback.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DelayedCallbackAttribute : PropertyAttribute {
        private const float DEFAULT_TIME = .1f;
        public readonly string propertyName;
        public readonly float time;

        public DelayedCallbackAttribute(string propertyName, float time) {
            this.propertyName = propertyName;
            this.time = time;
        }

        public DelayedCallbackAttribute(string propertyName) {
            this.propertyName = propertyName;
            time = DEFAULT_TIME;
        }
    }
}