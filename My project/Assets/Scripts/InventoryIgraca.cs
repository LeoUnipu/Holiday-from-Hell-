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
    public GameObject ikonaSira;    
    public GameObject ikonaWrencha;    
    public GameObject ikonaPina;    
    public GameObject ikonaDynamita;    
    public GameObject ikonaRazredjivaca;    
    public GameObject ikonaBrasna;    
    
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
    
    public void OdaberiSir()    
    {    
        OdaberiPredmet("Sir");    
    }    
    
    public void OdaberiWrench()    
    {    
        OdaberiPredmet("Wrench");    
    }    
    
    public void OdaberiPin()    
    {    
        OdaberiPredmet("Pin");    
    }    
    
    public void OdaberiDynamite()    
    {    
        OdaberiPredmet("Dynamite");    
    }    
 
    public void OdaberiRazredjivac()    
    {    
        OdaberiPredmet("Razredjivac");    
    }    

    public void OdaberiBrasno()    
    {    
        OdaberiPredmet("Brasno");    
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
    
        if (ikonaSira != null)    
        {    
            ikonaSira.SetActive(ImaPredmet("Sir"));    
        }    
    
        if (ikonaWrencha != null)    
        {    
            ikonaWrencha.SetActive(ImaPredmet("Wrench"));    
        }    
    
        if (ikonaPina != null)    
        {    
            ikonaPina.SetActive(ImaPredmet("Pin"));    
        }    
    
        if (ikonaDynamita != null)    
        {    
            ikonaDynamita.SetActive(ImaPredmet("Dynamite"));    
        }    
 
        if (ikonaRazredjivaca != null)    
        {    
            ikonaRazredjivaca.SetActive(ImaPredmet("Razredjivac"));    
        }    

        if (ikonaBrasna != null)    
        {    
            ikonaBrasna.SetActive(ImaPredmet("Brasno"));    
        }    
    }    
}