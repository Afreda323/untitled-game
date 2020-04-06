using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [HideInInspector]
    public enum Types
    {
        MELEE = 1,
        RANGED = 2,
        BURST = 3
    }

    public Types type;
    public float attackTime;
    public int burstSize;
    public Projectile projectile;

    public void Attack(Vector3 position, Quaternion rotation)
    {
        if (type == Types.BURST)
        {
            for (int i = 0; i < 360; i += 360 / burstSize)
            {
                float angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
                Instantiate(projectile, position, Quaternion.AngleAxis(angle - i, Vector3.forward));
            }
        } else
        {
            Instantiate(projectile, position, rotation);
        }
    }
}
