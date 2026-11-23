using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ControlPruebas
{
    public const string escenaFinal = "aftermath";
    public const int pruebasNecesarias = 3;

    private static readonly HashSet<object> recogidas = new HashSet<object>();
    private static int abiertas = 0;

    public static bool PantallaQuieta => abiertas > 0;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reiniciar()
    {
        recogidas.Clear();
        abiertas = 0;
    }

    public static void Abrir(object prueba)
    {
        abiertas++;
        recogidas.Add(prueba); 
    }

    public static void Cerrar(object prueba)
    {
        abiertas = Mathf.Max(0, abiertas - 1);

        if (abiertas == 0 && recogidas.Count >= pruebasNecesarias)
        {
            Reiniciar();
            SceneManager.LoadScene(escenaFinal);
        }
    }
}