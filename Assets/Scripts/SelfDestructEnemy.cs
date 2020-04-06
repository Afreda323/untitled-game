using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructEnemy : Enemy
{
    CameraShake cameraShake;

    [HideInInspector]
    public override void Start()
    {
        base.Start();
        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
    }

    // Update is called once per frame
    [HideInInspector]
    public override void Update()
    {
        base.Update();
        if (player != null)
        {
            if (Vector2.Distance(transform.position, player.position) > 0.2f)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.tag);
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }


    private void OnDestroy()
    {
        cameraShake.Shake();
        Instantiate(deathParticles, transform.position, transform.rotation);
    }
}
