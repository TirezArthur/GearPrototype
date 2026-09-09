using UnityEngine;
using Random = Unity.Mathematics.Random;
using System.Linq;
using System;

public sealed class ItemGenerator : Service
{
    private ModifierTable _weaponModifiers;
    private Random _randomState;

    protected override void Awake()
    {
        base.Awake();
        _weaponModifiers = Resources.Load<ModifierTable>("WeaponModifiers");
        _randomState = new Random((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
    }

    // TODO cleanup this mess when more item types are supported
    public Item GenerateItem(Rarity rarity)
    {
        Debug.Log(string.Format("Generating Item with {0}: {1}", nameof(Rarity), rarity));
        Item item = new();
        item.Rarity = rarity;
        item.BorderType = BorderType.Default;
        switch (rarity)
        {
            default:
            case Rarity.Common:
                item.Modifiers = new IItemModifier[0];
                return item;
            case Rarity.Uncommon:
                item.Modifiers = new IItemModifier[1];
                GenerateModifier(item.Modifiers, 0);
                return item;
            case Rarity.Rare:
                item.Modifiers = new IItemModifier[2];
                GenerateModifiers(item.Modifiers, 0, 1);
                return item;
            case Rarity.Epic:
                item.Modifiers = new IItemModifier[3];
                GenerateModifiers(item.Modifiers, 0, 2);
                return item;
            case Rarity.Special:
                item.Modifiers = new IItemModifier[3];
                GenerateModifiers(item.Modifiers, 0, 2);
                return item;
        }
    }

    public void GenerateModifiers(IItemModifier[] modifierArray, int startIndex, int endIndex)
    {
        Array.Clear(modifierArray, startIndex, 1 + endIndex - startIndex);
        for (int index = startIndex; index <= endIndex; index++) GenerateModifier(modifierArray, index);
    }

    public void GenerateModifier(IItemModifier[] modifierArray, int index)
    {
        modifierArray[index] = null;
        IItemModifier modifier = null;
        do modifier = _weaponModifiers.GetModifier(ref _randomState);
        while (modifierArray.Any(existing => existing != null && existing.GetType() == modifier.GetType()));
        Debug.Log(string.Format("Generated mod {0} with random state: {1}", modifier, _randomState.state));
        modifier.Randomize(ref _randomState);
        modifierArray[index] = modifier;
    }
}
