using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    void Awake()
    {
        // 1. Lógica para que no se duplique la música al reiniciar niveles
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // 2. Guardar esta instancia y hacerla persistente
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
