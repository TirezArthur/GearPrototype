using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(EnumMapping<,>), true)]
public class EnumMappingDrawer : PropertyDrawer
{
    private ReorderableList _reorderableList;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty valuesProp = property.FindPropertyRelative("values");

        EditorGUI.BeginProperty(position, label, property);

        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);
        position.y += EditorGUIUtility.singleLineHeight;

        if (!property.isExpanded || valuesProp == null)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;

        string[] enumNames = GetEnumNames();
        if (valuesProp.arraySize != enumNames.Length) valuesProp.arraySize = enumNames.Length;
        if (_reorderableList == null) _reorderableList = new(property.serializedObject, valuesProp, true, false, false, false);
        _reorderableList.serializedProperty = valuesProp;
        _reorderableList.footerHeight = 0;
        _reorderableList.drawElementCallback = (rect, index, active, focused) =>
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            float labelWidth = rect.width * 0.2f;
            Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
            Rect propertyRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
            EditorGUI.LabelField(labelRect, enumNames[index]);
            EditorGUI.PropertyField(propertyRect, valuesProp.GetArrayElementAtIndex(index), GUIContent.none, true);
        };
        _reorderableList.elementHeightCallback = index =>
        {
            SerializedProperty element = valuesProp.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
        };
        _reorderableList.DoList(position);

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;
        if (!property.isExpanded) return height;

        if (_reorderableList != null) height += _reorderableList.GetHeight();

        return height;
    }

    private string[] GetEnumNames()
    {
        Type enumType = fieldInfo.FieldType.GetGenericArguments()[0];
        return Enum.GetNames(enumType);
    }
}
