
using UnityEngine;
using UnityEngine.EventSystems;

public class EvidenciaDrag : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Transform posicionOriginal;
    private Vector2 posicionInicial;
    private bool fueSoltada;

    public string categoriaCorrecta;
    public string categoriaElegida;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        posicionOriginal = transform.parent;
        posicionInicial = rectTransform.anchoredPosition;

        fueSoltada = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!fueSoltada)
        {
            transform.SetParent(posicionOriginal);
            rectTransform.anchoredPosition = posicionInicial;
        }
    }

    public void SoltarEnZona(
        Transform zona,
        string categoria)
    {
        transform.SetParent(zona);
        rectTransform.anchoredPosition = Vector2.zero;

        categoriaElegida = categoria;
        fueSoltada = true;
    }
}
