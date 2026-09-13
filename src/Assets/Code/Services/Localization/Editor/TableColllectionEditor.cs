using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(TableCollection))]
public class TableColllectionEditor : Editor
{
    private SerializedProperty _localesProperty;
    private SerializedProperty _stringTablesProperty;
    private SerializedProperty _stringReferencesProperty;
    private SerializedProperty _formattedStringsProperty;
    private ReorderableList _localeList;
    private ReorderableList _stringReferenceList;
    private int _localePickerControlId;

    private void OnEnable()
    {
        _localesProperty = serializedObject.FindProperty("_locales");
        _stringTablesProperty = serializedObject.FindProperty("_stringTables");
        _stringReferencesProperty = serializedObject.FindProperty("_stringReferences");
        _formattedStringsProperty = serializedObject.FindProperty("_formattedStrings");

        _localeList = new ReorderableList(serializedObject, _localesProperty, true, true, true, true);
        _localeList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Supported Locales");
        _localeList.drawElementCallback += DrawLocale;
        _localeList.onAddCallback += AddLocale;
        _localeList.onRemoveCallback += RemoveLocale;
        _localeList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 2;

        _stringReferenceList = new ReorderableList(serializedObject, _stringReferencesProperty, true, true, true, true);
        _stringReferenceList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "String Keys");
        _stringReferenceList.drawElementCallback += DrawStringReference;
        _stringReferenceList.onAddCallback += AddStringReference;
        _stringReferenceList.onRemoveCallback += RemoveStringReference;
        _stringReferenceList.onReorderCallbackWithDetails += ReorderStringReference;
        _stringReferenceList.elementHeight = EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        _localeList.DoLayoutList();
        _stringReferenceList.DoLayoutList();

        HandleLocalePicker();

        serializedObject.ApplyModifiedProperties();
    }

    private void RemoveLocale(ReorderableList reorderableList)
    {
        if (!EditorUtility.DisplayDialog("Remove locale", "Are you sure you want to remove this locale from this table collection and delete the associated table?", "Yes", "Cancel")) return;

        int index = reorderableList.index;

        if (index < 0 || index >= _stringTablesProperty.arraySize) return;

        StringTable stringTable = _stringTablesProperty.GetArrayElementAtIndex(index).objectReferenceValue as StringTable;

        if (stringTable != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(stringTable);

            if (!string.IsNullOrEmpty(assetPath)) AssetDatabase.DeleteAsset(assetPath);
        }

        _stringTablesProperty.DeleteArrayElementAtIndex(index);
        _localesProperty.DeleteArrayElementAtIndex(index);

        serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }

    private void AddLocale(ReorderableList reorderableList)
    {
        _localePickerControlId = GUIUtility.GetControlID(FocusType.Passive);
        EditorGUIUtility.ShowObjectPicker<Locale>(null, false, string.Empty, _localePickerControlId);
    }

    private void DrawLocale(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index >= _localesProperty.arraySize || index >= _stringTablesProperty.arraySize) return;

        rect.y += EditorGUIUtility.standardVerticalSpacing;
        rect.height = EditorGUIUtility.singleLineHeight;
        SerializedProperty localeProperty = _localesProperty.GetArrayElementAtIndex(index);
        SerializedProperty stringTableProperty = _stringTablesProperty.GetArrayElementAtIndex(index);

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(rect, localeProperty, GUIContent.none);
        if (EditorGUI.EndChangeCheck())
        {
            StringTable stringTable = _stringTablesProperty.GetArrayElementAtIndex(index).objectReferenceValue as StringTable;

            if (stringTable != null)
            {
                SerializedObject tableSerializedObject = new SerializedObject(stringTable);
                SerializedProperty tableLocaleProperty = tableSerializedObject.FindProperty("_locale");

                tableLocaleProperty.objectReferenceValue = localeProperty.objectReferenceValue;

                tableSerializedObject.ApplyModifiedProperties();
            }
        }

        using (new EditorGUI.DisabledScope(true))
        {
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(rect, stringTableProperty, GUIContent.none);
        }
    }

    private void CreateLocaleEntry(Locale locale)
    {
        if (locale == null) return;

        serializedObject.Update();

        int index = _localesProperty.arraySize;
        _localesProperty.arraySize++;
        _stringTablesProperty.arraySize++;

        _localesProperty.GetArrayElementAtIndex(index).objectReferenceValue = locale;

        StringTable newTable = CreateStringTableAsset(locale);
        _stringTablesProperty.GetArrayElementAtIndex(index).objectReferenceValue = newTable;

        serializedObject.ApplyModifiedProperties();
    }

    private StringTable CreateStringTableAsset(Locale locale)
    {
        string collectionPath = AssetDatabase.GetAssetPath(serializedObject.targetObject);
        string directory = Path.GetDirectoryName(collectionPath);
        string collectionName = Path.GetFileNameWithoutExtension(collectionPath);

        string fileName = string.Format("{0}-{1}", collectionName, locale.CultureString);

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(directory, $"{fileName}.asset"));

        StringTable newTable = ScriptableObject.CreateInstance<StringTable>();
        AssetDatabase.CreateAsset(newTable, assetPath);

        SerializedObject tableSerialized = new SerializedObject(newTable);
        tableSerialized.FindProperty("_locale").objectReferenceValue = locale;
        tableSerialized.FindProperty("_parentCollection").objectReferenceValue = serializedObject.targetObject;
        tableSerialized.FindProperty("_strings").arraySize = _stringReferencesProperty.arraySize;
        tableSerialized.ApplyModifiedProperties();

        AssetDatabase.SaveAssets();

        return newTable;
    }

    private void HandleLocalePicker()
    {
        Event evt = Event.current;

        if (evt.type != EventType.ExecuteCommand) return;
        if (evt.commandName != "ObjectSelectorClosed") return;
        if (EditorGUIUtility.GetObjectPickerControlID() != _localePickerControlId) return;

        Locale picked = EditorGUIUtility.GetObjectPickerObject() as Locale;

        if (picked == null) return;
        foreach (SerializedProperty localeProperty in _localesProperty) if (localeProperty.objectReferenceValue == picked) return;
        CreateLocaleEntry(picked);
    }

    private void DrawStringReference(Rect rect, int index, bool isActive, bool isFocused)
    {
        serializedObject.Update();

        EditorGUI.PropertyField(rect, _stringReferencesProperty.GetArrayElementAtIndex(index), GUIContent.none);
        rect.y += EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        rect.height = EditorGUIUtility.singleLineHeight;
        _formattedStringsProperty.GetArrayElementAtIndex(index).boolValue = EditorGUI.Toggle(rect, "Formatted String: ", _formattedStringsProperty.GetArrayElementAtIndex(index).boolValue);

        serializedObject.ApplyModifiedProperties();
    }

    private void AddStringReference(ReorderableList reorderableList)
    {
        serializedObject.Update();

        _stringReferencesProperty.arraySize++;
        _formattedStringsProperty.arraySize++;
        for (int index = 0; index < _stringTablesProperty.arraySize; index++)
        {
            StringTable stringTable = _stringTablesProperty.GetArrayElementAtIndex(index).objectReferenceValue as StringTable;

            if (stringTable == null) continue;

            SerializedObject tableSerializedObject = new SerializedObject(stringTable);
            SerializedProperty stringsProperty = tableSerializedObject.FindProperty("_strings");

            stringsProperty.arraySize++;

            tableSerializedObject.ApplyModifiedProperties();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void RemoveStringReference(ReorderableList reorderableList)
    {
        serializedObject.Update();

        int selectedIndex = reorderableList.index;

        for (int index = 0; index < _stringTablesProperty.arraySize; index++)
        {
            StringTable stringTable = _stringTablesProperty.GetArrayElementAtIndex(index).objectReferenceValue as StringTable;

            if (stringTable == null) continue;

            SerializedObject tableSerializedObject = new SerializedObject(stringTable);
            SerializedProperty stringsProperty = tableSerializedObject.FindProperty("_strings");

            stringsProperty.DeleteArrayElementAtIndex(selectedIndex);

            tableSerializedObject.ApplyModifiedProperties();
        }
        _stringReferencesProperty.DeleteArrayElementAtIndex(selectedIndex);
        _formattedStringsProperty.DeleteArrayElementAtIndex(selectedIndex);

        serializedObject.ApplyModifiedProperties();
    }

    private void ReorderStringReference(ReorderableList list, int oldIndex, int newIndex)
    {
        serializedObject.Update();

        _stringReferencesProperty.MoveArrayElement(oldIndex, newIndex);
        _formattedStringsProperty.MoveArrayElement(oldIndex, newIndex);

        for (int index = 0; index < _stringTablesProperty.arraySize; index++)
        {
            StringTable stringTable = _stringTablesProperty.GetArrayElementAtIndex(index).objectReferenceValue as StringTable;

            if (stringTable == null) continue;

            SerializedObject tableSerializedObject = new SerializedObject(stringTable);
            SerializedProperty stringsProperty = tableSerializedObject.FindProperty("_strings");

            stringsProperty.MoveArrayElement(oldIndex, newIndex);

            tableSerializedObject.ApplyModifiedProperties();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(StringReference))]
public class StringReferencePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty stringProperty = property.FindPropertyRelative("_string");
        SerializedProperty keyProperty = property.FindPropertyRelative("_key");

        EditorGUI.BeginProperty(position, label, property);

        Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect keyRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.BeginChangeCheck();
        string newValue = EditorGUI.TextField(fieldRect, label, stringProperty.stringValue);

        if (EditorGUI.EndChangeCheck())
        {
            stringProperty.stringValue = newValue;
            keyProperty.ulongValue = string.IsNullOrEmpty(newValue) ? 0 : LocalizationService.HashKey(newValue);
        }

        using (new EditorGUI.DisabledScope(true))
        {
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 30f;

            EditorGUI.LabelField(keyRect, "Key: ", keyProperty.ulongValue.ToString());

            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
    }
}
