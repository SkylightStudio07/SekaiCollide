using BerserkPixel.Tilemap_Generator.Attributes;
using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.Utilities
{
    [CustomPropertyDrawer(typeof(NoteAttribute))]
    public class NoteDrawer : DecoratorDrawer
    {
        private const float _padding = 10;
        private float _height;

        public override float GetHeight()
        {
            if (attribute is not NoteAttribute noteAttribute) {
                return base.GetHeight();
            }

            var style = EditorStyles.helpBox;
            style.alignment = TextAnchor.MiddleLeft;
            style.wordWrap = true;

            var horizontalPadding = noteAttribute.HorizontalPadding;
            var verticalPadding = noteAttribute.VerticalPadding;
            
            style.padding = new RectOffset(horizontalPadding, horizontalPadding, verticalPadding, verticalPadding);
            style.fontSize = 12;

            _height = style.CalcHeight(new GUIContent(noteAttribute.Text), Screen.width);

            return _height + _padding;
        }

        public override void OnGUI(Rect position)
        {
            var noteAttribute = attribute as NoteAttribute;

            position.height = _height;
            position.y += _padding * .5f;

            EditorGUI.HelpBox(position, noteAttribute.Text, MessageType.None);
        }
    }
}