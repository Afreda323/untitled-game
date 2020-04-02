using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [HideInInspector]
    public enum Types
    {
        MELEE = 1,
        RANGED = 2
    }

    public Types type;
}
