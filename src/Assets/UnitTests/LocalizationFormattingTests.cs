using System.Collections.Generic;
using NUnit.Framework;

internal class LocalizationFormattingTests
{
    private class IntValues
    {
        public int _field = 40;
        public int Property => _field;
    }

    private class ArrayValues
    {
        public int[] _field = new int[] { 0, 1, 2, 3 };
        public int[] Property => _field;
    }

    private class ListValues
    {
        private List<int> _field = new() { 0, 1, 2, 3 };
        public List<int> Property => _field;
    }

    private class ClassWithCustomIndexer
    {
        private readonly Dictionary<int, string> _data = new Dictionary<int, string>() { { 0, "zero" }, { 1, "one" }, { 2, "two" }, { 3, "three" } };
        public string this[int index]
        {
            get => _data.TryGetValue(index, out var v) ? v : null;
            set => _data[index] = value;
        }
    }

    private class IndexerValues
    {
        private ClassWithCustomIndexer _field = new();
        public ClassWithCustomIndexer Property => _field;
    }

    private class NestedArrayValues
    {
        private ArrayValues[] _field = new ArrayValues[] { new(), new() };
        public ArrayValues[] Property => _field;
    }

    [Test]
    public void Preprocessor()
    {
        string test = LocalizationFormatter.PreProcess("Test: {Value}");
        Assert.AreEqual("Test: {0:Value}", test);

        test = LocalizationFormatter.PreProcess("Test: {{{0}}}");
        Assert.AreEqual("Test: {{{0}}}", test);

        test = LocalizationFormatter.PreProcess("Test: {0.Value}");
        Assert.AreEqual("Test: {0:Value}", test);

        test = LocalizationFormatter.PreProcess("Test: {0:Value}");
        Assert.AreEqual("Test: {0:Value}", test);

        test = LocalizationFormatter.PreProcess("Test: {{{Value}}}");
        Assert.AreEqual("Test: {{{0:Value}}}", test);

        test = LocalizationFormatter.PreProcess("Test: {{{{{Value}}}}}");
        Assert.AreEqual("Test: {{{{{0:Value}}}}}", test);

        test = LocalizationFormatter.PreProcess("Test: {{ {{{Value}}} }}");
        Assert.AreEqual("Test: {{ {{{0:Value}}} }}", test);

        test = LocalizationFormatter.PreProcess("Test: {Value.Inner}");
        Assert.AreEqual("Test: {0:Value.Inner}", test);

        test = LocalizationFormatter.PreProcess("Test: {Value.Inner[3]}");
        Assert.AreEqual("Test: {0:Value.Inner[3]}", test);

        test = LocalizationFormatter.PreProcess("Test: {Value.Inner[1][3]}");
        Assert.AreEqual("Test: {0:Value.Inner[1][3]}", test);

        test = LocalizationFormatter.PreProcess("Test: {1.Value[5].Inner[1][3]}");
        Assert.AreEqual("Test: {1:Value[5].Inner[1][3]}", test);
    }

    [Test]
    public void IntValue()
    {
        IntValues input = new IntValues();

        string test = string.Format(new LocalizationFormatter(), "{0:_field}", input);
        Assert.AreEqual("40", test);

        test = string.Format(new LocalizationFormatter(), LocalizationFormatter.PreProcess("{0:Property}"), input);
        Assert.AreEqual("40", test);
    }

    [Test]
    public void ArrayValue()
    {
        ArrayValues input = new ArrayValues();

        string test = string.Format(new LocalizationFormatter(), "{0:_field[0]}", input);
        Assert.AreEqual("0", test);
        test = string.Format(new LocalizationFormatter(), "{0:_field[2]}", input);
        Assert.AreEqual("2", test);

        test = string.Format(new LocalizationFormatter(), "{0:Property[0]}", input);
        Assert.AreEqual("0", test);
        test = string.Format(new LocalizationFormatter(), "{0:Property[2]}", input);
        Assert.AreEqual("2", test);
    }

    [Test]
    public void ListValue()
    {
        ListValues input = new ListValues();

        string test = string.Format(new LocalizationFormatter(), "{0:_field[0]}", input);
        Assert.AreEqual("0", test);
        test = string.Format(new LocalizationFormatter(), "{0:_field[2]}", input);
        Assert.AreEqual("2", test);

        test = string.Format(new LocalizationFormatter(), "{0:Property[0]}", input);
        Assert.AreEqual("0", test);
        test = string.Format(new LocalizationFormatter(), "{0:Property[2]}", input);
        Assert.AreEqual("2", test);
    }

    [Test]
    public void CustomIndexerValue()
    {
        IndexerValues input = new IndexerValues();

        string test = string.Format(new LocalizationFormatter(), "{0:_field[0]}", input);
        Assert.AreEqual("zero", test);
        test = string.Format(new LocalizationFormatter(), "{0:_field[2]}", input);
        Assert.AreEqual("two", test);

        test = string.Format(new LocalizationFormatter(), "{0:Property[0]}", input);
        Assert.AreEqual("zero", test);
        test = string.Format(new LocalizationFormatter(), "{0:Property[2]}", input);
        Assert.AreEqual("two", test);
    }

    [Test]
    public void NestedArrayValue()
    {
        NestedArrayValues input = new NestedArrayValues();

        string test = string.Format(new LocalizationFormatter(), "{0:_field[0]._field[0]}", input);
        Assert.AreEqual("0", test);
        test = string.Format(new LocalizationFormatter(), "{0:_field[1].Property[1]}", input);
        Assert.AreEqual("1", test);

        test = string.Format(new LocalizationFormatter(), "{0:Property[0]._field[2]}", input);
        Assert.AreEqual("2", test);
        test = string.Format(new LocalizationFormatter(), "{0:Property[1].Property[3]}", input);
        Assert.AreEqual("3", test);
    }
}
