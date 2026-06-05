using System.Collections.Generic;
using UnityEngine;

public class InventoryIgraca : MonoBehaviour
{
    private List<string> predmeti = new List<string>();

    [Header("Inventory UI")]
    public GameObject ikonaSapuna;
    public GameObject ikonaJajeta;

    private void Start()
    {
        OsvjeziInventoryUI();
    }

    public void DodajPredmet(string imePredmeta)
    {
        if (!predmeti.Contains(imePredmeta))
        {
            predmeti.Add(imePredmeta);
            Debug.Log("Dodan predmet: " + imePredmeta);
        }

        OsvjeziInventoryUI();
    }

    public bool ImaPredmet(string imePredmeta)
    {
        return predmeti.Contains(imePredmeta);
    }

    public void UkloniPredmet(string imePredmeta)
    {
        if (predmeti.Contains(imePredmeta))
        {
            predmeti.Remove(imePredmeta);
            Debug.Log("Uklonjen predmet: " + imePredmeta);
        }

        OsvjeziInventoryUI();
    }

    private void OsvjeziInventoryUI()
    {
        if (ikonaSapuna != null)
        {
            ikonaSapuna.SetActive(ImaPredmet("Sapun"));
        }

        if (ikonaJajeta != null)
        {
            ikonaJajeta.SetActive(ImaPredmet("Jaje"));
        }
    }
}