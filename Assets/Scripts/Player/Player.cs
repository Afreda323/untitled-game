using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health;
    public float speed;
    public float dashSpeed;
    public float dashTime;
    public Transform shotPoint;
    public Transform meleeGrip;
    public Transform rangedGrip;
    public GameObject splatter;
    public ParticleSystem dashParticles;
    public ParticleSystem deathParticles;
    public Image healthbar;
    public Image cooldownbar;

    Vector2 moveAmount;
    Rigidbody2D rb;
    Animator animator;
    List<Weapon> weapons = new List<Weapon> { };
    Weapon equippedWeapon;
    int equippedWeaponIndex;
    bool isSwitchingWeapons;
    bool facingRight = true;
    float attackTime;
    float recentAttack;
    float _dashTime;
    bool isDashing = false;
    CameraShake cameraShake;
    int startHealth;

    // Start is called before the first frame update
    void Start()
    {
        startHealth = health;
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        cameraShake = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>();
    }

    private void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        animator.SetBool("isWalking", moveInput.y != 0 || moveInput.x != 0);
        if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
        {
            speed += dashSpeed;
            isDashing = true;
            _dashTime = dashTime;
            cameraShake.Shake();
            Instantiate(dashParticles, transform.position, Quaternion.identity);
        }
        if (_dashTime <= 0 && isDashing == true)
        {
            isDashing = false;
            speed -= dashSpeed;
        }
        else
        {
            _dashTime -= Time.deltaTime;
        }

        moveAmount = moveInput.normalized * speed;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if ((facingRight && mousePos.x < 0) || (!facingRight && mousePos.x > 0))
        {
            Flip();
        }
        if (Input.GetKey(KeyCode.RightShift) && weapons.Count > 1 && !isSwitchingWeapons)
        {
            StartCoroutine(CycleWeapons());
        }
        if (Input.GetMouseButton(0) && equippedWeapon != null && Time.time >= attackTime)
        {
            attackTime = equippedWeapon.attackTime + Time.time;
            recentAttack = Time.time;
            if (equippedWeapon.type == Weapon.Types.MELEE)
            {
                animator.SetTrigger("meleeAttack");
            }
            else if (equippedWeapon.type == Weapon.Types.RANGED)
            {
                animator.SetTrigger("rangedAttack");
            }
        }
        if (recentAttack > 0 && attackTime > 0 && equippedWeapon != null)
        {
            float clamped = Mathf.Clamp((attackTime - Time.time) / equippedWeapon.attackTime, 0, 1);
            Vector3 cooldownScale = new Vector3(clamped, 1);
            cooldownbar.transform.localScale = cooldownScale;
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
        attackTime = Time.time;

        if (equippedWeapon.type == Weapon.Types.MELEE)
        {
            Instantiate(equippedWeapon, meleeGrip.position, new Quaternion(0, 0, 0, 0), meleeGrip);
        } else if (equippedWeapon.type == Weapon.Types.RANGED)
        {
            Instantiate(equippedWeapon, rangedGrip.position, new Quaternion(0, 0, 0, 0), rangedGrip);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    public void Attack()
    {
        Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        equippedWeapon.Attack(shotPoint.position, rotation);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Vector3 healthScale = new Vector3((float)health / (float)startHealth, 1);
        healthbar.transform.localScale = healthScale;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }


    private void OnDestroy()
    {
        Instantiate(deathParticles, transform.position, transform.rotation);
        Instantiate(splatter, transform.position, transform.rotation);
    }

}
