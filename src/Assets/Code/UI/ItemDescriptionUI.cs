using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ItemDescriptionUI : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private Vector4 _margins = Vector4.zero;
    [SerializeField] private EnumMapping<BorderStyle, Sprite> _borderSprites = new();

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
        Sprite border = _borderSprites[item.BorderStyle];
        if (border == null) throw new ArgumentException($"Border sprite for {item.BorderStyle} is not supported");
        _border.sprite = border;
        SetInnerMargins(border);

        for (int modifierIndex = 0; modifierIndex < math.min(_modifiers.Length, item.Modifiers.Length); modifierIndex++)
        {
            LocalizeStringEvent localizeStringEvent = _modifiers[modifierIndex];
            IItemModifier modifier = item.Modifiers[modifierIndex];

            localizeStringEvent.StringReference.Arguments = new[] { modifier };
            localizeStringEvent.StringReference.SetReference(modifier.TableReference, modifier.TableEntryReference);
        }
    }

    private void SetInnerMargins(Sprite backgroundSprite)
    {
        CanvasScaler canvasScaler = (transform as RectTransform).GetCanvasScaler();
        int spritePPU = Mathf.FloorToInt(backgroundSprite.pixelsPerUnit);
        int canvasPPU = Mathf.FloorToInt(canvasScaler.referencePixelsPerUnit);
        int pixelScale = canvasPPU / spritePPU;

#if UNITY_EDITOR
        if (pixelScale * spritePPU != canvasPPU) throw new ArithmeticException("Pixels per unit of canvas are not an integer multiple of sprite pixels per unit");
#endif

        Vector4 margins = backgroundSprite.border;

        _margin.padding.left = ((int)margins.x + (int)_margins.x) * pixelScale;
        _margin.padding.bottom = ((int)margins.y + (int)_margins.y) * pixelScale;
        _margin.padding.right = ((int)margins.z + (int)_margins.z) * pixelScale;
        _margin.padding.top = ((int)margins.w + (int)_margins.w) * pixelScale;
    }
}
