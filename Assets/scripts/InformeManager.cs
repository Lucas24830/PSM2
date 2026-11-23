
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InformeManager : MonoBehaviour
{
    public EvidenciaDrag[] evidencias;
    public TMP_Text textoResultado;
    public TMP_Text text;
    public GameObject panel;
    public bool flag = false;
    public GameObject continueButton;

    public TMP_Text texto;
    public string[] dialogos;
    public float velocidad = 0.05f;
    public string nombreEscena;

    private int indice = -1;
    private bool escribiendo = false;
    private bool dialogoTerminado = false;
    private Coroutine corrutina;

    public void Start()
    {
        panel.SetActive(false);
    }

    public void Update()
    {
        if (flag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                AvanzarDialogo();
            }
        }
    }

    public void EvaluarCaso()
    {
        panel.SetActive(true);
        int correctas = 0;

        foreach (EvidenciaDrag evidencia in evidencias)
        {
            if (evidencia.categoriaElegida ==
                evidencia.categoriaCorrecta)
            {
                correctas++;
            }
        }

        textoResultado.text =
            "Evidencias correctas: " +
            correctas + "/" + evidencias.Length;

        if (correctas == 3)
        {
            textoResultado.text +=
                "\n¡Excelente investigación!";
        }
        else if (correctas == 2)
        {
            textoResultado.text +=
                "\nInvestigación parcialmente correcta.";
        }
        else
        {
            textoResultado.text +=
                "\nEl informe presenta inconsistencias.";
        }
    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void AvanzarDialogo()
    {
        if (dialogoTerminado)
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else if (escribiendo)
        {
            StopCoroutine(corrutina);
            texto.text = dialogos[indice];
            escribiendo = false;
        }
        else
        {
            SiguienteDialogo();
        }
    }

    void SiguienteDialogo()
    {
        indice++;

        if (indice >= dialogos.Length)
        {
            dialogoTerminado = true;
            //texto.text = "Toca la pantalla para continuar...";
            return;
        }

        corrutina = StartCoroutine(
            EscribirTexto(dialogos[indice])
        );
    }

    IEnumerator EscribirTexto(string frase)
    {
        escribiendo = true;
        texto.text = "";

        foreach (char letra in frase)
        {
            texto.text += letra;
            yield return new WaitForSeconds(velocidad);
        }

        escribiendo = false;
    }

    public void Continue()
    {
        flag = true;
        text.text = "Resumen de los acontecimientos";
        texto.text = "Toque la pantalla para continuar...";
        continueButton.SetActive(false);
    }
}

