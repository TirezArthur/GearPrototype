using System.Globalization;
using UnityEngine;

[CreateAssetMenu(fileName = "Locale", menuName = "Localization/Locale")]
public class Locale : ScriptableObject
{
    [SerializeField] private string _cultureString = "";
    private CultureInfo _cultureInfo;

    public string CultureString => _cultureString;
    public CultureInfo CultureInfo => _cultureInfo;

    private void Awake()
    {
        _cultureInfo = new CultureInfo(_cultureString);
    }
}
