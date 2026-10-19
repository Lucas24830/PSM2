using UnityEngine;
using UnityEngine.InputSystem;

public class CamaraInclinacion : MonoBehaviour
{
    [Header("Limites (en unidades de Unity, respecto a la posicion inicial)")]
    public float limiteIzquierda = -3f;
    public float limiteDerecha = 3f;

    [Header("Ajustes")]
    [Tooltip("Cuanto se mueve la camara por cada 1 g de inclinacion")]
    public float sensibilidad = 6f;

    [Tooltip("Inclinacion minima para empezar a mover (evita temblor)")]
    public float zonaMuerta = 0.05f;

    [Tooltip("Mas alto = movimiento mas rapido, mas bajo = mas suave")]
    public float suavizado = 5f;

    [Tooltip("Marcar si la camara se mueve al reves de lo esperado")]
    public bool invertir = false;

    private float posicionInicialX;
    private float inclinacionSuavizada = 0f;

    // Unidad del acelerometro: en g (reposo = 1) o en m/s2 (reposo = 9.8)
    private float escalaAcelerometro = 1f;
    private bool escalaLista = false;

    void Start()
    {
        posicionInicialX = transform.position.x;

        if (Accelerometer.current != null)
            InputSystem.EnableDevice(Accelerometer.current);
    }

    Vector3 LeerAceleracion()
    {
        Vector3 a = Accelerometer.current.acceleration.ReadValue();

        // Detectar la unidad la primera vez que llegan datos
        if (!escalaLista && a.magnitude > 0.5f)
        {
            escalaAcelerometro = a.magnitude > 3f ? 1f / 9.81f : 1f;
            escalaLista = true;
        }

        return a * escalaAcelerometro;
    }

    void Update()
    {
        if (Accelerometer.current == null)
            return;

        // x = inclinacion izquierda/derecha (aprox. de -1 a 1)
        float inclinacion = LeerAceleracion().x;

        if (invertir)
            inclinacion = -inclinacion;

        // Zona muerta
        if (Mathf.Abs(inclinacion) < zonaMuerta)
            inclinacion = 0f;

        // Filtro para que las sacudidas no hagan brincar la camara
        inclinacionSuavizada = Mathf.Lerp(
            inclinacionSuavizada, inclinacion,
            suavizado * Time.deltaTime);

        // Posicion objetivo dentro de los limites
        float objetivoX = posicionInicialX +
            inclinacionSuavizada * sensibilidad;

        objetivoX = Mathf.Clamp(
            objetivoX,
            posicionInicialX + limiteIzquierda,
            posicionInicialX + limiteDerecha);

        Vector3 pos = transform.position;
        pos.x = objetivoX;
        transform.position = pos;
    }
}