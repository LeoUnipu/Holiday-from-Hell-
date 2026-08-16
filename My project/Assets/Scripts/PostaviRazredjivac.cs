using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PostaviRazredjivac : MonoBehaviour
{
    [Header("Igrač")]
    public Transform igrac;

    [Header("Razredjivac zamka")]
    public GameObject razredjivacNaMjestu;
    public float udaljenostZaPostavljanje = 2f;

    [Header("Animacija")]
    [Tooltip("Animator koji izvodi animaciju postavljanja.")]
    public Animator animatorIgraca;

    [Tooltip("Točan naziv Idle stanja u Animatoru.")]
    public string idleStateName = "Idle";

    [Tooltip("1 = normalno, 1.5 = brže, 2 = dvostruko brže.")]
    public float brzinaAnimacije = 1.5f;

    [Tooltip("Ukupno trajanje odbrojavanja.")]
    public float ukupnoTrajanjePostavljanja = 5f;

    [Tooltip("Najduže čekanje povratka u Idle.")]
    public float maksimalnoCekanjeIdle = 10f;

    [Header("Animacija pogrešnog predmeta")]
    [Tooltip(
        "Trigger koji se pokreće kada igrač odabere pogrešan predmet. " +
        "Primjer: WrongItem."
    )]
    public string triggerPogresnogPredmeta = "WrongItem";

    [Tooltip(
        "Točan naziv Animator stanja za pogrešan predmet. " +
        "Primjer: WrongItem."
    )]
    public string stanjePogresnogPredmeta = "WrongItem";

    [Tooltip(
        "Najduže čekanje završetka animacije pogrešnog predmeta."
    )]
    public float maksimalnoTrajanjePogresneAnimacije = 5f;

    [Header("Progress bar")]
    public GameObject progressBar;
    public Image progressBarFill;

    [Header("Zvukovi")]
    public AudioSource audioSource;
    public AudioClip zvukTajmera;
    public AudioClip zvukPostavljeno;
    public float jacinaZvuka = 1f;

    public bool postavljeno = false;

    private bool postavljaSe = false;
    private bool pogresnaAnimacijaUTijeku = false;

    private KretanjeMisem kretanjeIgraca;
    private AnimacijeInterakcije animacije;
    private Rigidbody rb;

    private Coroutine postavljanjeCoroutine;
    private Coroutine pogresnaAnimacijaCoroutine;

    private float originalnaBrzinaAnimatora = 1f;

    private void Start()
    {
        if (igrac != null)
        {
            kretanjeIgraca =
                igrac.GetComponent<KretanjeMisem>();

            animacije =
                igrac.GetComponentInChildren<AnimacijeInterakcije>(true);

            rb =
                igrac.GetComponent<Rigidbody>();

            if (animatorIgraca == null)
            {
                animatorIgraca =
                    igrac.GetComponentInChildren<Animator>(true);
            }
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (progressBar != null)
        {
            progressBar.SetActive(false);
        }

        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = 0f;
        }

        if (razredjivacNaMjestu != null)
        {
            razredjivacNaMjestu.SetActive(false);
        }
    }

    private void Update()
    {
        if (!postavljaSe)
        {
            return;
        }

        DrziIgracaZakljucanog();

        if (Input.GetMouseButtonDown(1))
        {
            PrekiniPostavljanje();
        }
    }

    private void FixedUpdate()
    {
        if (!postavljaSe &&
            !pogresnaAnimacijaUTijeku)
        {
            return;
        }

        ZaustaviRigidbody();
    }

    private void LateUpdate()
    {
        if (!postavljaSe &&
            !pogresnaAnimacijaUTijeku)
        {
            return;
        }

        ZaustaviRigidbody();
    }

    private void OnMouseDown()
    {
        if (postavljaSe ||
            postavljeno ||
            pogresnaAnimacijaUTijeku)
        {
            return;
        }

        if (igrac == null)
        {
            Debug.LogWarning(
                "Igrač nije postavljen u PostaviRazredjivac skripti."
            );

            return;
        }

        float udaljenost = Vector3.Distance(
            igrac.position,
            transform.position
        );

        if (udaljenost > udaljenostZaPostavljanje)
        {
            Debug.Log(
                "Igrač je predaleko od mikrovalne."
            );

            return;
        }

        InventoryIgraca inventory =
            igrac.GetComponent<InventoryIgraca>();

        if (inventory == null)
        {
            Debug.LogWarning(
                "InventoryIgraca nije pronađen na igraču."
            );

            return;
        }

        if (string.IsNullOrEmpty(
            inventory.DohvatiOdabraniPredmet()))
        {
            Debug.Log(
                "Prvo moraš odabrati predmet iz inventoryja."
            );

            PokreniPogresnuAnimaciju();
            return;
        }

        if (!inventory.JePredmetOdabran("Razredjivac"))
        {
            Debug.Log(
                "U mikrovalnu moraš staviti Razredjivac. " +
                "Odabrani predmet je: " +
                inventory.DohvatiOdabraniPredmet()
            );

            PokreniPogresnuAnimaciju();
            return;
        }

        if (!inventory.ImaPredmet("Razredjivac"))
        {
            Debug.Log(
                "Nemaš razredjivac u inventoryju."
            );

            return;
        }

        if (animacije == null)
        {
            Debug.LogWarning(
                "AnimacijeInterakcije nije pronađena."
            );

            return;
        }

        if (animacije.JeLiAnimacijaUTijeku())
        {
            return;
        }

        postavljanjeCoroutine = StartCoroutine(
            PostaviRazredjivacNakonAnimacije(inventory)
        );
    }

    private void PokreniPogresnuAnimaciju()
    {
        if (pogresnaAnimacijaUTijeku)
        {
            return;
        }

        if (animatorIgraca == null)
        {
            Debug.LogWarning(
                "Animator igrača nije postavljen za pogrešnu animaciju."
            );

            return;
        }

        if (string.IsNullOrEmpty(
            triggerPogresnogPredmeta))
        {
            Debug.LogWarning(
                "Trigger pogrešnog predmeta nije postavljen."
            );

            return;
        }

        if (string.IsNullOrEmpty(
            stanjePogresnogPredmeta))
        {
            Debug.LogWarning(
                "Stanje pogrešnog predmeta nije postavljeno."
            );

            return;
        }

        if (!AnimatorImaTrigger(
            animatorIgraca,
            triggerPogresnogPredmeta))
        {
            Debug.LogWarning(
                "Animator nema Trigger parametar: " +
                triggerPogresnogPredmeta
            );

            return;
        }

        pogresnaAnimacijaCoroutine =
            StartCoroutine(
                PokreniICekajPogresnuAnimaciju()
            );
    }

    private IEnumerator PokreniICekajPogresnuAnimaciju()
    {
        pogresnaAnimacijaUTijeku = true;

        ZakljucajKretanje();
        OkreniIgracaPremaMikrovalnoj();

        animatorIgraca.ResetTrigger(
            triggerPogresnogPredmeta
        );

        animatorIgraca.SetTrigger(
            triggerPogresnogPredmeta
        );

        float maksimalnoVrijeme =
            Mathf.Max(
                0.5f,
                maksimalnoTrajanjePogresneAnimacije
            );

        float timer = 0f;
        bool animacijaJePocela = false;

        while (timer < maksimalnoVrijeme)
        {
            ZaustaviRigidbody();

            AnimatorStateInfo trenutnoStanje =
                animatorIgraca.GetCurrentAnimatorStateInfo(0);

            AnimatorStateInfo sljedeceStanje =
                animatorIgraca.GetNextAnimatorStateInfo(0);

            bool trenutnoJePogresnaAnimacija =
                trenutnoStanje.IsName(
                    stanjePogresnogPredmeta
                );

            bool sljedeceJePogresnaAnimacija =
                animatorIgraca.IsInTransition(0) &&
                sljedeceStanje.IsName(
                    stanjePogresnogPredmeta
                );

            if (trenutnoJePogresnaAnimacija ||
                sljedeceJePogresnaAnimacija)
            {
                animacijaJePocela = true;
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }

        if (!animacijaJePocela)
        {
            Debug.LogWarning(
                "Animator nije ušao u stanje pogrešnog predmeta: " +
                stanjePogresnogPredmeta
            );

            ZavrsiPogresnuAnimaciju();
            yield break;
        }

        timer = 0f;

        while (timer < maksimalnoVrijeme)
        {
            ZaustaviRigidbody();

            AnimatorStateInfo trenutnoStanje =
                animatorIgraca.GetCurrentAnimatorStateInfo(0);

            bool animatorJeUPrijelazu =
                animatorIgraca.IsInTransition(0);

            bool trenutnoJePogresnaAnimacija =
                trenutnoStanje.IsName(
                    stanjePogresnogPredmeta
                );

            bool animacijaJeZavrsila =
                trenutnoJePogresnaAnimacija &&
                trenutnoStanje.normalizedTime >= 1f;

            if (animacijaJeZavrsila &&
                animatorJeUPrijelazu)
            {
                break;
            }

            if (!trenutnoJePogresnaAnimacija &&
                !animatorJeUPrijelazu)
            {
                break;
            }

            timer += Time.deltaTime;

            yield return null;
        }

        timer = 0f;

        while (animatorIgraca.IsInTransition(0) &&
               timer < 2f)
        {
            ZaustaviRigidbody();

            timer += Time.deltaTime;

            yield return null;
        }

        ZavrsiPogresnuAnimaciju();
    }

    private void ZavrsiPogresnuAnimaciju()
    {
        pogresnaAnimacijaUTijeku = false;
        pogresnaAnimacijaCoroutine = null;

        OtkljucajKretanje();

        Debug.Log(
            "Završena je animacija pogrešnog predmeta."
        );
    }

    private IEnumerator PostaviRazredjivacNakonAnimacije(
        InventoryIgraca inventory)
    {
        postavljaSe = true;

        ZakljucajKretanje();
        OkreniIgracaPremaMikrovalnoj();

        yield return null;

        DrziIgracaZakljucanog();

        float sigurnaBrzinaAnimacije =
            Mathf.Max(0.1f, brzinaAnimacije);

        if (animatorIgraca != null)
        {
            originalnaBrzinaAnimatora =
                animatorIgraca.speed;

            animatorIgraca.speed =
                sigurnaBrzinaAnimacije;
        }

        animacije.PokreniPostavljanjeVisoko();

        if (progressBar != null)
        {
            progressBar.SetActive(true);
        }

        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = 0f;
        }

        PokreniZvukTajmera();

        float ukupnoVrijeme =
            Mathf.Max(
                0.1f,
                ukupnoTrajanjePostavljanja
            );

        float preostaloVrijeme = ukupnoVrijeme;

        while (preostaloVrijeme > 0f)
        {
            if (!postavljaSe)
            {
                yield break;
            }

            DrziIgracaZakljucanog();

            preostaloVrijeme -= Time.deltaTime;

            if (progressBarFill != null)
            {
                float protekloVrijeme =
                    ukupnoVrijeme -
                    preostaloVrijeme;

                progressBarFill.fillAmount =
                    Mathf.Clamp01(
                        protekloVrijeme /
                        ukupnoVrijeme
                    );
            }

            yield return null;
        }

        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = 1f;
        }

        while (animacije.JeLiAnimacijaUTijeku())
        {
            if (!postavljaSe)
            {
                yield break;
            }

            DrziIgracaZakljucanog();

            yield return null;
        }

        yield return StartCoroutine(
            CekajPotpuniPovratakUIdle()
        );

        VratiBrzinuAnimatora();
        ZaustaviZvukTajmera();

        if (!postavljaSe)
        {
            yield break;
        }

        if (animacije.JeLiZadnjaAnimacijaPrekinuta())
        {
            PrekiniPostavljanje();
            yield break;
        }

        if (razredjivacNaMjestu != null)
        {
            razredjivacNaMjestu.SetActive(true);
        }

        inventory.UkloniPredmet("Razredjivac");

        PustiZvukPostavljeno();

        float vrijemePunogBara = 0.5f;

        while (vrijemePunogBara > 0f)
        {
            if (!postavljaSe)
            {
                yield break;
            }

            DrziIgracaZakljucanog();

            vrijemePunogBara -= Time.deltaTime;

            yield return null;
        }

        if (progressBar != null)
        {
            progressBar.SetActive(false);
        }

        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = 0f;
        }

        postavljeno = true;
        postavljaSe = false;
        postavljanjeCoroutine = null;

        OtkljucajKretanje();

        PromijeniKursor kursor =
            GetComponent<PromijeniKursor>();

        if (kursor != null)
        {
            kursor.IskljuciInterakciju();
        }

        Debug.Log(
            "Razredjivac je postavljeno u mikrovalnu."
        );
    }

    private IEnumerator CekajPotpuniPovratakUIdle()
    {
        if (animatorIgraca == null)
        {
            yield break;
        }

        yield return null;

        float timer = 0f;

        while (timer < maksimalnoCekanjeIdle)
        {
            if (!postavljaSe)
            {
                yield break;
            }

            DrziIgracaZakljucanog();

            AnimatorStateInfo trenutnoStanje =
                animatorIgraca.GetCurrentAnimatorStateInfo(0);

            bool animatorJeUPrijelazu =
                animatorIgraca.IsInTransition(0);

            bool animatorJeUIdle =
                trenutnoStanje.IsName(idleStateName);

            if (!animatorJeUPrijelazu &&
                animatorJeUIdle)
            {
                yield break;
            }

            timer += Time.deltaTime;

            yield return null;
        }

        Debug.LogWarning(
            "Animator se nije vratio u stanje: " +
            idleStateName
        );
    }

    private void ZakljucajKretanje()
    {
        if (kretanjeIgraca != null)
        {
            kretanjeIgraca.ZaustaviKretanje();
            kretanjeIgraca.enabled = false;
        }

        ZaustaviRigidbody();
    }

    private void DrziIgracaZakljucanog()
    {
        if (kretanjeIgraca != null)
        {
            if (kretanjeIgraca.enabled)
            {
                kretanjeIgraca.ZaustaviKretanje();
                kretanjeIgraca.enabled = false;
            }
        }

        ZaustaviRigidbody();
    }

    private void ZaustaviRigidbody()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OtkljucajKretanje()
    {
        ZaustaviRigidbody();

        if (kretanjeIgraca != null)
        {
            kretanjeIgraca.enabled = true;
            kretanjeIgraca.ZaustaviKretanje();
            kretanjeIgraca.IgnorirajSljedeciKlik();
        }
    }

    private void VratiBrzinuAnimatora()
    {
        if (animatorIgraca != null)
        {
            animatorIgraca.speed =
                originalnaBrzinaAnimatora;
        }
    }

    private void OkreniIgracaPremaMikrovalnoj()
    {
        if (kretanjeIgraca == null ||
            kretanjeIgraca.modelLika == null)
        {
            return;
        }

        if (transform.position.x > igrac.position.x)
        {
            kretanjeIgraca.modelLika.localRotation =
                Quaternion.Euler(
                    0f,
                    84.79f,
                    0f
                );
        }
        else
        {
            kretanjeIgraca.modelLika.localRotation =
                Quaternion.Euler(
                    0f,
                    264.79f,
                    0f
                );
        }
    }

    private void PrekiniPostavljanje()
    {
        if (!postavljaSe)
        {
            return;
        }

        postavljaSe = false;

        if (postavljanjeCoroutine != null)
        {
            StopCoroutine(postavljanjeCoroutine);
            postavljanjeCoroutine = null;
        }

        VratiBrzinuAnimatora();
        ZaustaviZvukTajmera();

        if (animacije != null)
        {
            animacije.PrekiniTrenutnuAnimaciju();
        }

        if (progressBar != null)
        {
            progressBar.SetActive(false);
        }

        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = 0f;
        }

        OtkljucajKretanje();

        Debug.Log(
            "Postavljanje razredjivaca je prekinuto desnim klikom."
        );
    }

    private void PokreniZvukTajmera()
    {
        if (audioSource == null ||
            zvukTajmera == null)
        {
            return;
        }

        audioSource.clip = zvukTajmera;
        audioSource.loop = true;
        audioSource.volume = jacinaZvuka;
        audioSource.Play();
    }

    private void ZaustaviZvukTajmera()
    {
        if (audioSource == null)
        {
            return;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.loop = false;
    }

    private void PustiZvukPostavljeno()
    {
        if (zvukPostavljeno == null ||
            Camera.main == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(
            zvukPostavljeno,
            Camera.main.transform.position,
            jacinaZvuka
        );
    }

    private bool AnimatorImaTrigger(
        Animator ciljaniAnimator,
        string nazivTriggera)
    {
        if (ciljaniAnimator == null ||
            string.IsNullOrEmpty(nazivTriggera))
        {
            return false;
        }

        foreach (
            AnimatorControllerParameter parametar
            in ciljaniAnimator.parameters)
        {
            if (parametar.name == nazivTriggera &&
                parametar.type ==
                AnimatorControllerParameterType.Trigger)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDisable()
    {
        VratiBrzinuAnimatora();

        if (progressBar != null)
        {
            progressBar.SetActive(false);
        }

        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = 0f;
        }

        if (postavljaSe)
        {
            postavljaSe = false;
            OtkljucajKretanje();
        }

        if (pogresnaAnimacijaUTijeku)
        {
            pogresnaAnimacijaUTijeku = false;

            if (pogresnaAnimacijaCoroutine != null)
            {
                StopCoroutine(
                    pogresnaAnimacijaCoroutine
                );

                pogresnaAnimacijaCoroutine = null;
            }

            OtkljucajKretanje();
        }
    }
}