using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public Transform meleeGrip;
    public Transform rangedGrip;

    Vector2 moveAmount;
    Rigidbody2D rb;
    Animator animator;
    List<Weapon> weapons = new List<Weapon> { };
    Weapon equippedWeapon;
    int equippedWeaponIndex;
    bool isSwitchingWeapons;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveAmount = moveInput.normalized * speed;
        if (Input.GetKey(KeyCode.RightShift) && weapons.Count > 1 && !isSwitchingWeapons)
        {
            StartCoroutine(CycleWeapons());
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveAmount * Time.fixedDeltaTime);
    }

    public void AddWeapon(Weapon weapon)
    {
        weapons.Add(weapon);
        EquipWeapon(weapons.Count - 1);

    }

    IEnumerator CycleWeapons()
    {
        isSwitchingWeapons = true;
        EquipWeapon(equippedWeaponIndex + 1);
        yield return new WaitForSeconds(.5f);
        isSwitchingWeapons = false;

    }

    void EquipWeapon(int index)
    {
        // Remove currently equipped weapon
        if (equippedWeapon != null)
        {
            if (equippedWeapon.type == Weapon.Types.MELEE)
            {
                Destroy(meleeGrip.GetChild(0).gameObject);
            }
            else if (equippedWeapon.type == Weapon.Types.RANGED)
            {
                Destroy(rangedGrip.GetChild(0).gameObject);
            }

        }

        // Instantiate the new weapon on the grip
        equippedWeaponIndex = index > weapons.Count - 1 ? 0 : index;
        equippedWeapon = weapons[equippedWeaponIndex];

        if (equippedWeapon.type == Weapon.Types.MELEE)
        {
            Instantiate(equippedWeapon, meleeGrip.position, Quaternion.identity, meleeGrip);
        } else if (equippedWeapon.type == Weapon.Types.RANGED)
        {
            Instantiate(equippedWeapon, rangedGrip.position, Quaternion.identity, rangedGrip);
        }
    }

}
