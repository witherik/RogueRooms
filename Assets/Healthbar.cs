using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{

    [SerializeField] private float healthPerUnit = 100f;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer fill;

    public void DisplayHealth(float maxHp, float hp)
    {
        Debug.Log("DISPLAYING");
        var width = maxHp / healthPerUnit;
        background.transform.localPosition = new Vector3(-width / 2, 0, 0);
        var fillwidth = hp / healthPerUnit;
        background.size = new Vector2(width, 1);
        fill.size = new Vector2(fillwidth, 1);
    }
}
