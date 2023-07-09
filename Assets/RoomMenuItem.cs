using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomMenuItem : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI percentageText;

    private int _percentage = 0;
    private int _itemIndex;
    private RoomMenu _roomMenu;

    public void Init(Sprite iconSprite, string description, RoomMenu menu, int itemIndex) {
        icon.sprite = iconSprite;
        text.text = description;
        _roomMenu = menu;
        _itemIndex = itemIndex;
        percentageText.text = $"{_percentage}%";
    }

    public void OnPercentageIncrease() {
        _percentage = Mathf.Clamp(_percentage + 10, 0, 100);
        percentageText.text = $"{_percentage}%";
        _roomMenu.OnPercentageChanged(_itemIndex, _percentage);
    }

    public void OnPercentageDecrease() {
        _percentage = Mathf.Clamp(_percentage - 10, 0, 100);
        percentageText.text = $"{_percentage}%";
        _roomMenu.OnPercentageChanged(_itemIndex, _percentage);
    }
}