using UnityEngine;

public enum BorderStyle
{
    Default     = 0,
    Paper       = 1,
}

public class Item
{
    public IItemModifier[] Modifiers;
    public BorderStyle BorderStyle;
}
