using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using Random = Unity.Mathematics.Random;

public interface IItemModifier : ISerializationCallbackReceiver
{
    public static TableReference TableReference => "ModifierList";
    public long TableEntryReference { get; }
    public void Randomize(ref Random random);
}

[Serializable]
public struct AddHealthModifier : IItemModifier
{
    public const long _tableEntryReference = 19054575616; //LocalizationSettings.StringDatabase.GetTableEntry(IItemModifier.TableReference, "ADD_HEALTH").Entry.KeyId;
    [SerializeField] private IntModifierValue _value;

    public readonly long TableEntryReference => _tableEntryReference;
    public int Value => _value.Value;

    public void Randomize(ref Random random)
    {
        _value.Randomize(ref random);
        Debug.Log(string.Format("Randomized mod value {{{0}}} with random state: {1}", _value.Value, random.state));
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        _value.SetRange(30, 50);
    }
}

[Serializable]
public struct IncreaseHealthModifier : IItemModifier
{
    public const long _tableEntryReference = 78030684160; //LocalizationSettings.StringDatabase.GetTableEntry(IItemModifier.TableReference, "INCREASE_HEALTH").Entry.KeyId;
    [SerializeField] private PercentModifierValue _value;

    public readonly long TableEntryReference => _tableEntryReference;
    public int Value => _value.Value;

    public void Randomize(ref Random random)
    {
        _value.Randomize(ref random);
        Debug.Log(string.Format("Randomized mod value {{{0}}} with random state: {1}", _value.Value, random.state));
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        _value.SetRange(15, 20);
    }
}

[Serializable]
public struct IncreaseLevelModifier : IItemModifier
{
    public const long _tableEntryReference = 230325862400; //LocalizationSettings.StringDatabase.GetTableEntry(IItemModifier.TableReference, "INCREASE_SKILL_LVL").Entry.KeyId;
    [SerializeField] private IntModifierValue _value;
    [SerializeField] private string _skill;

    public long TableEntryReference => _tableEntryReference;
    public int Value => _value.Value;
    public string Skill => _skill;

    public void Randomize(ref Random random)
    {
        _value.Randomize(ref random);
        Debug.Log(string.Format("Randomized mod value {{{0}}} with random state: {1}", _value.Value, random.state));
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        _value.SetRange(1, 3);
    }
}
