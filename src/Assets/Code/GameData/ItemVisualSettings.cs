using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemVisualSettings", menuName = "GameData/ItemVisualSettings")]
public class ItemVisualSettings : ScriptableObject
{
    [Serializable]
    public struct BorderStyle
    {
        [SerializeField] public Sprite Sprite;
        [Header("Margins")]
        [SerializeField] public int Left;
        [SerializeField] public int Right;
        [SerializeField] public int Top;
        [SerializeField] public int Bottom;
    }

    [SerializeField] private EnumMapping<BorderType, BorderStyle> _borders = new();
    [SerializeField] private EnumMapping<Rarity, Color> _rarityColors = new();

    public EnumMapping<BorderType, BorderStyle> BorderStyles => _borders;
    public EnumMapping<Rarity, Color> RarityColors => _rarityColors;
}
