using UnityEngine;
using System.Collections;

public class PokupiJaje : MonoBehaviour
{
    [Header("Igrač")]
    public Transform igrac;

    [Header("Skupljanje")]
    public float udaljenostZaSkupljanje = 2f;

    [Header("Model predmeta")]
    public GameObject modelJajeta;

    [Header("Animacija")]
    [Tooltip("Animator koji izvodi animaciju skupljanja.")]
    public Animator animatorIgraca;

    [Tooltip("Točan naziv Idle stanja u Animator Controlleru.")]
    public string idleStateName = "Idle";

    [Tooltip("1 = normalno, 1.5 = brže, 2 = dvostruko brže.")]
    public float brzinaAnimacije = 1.5f;

    [Tooltip("Najduže čekanje povratka u Idle.")]
    public float maksimalnoCekanjeIdle = 10f;

    [Header("Zvuk skupljanja")]
    public AudioClip zvukSkupljanja;
    public float jacinaZvuka = 1f;

    private bool jajeJePokupljeno = false;
    private bool radnjaUTijeku = false;
    private bool praznaPretragaUTijeku = false;

    private KretanjeMisem kretanjeIgraca;
    private AnimacijeInterakcije animacije;
    private Rigidbody rb;

    private Coroutine radnjaCoroutine;

    private float originalnaBrzinaAnimatora = 1f;

    private void Start()
    {
        if (igrac == null)
        {
            Debug.LogWarning(
                "Igrač nije postavljen u PokupiJaje skripti."
            );

            return;
        }

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

    private void Update()
    {
        if (!radnjaUTijeku)
        {
            return;
        }

        DrziIgracaZakljucanog();

        if (Input.GetMouseButtonDown(1))
        {
            PrekiniTrenutnuRadnju();
        }
    }

    private void FixedUpdate()
    {
        if (!radnjaUTijeku)
        {
            return;
        }

        ZaustaviRigidbody();
    }

    private void LateUpdate()
    {
        if (!radnjaUTijeku)
        {
            return;
        }

        ZaustaviRigidbody();
    }

    private void OnMouseDown()
    {
        if (radnjaUTijeku)
        {
            return;
        }

        if (igrac == null)
        {
            Debug.LogWarning(
                "Igrač nije postavljen."
            );

            return;
        }

        float udaljenost = Vector3.Distance(
            igrac.position,
            transform.position
        );

        if (udaljenost > udaljenostZaSkupljanje)
        {
            Debug.Log(
                "Igrač je predaleko od jajeta."
            );

            return;
        }

        if (animacije == null)
        {
            animacije =
                igrac.GetComponentInChildren<AnimacijeInterakcije>(true);
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

        if (jajeJePokupljeno)
        {
            radnjaCoroutine = StartCoroutine(
                PokreniPraznuPretragu()
            );

            return;
        }

        radnjaCoroutine = StartCoroutine(
            PokupiJajeNakonAnimacije()
        );
    }

    private IEnumerator PokupiJajeNakonAnimacije()
    {
        radnjaUTijeku = true;
        praznaPretragaUTijeku = false;

        InventoryIgraca inventory =
            igrac.GetComponent<InventoryIgraca>();

        if (inventory == null)
        {
            Debug.LogWarning(
                "InventoryIgraca nije pronađen na igraču."
            );

            ZavrsiRadnju();
            yield break;
        }

        if (animacije == null)
        {
            Debug.LogWarning(
                "AnimacijeInterakcije nije pronađena."
            );

            ZavrsiRadnju();
            yield break;
        }

        ZakljucajKretanje();
        OkreniIgracaPremaJajetu();

        yield return null;

        DrziIgracaZakljucanog();

        PostaviBrzinuAnimatora();

        animacije.PokreniSkupljanje();

        yield return null;

        while (animacije.JeLiAnimacijaUTijeku())
        {
            if (!radnjaUTijeku)
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

        if (!radnjaUTijeku)
        {
            yield break;
        }

        if (animacije.JeLiZadnjaAnimacijaPrekinuta())
        {
            Debug.Log(
                "Skupljanje jajeta je prekinuto. Jaje nije pokupljeno."
            );

            ZavrsiRadnju();
            yield break;
        }

        inventory.DodajPredmet("Jaje");

        PustiZvukSkupljanja();

        if (modelJajeta != null)
        {
            modelJajeta.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "Model Jajeta nije spojen u Inspectoru."
            );
        }

        jajeJePokupljeno = true;

        ZavrsiRadnju();

        Debug.Log(
            "Igrač je pokupio jaje."
        );
    }

    private IEnumerator PokreniPraznuPretragu()
    {
        radnjaUTijeku = true;
        praznaPretragaUTijeku = true;

        if (animacije == null)
        {
            ZavrsiRadnju();
            yield break;
        }

        ZakljucajKretanje();
        OkreniIgracaPremaJajetu();

        yield return null;

        DrziIgracaZakljucanog();

        PostaviBrzinuAnimatora();

        animacije.PokreniPraznuPretragu();

        yield return null;

        while (animacije.JeLiAnimacijaUTijeku())
        {
            if (!radnjaUTijeku)
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

        if (!radnjaUTijeku)
        {
            yield break;
        }

        ZavrsiRadnju();

        Debug.Log(
            "Ovdje više nema jajeta."
        );
    }

    private IEnumerator CekajPotpuniPovratakUIdle()
    {
        if (animatorIgraca == null)
        {
            yield break;
        }

        yield return null;

        float protekloVrijeme = 0f;

        while (protekloVrijeme < maksimalnoCekanjeIdle)
        {
            if (!radnjaUTijeku)
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

            protekloVrijeme += Time.deltaTime;

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
        if (kretanjeIgraca != null &&
            kretanjeIgraca.enabled)
        {
            kretanjeIgraca.ZaustaviKretanje();
            kretanjeIgraca.enabled = false;
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

    private void PostaviBrzinuAnimatora()
    {
        if (animatorIgraca == null)
        {
            return;
        }

        originalnaBrzinaAnimatora =
            animatorIgraca.speed;

        animatorIgraca.speed =
            Mathf.Max(0.1f, brzinaAnimacije);
    }

    private void VratiBrzinuAnimatora()
    {
        if (animatorIgraca == null)
        {
            return;
        }

        animatorIgraca.speed =
            originalnaBrzinaAnimatora;
    }

    private void PrekiniTrenutnuRadnju()
    {
        if (!radnjaUTijeku)
        {
            return;
        }

        bool bilaJePraznaPretraga =
            praznaPretragaUTijeku;

        radnjaUTijeku = false;
        praznaPretragaUTijeku = false;

        if (radnjaCoroutine != null)
        {
            StopCoroutine(radnjaCoroutine);
            radnjaCoroutine = null;
        }

        VratiBrzinuAnimatora();

        if (animacije != null &&
            animacije.JeLiAnimacijaUTijeku())
        {
            animacije.PrekiniTrenutnuAnimaciju();
        }

        OtkljucajKretanje();

        if (bilaJePraznaPretraga)
        {
            Debug.Log(
                "Pretraga jajeta je prekinuta desnim klikom."
            );
        }
        else
        {
            Debug.Log(
                "Skupljanje jajeta je prekinuto desnim klikom."
            );
        }
    }

    private void ZavrsiRadnju()
    {
        VratiBrzinuAnimatora();

        radnjaUTijeku = false;
        praznaPretragaUTijeku = false;
        radnjaCoroutine = null;

        OtkljucajKretanje();
    }

    private void OkreniIgracaPremaJajetu()
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

    private void PustiZvukSkupljanja()
    {
        if (zvukSkupljanja == null ||
            Camera.main == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(
            zvukSkupljanja,
            Camera.main.transform.position,
            jacinaZvuka
        );
    }

    private void OnDisable()
    {
        VratiBrzinuAnimatora();

        if (radnjaUTijeku)
        {
            radnjaUTijeku = false;
            praznaPretragaUTijeku = false;
            OtkljucajKretanje();
        }
    }
}