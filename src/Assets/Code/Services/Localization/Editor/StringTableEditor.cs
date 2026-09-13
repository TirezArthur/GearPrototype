using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StringTable))]
public class StringTableEditor : Editor
{
    private SerializedProperty _localeProperty;
    private SerializedProperty _parentCollectionProperty;
    private SerializedProperty _stringsProperty;

    private const float LabelWidth = 180f;
    private const float RowHeight = 60f;
    private Vector2 _scrollPosition;

    private void OnEnable()
    {
        _localeProperty = serializedObject.FindProperty("_locale");
        _parentCollectionProperty = serializedObject.FindProperty("_parentCollection");
        _stringsProperty = serializedObject.FindProperty("_strings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(_localeProperty);
            EditorGUILayout.PropertyField(_parentCollectionProperty);
        }

        TableCollection tableCollection = (TableCollection)_parentCollectionProperty.objectReferenceValue;

        if (tableCollection == null) return;

        IReadOnlyList<TableCollection.StringReference> stringReferences = tableCollection.StringReferences;

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(Mathf.Min(400f, stringReferences.Count * (RowHeight + 4f) + 4f)));

        for (int index = 0; index < stringReferences.Count && index < _stringsProperty.arraySize; index++)
        {
            SerializedProperty stringProperty = _stringsProperty.GetArrayElementAtIndex(index);

            Rect rowRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(RowHeight));

            if (index % 2 == 0)
            {
                EditorGUI.DrawRect(rowRect, new Color(0f, 0f, 0f, 0.08f));
            }

            EditorGUILayout.LabelField(
                stringReferences[index].String,
                EditorStyles.miniBoldLabel,
                GUILayout.Width(LabelWidth),
                GUILayout.Height(RowHeight));

            stringProperty.stringValue = EditorGUILayout.TextArea(
                stringProperty.stringValue,
                GUILayout.Height(RowHeight),
                GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();
    }
}
