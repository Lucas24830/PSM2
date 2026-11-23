
using UnityEngine;
using UnityEngine.EventSystems;

public class ZonaDrop : MonoBehaviour, IDropHandler
{
    public string nombreCategoria;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objeto = eventData.pointerDrag;

        if (objeto == null) return;

        EvidenciaDrag evidencia =
            objeto.GetComponent<EvidenciaDrag>();

        if (evidencia != null)
        {
            evidencia.SoltarEnZona(
                transform,
                nombreCategoria
            );
        }
    }
}

