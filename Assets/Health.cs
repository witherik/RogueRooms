using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp = 100.0f;
    private float hp;

    private void Start()
    {
        hp = maxHp;
    }

    public void Damage(float value)
    {
        hp -= value;
    }
    public void Heal(float value)
    {
        hp += value;
    }
}
