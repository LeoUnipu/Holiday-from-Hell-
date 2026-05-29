using UnityEngine;
using TMPro;
using System.Collections;

public class PostaviSapun : MonoBehaviour
{
    public Transform igrac;
    public GameObject sapunZamka;

    public float udaljenostZaPostavljanje = 1.5f;
    public float vrijemePostavljanja = 3f;

    public TextMeshPro timerTekst;

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
    }

    private void OnMouseDown()
    {
        if (postavljeno) return;
        if (postavljaSe) return;

        float udaljenost = Vector3.Distance(igrac.position, transform.position);

        if (udaljenost <= udaljenostZaPostavljanje)
        {
            StartCoroutine(PostaviZamkuNakonVremena());
        }
        else
        {
            Debug.Log("Igrač je predaleko od mjesta za sapun.");
        }
    }

    private IEnumerator PostaviZamkuNakonVremena()
    {
        postavljaSe = true;

        // Ovdje zaustavljam igrača dok postavlja zamku.
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

        sapunZamka.SetActive(true);

        if (timerTekst != null)
        {
            timerTekst.text = "OK";
            yield return new WaitForSeconds(0.5f);
            timerTekst.gameObject.SetActive(false);
        }

        // Ovdje vraćam kretanje nakon što se zamka postavi.
        if (kretanjeIgraca != null)
        {
            kretanjeIgraca.enabled = true;
            kretanjeIgraca.ZaustaviKretanje();
        }

        postavljeno = true;
        postavljaSe = false;

        Debug.Log("Sapun je postavljen!");
    }
}