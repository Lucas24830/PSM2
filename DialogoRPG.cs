
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class DialogoRPG : MonoBehaviour
{
    public TMP_Text texto;
    public string[] dialogos;
    public float velocidad = 0.05f;
    public string nombreEscena;

    private int indice = -1;
    private bool escribiendo = false;
    private bool dialogoTerminado = false;
    private Coroutine corrutina;

    void Start()
    {
        SiguienteDialogo();
    }

    void Update()
    {
        // Detectar un toque en celular o un clic en PC
        if (Input.GetMouseButtonDown(0))
        {
            AvanzarDialogo();
        }
    }

    void AvanzarDialogo()
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
}
