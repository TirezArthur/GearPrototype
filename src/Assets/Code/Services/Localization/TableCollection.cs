using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StringReference : IEquatable<StringReference>
{
    [SerializeField] private string _string;
    [SerializeField] private ulong _key;

    public readonly ulong Key => _key;
    public readonly string String => _string;

    public StringReference(string stringKey)
    {
        _string = stringKey;
        _key = LocalizationService.HashKey(stringKey);
    }
    public StringReference(StringReference stringReference)
    {
        _string = stringReference._string;
        _key = stringReference._key;
    }

    public readonly bool Equals(StringReference other)
    {
        return _key == other._key && _string == other._string;
    }

    public override readonly bool Equals(object obj)
    {
        return obj is StringReference other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return _key.GetHashCode();
    }

    public static bool operator ==(StringReference left, StringReference right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(StringReference left, StringReference right)
    {
        return !left.Equals(right);
    }
}

[CreateAssetMenu(fileName = "TableCollection", menuName = "Localization/Table Collection")]
public class TableCollection : ScriptableObject
{
    [SerializeField] private Locale[] _locales;
    [SerializeField] private StringTable[] _stringTables;
    [SerializeField] private StringReference[] _stringReferences;
    [SerializeField] private bool[] _formattedStrings;

    public IReadOnlyList<StringReference> StringReferences => _stringReferences;
    public IReadOnlyList<bool> FormattedStrings => _formattedStrings;

    public StringTable GetTable(Locale locale)
    {
        int index = Array.IndexOf(_locales, locale);
        if (index == -1) return null;
        return _stringTables[index];
    }
}
