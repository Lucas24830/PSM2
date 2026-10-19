
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class InvestigarCuchillos : MonoBehaviour
{
    public Sprite[] fotos;
    public float escalaAmpliada = 4f;
    public float duracion = 0.4f;
    public int ordenAmpliado = 100;

    private SpriteRenderer spriteRenderer;
    private Vector3 escalaOriginal;
    private Vector3 posicionOriginal;
    private int ordenOriginal;
    private bool ampliada = false;
    private bool animando = false;
    private int indiceFoto = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        escalaOriginal = transform.localScale;
        posicionOriginal = transform.position;
        ordenOriginal = spriteRenderer.sortingOrder;

        if (fotos != null && fotos.Length > 0)
            spriteRenderer.sprite = fotos[0];
    }

    void Update()
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
            if (tocoObjeto)
                CambiarFoto();
            else
                StartCoroutine(Regresar());
        }
    }

    void CambiarFoto()
    {
        if (fotos == null || indiceFoto >= fotos.Length - 1)
            return;

        indiceFoto++;
        spriteRenderer.sprite = fotos[indiceFoto];
    }

    IEnumerator Ampliar()
    {
        animando = true;
        ampliada = true;
        spriteRenderer.sortingOrder = ordenAmpliado;

        Vector3 posicionFinal = Camera.main.transform.position;
        posicionFinal.z = transform.position.z;

        Vector3 escalaFinal = escalaOriginal * escalaAmpliada;
        Vector3 posicionInicial = transform.position;
        Vector3 escalaInicial = transform.localScale;
        float tiempo = 0;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracion);

            transform.position = Vector3.Lerp(
                posicionInicial, posicionFinal, t);

            transform.localScale = Vector3.Lerp(
                escalaInicial, escalaFinal, t);

            yield return null;
        }

        transform.position = posicionFinal;
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
        spriteRenderer.sortingOrder = ordenOriginal;

        ampliada = false;
        animando = false;
    }
}
