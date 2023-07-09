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
    public Transform target = null;
    public float seekingSrength;
    // Other
    public List<string> dealDamageTo = new List<string>();
    private int timeBounced;
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = transform.right * speed;
        Destroy(gameObject, 3);
    }

    private void Update()
    {
        if (target && timeBounced == 0 && seekingSrength > 0.01f) // I SEEK
        {
            var currVel = rigidBody.velocity;
            var wishDir = (Vector2)(target.position - transform.position).normalized;

            rigidBody.velocity = Vector2.MoveTowards(currVel.normalized, wishDir, seekingSrength * Time.deltaTime) * speed;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        var colliderTag = other.gameObject.tag;
        if (dealDamageTo.Contains(other.gameObject.tag))
        {
            if (other.gameObject.TryGetComponent<Health>(out Health healthScript))
            {
                healthScript.Damage(damage);
            }
            Instantiate(hitTargetParticles, transform.position, transform.rotation);
            Explode();
        }
        else
        {
            timeBounced += 1;
            if (timeBounced > bounceCount)
            {
                Instantiate(hitWallParticles, transform.position, transform.rotation);
                Explode();
            }
        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        timeBounced += 1;
        if (timeBounced > bounceCount)
        {
            Instantiate(hitWallParticles, transform.position, transform.rotation);
            Explode();
        }

    }
    private void Explode()
    {
        if (heroScript != null)
        {
        }
        Destroy(gameObject);
    }
}
