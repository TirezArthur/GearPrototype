using System;
using UnityEngine.Localization.Tables;

public interface IItemModifier
{
    public TableReference TableReference { get; }
    public long TableEntryReference { get; }
    public void Apply();
}

public struct AddHealthModifier : IItemModifier
{
    public int _value;

    public TableReference TableReference => "ModifierList";
    public long TableEntryReference => 19054575616;
    public int Value => _value;

    public AddHealthModifier(int value)
    {
        _value = value;
    }

    public void Apply()
    {
        throw new System.NotImplementedException();
    }
}

[Serializable]
public struct IncreaseHealthModifier : IItemModifier
{
    private int _value;

    public TableReference TableReference => "ModifierList";
    public long TableEntryReference => 78030684160;
    public int Value => _value;

    public IncreaseHealthModifier(int value)
    {
        _value = value;
    }

    public void Apply()
    {
        throw new System.NotImplementedException();
    }
}

[Serializable]
public struct IncreaseLevelModifier : IItemModifier
{
    private int _value;
    private string _skill;

    public TableReference TableReference => "ModifierList";
    public long TableEntryReference => 230325862400;
    public int Value => _value;
    public string Skill => _skill;

    public IncreaseLevelModifier(string skill, int value)
    {
        _skill = skill;
        _value = value;
    }

    public void Apply()
    {
        throw new System.NotImplementedException();
    }
}
