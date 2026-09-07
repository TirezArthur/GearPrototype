using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public interface IItemModifier
{
    public static TableReference TableReference => "ModifierList";
    public long TableEntryReference { get; }
}

[Serializable]
public struct AddHealthModifier : IItemModifier
{
    public static long _tableEntryReference = LocalizationSettings.StringDatabase.GetTableEntry(IItemModifier.TableReference, "ADD_HEALTH").Entry.KeyId;
    [SerializeField] private int _value;

    public long TableEntryReference => _tableEntryReference;
    public int Value => _value;

    public AddHealthModifier(int value)
    {
        _value = value;
    }
}

[Serializable]
public struct IncreaseHealthModifier : IItemModifier
{
    public static long _tableEntryReference = LocalizationSettings.StringDatabase.GetTableEntry(IItemModifier.TableReference, "INCREASE_HEALTH").Entry.KeyId;
    [SerializeField] private int _value;

    public long TableEntryReference => _tableEntryReference;
    public int Value => _value;

    public IncreaseHealthModifier(int value)
    {
        _value = value;
    }
}

[Serializable]
public struct IncreaseLevelModifier : IItemModifier
{
    public static long _tableEntryReference = LocalizationSettings.StringDatabase.GetTableEntry(IItemModifier.TableReference, "INCREASE_SKILL_LVL").Entry.KeyId;
    [SerializeField] private int _value;
    [SerializeField] private string _skill;

    public long TableEntryReference => _tableEntryReference;
    public int Value => _value;
    public string Skill => _skill;

    public IncreaseLevelModifier(string skill, int value)
    {
        _skill = skill;
        _value = value;
    }
}
