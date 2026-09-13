using System;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public interface IItemModifier : ISerializationCallbackReceiver
{
    public StringReference TableEntryReference { get; }
    public void Randomize(ref Random random);
}

[Serializable]
public struct AddHealthModifier : IItemModifier
{
    public static readonly StringReference _stringReference = new("ADD_HEALTH");
    [SerializeField] private IntModifierValue _value;

    public readonly StringReference TableEntryReference => _stringReference;
    public int Value => _value.Value;

    public void Randomize(ref Random random)
    {
        _value.Randomize(ref random);
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
    public static readonly StringReference _stringReference = new("INCREASE_HEALTH");
    [SerializeField] private PercentModifierValue _value;

    public readonly StringReference TableEntryReference => _stringReference;
    public int Value => _value.Value;

    public void Randomize(ref Random random)
    {
        _value.Randomize(ref random);
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
    public static readonly StringReference _stringReference = new("INCREASE_LEVEL");
    [SerializeField] private IntModifierValue _value;
    [SerializeField] private string _skill;

    public readonly StringReference TableEntryReference => _stringReference;
    public int Value => _value.Value;
    public string Skill => _skill;

    public void Randomize(ref Random random)
    {
        _value.Randomize(ref random);

        object obj = random;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        _value.SetRange(1, 3);
    }
}
