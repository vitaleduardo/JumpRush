using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 10f;

    public Transform player;
    public EnemySpawner spawner;

    public int health = 1;
    public int damage = 1;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 🔍 Buscar player si no está asignado
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");

            if (p != null)
            {
                player = p.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // 💥 RECIBE DAÑO
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (spawner != null)
        {
            spawner.EnemyDied();
        }

        Destroy(gameObject);
    }

    // 💥 DAÑO AL PLAYER (COLLISION)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player1 playerScript = collision.gameObject.GetComponent<Player1>();

            if (playerScript != null)
            {
                playerScript.TakeDamage(damage);
                Debug.Log("💥 Enemy hizo daño");
            }
        }
    }
}