using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RoomMenu : MonoBehaviour {
    [SerializeField] private Slider monsterLevelSlider;
    [SerializeField] private RoomMenuItem itemOptionPrefab;
    [SerializeField] private ItemOption[] itemOptions;
    [SerializeField] private Transform itemOptionsParent;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI roomCounter;

    private List<Tuple<int, ItemOption>> _weightedOptions;
    private int _totalOptionWeight;
    private List<RoomMenuItem> _itemOptionInstances = new();
    private bool _isInitialized = false;
    private List<Tuple<int, ItemOption>> _currentItemOptions = new();
    private int _baseMonsterLevel = 0;
    private int _currentMonsterLevel = 0;

    private void Init() {
        _weightedOptions = new List<Tuple<int, ItemOption>>();
        var runningWeightSum = 0;
        foreach (var itemOption in itemOptions) {
            _weightedOptions.Add(new Tuple<int, ItemOption>(runningWeightSum, itemOption));
            runningWeightSum += itemOption.weight;
        }

        _totalOptionWeight = runningWeightSum;
    }

    public void Show(int numberOfItems, int baseMonsterLevel, int roomCount) {
        if (!_isInitialized) {
            Init();
            _isInitialized = true;
        }
        foreach (var itemOptionInstance in _itemOptionInstances) {
            Destroy(itemOptionInstance.gameObject);
        }
        _itemOptionInstances.Clear();

        _baseMonsterLevel = baseMonsterLevel;
        _currentMonsterLevel = baseMonsterLevel;
        monsterLevelSlider.value = (float)baseMonsterLevel / 100;

        roomCounter.text = "Rooms defeated: " + roomCount;

        var pickedItems = new ItemOption[numberOfItems];
        for (var i = 0; i < numberOfItems; i++) {
            var randomWeightIndex = Random.Range(0, _totalOptionWeight);
            var pickedItem = _weightedOptions.LastOrDefault(tuple => tuple.Item1 <= randomWeightIndex && !pickedItems.Contains(tuple.Item2));
            while (pickedItem == null) { //ugly code to fix rare case where no items can be found
                randomWeightIndex = Random.Range(0, _totalOptionWeight);
                pickedItem = _weightedOptions.LastOrDefault(tuple => tuple.Item1 <= randomWeightIndex && !pickedItems.Contains(tuple.Item2));
            }

            pickedItems[i] = pickedItem.Item2;
        }

        _currentItemOptions.Clear();
        for (var index = 0; index < pickedItems.Length; index++) {
            var pickedItem = pickedItems[index];
            var optionInstance = Instantiate(itemOptionPrefab, itemOptionsParent);
            optionInstance.Init(pickedItem.icon, pickedItem.description, this, index);
            _itemOptionInstances.Add(optionInstance);
            _currentItemOptions.Add(new Tuple<int, ItemOption>(0, pickedItem));
        }

        gameObject.SetActive(true);
    }

    public void OnSubmit() {
        gameManager.StartRoom(_currentItemOptions, _currentMonsterLevel);
        gameObject.SetActive(false);
    }

    public void OnPercentageChanged(int itemIndex, int percentage) {
        var previousValue = _currentItemOptions[itemIndex];
        _currentItemOptions[itemIndex] = new Tuple<int, ItemOption>(percentage, previousValue.Item2);
        RecalculateMonsterLevel();
    }

    private void RecalculateMonsterLevel() {
        var resultingLevel = (float)_baseMonsterLevel;
        foreach (var currentItemOption in _currentItemOptions) {
            var percentage = currentItemOption.Item1;
            var multiplier = 0.25f;
            while (percentage > 0) {
                if (percentage >= 20) {
                    resultingLevel += 20 * multiplier;
                } else {
                    resultingLevel += percentage * multiplier;
                }
                percentage -= 20;
                multiplier += 0.25f;
            }
        }

        _currentMonsterLevel = Mathf.Clamp((int)resultingLevel, 0, 100);
        monsterLevelSlider.value = (float)_currentMonsterLevel / 100;
    }
}

[Serializable]
public struct ItemOption {
    public Sprite icon;
    public string description;
    public int weight;
    public WeaponModifier modifier;
    public WeaponObject weapon;
    public EntityStatModifier statModifier;
}