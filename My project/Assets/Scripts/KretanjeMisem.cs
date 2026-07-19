using UnityEngine;

public class KretanjeMisem : MonoBehaviour
{
    public float brzina = 4f;
    public Camera glavnaKamera;

    public Animator animator;
    public Transform modelLika;

    [Header("Provjera prepreka")]
    public float udaljenostProvjerePrepreke = 0.55f;
    public float visinaProvjerePrepreke = 0.6f;
    public float radijusProvjerePrepreke = 0.25f;

    [Header("Zvuk hodanja")]
    public AudioSource audioHodanja;
    public AudioClip zvukHodanja;

    private Vector3 ciljnaPozicija;
    private bool imaCilj = false;

    private float ignorirajKlikDo = 0f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (glavnaKamera == null)
        {
            glavnaKamera = Camera.main;
        }

        if (audioHodanja == null)
        {
            audioHodanja = GetComponent<AudioSource>();
        }

        ciljnaPozicija = transform.position;

        PostaviAnimacijuStajanja();
        ZaustaviZvukHodanja();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if (Time.time < ignorirajKlikDo)
        {
            return;
        }

        if (glavnaKamera == null)
        {
            return;
        }

        Ray zraka =
            glavnaKamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(
            zraka,
            out RaycastHit prviPogodak,
            100f,
            ~0,
            QueryTriggerInteraction.Collide))
        {
            VrataKlik vrata =
                prviPogodak.collider.GetComponentInParent<VrataKlik>();

            if (vrata != null)
            {
                ZaustaviKretanje();
                vrata.Teleportiraj();
                return;
            }
        }

        Plane ravninaKretanja =
            new Plane(Vector3.forward, transform.position);

        if (ravninaKretanja.Raycast(
            zraka,
            out float udaljenost))
        {
            Vector3 klikPozicija =
                zraka.GetPoint(udaljenost);

            ciljnaPozicija = new Vector3(
                klikPozicija.x,
                transform.position.y,
                transform.position.z
            );

            imaCilj = true;
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        if (!imaCilj)
        {
            PostaviAnimacijuStajanja();
            ZaustaviZvukHodanja();
            return;
        }

        float udaljenostDoCilja =
            Mathf.Abs(rb.position.x - ciljnaPozicija.x);

        if (udaljenostDoCilja < 0.25f)
        {
            ZaustaviKretanje();
            return;
        }

        float smjerX =
            Mathf.Sign(ciljnaPozicija.x - rb.position.x);

        if (PostojiPreprekaIspred(smjerX))
        {
            ZaustaviKretanje();
            return;
        }

        PostaviAnimacijuHodanja();
        PokreniZvukHodanja();

        if (modelLika != null)
        {
            if (smjerX > 0f)
            {
                modelLika.localRotation =
                    Quaternion.Euler(0f, 84.79f, 0f);
            }
            else if (smjerX < 0f)
            {
                modelLika.localRotation =
                    Quaternion.Euler(0f, 264.79f, 0f);
            }
        }

        Vector3 novaPozicija = Vector3.MoveTowards(
            rb.position,
            ciljnaPozicija,
            brzina * Time.fixedDeltaTime
        );

        rb.MovePosition(novaPozicija);
    }

    private bool PostojiPreprekaIspred(float smjerX)
    {
        Vector3 pocetak =
            rb.position + Vector3.up * visinaProvjerePrepreke;

        Vector3 smjer =
            new Vector3(smjerX, 0f, 0f);

        RaycastHit[] pogoci = Physics.SphereCastAll(
            pocetak,
            radijusProvjerePrepreke,
            smjer,
            udaljenostProvjerePrepreke,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        foreach (RaycastHit pogodak in pogoci)
        {
            if (pogodak.collider == null)
            {
                continue;
            }

            Transform pogodeniObjekt =
                pogodak.collider.transform;

            if (pogodeniObjekt == transform)
            {
                continue;
            }

            if (pogodeniObjekt.IsChildOf(transform))
            {
                continue;
            }

            if (transform.IsChildOf(pogodeniObjekt))
            {
                continue;
            }

            Debug.Log(
                "Prepreka ispred igrača: " +
                pogodak.collider.name
            );

            return true;
        }

        return false;
    }

    public void ZaustaviKretanje()
    {
        imaCilj = false;

        if (rb != null)
        {
            ciljnaPozicija = rb.position;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            ciljnaPozicija = transform.position;
        }

        PostaviAnimacijuStajanja();
        ZaustaviZvukHodanja();
    }

    public void IgnorirajSljedeciKlik()
    {
        ignorirajKlikDo = Time.time + 0.15f;
    }

    private void PokreniZvukHodanja()
    {
        if (audioHodanja == null ||
            zvukHodanja == null)
        {
            return;
        }

        if (!audioHodanja.isPlaying)
        {
            audioHodanja.clip = zvukHodanja;
            audioHodanja.loop = true;
            audioHodanja.Play();
        }
    }

    private void ZaustaviZvukHodanja()
    {
        if (audioHodanja == null)
        {
            return;
        }

        if (audioHodanja.isPlaying)
        {
            audioHodanja.Stop();
        }
    }

    private void PostaviAnimacijuHodanja()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat("State", 0f);
        animator.SetFloat("Hor", 0f);
        animator.SetFloat("Vert", 1f);
    }

    private void PostaviAnimacijuStajanja()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat("State", 0f);
        animator.SetFloat("Hor", 0f);
        animator.SetFloat("Vert", 0f);
    }
}