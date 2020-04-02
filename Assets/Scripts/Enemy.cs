using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    public int damage;
    public float speed;
    public float timeBetweenAttacks;
    public bool facingRight;
    public GameObject splatter;
    public ParticleSystem deathParticles;

    [HideInInspector]
    public Transform player;

    [HideInInspector]
    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    [HideInInspector]
    public virtual void Update()
    {
        if (player != null)
        {
            Vector2 relativePlayer = player.position - transform.position;
            if ((facingRight && relativePlayer.x < 0) || (!facingRight && relativePlayer.x > 0))
            {
                Flip();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    private void OnDestroy()
    {
        Instantiate(deathParticles, transform.position, transform.rotation);
        Instantiate(splatter, transform.position, transform.rotation);
    }
}
