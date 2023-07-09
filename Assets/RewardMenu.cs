using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardMenu : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Sprite happyIcon;
    [SerializeField] private Sprite sadIcon;

    public void Show(List<ItemOption> rewards) {
        gameObject.SetActive(true);
        if (rewards.Count == 0) {
            icon.sprite = sadIcon;
            text.text = $"You didn't find any treasure.";
        } else {
            icon.sprite = happyIcon;
            var message = "You received: ";
            for (var i = 0; i < rewards.Count; i++) {
                var reward = rewards[i];
                message += reward.description;
                if (i != rewards.Count - 1) {
                    message += ", ";
                } else {
                    message += ".";
                }
            }

            text.text = message;
        }
        
    }

    public void OnSubmit() {
        gameManager.OnRewardSubmit();
        gameObject.SetActive(false);
    }
}