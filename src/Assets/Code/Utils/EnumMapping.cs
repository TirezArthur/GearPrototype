using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[Serializable]
public class EnumMapping<TEnum, TValue>
{
    [SerializeField] private TValue[] values = new TValue[EnumValues.Length];
    private static readonly TEnum[] EnumValues = (TEnum[])Enum.GetValues(typeof(TEnum));

    private static readonly Dictionary<TEnum, int> IndexLookup = BuildIndexLookup();
    private static Dictionary<TEnum, int> BuildIndexLookup()
    {
        var dict = new Dictionary<TEnum, int>();
        for (int i = 0; i < EnumValues.Length; i++)
            dict[EnumValues[i]] = i;
        return dict;
    }

    public TValue this[TEnum key]
    {
        get => values[IndexLookup[key]];
        set => values[IndexLookup[key]] = value;
    }
}

#if UNITY_EDITOR
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
        _reorderableList.headerHeight = 0;
        _reorderableList.footerHeight = 0;
        _reorderableList.drawElementCallback = (rect, index, active, focused) =>
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            float labelWidth = rect.width * 0.4f;
            Rect labelRect = new Rect(rect.x, rect.y, labelWidth, rect.height);
            Rect propertyRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, rect.height);
            EditorGUI.LabelField(labelRect, enumNames[index]);
            EditorGUI.PropertyField(propertyRect, valuesProp.GetArrayElementAtIndex(index), GUIContent.none);
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

        SerializedProperty valuesProp = property.FindPropertyRelative("values");
        if (valuesProp == null) return height;

        for (int index = 0; index < valuesProp.arraySize; index++)
        {
            SerializedProperty element = valuesProp.GetArrayElementAtIndex(index);
            height += EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }

    private string[] GetEnumNames()
    {
        Type enumType = fieldInfo.FieldType.GetGenericArguments()[0];
        return Enum.GetNames(enumType);
    }
}
#endif
