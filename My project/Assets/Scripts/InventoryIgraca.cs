using System.Collections.Generic;
using UnityEngine;

public class InventoryIgraca : MonoBehaviour
{
    private List<string> predmeti = new List<string>();

    [Header("Inventory UI")]
    public GameObject ikonaSapuna;
    public GameObject ikonaJajeta;
    public GameObject ikonaLjepila;
    public GameObject ikonaLjutogUmaka;

    [Header("Odabrani predmet")]
    [SerializeField]
    private string odabraniPredmet = "";

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

        if (odabraniPredmet == imePredmeta)
        {
            odabraniPredmet = "";
        }

        OsvjeziInventoryUI();
    }

    public void OdaberiSapun()
    {
        OdaberiPredmet("Sapun");
    }

    public void OdaberiJaje()
    {
        OdaberiPredmet("Jaje");
    }

    public void OdaberiLjepilo()
    {
        OdaberiPredmet("Ljepilo");
    }

    public void OdaberiLjutiUmak()
    {
        OdaberiPredmet("LjutiUmak");
    }

    public void OdaberiPredmet(string imePredmeta)
    {
        if (!ImaPredmet(imePredmeta))
        {
            Debug.Log(
                "Igrač nema predmet: " +
                imePredmeta
            );

            return;
        }

        odabraniPredmet = imePredmeta;

        Debug.Log(
            "Odabran predmet: " +
            odabraniPredmet
        );
    }

    public bool JePredmetOdabran(string imePredmeta)
    {
        return odabraniPredmet == imePredmeta;
    }

    public string DohvatiOdabraniPredmet()
    {
        return odabraniPredmet;
    }

    public void PonistiOdabraniPredmet()
    {
        odabraniPredmet = "";

        Debug.Log(
            "Odabir predmeta je poništen."
        );
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

        if (ikonaLjepila != null)
        {
            ikonaLjepila.SetActive(ImaPredmet("Ljepilo"));
        }

        if (ikonaLjutogUmaka != null)
        {
            ikonaLjutogUmaka.SetActive(ImaPredmet("LjutiUmak"));
        }
    }
}