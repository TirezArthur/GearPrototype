using System;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public interface IModifierValue<TValue>
{
    public TValue Value { get; }
    public void Randomize(ref Random random);
}

[Serializable]
public struct IntModifierValue : IModifierValue<int>
{
    [NonSerialized] private int _min;
    [NonSerialized] private int _max;
    [SerializeField] private float _value;

    public readonly int Min => _min;
    public readonly int Max => _max;
    public readonly int Value => Mathf.RoundToInt(math.lerp(_min, _max, _value));

    public IntModifierValue(int min, int max)
    {
        _min = min;
        _max = max;
        _value = 0f;
    }

    public void Randomize(ref Random random)
    {
        _value = random.NextFloat();
    }

    public void SetRange(int min, int max)
    {
        _min = min;
        _max = max;
    }
}

[Serializable]
public struct PercentModifierValue : IModifierValue<int>
{
    [NonSerialized] private int _min;
    [NonSerialized] private int _max;
    [SerializeField] private float _value;

    public readonly int Min => _min;
    public readonly int Max => _max;
    public readonly int Value => Mathf.RoundToInt(math.lerp(_min, _max, _value));
    public readonly float FloatValue => Value / 100f;

    public PercentModifierValue(int min, int max)
    {
        _min = min;
        _max = max;
        _value = 0f;
    }

    public void Randomize(ref Random random)
    {
        _value = random.NextFloat();
    }

    public void SetRange(int min, int max)
    {
        _min = min;
        _max = max;
    }
}
