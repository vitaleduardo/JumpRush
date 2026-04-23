using UnityEngine;

public class Engrane : MonoBehaviour
{
    public int valorPuntos = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si lo toca el jugador (asegúrate que tu personaje tenga el tag "Player")
        if (collision.CompareTag("Player"))
        {
            // Sumamos los puntos al cerebro del juego
            GameManager.instance.SumarPuntos(valorPuntos);

            // Desaparece el engrane
            Destroy(gameObject);
        }
    }
}