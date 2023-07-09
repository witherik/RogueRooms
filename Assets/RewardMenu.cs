using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardMenu : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameManager gameManager;

    public void Show(ItemOption reward) {
        gameObject.SetActive(true);
        icon.sprite = reward.icon;
        text.text = $"You received: {reward.description}";
    }

    public void OnSubmit() {
        gameManager.OnRewardSubmit();
        gameObject.SetActive(false);
    }
}