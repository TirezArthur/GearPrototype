using UnityEngine;

[CreateAssetMenu(fileName = "StringTable", menuName = "Localization/String Table")]
public class StringTable : ScriptableObject
{
    [SerializeField] private Locale _locale;
    [SerializeField] private TableCollection _parentCollection;
    [SerializeField] private string[] _strings;
}
