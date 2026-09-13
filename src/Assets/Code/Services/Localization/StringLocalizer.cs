using System;
using UnityEngine;
using UnityEngine.Events;

public class StringLocalizer : MonoBehaviour
{
    [SerializeField] private string _tableName;
    [SerializeField] private StringReference _stringReference;
    [SerializeField] private UnityEvent<string> _localizeStringEvent;
    private LocalizationService _localizationService;
    private string _currentString;
    private object[] _args = new object[0];

    public string CurrentString => _currentString;

    private void OnEnable()
    {
        _localizationService = ServiceLocator.GetService<LocalizationService>();
        _localizationService.LocaleChanged += RefreshString;
    }

    private void OnDisable()
    {
        _localizationService.LocaleChanged -= RefreshString;
    }

    public void SetStringReference(string tableName, StringReference stringReference)
    {
        _tableName = tableName;
        _stringReference = stringReference;
    }

    public void SetArguments(params object[] args)
    {
        _args = args;
    }

    public void RefreshString()
    {
        try
        {
            _currentString = _localizationService.LocalizeString(_tableName, _stringReference, _args);
            _localizeStringEvent.Invoke(_currentString);
        }
        catch (Exception exception)
        {
            Debug.LogError(string.Format("{} failed to localize strings due to the following exception", nameof(StringLocalizer)));
            Debug.LogException(exception);
        }
    }
}
