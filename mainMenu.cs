using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public GameObject CreditsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreditsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Credits()
    {
        CreditsPanel.SetActive(true);
    }

    public void Inicio()
    {
        Debug.Log("Start");
        SceneManager.LoadScene("Cutscene");
    }

    public void Salir()
    {
        Application.Quit();
    }

    public void Cerrar()
    {
        CreditsPanel.SetActive(false);
    }
}
