using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class InvestigarFoto : MonoBehaviour
{
    [Header("Fotografias")]
    public Sprite[] fotos;

    [Header("Zoom")]
    public float escalaAmpliada = 4f;
    public float duracion = 0.4f;
    public int ordenAmpliado = 100;

    [Header("Agitar celular")]
    public float umbralAgitado = 1.0f;
    public float tiempoEntreAgitadas = 0.8f;

    private SpriteRenderer spriteRenderer;
    private Vector3 escalaOriginal;
    private Vector3 posicionOriginal;
    private int ordenOriginal;
    private bool ampliada = false;
    private bool animando = false;
    private int indiceFoto = 0;

    private Vector3 posicionAmpliada;

    // Agitado
    private Vector3 ultimaAceleracion;
    private float siguienteAgitado = 0f;

    // Unidad del acelerometro: en g (reposo = 1) o en m/s2 (reposo = 9.8)
    private float escalaAcelerometro = 1f;
    private bool escalaLista = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        escalaOriginal = transform.localScale;
        posicionOriginal = transform.position;
        ordenOriginal = spriteRenderer.sortingOrder;

        if (fotos != null && fotos.Length > 0)
            spriteRenderer.sprite = fotos[0];

        if (Camera.main != null)
        {
            posicionAmpliada = Camera.main.transform.position;
            posicionAmpliada.z = transform.position.z;
        }

        // El acelerometro viene desactivado por defecto en el Input System
        if (Accelerometer.current != null)
            InputSystem.EnableDevice(Accelerometer.current);
    }

    void Update()
    {
        DetectarAgitada();
        DetectarToque();
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

    void DetectarAgitada()
    {
        if (Accelerometer.current == null)
            return;

        // Siempre se actualiza la ultima aceleracion, aunque la foto
        // no este ampliada, para no comparar contra un valor viejo
        Vector3 aceleracion = LeerAceleracion();

        // Hasta que llegue el primer dato valido no se compara nada
        if (!escalaLista)
        {
            ultimaAceleracion = aceleracion;
            return;
        }

        float movimiento = (aceleracion - ultimaAceleracion).magnitude;
        ultimaAceleracion = aceleracion;

        if (!ampliada || animando)
            return;

        if (movimiento >= umbralAgitado &&
            Time.time >= siguienteAgitado)
        {
            CambiarFoto();
            siguienteAgitado = Time.time + tiempoEntreAgitadas;
        }
    }

    void CambiarFoto()
    {
        // Si ya es la ultima foto, no hacer nada
        if (fotos == null || indiceFoto >= fotos.Length - 1)
            return;

        indiceFoto++;
        spriteRenderer.sprite = fotos[indiceFoto];
    }

    void DetectarToque()
    {
        if (animando || Camera.main == null)
            return;

        Vector2 posicionToque = Vector2.zero;
        bool huboToque = false;

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            posicionToque = Touchscreen.current.primaryTouch.position.ReadValue();
            huboToque = true;
        }
        else if (Mouse.current != null &&
                 Mouse.current.leftButton.wasPressedThisFrame)
        {
            posicionToque = Mouse.current.position.ReadValue();
            huboToque = true;
        }

        if (!huboToque)
            return;

        Ray rayo = Camera.main.ScreenPointToRay(posicionToque);
        RaycastHit2D impacto = Physics2D.GetRayIntersection(rayo);

        bool tocoObjeto = impacto.collider != null &&
            impacto.collider.gameObject == gameObject;

        if (!ampliada && tocoObjeto)
        {
            StartCoroutine(Ampliar());
        }
        else if (ampliada)
        {
            if (!tocoObjeto)
            {
                StartCoroutine(Regresar());
            }
            // Si toca la foto ampliada no pasa nada:
            // las imagenes cambian al agitar el celular.
        }
    }

    IEnumerator Ampliar()
    {
        animando = true;
        ampliada = true;

        // Se pone por encima de los demas objetos
        spriteRenderer.sortingOrder = ordenAmpliado;

        posicionAmpliada = Camera.main.transform.position;
        posicionAmpliada.z = transform.position.z;

        Vector3 escalaFinal = escalaOriginal * escalaAmpliada;
        Vector3 posicionInicial = transform.position;
        Vector3 escalaInicial = transform.localScale;

        float tiempo = 0;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracion);

            transform.position = Vector3.Lerp(
                posicionInicial, posicionAmpliada, t);

            transform.localScale = Vector3.Lerp(
                escalaInicial, escalaFinal, t);

            yield return null;
        }

        transform.position = posicionAmpliada;
        transform.localScale = escalaFinal;
        animando = false;
    }

    IEnumerator Regresar()
    {
        animando = true;

        Vector3 posicionInicial = transform.position;
        Vector3 escalaInicial = transform.localScale;
        float tiempo = 0;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracion);

            transform.position = Vector3.Lerp(
                posicionInicial, posicionOriginal, t);

            transform.localScale = Vector3.Lerp(
                escalaInicial, escalaOriginal, t);

            yield return null;
        }

        transform.position = posicionOriginal;
        transform.localScale = escalaOriginal;

        // Regresa al orden que tenia antes
        spriteRenderer.sortingOrder = ordenOriginal;

        ampliada = false;
        animando = false;
    }
}