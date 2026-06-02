using UnityEngine;
using TMPro;
using System.Collections;

public class PostaviSapun : MonoBehaviour
{
    public Transform igrac;

    [Header("Sapun zamka")]
    public GameObject sapunZamka;
    public float udaljenostZaPostavljanje = 2f;
    public float vrijemePostavljanja = 3f;

    [Header("Timer")]
    public TMP_Text timerTekst;

    private bool postavljeno = false;
    private bool postavljaSe = false;

    private KretanjeMisem kretanjeIgraca;
    private Rigidbody rb;

    private void Start()
    {
        kretanjeIgraca = igrac.GetComponent<KretanjeMisem>();
        rb = igrac.GetComponent<Rigidbody>();

        if (timerTekst != null)
        {
            timerTekst.gameObject.SetActive(false);
        }

        if (sapunZamka != null)
        {
            sapunZamka.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (postavljeno || postavljaSe) return;

        if (igrac == null) return;

        float udaljenost = Vector3.Distance(igrac.position, transform.position);

        if (udaljenost > udaljenostZaPostavljanje)
        {
            Debug.Log("Igrač je predaleko od mjesta za postavljanje sapuna.");
            return;
        }

        InventoryIgraca inventory = igrac.GetComponent<InventoryIgraca>();

        if (inventory == null) return;

        if (!inventory.ImaPredmet("Sapun"))
        {
            Debug.Log("Nemaš sapun u inventoryju.");
            return;
        }

        StartCoroutine(PostaviZamkuNakonVremena(inventory));
    }

    private IEnumerator PostaviZamkuNakonVremena(InventoryIgraca inventory)
    {
        postavljaSe = true;

        if (kretanjeIgraca != null)
        {
            kretanjeIgraca.ZaustaviKretanje();
            kretanjeIgraca.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (timerTekst != null)
        {
            timerTekst.gameObject.SetActive(true);
        }

        float preostaloVrijeme = vrijemePostavljanja;

        while (preostaloVrijeme > 0)
        {
            if (timerTekst != null)
            {
                timerTekst.text = Mathf.Ceil(preostaloVrijeme).ToString();
            }

            preostaloVrijeme -= Time.deltaTime;
            yield return null;
        }

        if (sapunZamka != null)
        {
            sapunZamka.SetActive(true);
        }

        inventory.UkloniPredmet("Sapun");

        if (timerTekst != null)
        {
            timerTekst.text = "OK";
            yield return new WaitForSeconds(0.5f);
            timerTekst.gameObject.SetActive(false);
        }

        if (kretanjeIgraca != null)
        {
            kretanjeIgraca.enabled = true;
            kretanjeIgraca.ZaustaviKretanje();
            kretanjeIgraca.IgnorirajSljedeciKlik();
        }

        postavljeno = true;
        postavljaSe = false;

        Debug.Log("Sapun je postavljen kao zamka.");
    }
}