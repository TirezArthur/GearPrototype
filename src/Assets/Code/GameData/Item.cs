using UnityEngine;

public enum BorderType
{
    Default = 0,
    Paper = 1,
}

public enum Rarity
{
    Common      = 0,
    Uncommon    = 1,
    Rare        = 2,
    Epic        = 3,
    Special     = 4
}

public class Item
{
    public IItemModifier[] Modifiers;
    public BorderType BorderType;
    public Rarity Rarity;
}
