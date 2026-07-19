using UnityEngine;
using System.Collections;

public class PokupiSapun : MonoBehaviour
{
    [Header("Igrač")]
    public Transform igrac;

    [Header("Skupljanje")]
    public float udaljenostZaSkupljanje = 2f;

    [Header("Model predmeta")]
    public GameObject modelSapuna;

    [Header("Animacija")]
    [Tooltip("Animator koji izvodi animaciju skupljanja.")]
    public Animator animatorIgraca;

    [Tooltip("Točan naziv Idle stanja u Animator Controlleru.")]
    public string idleStateName = "Idle";

    [Tooltip("1 = normalna brzina, 1.5 = brže, 2 = dvostruko brže.")]
    public float brzinaAnimacije = 1.5f;

    [Tooltip("Najduže čekanje povratka u Idle.")]
    public float maksimalnoCekanjeIdle = 10f;

    [Header("Zvuk skupljanja")]
    public AudioClip zvukSkupljanja;
    public float jacinaZvuka = 1f;

    private bool sapunJePokupljen = false;
    private bool skupljanjeUTijeku = false;

    private KretanjeMisem kretanjeIgraca;
    private AnimacijeInterakcije animacije;
    private Rigidbody rb;

    private Coroutine skupljanjeCoroutine;

    private float originalnaBrzinaAnimatora = 1f;

    private void Start()
    {
        if (igrac == null)
        {
            Debug.LogWarning(
                "Igrač nije postavljen u PokupiSapun skripti."
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
        if (!skupljanjeUTijeku)
        {
            return;
        }

        DrziIgracaZakljucanog();

        if (Input.GetMouseButtonDown(1))
        {
            PrekiniSkupljanje();
        }
    }

    private void FixedUpdate()
    {
        if (!skupljanjeUTijeku)
        {
            return;
        }

        ZaustaviRigidbody();
    }

    private void LateUpdate()
    {
        if (!skupljanjeUTijeku)
        {
            return;
        }

        ZaustaviRigidbody();
    }

    private void OnMouseDown()
    {
        if (skupljanjeUTijeku)
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
                "Igrač je predaleko od sapuna."
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

        if (sapunJePokupljen)
        {
            skupljanjeCoroutine = StartCoroutine(
                PokreniPraznuPretragu()
            );

            return;
        }

        skupljanjeCoroutine = StartCoroutine(
            PokupiSapunNakonAnimacije()
        );
    }

    private IEnumerator PokupiSapunNakonAnimacije()
    {
        skupljanjeUTijeku = true;

        InventoryIgraca inventory =
            igrac.GetComponent<InventoryIgraca>();

        if (inventory == null)
        {
            Debug.LogWarning(
                "InventoryIgraca nije pronađen na igraču."
            );

            ZavrsiBezSkupljanja();
            yield break;
        }

        if (animacije == null)
        {
            Debug.LogWarning(
                "AnimacijeInterakcije nije pronađena."
            );

            ZavrsiBezSkupljanja();
            yield break;
        }

        ZakljucajKretanje();
        OkreniIgracaPremaSapunu();

        yield return null;

        DrziIgracaZakljucanog();

        PostaviBrzinuAnimatora();

        animacije.PokreniSkupljanje();

        yield return null;

        while (animacije.JeLiAnimacijaUTijeku())
        {
            if (!skupljanjeUTijeku)
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

        if (!skupljanjeUTijeku)
        {
            yield break;
        }

        if (animacije.JeLiZadnjaAnimacijaPrekinuta())
        {
            Debug.Log(
                "Skupljanje sapuna je prekinuto. Sapun nije pokupljen."
            );

            ZavrsiBezSkupljanja();
            yield break;
        }

        inventory.DodajPredmet("Sapun");

        PustiZvukSkupljanja();

        if (modelSapuna != null)
        {
            modelSapuna.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "Model Sapuna nije spojen u Inspectoru."
            );
        }

        sapunJePokupljen = true;
        skupljanjeUTijeku = false;
        skupljanjeCoroutine = null;

        OtkljucajKretanje();

        Debug.Log(
            "Igrač je pokupio sapun."
        );
    }

    private IEnumerator PokreniPraznuPretragu()
    {
        skupljanjeUTijeku = true;

        if (animacije == null)
        {
            ZavrsiBezSkupljanja();
            yield break;
        }

        ZakljucajKretanje();
        OkreniIgracaPremaSapunu();

        yield return null;

        DrziIgracaZakljucanog();

        PostaviBrzinuAnimatora();

        animacije.PokreniPraznuPretragu();

        yield return null;

        while (animacije.JeLiAnimacijaUTijeku())
        {
            if (!skupljanjeUTijeku)
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

        if (!skupljanjeUTijeku)
        {
            yield break;
        }

        skupljanjeUTijeku = false;
        skupljanjeCoroutine = null;

        OtkljucajKretanje();

        Debug.Log(
            "Ovdje više nema sapuna."
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
            if (!skupljanjeUTijeku)
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

    private void PrekiniSkupljanje()
    {
        if (!skupljanjeUTijeku)
        {
            return;
        }

        skupljanjeUTijeku = false;

        if (skupljanjeCoroutine != null)
        {
            StopCoroutine(skupljanjeCoroutine);
            skupljanjeCoroutine = null;
        }

        VratiBrzinuAnimatora();

        if (animacije != null &&
            animacije.JeLiAnimacijaUTijeku())
        {
            animacije.PrekiniTrenutnuAnimaciju();
        }

        OtkljucajKretanje();

        Debug.Log(
            "Skupljanje sapuna je prekinuto desnim klikom."
        );
    }

    private void ZavrsiBezSkupljanja()
    {
        VratiBrzinuAnimatora();

        skupljanjeUTijeku = false;
        skupljanjeCoroutine = null;

        OtkljucajKretanje();
    }

    private void OkreniIgracaPremaSapunu()
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

        if (skupljanjeUTijeku)
        {
            skupljanjeUTijeku = false;
            OtkljucajKretanje();
        }
    }
}