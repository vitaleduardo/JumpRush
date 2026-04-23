using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player1 : MonoBehaviour
{
    [Header("Referencias UI")]
    public List<Image> listaCorazones;
    public Sprite corazonLleno;
    public Sprite corazonVacio;
    // Ya no necesitamos textoPuntos aquí, lo maneja el GameManager

    [Header("Configuración")]
    public int maxHealth = 3;
    public float speed = 6f;
    public float jumpForce = 12f; // Asegúrate de que este valor sea suficiente para saltar
    public Transform groundCheck;
    public LayerMask groundLayer;
    public int maxJumps = 2;
    private int jumpCount;

    [Header("Efectos de Daño")]
    public float fuerzaRebote = 8f;
    public GameObject efectoChispa;
    public AudioSource audioSource;
    public AudioClip sonidoGolpe;

    [Header("Estadísticas")]
    public int health = 3;
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
    public float invincibleTime = 1.5f;
    private bool isInvincible = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        health = maxHealth;
        ActualizarCorazones();

        // BORRAMOS: ActualizarInterfazPuntos(); -> Esto lo hace el GameManager solo

        checkpoint = transform.position;
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal"); // GetAxisRaw es más preciso para plataformas
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

        if (Input.GetKey(KeyCode.X) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
            anim.SetTrigger("Shoot");
        }

        if (move > 0 && !facingRight) Flip();
        else if (move < 0 && facingRight) Flip();
    }

    void ActualizarCorazones()
    {
        if (listaCorazones == null || listaCorazones.Count == 0) return;
        for (int i = 0; i < listaCorazones.Count; i++)
        {
            if (listaCorazones[i] != null)
                listaCorazones[i].sprite = (i < health) ? corazonLleno : corazonVacio;
        }
    }

    public void TakeDamage(int damage, Vector2 posicionEnemigo)
    {
        if (isInvincible) return;

        health -= damage;
        ActualizarCorazones();

        if (audioSource != null && sonidoGolpe != null)
            audioSource.PlayOneShot(sonidoGolpe);

        if (efectoChispa != null)
            Instantiate(efectoChispa, transform.position, Quaternion.identity);

        Vector2 direccionEmpuje = (new Vector2(transform.position.x, transform.position.y) - posicionEnemigo).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direccionEmpuje * fuerzaRebote, ForceMode2D.Impulse);

        if (health <= 0)
        {
            if (GameManager.instance != null) GameManager.instance.PerderVida();
            Die();
        }
        else
        {
            StartCoroutine(Invincibility());
        }
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
        if (smokePrefab != null) Instantiate(smokePrefab, firePoint.position, firePoint.rotation);
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null) rbBullet.linearVelocity = new Vector2(direction * 20f, 0f);

        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        Collider2D playerCol = GetComponent<Collider2D>();
        if (bulletCol != null && playerCol != null) Physics2D.IgnoreCollision(bulletCol, playerCol);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            TakeDamage(1, collision.transform.position);
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
        ActualizarCorazones();
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(Invincibility());
    }
}