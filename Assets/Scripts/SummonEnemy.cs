using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEnemy : Enemy
{
    public float stopDistance;
    public Enemy spawn;

    Animator animator;
    float _timeBetweenAttacks;

    [HideInInspector]
    public override void Start()
    {
        base.Start();
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    [HideInInspector]
    public override void Update()
    {
        base.Update();
        if (player != null)
        {
            if (Vector2.Distance(transform.position, player.position) > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
            else
            {
                if (Time.time > _timeBetweenAttacks)
                {
                    _timeBetweenAttacks = timeBetweenAttacks + Time.time;
                    animator.SetTrigger("attack");
                }
            }
        }
    }

    public void Attack()
    {
        Instantiate(spawn, transform.position, transform.rotation);
    }

}
