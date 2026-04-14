using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 🔍 Buscar enemigo en el objeto o en sus padres
        Enemy enemy = collision.GetComponentInParent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Suelo
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
            return;
        }

        //  Ignorar jugador
        if (collision.CompareTag("Player"))
        {
            return;
        }

        //  Cualquier otra cosa destruye la bala
        Destroy(gameObject);
    }
}