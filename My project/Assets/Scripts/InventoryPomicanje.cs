using UnityEngine;

public class InventoryPomicanje : MonoBehaviour
{
    [Header("Itemi")]
    public RectTransform itemi;

    [Header("Pomicanje")]
    public float pomakPoKlikU = 80f;

    [Header("Infinite granice")]
    public float lijevaGranica = -400f;
    public float desnaGranica = 400f;

    public void PomakniLijevo()
    {
        if (itemi == null)
            return;

        Vector2 pozicija = itemi.anchoredPosition;
        pozicija.x += pomakPoKlikU;

        if (pozicija.x > desnaGranica)
        {
            pozicija.x = lijevaGranica;
        }

        itemi.anchoredPosition = pozicija;
    }

    public void PomakniDesno()
    {
        if (itemi == null)
            return;

        Vector2 pozicija = itemi.anchoredPosition;
        pozicija.x -= pomakPoKlikU;

        if (pozicija.x < lijevaGranica)
        {
            pozicija.x = desnaGranica;
        }

        itemi.anchoredPosition = pozicija;
    }
}