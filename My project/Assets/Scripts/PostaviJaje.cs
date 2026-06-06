using UnityEngine;
using TMPro;
using System.Collections;

public class PostaviJaje : MonoBehaviour
{
    public Transform igrac;

    [Header("Jaje zamka")]
    public GameObject jajeUMikrovalnoj;
    public float udaljenostZaPostavljanje = 2f;
    public float vrijemePostavljanja = 3f;

    [Header("Timer")]
    public TMP_Text timerTekst;

    [Header("Zvukovi")]
    public AudioSource audioSource;
    public AudioClip zvukTajmera;
    public AudioClip zvukPostavljeno;
    public float jacinaZvuka = 1f;

    public bool postavljeno = false;
    private bool postavljaSe = false;

    private KretanjeMisem kretanjeIgraca;
    private Rigidbody rb;

    private void Start()
    {
        kretanjeIgraca = igrac.GetComponent<KretanjeMisem>();
        rb = igrac.GetComponent<Rigidbody>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (timerTekst != null)
        {
            timerTekst.gameObject.SetActive(false);
        }

        if (jajeUMikrovalnoj != null)
        {
            jajeUMikrovalnoj.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (postavljeno || postavljaSe) return;

        if (igrac == null) return;

        float udaljenost = Vector3.Distance(igrac.position, transform.position);

        if (udaljenost > udaljenostZaPostavljanje)
        {
            Debug.Log("Igrač je predaleko od mikrovalne.");
            return;
        }

        InventoryIgraca inventory = igrac.GetComponent<InventoryIgraca>();

        if (inventory == null) return;

        if (!inventory.ImaPredmet("Jaje"))
        {
            Debug.Log("Nemaš jaje u inventoryju.");
            return;
        }

        StartCoroutine(PostaviJajeNakonVremena(inventory));
    }

    private IEnumerator PostaviJajeNakonVremena(InventoryIgraca inventory)
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

        PokreniZvukTajmera();

        float preostaloVrijeme = vrijemePostavljanja;

        while (preostaloVrijeme > 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                PrekiniPostavljanje();
                yield break;
            }

            if (timerTekst != null)
            {
                timerTekst.text = Mathf.Ceil(preostaloVrijeme).ToString();
            }

            preostaloVrijeme -= Time.deltaTime;
            yield return null;
        }

        ZaustaviZvukTajmera();

        if (jajeUMikrovalnoj != null)
        {
            jajeUMikrovalnoj.SetActive(true);
        }

        inventory.UkloniPredmet("Jaje");

        PustiZvukPostavljeno();

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

        Debug.Log("Jaje je postavljeno u mikrovalnu.");
    }

    private void PrekiniPostavljanje()
    {
        ZaustaviZvukTajmera();

        if (timerTekst != null)
        {
            timerTekst.text = "PREKINUTO";
            timerTekst.gameObject.SetActive(false);
        }

        if (kretanjeIgraca != null)
        {
            kretanjeIgraca.enabled = true;
            kretanjeIgraca.ZaustaviKretanje();
            kretanjeIgraca.IgnorirajSljedeciKlik();
        }

        postavljaSe = false;

        Debug.Log("Postavljanje jajeta je prekinuto.");
    }

    private void PokreniZvukTajmera()
    {
        if (audioSource == null || zvukTajmera == null) return;

        audioSource.clip = zvukTajmera;
        audioSource.loop = true;
        audioSource.volume = jacinaZvuka;
        audioSource.Play();
    }

    private void ZaustaviZvukTajmera()
    {
        if (audioSource == null) return;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.loop = false;
    }

    private void PustiZvukPostavljeno()
    {
        if (zvukPostavljeno == null) return;

        AudioSource.PlayClipAtPoint(
            zvukPostavljeno,
            Camera.main.transform.position,
            jacinaZvuka
        );
    }
}