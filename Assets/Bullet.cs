using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifetime = 2f;

    void Start()
    {
        // Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Si choca con el jugador o con cualquier cosa que diga "Player", NO hacer nada
        if (collision.CompareTag("Player"))
        {
            return;
        }

        // 2. Buscar enemigo
        Enemy enemy = collision.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 3. Suelo u otros obstáculos (Asegúrate de que tus plataformas tengan el Tag "Ground")
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}