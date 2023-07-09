using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp = 100.0f;
    private float hp;
    private Hero hero = null;
    private Enemy enemy = null;
    private void Start()
    {
        hp = maxHp;
        TryGetComponent<Hero>(out hero);
        TryGetComponent<Enemy>(out enemy);
    }

    public void Damage(float value)
    {
        hp -= value;
        if (hp <= 0)
        {
            if (hero) { hero.Death(); }
            if (enemy) { enemy.Death(); }
        }
    }
    public void Heal(float value)
    {
        hp += value;
    }
}
