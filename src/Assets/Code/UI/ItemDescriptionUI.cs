using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ItemDescriptionUI : MonoBehaviour
{
    [SerializeField] private ItemVisualSettings _itemVisualSettings;
    [Header("References")]
    [SerializeField] private Image _border;
    [SerializeField] private VerticalLayoutGroup _margin;
    [SerializeField] private LocalizeStringEvent[] _modifiers = new LocalizeStringEvent[3];

    public void SetItem(Item item)
    {
        ItemVisualSettings.BorderStyle border = _itemVisualSettings.BorderStyles[item.BorderType];
        if (border.Sprite == null) throw new ArgumentException($"Border sprite for {item.BorderType} is not supported");
        SetBorder(border);

        for (int modifierIndex = 0; modifierIndex < _modifiers.Length; modifierIndex++)
        {
            LocalizeStringEvent localizeStringEvent = _modifiers[modifierIndex];
            IItemModifier modifier = modifierIndex < item.Modifiers.Length ? item.Modifiers[modifierIndex] : null;

            if (modifier != null)
            {
                localizeStringEvent.StringReference.Arguments = new[] { modifier };
                localizeStringEvent.StringReference.SetReference(IItemModifier.TableReference, modifier.TableEntryReference);
                localizeStringEvent.RefreshString();
            }
            else
            {
                localizeStringEvent.StringReference.SetReference(null, null);
                localizeStringEvent.GetComponent<TextMeshProUGUI>().text = ""; // TODO Cleanup when modifiers get moved to their own UI component
            }
        }
    }

    private void SetBorder(ItemVisualSettings.BorderStyle border)
    {
        _border.sprite = border.Sprite;

        CanvasScaler canvasScaler = (transform as RectTransform).GetCanvasScaler();
        int spritePPU = Mathf.FloorToInt(_border.pixelsPerUnitMultiplier * border.Sprite.pixelsPerUnit);
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
