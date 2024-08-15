using System;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.Attributes {
    [AttributeUsage(AttributeTargets.Field)]
    public class NoteAttribute : PropertyAttribute {
        public string Text = string.Empty;
        public int HorizontalPadding = 8;
        public int VerticalPadding = 8;

        public NoteAttribute(string text) {
            Text = text;
        }
        
        public NoteAttribute(string text, int horizontalPadding = 8, int verticalPadding = 8) {
            Text = text;
            HorizontalPadding = horizontalPadding;
            VerticalPadding = verticalPadding;
        }
    }
}