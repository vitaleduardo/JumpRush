using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 10f;
    public Transform player;
    public EnemySpawner spawner;
    public int health = 1;
    public int damage = 1;

    [Header("Puntuación")]
    public int puntosAlMorir = 50; // Cantidad de puntos que otorga este enemigo

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Si no se asignó el jugador en el inspector, lo buscamos por Tag
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            // Usamos linearVelocity (o velocity según tu versión de Unity)
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    void Die()
    {
        // NOTIFICAR AL GAMEMANAGER PARA SUMAR PUNTOS
        if (GameManager.instance != null)
        {
            GameManager.instance.SumarPuntos(puntosAlMorir);
        }

        // Notificar al spawner si existe
        if (spawner != null) spawner.EnemyDied();

        // Destruir el objeto del enemigo
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player1 playerScript = collision.gameObject.GetComponent<Player1>();
            if (playerScript != null)
            {
                // Pasamos daño y posición para el efecto de rebote
                playerScript.TakeDamage(damage, transform.position);
                Debug.Log("💥 Enemy hizo daño y causó rebote");
            }
        }
    }
}