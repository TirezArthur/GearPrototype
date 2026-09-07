using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ItemVisualSettings _itemVisualSettings;
    [SerializeField] private Slider _raritySlider;
    [SerializeField] private TextMeshProUGUI _rarityField;
    [SerializeField] private Button _generateButton;
    [SerializeField] private ItemDescriptionUI _itemDescriptionBox;

    private void Start()
    {
        _raritySlider.maxValue = Enum.GetValues(typeof(Rarity)).Length - 1;
        _generateButton.onClick.AddListener(GenerateItem);
        _raritySlider.onValueChanged.AddListener(UpdateRarity);

        UpdateRarity(_raritySlider.value);
        GenerateItem();
    }

    private void GenerateItem()
    {
        Rarity rarity = (Rarity)Enum.GetValues(typeof(Rarity)).GetValue((int)_raritySlider.value);
        _itemDescriptionBox.SetItem(ServiceLocator.GetService<ItemGenerator>().GenerateItem(rarity));
    }

    private void UpdateRarity(float value)
    {
        Rarity rarity = (Rarity)Enum.GetValues(typeof(Rarity)).GetValue((int)value);
        _rarityField.text = rarity.ToString();
        _rarityField.color = _itemVisualSettings.RarityColors[rarity];
    }
}
