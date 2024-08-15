using System;
using System.Reflection;
using BerserkPixel.Tilemap_Generator.Attributes;
using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Tilemap_Generator.Utilities
{
    [CustomPropertyDrawer(typeof(DelayedCallbackAttribute), false)]
    public class DelayedCallbackDrawer : PropertyDrawer
    {
        private const float _dragTolerance = .2f;
        private const float _singleClickTolerance = .2f;

        // Flag for doing Setup only once
        private bool _setup;
        private SerializedProperty _property;

        private float _sliderValue;
        private double _nextSave;
        private bool _pressed;
        private float _sqrMagnitude;

        private int _lastHash;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            if (!_setup)
            {
                // store the selected Object (should be the one with this drawer active)
                _property = property;
                _setup = true;
                _lastHash = GetFirstHash(_property);
            }

            EditorGUI.PropertyField(position, property, label, true);

            if (property.propertyPath != _property.propertyPath)
                return;

            var current = Event.current;

            if (current == null || current.type is EventType.Layout || current.type is EventType.Repaint) return;
            
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            // check if it's just a click and compare hashes
            if (!ProcessSingleMouseClick(position, current))
            {
                // if it's a moving mouse check hashes
                if (!ProcessMouseEvents(position, current))
                {
                    // not mouse, handle another way
                    ProcessEditorChanges(property);
                }
            }
            
            EditorGUI.EndProperty();
        }

        private int GetFirstHash(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    return property.floatValue.GetHashCode();
                case SerializedPropertyType.Integer:
                    return property.intValue.GetHashCode();
                case SerializedPropertyType.String:
                    return property.stringValue.GetHashCode();
                case SerializedPropertyType.Boolean:
                    return property.boolValue.GetHashCode();
                case SerializedPropertyType.Enum:
                    return property.enumValueIndex.GetHashCode();
                case SerializedPropertyType.Vector2Int:
                    return property.vector2IntValue.GetHashCode();
                case SerializedPropertyType.Vector2:
                    return property.vector2Value.GetHashCode();
            }

            // other fields are not supported
            return 0;
        }

        private void ProcessEditorChanges(SerializedProperty property)
        {
            var currentHash = 0;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    currentHash = property.floatValue.GetHashCode();
                    break;
                case SerializedPropertyType.Integer:
                    currentHash = property.intValue.GetHashCode();
                    break;
                case SerializedPropertyType.String:
                    currentHash = property.stringValue.GetHashCode();
                    break;
                case SerializedPropertyType.Boolean:
                    currentHash = property.boolValue.GetHashCode();
                    break;
                case SerializedPropertyType.Enum:
                    currentHash = property.enumValueIndex.GetHashCode();
                    break;
                case SerializedPropertyType.Vector2Int:
                    currentHash =  property.vector2IntValue.GetHashCode();
                    break;
                case SerializedPropertyType.Vector2:
                    currentHash =  property.vector2Value.GetHashCode();
                    break;
            }

            if (_lastHash == currentHash) return;
            
            var attr = (DelayedCallbackAttribute) attribute;
            InvokeMethod(_property, attr.propertyName);

            _lastHash = currentHash;
        }

        private bool ProcessSingleMouseClick(Rect position, Event current)
        {
            var mouseContained = position.Contains(current.mousePosition);

            var attr = (DelayedCallbackAttribute) attribute;

            if (!mouseContained || current.clickCount != 1 || current.type != EventType.Used ||
                current.delta != Vector2.zero) return false;
            
            switch (_pressed)
            {
                // delta 0 means mouse down
                case false:
                    HandleSingleMouseDown();
                    return true;
                case true:
                    HandleSingleMouseUp(attr);
                    return true;
            }
        }
        
        /// <summary>
        /// Process the MouseDown and MouseUp events
        /// </summary>
        /// <param name="position">The current property position</param>
        /// <param name="current">The current Event</param>
        /// <returns>True if it was handled, False otherwise</returns>
        private bool ProcessMouseEvents(Rect position, Event current)
        {
            var mouseContained = position.Contains(current.mousePosition);

            var attr = (DelayedCallbackAttribute) attribute;

            if (mouseContained && current.clickCount == 1 && current.type == EventType.Used)
            {
                // delta 0 means mouse down
                if (!_pressed && current.delta == Vector2.zero)
                {
                    HandleMouseDown(current, attr);
                    return true;
                }

                if (Math.Abs(current.mousePosition.sqrMagnitude - _sqrMagnitude) > _dragTolerance && _pressed)
                {
                    HandleMouseUp(attr);
                    return true;
                }
            }

            return false;
        }

        private void HandleMouseDown(Event current, DelayedCallbackAttribute attr)
        {
            _nextSave = EditorApplication.timeSinceStartup + attr.time;
            _pressed = true;

            _sqrMagnitude = current.mousePosition.sqrMagnitude;
            Event.current.Use();
        }

        private void HandleMouseUp(DelayedCallbackAttribute attr)
        {
            if (EditorApplication.timeSinceStartup > _nextSave)
            {
                _nextSave = EditorApplication.timeSinceStartup + attr.time;
                InvokeMethod(_property, attr.propertyName);
            }

            _pressed = false;
        }
        
        private void HandleSingleMouseDown()
        {
            _nextSave = EditorApplication.timeSinceStartup;
            _pressed = true;

            Event.current.Use();
        }

        private void HandleSingleMouseUp(DelayedCallbackAttribute attr)
        {
            var diff = EditorApplication.timeSinceStartup - _nextSave;
            if (diff < _singleClickTolerance)
            {
                _nextSave = EditorApplication.timeSinceStartup + attr.time;
                InvokeMethod(_property, attr.propertyName);
            }

            _pressed = false;
        }

        private void InvokeMethod(SerializedProperty property, string methodName,
            BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                    BindingFlags.NonPublic)
        {
            var targetObject = property.serializedObject.targetObject;
            var thisType = targetObject.GetType();

            var theMethod = thisType.GetMethod(methodName, bindings);

            theMethod?.Invoke(targetObject, null);
        }
    }
}