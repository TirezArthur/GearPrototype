using System;
using System.Globalization;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(Locale))]
public class LocaleEditor : Editor
{
    private SerializedProperty _cultureStringProperty;
    private CultureInfo[] _cultures;
    private string[] _cultureDisplayNames;


    private void OnEnable()
    {
        _cultureStringProperty = serializedObject.FindProperty("_cultureString");
        _cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures).OrderBy(culture => culture.Name, StringComparer.Ordinal).ToArray();
        _cultureDisplayNames = _cultures.Select(culture => string.IsNullOrEmpty(culture.Name) ? "Invariant" : string.Format("{0} ({1})", culture.Name, culture.EnglishName)).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        int currentIndex = Array.FindIndex(_cultures, culture => culture.Name == _cultureStringProperty.stringValue);

        if (currentIndex < 0) currentIndex = 0;

        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUILayout.Popup("Culture", currentIndex, _cultureDisplayNames);
        bool changed = EditorGUI.EndChangeCheck();

        if (changed || string.IsNullOrEmpty(_cultureStringProperty.stringValue))
        {
            _cultureStringProperty.stringValue = _cultures[newIndex].Name;
        }

        EditorGUILayout.PropertyField(_cultureStringProperty);

        serializedObject.ApplyModifiedProperties();

        DrawPreview(_cultures[newIndex]);
    }

    private void DrawPreview(CultureInfo culture)
    {
        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

        DateTime now = DateTime.Now;
        double sampleNumber = 1234567.891;

        string shortDate = now.ToString("d", culture);
        string longDate = now.ToString("D", culture);
        string shortTime = now.ToString("t", culture);
        string number = sampleNumber.ToString("N", culture);
        string currency = sampleNumber.ToString("C", culture);
        string percent = (0.4567f).ToString("P", culture);

        EditorGUILayout.HelpBox(
            $"Short date:\t{shortDate}\n" +
            $"Long date:\t{longDate}\n" +
            $"Time:\t\t{shortTime}\n" +
            $"Number:\t\t{number}\n" +
            $"Currency:\t{currency}\n" +
            $"Percent:\t\t{percent}",
            MessageType.None);
    }
}
