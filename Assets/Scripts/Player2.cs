using UnityEngine;

public class Player2 : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth = 3;
    public float speed = 6f;
    public float jumpForce = 5f;

    public Transform groundCheck;
    public LayerMask groundLayer;

    public int maxJumps = 2;
    private int jumpCount;

    public int health = 3;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;
    private bool facingRight = true;

    public GameObject bulletPrefab;
    public GameObject smokePrefab;

    public Transform firePoint;

    public float fireRate = 0.3f;
    private float nextFireTime = 0f;

    public float invincibleTime = 1f;
    private bool isInvincible = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        healthBar.SetHealth(health, maxHealth);
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        anim.SetBool("running", move != 0);
        anim.SetBool("isJumping", !isGrounded);

        if (isGrounded)
        {
            jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded || jumpCount < maxJumps - 1)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpCount++;
            }
        }

        if (Input.GetKey(KeyCode.UpArrow) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
            anim.SetTrigger("Shoot");
        }

        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Shoot()
    {
        float direction = facingRight ? 1f : -1f;

        if (smokePrefab != null)
        {
            Instantiate(smokePrefab, firePoint.position, firePoint.rotation);
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (rbBullet != null && bulletScript != null)
        {
            rbBullet.linearVelocity = new Vector2(direction * bulletScript.speed, 0f);
        }

        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        Collider2D playerCol = GetComponent<Collider2D>();

        if (bulletCol != null && playerCol != null)
        {
            Physics2D.IgnoreCollision(bulletCol, playerCol);
        }
    }

    // 💥 DETECTAR ENEMIGO Y RECIBIR DAÑO
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("💥 Player tocó enemigo");
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
{
    if (isInvincible) return;

    health -= damage;

    // 🔥 ACTUALIZA LA BARRA
    healthBar.SetHealth(health, maxHealth);

    Debug.Log("❤️ Vida del player: " + health);

    if (health <= 0)
    {
        Die();
    }
    else
    {
        StartCoroutine(Invincibility());
    }
}

    System.Collections.IEnumerator Invincibility()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("💀 Player murió");
        gameObject.SetActive(false);
    }
}