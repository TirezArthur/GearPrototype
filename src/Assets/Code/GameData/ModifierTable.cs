using UnityEngine;
using System.Runtime.Serialization;
using Random = Unity.Mathematics.Random;

[CreateAssetMenu(fileName = "ModifierTable", menuName = "GameData/ModifierTable")]
public class ModifierTable : ScriptableObject
{
    [SerializeField] private float[] _weights = new float[0];
    [SerializeField] private float _totalWeight = 0f;
    [SerializeReference] private IItemModifier[] _modifiers = new IItemModifier[0];

    // TODO Optimize with binary trees if this gets expensive
    public IItemModifier GetModifier(ref Random random)
    {
        if (_weights.Length != _modifiers.Length) throw new SerializationException("Array sizes do not match");

        float value = random.NextFloat(_totalWeight);
        for(int index = 0; index < _modifiers.Length; index++)
        {
            if (value <= _weights[index]) return _modifiers[index];
            else value -= _weights[index];
        }
        return null; // Should never get here
    }
}
