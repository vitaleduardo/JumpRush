using UnityEngine;
using UnityEngine.SceneManagement;

public class cambioNivel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int siguienteEscena = SceneManager.GetActiveScene().buildIndex + 1;

            // Verificamos si la siguiente escena existe en el Build Settings
            if (siguienteEscena < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(siguienteEscena);
            }
            else
            {
                // Si ya no hay más niveles, mandamos al jugador al menú de inicio
                Debug.Log("¡Juego terminado! Volviendo al menú...");
                SceneManager.LoadScene(0); // El 0 siempre es tu MainMenu ahora
            }
        }
    }
}