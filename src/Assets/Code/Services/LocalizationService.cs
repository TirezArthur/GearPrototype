using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public sealed class LocalizationService : Service
{
    public struct Translation
    {
        public bool Formatted;
        public string Value;
    }

    private Locale[] _locales;
    private TableCollection[] _tables;
    private Dictionary<StringReference, Translation>[] _tableDictionaries;
    private Locale _current;

    public event UnityAction LocaleChanged = delegate { };

    public static ulong HashKey(string key)
    {
        Span<byte> bytes = stackalloc byte[Encoding.UTF8.GetByteCount(key)];
        Encoding.UTF8.GetBytes(key, bytes);
        return XxHash64.HashToUInt64(bytes);
    }

    protected override void Awake()
    {
        base.Awake();
        _locales = Resources.LoadAll<Locale>("Localization/");
        _tables = Resources.LoadAll<TableCollection>("Localization/");
        _tableDictionaries = (Dictionary<StringReference, Translation>[]) Array.CreateInstance(typeof(Dictionary<StringReference, Translation>), _tables.Length);

        SetLocale(_locales.FirstOrDefault(locale => locale.CultureString == "en")); // TODO Make a way to change this default easily
    }

    public void SetLocale(Locale locale)
    {
        if (!_locales.Contains(locale)) throw new ArgumentException("Locale is not in list of loaded locales", nameof(locale));
        if (_current == locale) return;
        _current = locale;

        for (int index = 0; index < _tables.Length; index++)
        {
            TableCollection table = _tables[index];
            _tableDictionaries[index] = LoadTable(table, _current);
        }
    }

    private Dictionary<StringReference, Translation> LoadTable(TableCollection table, Locale locale)
    {
        StringTable stringTable = table.GetTable(locale);
        Dictionary<StringReference, Translation> dictionary = new();

        if (stringTable == null) return dictionary;

        for (int index = 0; index < table.StringReferences.Count && index < table.FormattedStrings.Count && index < stringTable.Strings.Count; index++)
        {
            StringReference stringReference = table.StringReferences[index];
            bool formatted = table.FormattedStrings[index];
            string value = stringTable.Strings[index];

            if (formatted) value = LocalizationFormatter.PreProcess(value);

            Translation translation = new();
            translation.Formatted = formatted;
            translation.Value = value;
            dictionary[stringReference] = translation;
        }

        return dictionary;
    }

    public string LocalizeString(string tableName, StringReference stringReference, params object[] args)
    {
        if (string.IsNullOrEmpty(tableName)) return "NO_TRANSLATION_FOUND";
        if (string.IsNullOrEmpty(stringReference.String)) return "NO_TRANSLATION_FOUND";

        TableCollection table = _tables.FirstOrDefault(table => table.name == tableName);
        if (table == null) return "NO_TRANSLATION_FOUND";
        int tableIndex = Array.IndexOf(_tables, table);

        Dictionary<StringReference, Translation> dictionary = _tableDictionaries[tableIndex];
        if (!dictionary.ContainsKey(stringReference)) return "NO_TRANSLATION_FOUND";

        Translation translation = dictionary[stringReference];
        if (translation.Formatted) return string.Format(new LocalizationFormatter(), translation.Value, args);
        else return translation.Value;
    }
}
