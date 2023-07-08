using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject hitWallParticles;
    [SerializeField] private GameObject hitTargetParticles;

    // Components
    private Rigidbody2D rigidBody;
    private Hero heroScript;
    // Stats
    private float damage;
    private int bounceCount;
    // Other
    private List<string> dealDamageTo = new List<string>();

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void SetProperties(float damage, int bounceCount, Vector2 direction, float speed, List<string> dealDamageTo, Hero heroScript = null)
    {
        this.damage = damage;
        this.bounceCount = bounceCount;
        this.dealDamageTo = dealDamageTo;
        this.heroScript = heroScript;
        rigidBody.AddForce(direction * speed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var colliderTag = other.gameObject.tag;
        if (dealDamageTo.Contains(other.gameObject.tag))
        {
            if (TryGetComponent<Health>(out Health healthScript))
            {
                healthScript.Damage(damage);
            }
            Explode();
        }
        else
        {
            bounceCount -= 1;
            if (bounceCount < 0)
            {
                Explode();
            }
        }
    }
    private void Explode()
    {
        if (heroScript != null)
        {
            heroScript.OnProjectileDeath(this);
        }
        Destroy(gameObject);
    }
}
