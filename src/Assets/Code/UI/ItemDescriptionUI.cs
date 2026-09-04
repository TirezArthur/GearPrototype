using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

public class ItemDescriptionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LocalizeStringEvent[] _modifiers;

    private void Start()
    {
        Item item = new();
        item.Modifiers = new IItemModifier[] { new AddHealthModifier(67), new IncreaseHealthModifier(420), new IncreaseLevelModifier("Ligma", 80) };
        SetItem(item);
    }

    public void SetItem(Item item)
    {
        for (int modifierIndex = 0; modifierIndex < math.min(_modifiers.Length, item.Modifiers.Length); modifierIndex++)
        {
            LocalizeStringEvent localizeStringEvent = _modifiers[modifierIndex];
            IItemModifier modifier = item.Modifiers[modifierIndex];

            localizeStringEvent.StringReference.Arguments = new[] { modifier };
            localizeStringEvent.StringReference.SetReference(modifier.TableReference, modifier.TableEntryReference);
        }
    }
}
