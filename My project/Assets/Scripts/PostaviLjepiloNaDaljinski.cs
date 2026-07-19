using UnityEngine;
using TMPro;
using System.Collections;

public class PostaviLjepiloNaDaljinski : MonoBehaviour
{
    [Header("Igrač")]
    public Transform igrac;

    [Header("Podvala s daljinskim")]
    public GameObject daljinskiZalijepljen;
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

    [Header("Timer")]
    public TMP_Text timerTekst;

    [Header("Zvukovi")]
    public AudioSource audioSource;
    public AudioClip zvukTajmera;
    public AudioClip zvukPostavljeno;
    public float jacinaZvuka = 1f;

    private bool postavljeno = false;
    private bool postavljaSe = false;

    private KretanjeMisem kretanjeIgraca;
    private AnimacijeInterakcije animacije;
    private Rigidbody rb;

    private Coroutine postavljanjeCoroutine;

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

        if (timerTekst != null)
        {
            timerTekst.gameObject.SetActive(false);
        }

        if (daljinskiZalijepljen != null)
        {
            daljinskiZalijepljen.SetActive(false);
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
        if (!postavljaSe)
        {
            return;
        }

        ZaustaviRigidbody();
    }

    private void LateUpdate()
    {
        if (!postavljaSe)
        {
            return;
        }

        ZaustaviRigidbody();
    }

    private void OnMouseDown()
    {
        if (postavljaSe || postavljeno)
        {
            return;
        }

        if (igrac == null)
        {
            Debug.LogWarning(
                "Igrač nije postavljen u PostaviLjepiloNaDaljinski skripti."
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
                "Igrač je predaleko od daljinskog."
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

        if (!inventory.ImaPredmet("Ljepilo"))
        {
            Debug.Log(
                "Nemaš ljepilo u inventoryju."
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
            PostaviLjepiloNakonAnimacije(inventory)
        );
    }

    private IEnumerator PostaviLjepiloNakonAnimacije(
        InventoryIgraca inventory)
    {
        postavljaSe = true;

        ZakljucajKretanje();
        OkreniIgracaPremaDaljinskom();

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

        if (timerTekst != null)
        {
            timerTekst.gameObject.SetActive(true);
        }

        PokreniZvukTajmera();

        float preostaloVrijeme =
            Mathf.Max(
                0.1f,
                ukupnoTrajanjePostavljanja
            );

        while (preostaloVrijeme > 0f)
        {
            if (!postavljaSe)
            {
                yield break;
            }

            DrziIgracaZakljucanog();

            preostaloVrijeme -= Time.deltaTime;

            if (timerTekst != null)
            {
                timerTekst.text =
                    Mathf.CeilToInt(
                        Mathf.Max(
                            0f,
                            preostaloVrijeme
                        )
                    ).ToString();

                timerTekst.ForceMeshUpdate();
            }

            yield return null;
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

        if (daljinskiZalijepljen != null)
        {
            daljinskiZalijepljen.SetActive(true);
        }

        inventory.UkloniPredmet("Ljepilo");

        PustiZvukPostavljeno();

        if (timerTekst != null)
        {
            timerTekst.text = "OK";
            timerTekst.ForceMeshUpdate();

            float vrijemeZaOK = 0.5f;

            while (vrijemeZaOK > 0f)
            {
                if (!postavljaSe)
                {
                    yield break;
                }

                DrziIgracaZakljucanog();

                vrijemeZaOK -= Time.deltaTime;

                yield return null;
            }

            timerTekst.gameObject.SetActive(false);
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
            "Ljepilo je postavljeno na daljinski."
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

    private void OkreniIgracaPremaDaljinskom()
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

        if (animacije != null &&
            animacije.JeLiAnimacijaUTijeku())
        {
            animacije.PrekiniTrenutnuAnimaciju();
        }

        if (timerTekst != null)
        {
            timerTekst.text = "PREKINUTO";
            timerTekst.gameObject.SetActive(false);
        }

        OtkljucajKretanje();

        Debug.Log(
            "Postavljanje ljepila je prekinuto desnim klikom."
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

    private void OnDisable()
    {
        VratiBrzinuAnimatora();

        if (postavljaSe)
        {
            postavljaSe = false;
            OtkljucajKretanje();
        }
    }
}