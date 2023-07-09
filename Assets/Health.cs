using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float maxHp = 0.0f;
    private float hp;
    private Hero hero = null;
    private Enemy enemy = null;
    private Healthbar healthbar;

    private void Start()
    {
        hp = maxHp;
        TryGetComponent<Hero>(out hero);
        TryGetComponent<Enemy>(out enemy);
        TryGetComponent<Healthbar>(out healthbar);
        healthbar = GetComponentInChildren<Healthbar>();

    }
    void Update()
    {
        if (healthbar)
        {
            healthbar.DisplayHealth(maxHp, hp);
        }
    }

    public void SetMaxHP(float maxHp)
    {
        this.maxHp = maxHp;
        this.hp = maxHp;
    }
    public void UpdateMaxHp(float maxHp)
    {
        var ratio = maxHp / this.maxHp;
        this.maxHp = maxHp;
        this.hp *= ratio;
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

    public void MultiplicativeHeal(float value)
    {
        hp = Mathf.Min(maxHp, maxHp * value + hp);

    }
    public void AddiveHeal(float value)
    {
        hp = Mathf.Min(maxHp, hp + value);
    }
}
