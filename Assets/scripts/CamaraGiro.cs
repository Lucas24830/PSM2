using UnityEngine;
using UnityEngine.InputSystem;

public class CamaraGiro : MonoBehaviour
{
    public enum EjeGiro { X, Y, Z }

    [Header("Limites (respecto a la posicion inicial)")]
    public float limiteIzquierda = -3f;
    public float limiteDerecha = 3f;

    [Header("Ajustes")]
    [Tooltip("Eje del celular alrededor del cual das la vuelta. " +
             "Celular parado en vertical: Y. Celular acostado en horizontal sobre la mesa: Z")]
    public EjeGiro eje = EjeGiro.Y;

    [Tooltip("Unidades que se mueve la camara por cada grado girado")]
    public float unidadesPorGrado = 0.05f;

    [Tooltip("Grados por segundo por debajo de los cuales se ignora (ruido, solo giroscopio)")]
    public float zonaMuerta = 2f;

    [Tooltip("Marcar si la camara se mueve al lado contrario")]
    public bool invertir = false;

    private float posicionInicialX;
    private float desplazamiento = 0f;

    // Sensor de orientacion (AttitudeSensor)
    private Quaternion orientacionInicial;
    private bool orientacionLista = false;
    private float anguloAcumulado = 0f;
    private float anguloAnterior = 0f;

    void Start()
    {
        posicionInicialX = transform.position.x;

        // Nombre completo porque "Gyroscope" tambien existe en el sistema viejo
        var giroscopio = UnityEngine.InputSystem.Gyroscope.current;
        if (giroscopio != null)
            InputSystem.EnableDevice(giroscopio);

        var orientacion = AttitudeSensor.current;
        if (orientacion != null)
            InputSystem.EnableDevice(orientacion);
    }

    void Update()
    {
        var giroscopio = UnityEngine.InputSystem.Gyroscope.current;
        var orientacion = AttitudeSensor.current;

        if (giroscopio != null)
        {
            if (!giroscopio.enabled)
                InputSystem.EnableDevice(giroscopio);
            UsarGiroscopio(giroscopio);
        }
        else if (orientacion != null)
        {
            if (!orientacion.enabled)
                InputSystem.EnableDevice(orientacion);
            UsarOrientacion(orientacion);
        }
        else
        {
            return;
        }

        AplicarPosicion();
    }

    void UsarGiroscopio(UnityEngine.InputSystem.Gyroscope giroscopio)
    {
        Vector3 velocidad = giroscopio.angularVelocity.ReadValue();

        float gradosPorSegundo = Componente(velocidad) * Mathf.Rad2Deg;

        if (Mathf.Abs(gradosPorSegundo) < zonaMuerta)
            return;

        float direccion = invertir ? 1f : -1f;

        desplazamiento += direccion * gradosPorSegundo *
            Time.deltaTime * unidadesPorGrado;
    }

    void UsarOrientacion(AttitudeSensor sensor)
    {
        Quaternion q = sensor.attitude.ReadValue();

        // Si todavia no manda datos validos, esperar
        if (q.x == 0f && q.y == 0f && q.z == 0f && q.w == 0f)
            return;

        if (!orientacionLista)
        {
            orientacionInicial = q;
            orientacionLista = true;
            anguloAnterior = 0f;
            anguloAcumulado = 0f;
            return;
        }

        // Rotacion respecto a la posicion en que empezo el juego
        Quaternion delta = Quaternion.Inverse(orientacionInicial) * q;
        if (delta.w < 0f)
            delta = new Quaternion(-delta.x, -delta.y, -delta.z, -delta.w);

        float parteEje;
        switch (eje)
        {
            case EjeGiro.X: parteEje = delta.x; break;
            case EjeGiro.Z: parteEje = delta.z; break;
            default: parteEje = delta.y; break;
        }

        float angulo = 2f * Mathf.Atan2(parteEje, delta.w) * Mathf.Rad2Deg;

        // Acumular sin brincos al pasar de 180 a -180
        anguloAcumulado += Mathf.DeltaAngle(anguloAnterior, angulo);
        anguloAnterior = angulo;

        float direccion = invertir ? 1f : -1f;

        // Limitar tambien el angulo para que no se "acumule" de mas
        float minAngulo = limiteIzquierda / unidadesPorGrado;
        float maxAngulo = limiteDerecha / unidadesPorGrado;
        if (!invertir)
        {
            float a = minAngulo;
            minAngulo = -maxAngulo;
            maxAngulo = -a;
        }
        anguloAcumulado = Mathf.Clamp(anguloAcumulado, minAngulo, maxAngulo);

        desplazamiento = direccion * anguloAcumulado * unidadesPorGrado;
    }

    float Componente(Vector3 v)
    {
        switch (eje)
        {
            case EjeGiro.X: return v.x;
            case EjeGiro.Z: return v.z;
            default: return v.y;
        }
    }

    void AplicarPosicion()
    {
        desplazamiento = Mathf.Clamp(
            desplazamiento, limiteIzquierda, limiteDerecha);

        Vector3 pos = transform.position;
        pos.x = posicionInicialX + desplazamiento;
        transform.position = pos;
    }

    // Opcional: llamar esto para regresar la camara al centro
    public void Recentrar()
    {
        desplazamiento = 0f;
        anguloAcumulado = 0f;
        anguloAnterior = 0f;
        orientacionLista = false;
    }
}