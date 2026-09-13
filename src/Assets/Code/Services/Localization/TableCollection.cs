using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TableCollection", menuName = "Localization/Table Collection")]
public class TableCollection : ScriptableObject
{
    [Serializable]
    public struct StringReference
    {
        public string String;
        public ulong Key;
    }

    [SerializeField] private Locale[] _locales;
    [SerializeField] private StringTable[] _stringTables;
    [SerializeField] private StringReference[] _stringReferences;

    public IReadOnlyList<StringReference> StringReferences => _stringReferences;
}
