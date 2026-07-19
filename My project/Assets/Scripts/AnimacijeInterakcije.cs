using UnityEngine;
using System.Collections;

public class AnimacijeInterakcije : MonoBehaviour
{
    [Header("Komponente igrača")]
    public Animator animator;
    public KretanjeMisem kretanjeIgraca;
    public Rigidbody rbIgraca;

    [Header("Trajanje animacija")]
    public float trajanjeSkupljanja = 2f;
    public float trajanjePretrage = 2f;
    public float trajanjePostavljanjaNisko = 2f;
    public float trajanjePostavljanjaVisoko = 2f;

    [Header("Prekid")]
    public float zastitaOdPrvogKlika = 0.3f;

    private bool animacijaUTijeku = false;
    private bool mozeSePrekinuti = false;
    private bool zadnjaAnimacijaJePrekinuta = false;

    private float vrijemePocetka;
    private Coroutine aktivnaCoroutine;

    private void Awake()
    {
        PronadiKomponente();
        ResetirajTriggere();
    }

    private void Update()
    {
        if (!animacijaUTijeku)
        {
            return;
        }

        if (!mozeSePrekinuti)
        {
            return;
        }


        if (Time.time <
            vrijemePocetka + zastitaOdPrvogKlika)
        {
            return;
        }

        
        if (Input.GetMouseButtonDown(1))
        {
            PrekiniTrenutnuAnimaciju();
        }
    }

    private void PronadiKomponente()
    {
        if (kretanjeIgraca == null)
        {
            kretanjeIgraca =
                GetComponent<KretanjeMisem>();
        }

        if (kretanjeIgraca == null)
        {
            kretanjeIgraca =
                GetComponentInParent<KretanjeMisem>();
        }

        if (rbIgraca == null &&
            kretanjeIgraca != null)
        {
            rbIgraca =
                kretanjeIgraca.GetComponent<Rigidbody>();
        }

        if (rbIgraca == null)
        {
            rbIgraca =
                GetComponentInParent<Rigidbody>();
        }

        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }
    }

    public bool JeLiAnimacijaUTijeku()
    {
        return animacijaUTijeku;
    }

    public bool JeLiZadnjaAnimacijaPrekinuta()
    {
        return zadnjaAnimacijaJePrekinuta;
    }

    public void PokreniSkupljanje()
    {
        
        PokreniAnimaciju(
            "Pickup",
            trajanjeSkupljanja,
            true
        );
    }

    public void PokreniPraznuPretragu()
    {
        
        PokreniAnimaciju(
            "Search",
            trajanjePretrage,
            true
        );
    }

    public void PokreniPostavljanjeNisko()
    {
        
        PokreniAnimaciju(
            "PlaceLow",
            trajanjePostavljanjaNisko,
            true
        );
    }

    public void PokreniPostavljanjeVisoko()
    {
        
        PokreniAnimaciju(
            "PlaceHigh",
            trajanjePostavljanjaVisoko,
            true
        );
    }

    private void PokreniAnimaciju(
        string trigger,
        float trajanje,
        bool dopustiPrekid)
    {
        if (animator == null)
        {
            Debug.LogWarning("Animator nije spojen.");
            return;
        }

        if (animacijaUTijeku)
        {
            return;
        }

        zadnjaAnimacijaJePrekinuta = false;
        animacijaUTijeku = true;
        mozeSePrekinuti = dopustiPrekid;
        vrijemePocetka = Time.time;

        aktivnaCoroutine = StartCoroutine(
            AnimacijaCoroutine(trigger, trajanje)
        );
    }

    private IEnumerator AnimacijaCoroutine(
        string trigger,
        float trajanje)
    {
        PronadiKomponente();
        ZakljucajIgraca();

        ResetirajTriggere();
        animator.SetTrigger(trigger);

        yield return new WaitForSeconds(trajanje);

        aktivnaCoroutine = null;
        animacijaUTijeku = false;
        mozeSePrekinuti = false;

        
        zadnjaAnimacijaJePrekinuta = false;

        OtkljucajIgraca();
    }

    public void PrekiniTrenutnuAnimaciju()
    {
        if (!animacijaUTijeku ||
            !mozeSePrekinuti)
        {
            return;
        }

        if (aktivnaCoroutine != null)
        {
            StopCoroutine(aktivnaCoroutine);
            aktivnaCoroutine = null;
        }

        zadnjaAnimacijaJePrekinuta = true;
        animacijaUTijeku = false;
        mozeSePrekinuti = false;

        ResetirajTriggere();

        if (animator != null)
        {
            animator.CrossFade(
                "Movement",
                0.1f
            );
        }

        OtkljucajIgraca();

        Debug.Log(
            "Animacija i radnja su prekinute desnim klikom."
        );
    }

    private void ZakljucajIgraca()
    {
        if (kretanjeIgraca != null)
        {
            kretanjeIgraca.ZaustaviKretanje();
            kretanjeIgraca.enabled = false;
        }
        else
        {
            Debug.LogWarning(
                "KretanjeMisem nije pronađen."
            );
        }

        if (rbIgraca != null)
        {
            rbIgraca.linearVelocity =
                Vector3.zero;

            rbIgraca.angularVelocity =
                Vector3.zero;
        }
    }

    private void OtkljucajIgraca()
    {
        if (rbIgraca != null)
        {
            rbIgraca.linearVelocity =
                Vector3.zero;

            rbIgraca.angularVelocity =
                Vector3.zero;
        }

        if (kretanjeIgraca != null)
        {
            kretanjeIgraca.enabled = true;
            kretanjeIgraca.ZaustaviKretanje();
            kretanjeIgraca.IgnorirajSljedeciKlik();
        }
    }

    private void ResetirajTriggere()
    {
        if (animator == null)
        {
            return;
        }

        animator.ResetTrigger("Pickup");
        animator.ResetTrigger("Search");
        animator.ResetTrigger("PlaceLow");
        animator.ResetTrigger("PlaceHigh");
    }
}