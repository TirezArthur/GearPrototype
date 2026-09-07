using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnumMapping<TEnum, TValue> where TEnum : Enum
{
    [SerializeField] private TValue[] values = new TValue[EnumValues.Length];
    private static readonly TEnum[] EnumValues = (TEnum[])Enum.GetValues(typeof(TEnum));

    private static readonly Dictionary<TEnum, int> IndexLookup = BuildIndexLookup();
    private static Dictionary<TEnum, int> BuildIndexLookup()
    {
        var dict = new Dictionary<TEnum, int>();
        for (int i = 0; i < EnumValues.Length; i++)
            dict[EnumValues[i]] = i;
        return dict;
    }

    public TValue this[TEnum key]
    {
        get => values[IndexLookup[key]];
        set => values[IndexLookup[key]] = value;
    }
}
