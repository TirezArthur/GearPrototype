using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public class LocalizationFormatter : IFormatProvider, ICustomFormatter
{
    private static readonly Regex ArgumentTokenRegex = new (@"\{\{|\}\}|\{([^{}]+)\}", RegexOptions.Compiled);
    private static readonly Regex ValidTokenRegex = new(@"^\d+(:\S+)?$", RegexOptions.Compiled);
    private static readonly Regex DotReplaceRegex = new(@"(^\d+)\.(\S+)$", RegexOptions.Compiled);
    private static readonly Regex IndexRegex = new(@"^(\w+)(?:\[(\d+)\])+$", RegexOptions.Compiled);

    public object GetFormat(Type formatType)
    {
        if (formatType == typeof(ICustomFormatter)) return this;
        else return null;
    }

    public static string PreProcess(string formatString)
    {
        return ArgumentTokenRegex.Replace(formatString, match =>
        {
            if (match.Value == "{{" || match.Value == "}}") return match.Value; // Escaped colon

            string content = match.Groups[1].Value;

            if (ValidTokenRegex.IsMatch(content)) return match.Value; // Already conforms to the pattern 0 or 0:Content

            Match dotSplit = DotReplaceRegex.Match(content); // Split values that conform to 0.Content and replace the . with :
            if (dotSplit.Success) return string.Format("{{{0}:{1}}}", dotSplit.Groups[1].Value, dotSplit.Groups[2].Value);

            return string.Format("{{0:{0}}}", content); // All other strings have no index specifier at all and should refer to 0
        });
    }

    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
        if (format == null || format == "") return arg.ToString();

        format = Regex.Replace(format, @"\s+", ""); // No whitespace >:(

        // Know issue with string.Format
        // https://learn.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/txafckwd(v=vs.100)#escaping-braces
        int trailingBraces = 0;
        if (format[^1] == '}')
        {
            int oldLength = format.Length;
            format = format.TrimEnd('}');
            trailingBraces = oldLength - format.Length;
        }

        string[] Identifiers = Regex.Split(format, @"\.");

        object currentValue = arg;
        for (int identifierIdx = 0; identifierIdx < Identifiers.Length; identifierIdx++)
        {
            Type currentType = currentValue.GetType();
            string identifier = Identifiers[identifierIdx];

            Match indexMatch = IndexRegex.Match(identifier);
            int[] indices = null;
            if (indexMatch.Success)
            {
                identifier = indexMatch.Groups[1].Value;
                indices = indexMatch.Groups[2].Captures
                    .Select(capture => int.Parse(capture.Value))
                    .ToArray();
            }

            PropertyInfo property = currentType.GetProperty(identifier, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null)
            {
                currentValue = property.GetValue(currentValue);
                if (indices != null) foreach (int index in indices) currentValue = GetIndexedValue(currentValue, index);
                continue;
            }

            FieldInfo field = currentType.GetField(identifier, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                currentValue = field.GetValue(currentValue);
                if (indices != null) foreach (int index in indices) currentValue = GetIndexedValue(currentValue, index);
                continue;
            }

            throw new FormatException(string.Format("Identifier \"{0}\" is not a valid field or property for object {{{1}}}", identifier, currentValue));
        }

        return currentValue.ToString() + new string('}', trailingBraces);
    }

    private object GetIndexedValue(object target, int index)
    {
        if (target is IList list) return list[index];

        // Support for custom index functions
        Type type = target.GetType();
        PropertyInfo indexer = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .FirstOrDefault(property => property.GetIndexParameters().Length == 1
                               && property.GetIndexParameters()[0].ParameterType == typeof(int));

        if (indexer != null) return indexer.GetValue(target, new object[] { index });

        throw new FormatException(string.Format("Value of type \"{0}\" does not support indexing", type.Name));
    }
}
