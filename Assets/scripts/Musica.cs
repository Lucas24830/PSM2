using UnityEngine;
using UnityEngine.SceneManagement;

public class Musica : MonoBehaviour
{
    public static Musica instancia;

    [SerializeField] private string escenaMainMenu = "MainMenu";

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (escena.name == escenaMainMenu)
        {
            SceneManager.sceneLoaded -= AlCargarEscena;
            instancia = null;
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (instancia == this)
            SceneManager.sceneLoaded -= AlCargarEscena;
    }
}
