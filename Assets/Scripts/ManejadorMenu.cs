using UnityEngine;
using UnityEngine.SceneManagement;

public class ManejadorMenu : MonoBehaviour
{
    public void EmpezarJuego()
    {
        // 1. Aseguramos que el tiempo del juego corra (por si acaso)
        Time.timeScale = 1f;

        // 2. Si el GameManager viejo sigue vivo, lo reseteamos antes de saltar
        if (GameManager.instance != null)
        {
            GameManager.instance.score = 0;
            GameManager.instance.vidas = 3;
        }

        // 3. Cargamos el Nivel 1
        SceneManager.LoadScene("Level1");
    }

    public void Salir()
    {
        Debug.Log("Saliste del juego");
        Application.Quit();
    }
}