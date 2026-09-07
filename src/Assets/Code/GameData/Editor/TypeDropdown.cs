using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class TypeDropdown : AdvancedDropdown
{
    private readonly Type[] _types;
    private readonly Action<Type> _onTypeSelected;

    public TypeDropdown(AdvancedDropdownState state, Type[] types, Action<Type> onTypeSelected) : base(state)
    {
        _types = types;
        _onTypeSelected = onTypeSelected;
        minimumSize = new Vector2(220, 300);
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        AdvancedDropdownItem root = new AdvancedDropdownItem("Types");

        foreach (Type type in _types) 
        {
            root.AddChild(new DropdownItem(ObjectNames.NicifyVariableName(type.Name), type));
        }

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        if (item is DropdownItem modifierType)
            _onTypeSelected?.Invoke(modifierType.Type);
    }

    private class DropdownItem : AdvancedDropdownItem
    {
        public readonly Type Type;
        public DropdownItem(string name, Type type) : base(name) => Type = type;
    }
}

