using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ItemDescriptionUI : MonoBehaviour
{
    [Serializable]
    private struct BorderSettings
    {
        [SerializeField] public Sprite Sprite;
        [Header("Margins")]
        [SerializeField] public int Left;
        [SerializeField] public int Right;
        [SerializeField] public int Top;
        [SerializeField] public int Bottom;
    }

    [Header("Layout")]
    [SerializeField] private EnumMapping<BorderStyle, BorderSettings> _borders = new();

    [Header("References")]
    [SerializeField] private Image _border;
    [SerializeField] private VerticalLayoutGroup _margin;
    [SerializeField] private LocalizeStringEvent[] _modifiers = new LocalizeStringEvent[3];

    private void Start()
    {
        Item item = new();
        item.Modifiers = new IItemModifier[] { new AddHealthModifier(67), new IncreaseHealthModifier(420), new IncreaseLevelModifier("Fireball", 2) };
        item.BorderStyle = BorderStyle.Paper;
        SetItem(item);
    }

    public void SetItem(Item item)
    {
        BorderSettings border = _borders[item.BorderStyle];
        if (border.Sprite == null) throw new ArgumentException($"Border sprite for {item.BorderStyle} is not supported");
        SetBorder(border);

        for (int modifierIndex = 0; modifierIndex < math.min(_modifiers.Length, item.Modifiers.Length); modifierIndex++)
        {
            LocalizeStringEvent localizeStringEvent = _modifiers[modifierIndex];
            IItemModifier modifier = item.Modifiers[modifierIndex];

            localizeStringEvent.StringReference.Arguments = new[] { modifier };
            localizeStringEvent.StringReference.SetReference(modifier.TableReference, modifier.TableEntryReference);
        }
    }

    private void SetBorder(BorderSettings border)
    {
        _border.sprite = border.Sprite;

        CanvasScaler canvasScaler = (transform as RectTransform).GetCanvasScaler();
        int spritePPU = Mathf.FloorToInt(border.Sprite.pixelsPerUnit);
        int canvasPPU = Mathf.FloorToInt(canvasScaler.referencePixelsPerUnit);
        int pixelScale = canvasPPU / spritePPU;

#if UNITY_EDITOR
        if (pixelScale * spritePPU != canvasPPU) throw new ArithmeticException("Pixels per unit of canvas are not an integer multiple of sprite pixels per unit");
#endif

        _margin.padding.left = border.Left * pixelScale;
        _margin.padding.bottom = border.Bottom * pixelScale;
        _margin.padding.right = border.Right * pixelScale;
        _margin.padding.top = border.Top * pixelScale;
    }
}
