using UnityEngine;
using UnityEngine.SceneManagement;

public class cambioNivel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica que quien toca la puerta tenga el Tag "Player"
        if (collision.CompareTag("Player"))
        {
            // Carga la siguiente escena en la lista (Build Settings)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}