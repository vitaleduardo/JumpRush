using UnityEngine;
using TMPro; // Necesario para controlar el texto de puntos

public class Player1 : MonoBehaviour
{
    [Header("Referencias")]
    public HealthBar healthBar; // Asigna aquí HealthBar_BG
    public TextMeshProUGUI textoPuntos; // Arrastra aquí tu objeto "TextoPuntos" del Canvas

    [Header("Configuración")]
    public int maxHealth = 3;
    public float speed = 6f;
    public float jumpForce = 5f;

    public Transform groundCheck;
    public LayerMask groundLayer;

    public int maxJumps = 2;
    private int jumpCount;

    [Header("Estadísticas")]
    public int health = 3;
    public int puntos = 0; // Aquí se guardan tus puntos
    private Vector2 checkpoint;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isGrounded;
    private bool facingRight = true;

    [Header("Combate")]
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

        health = maxHealth;

        // Actualiza la barra al iniciar si está conectada
        if (healthBar != null) healthBar.SetHealth(health, maxHealth);

        // Actualiza el texto de puntos al empezar
        ActualizarInterfazPuntos();

        // Guarda la posición inicial del nivel actual
        checkpoint = transform.position;
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        anim.SetBool("running", move != 0);
        anim.SetBool("isJumping", !isGrounded);

        if (isGrounded) jumpCount = 0;

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

    // --- SISTEMA DE PUNTOS ---
    public void GanarPuntos(int cantidad)
    {
        puntos += cantidad;
        ActualizarInterfazPuntos();
    }

    void ActualizarInterfazPuntos()
    {
        if (textoPuntos != null)
        {
            textoPuntos.text = "Puntos: " + puntos;
        }
    }
    // -------------------------

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
        if (smokePrefab != null) Instantiate(smokePrefab, firePoint.position, firePoint.rotation);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

        if (rbBullet != null)
        {
            float bSpeed = 10f;
            rbBullet.linearVelocity = new Vector2(direction * bSpeed, 0f);
        }

        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        Collider2D playerCol = GetComponent<Collider2D>();

        if (bulletCol != null && playerCol != null) Physics2D.IgnoreCollision(bulletCol, playerCol);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        health -= damage;
        if (healthBar != null) healthBar.SetHealth(health, maxHealth);

        if (health <= 0) Die();
        else StartCoroutine(Invincibility());
    }

    System.Collections.IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    void Die()
    {
        transform.position = checkpoint;
        health = maxHealth;
        if (healthBar != null) healthBar.SetHealth(health, maxHealth);
        rb.linearVelocity = Vector2.zero;
        gameObject.SetActive(true);
        StartCoroutine(Invincibility());

        //los puntos no se reinician al morir
    }
}