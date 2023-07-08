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
    [Header("Stats")]
    public float damage;
    public int bounceCount;
    public float speed;
    // Other
    public List<string> dealDamageTo = new List<string>();

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = transform.right * speed;
    }

    // public void SetProperties(float damage, int bounceCount, Vector2 direction, float speed, List<string> dealDamageTo)
    // {
    //     this.damage = damage;
    //     this.bounceCount = bounceCount;
    //     this.dealDamageTo = dealDamageTo;
    //     rigidBody.AddForce(direction * speed);
    // }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var colliderTag = other.gameObject.tag;
        if (dealDamageTo.Contains(other.gameObject.tag))
        {
            if (TryGetComponent<Health>(out Health healthScript))
            {
                healthScript.Damage(damage);
            }
            Instantiate(hitTargetParticles, transform.position, transform.rotation);
            Explode();
        }
        else
        {
            bounceCount -= 1;
            if (bounceCount < 0)
            {
                Instantiate(hitWallParticles, transform.position, transform.rotation);
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
