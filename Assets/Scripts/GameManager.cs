using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Configuración de Juego")]
    public int vidas = 3;
    public int score = 0;

    [Header("Interfaz de Usuario")]
    public TextMeshProUGUI textoScoreUI;

    void Awake()
    {
        // Patrón Singleton mejorado
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe uno, destruimos el nuevo y salimos
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable() { SceneManager.sceneLoaded += AlCargarEscena; }
    private void OnDisable() { SceneManager.sceneLoaded -= AlCargarEscena; }

    void AlCargarEscena(Scene scene, LoadSceneMode mode)
    {
        // SI VOLVEMOS AL MENÚ, RESETEAMOS TODO PARA PODER VOLVER A JUGAR
        if (scene.name == "MainMenu")
        {
            vidas = 3;
            score = 0;
            textoScoreUI = null;
            Debug.Log("GameManager reseteado al volver al Menú Principal");
        }
        else
        {
            // Si entramos a un nivel (1, 2 o 3), buscamos el texto de los puntos
            GameObject objTexto = GameObject.Find("TextoScore");

            if (objTexto != null)
            {
                textoScoreUI = objTexto.GetComponent<TextMeshProUGUI>();
                ActualizarInterfaz();
            }
        }
    }

    // --- SISTEMA DE PUNTOS ---
    public void SumarPuntos(int cantidad)
    {
        score += cantidad;
        ActualizarInterfaz();
        Debug.Log("Puntos sumados. Score actual: " + score);
    }

    public void ActualizarInterfaz()
    {
        if (textoScoreUI != null)
        {
            textoScoreUI.text = "POINTS: " + score;
        }
    }

    // --- SISTEMA DE VIDAS ---
    public void PerderVida()
    {
        vidas--;
        Debug.Log("Vidas restantes: " + vidas);
        if (vidas <= 0) GameOver();
    }

    public void GameOver()
    {
        // 1. Reiniciamos los valores para que el jugador empiece de cero
        vidas = 3;
        score = 0;

        // 2. IMPORTANTE: Aseguramos que el tiempo no esté pausado
        Time.timeScale = 1f;

        // 3. CARGA DIRECTA: En lugar de ir al menú, vamos al Nivel 1
        // Asegúrate de que el nombre sea exactamente "Level1" como en tus Assets
        SceneManager.LoadScene("Level1");
    }
}