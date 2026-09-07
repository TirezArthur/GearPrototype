using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(ModifierTable))]
public class ModifierTableEditor : Editor
{
    private SerializedProperty _weightsProp;
    private SerializedProperty _modifiersProp;
    private SerializedProperty _totalWeightProp;
    private ReorderableList _list;
    private Type[] _modifierTypes;
    private AdvancedDropdownState _dropdownState;

    private const float WeightFieldWidth = 60f;
    private const float Spacing = 6f;

    private void OnEnable()
    {
        _weightsProp = serializedObject.FindProperty("_weights");
        _modifiersProp = serializedObject.FindProperty("_modifiers");
        _totalWeightProp = serializedObject.FindProperty("_totalWeight");
        _dropdownState = new AdvancedDropdownState();

        // Create display classes
        _modifierTypes = TypeCache.GetTypesDerivedFrom<IItemModifier>()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => Attribute.IsDefined(t, typeof(SerializableAttribute)))
            .OrderBy(t => t.Name)
            .ToArray();

        _list = new ReorderableList(serializedObject, _weightsProp, true, true, true, true);
        _list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Weights Table");
        _list.drawElementCallback = DrawElement;
        _list.onAddCallback = OnAdd;
        _list.onRemoveCallback = OnRemove;
        _list.onReorderCallbackWithDetails = OnReorder;

        UpdateStats();
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (index >= _weightsProp.arraySize || index >= _modifiersProp.arraySize)
            return;

        rect.height = EditorGUIUtility.singleLineHeight;

        Rect weightRect = new Rect(rect.x, rect.y, WeightFieldWidth, rect.height);
        Rect modifierRect = new Rect(rect.x + WeightFieldWidth + Spacing, rect.y,
            rect.width - WeightFieldWidth - Spacing, rect.height);

        SerializedProperty weightElement = _weightsProp.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(weightRect, weightElement, GUIContent.none);

        SerializedProperty modifierElement = _modifiersProp.GetArrayElementAtIndex(index);
        string label = modifierElement.managedReferenceValue != null
            ? ObjectNames.NicifyVariableName(modifierElement.managedReferenceValue.GetType().Name)
            : "(None)";

        if (EditorGUI.DropdownButton(modifierRect, new GUIContent(label), FocusType.Keyboard))
        {
            TypeDropdown dropdown = new TypeDropdown(_dropdownState, _modifierTypes, selectedType =>
            {
                serializedObject.Update();
                SerializedProperty element = _modifiersProp.GetArrayElementAtIndex(index);
                element.managedReferenceValue = selectedType != null
                    ? Activator.CreateInstance(selectedType)
                    : null;
                serializedObject.ApplyModifiedProperties();
            });
            dropdown.Show(modifierRect);
        }
    }

    private void OnAdd(ReorderableList list)
    {
        serializedObject.Update();
        int index = _weightsProp.arraySize;

        _weightsProp.arraySize++;
        _modifiersProp.arraySize++;

        _weightsProp.GetArrayElementAtIndex(index).floatValue = 1f;
        _modifiersProp.GetArrayElementAtIndex(index).managedReferenceValue = null;

        serializedObject.ApplyModifiedProperties();
        UpdateStats();
    }

    private void OnRemove(ReorderableList list)
    {
        int index = list.index >= 0 ? list.index : _weightsProp.arraySize - 1;
        if (index < 0) return;

        serializedObject.Update();

        _modifiersProp.DeleteArrayElementAtIndex(index);
        _weightsProp.DeleteArrayElementAtIndex(index);

        serializedObject.ApplyModifiedProperties();

        UpdateStats();
    }

    private void OnReorder(ReorderableList list, int oldIndex, int newIndex)
    {
        serializedObject.Update();
        _modifiersProp.MoveArrayElement(oldIndex, newIndex);
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        _list.DoLayoutList();
        bool statsDirty = EditorGUI.EndChangeCheck();

        serializedObject.ApplyModifiedProperties();

        if (statsDirty) UpdateStats();
        DisplayStats();
    }

    private void DisplayStats()
    {
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Total Weight:", string.Format("{0:#.000}", _totalWeightProp.floatValue));

        if (_list.selectedIndices.Count <= 0) return;
        float weight = _weightsProp.GetArrayElementAtIndex(_list.selectedIndices[0]).floatValue;
        EditorGUILayout.LabelField("Selected:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Weight:", string.Format("{0:#.000}/{1:#.000}", weight, _totalWeightProp.floatValue));
        EditorGUILayout.LabelField("Chance:", string.Format("{0:#.0%}", weight / _totalWeightProp.floatValue));
    }

    private void UpdateStats()
    {
        serializedObject.Update();

        float totalWeight = 0f;
        for (int index = 0; index < _weightsProp.arraySize; index++) totalWeight += _weightsProp.GetArrayElementAtIndex(index).floatValue;

        _totalWeightProp.floatValue = totalWeight;
        serializedObject.ApplyModifiedProperties();
    }
}
