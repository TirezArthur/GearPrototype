using System;
using System.Reflection;
using System.Text.RegularExpressions;

public class LocalizationFormatter : IFormatProvider, ICustomFormatter
{
    private static readonly Regex ArgumentTokenRegex = new (@"\{\{|\}\}|\{([^{}]+)\}", RegexOptions.Compiled);
    private static readonly Regex ValidTokenRegex = new(@"^\d+(:\S+)?$", RegexOptions.Compiled);
    private static readonly Regex DotReplaceRegex = new(@"(^\d+)\.(\S+)$", RegexOptions.Compiled);

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

            PropertyInfo property = currentType.GetProperty(identifier, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null)
            {
                currentValue = property.GetValue(currentValue);
                continue;
            }

            FieldInfo field = currentType.GetField(identifier, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null)
            {
                currentValue = field.GetValue(currentValue);
                continue;
            }

            throw new FormatException(string.Format("Identifier \"{0}\" is not a valid field or property for object {{{1}}}", identifier, currentValue));
        }

        return currentValue.ToString() + new string('}', trailingBraces);
    }
}
