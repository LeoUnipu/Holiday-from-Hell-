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

    private Vector3 ciljnaPozicija;
    private bool imaCilj = false;

    private float ignorirajKlikDo = 0f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (glavnaKamera == null)
        {
            glavnaKamera = Camera.main;
        }

        ciljnaPozicija = transform.position;
        PostaviAnimacijuStajanja();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time < ignorirajKlikDo)
            {
                return;
            }

            Ray zraka = glavnaKamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] pogoci = Physics.RaycastAll(
                zraka,
                100f,
                ~0,
                QueryTriggerInteraction.Collide
            );

            foreach (RaycastHit pogodak in pogoci)
            {
                VrataKlik vrata = pogodak.collider.GetComponentInParent<VrataKlik>();

                if (vrata != null)
                {
                    ZaustaviKretanje();
                    vrata.Teleportiraj();
                    return;
                }
            }

            Plane ravninaKretanja = new Plane(Vector3.forward, transform.position);

            if (ravninaKretanja.Raycast(zraka, out float udaljenost))
            {
                Vector3 klikPozicija = zraka.GetPoint(udaljenost);

                ciljnaPozicija = new Vector3(
                    klikPozicija.x,
                    transform.position.y,
                    transform.position.z
                );

                imaCilj = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (!imaCilj)
        {
            PostaviAnimacijuStajanja();
            return;
        }

        float udaljenostDoCilja = Mathf.Abs(rb.position.x - ciljnaPozicija.x);

        if (udaljenostDoCilja < 0.25f)
        {
            ZaustaviKretanje();
            return;
        }

        float smjerX = Mathf.Sign(ciljnaPozicija.x - rb.position.x);

        if (PostojiPreprekaIspred(smjerX))
        {
            ZaustaviKretanje();
            return;
        }

        PostaviAnimacijuHodanja();

        if (modelLika != null)
        {
            if (ciljnaPozicija.x > transform.position.x)
            {
                modelLika.localRotation = Quaternion.Euler(0f, 84.79f, 0f);
            }
            else if (ciljnaPozicija.x < transform.position.x)
            {
                modelLika.localRotation = Quaternion.Euler(0f, 264.79f, 0f);
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
        Vector3 pocetakZrake = rb.position + Vector3.up * visinaProvjerePrepreke;
        Vector3 smjer = new Vector3(smjerX, 0f, 0f);

        if (Physics.Raycast(
            pocetakZrake,
            smjer,
            out RaycastHit pogodak,
            udaljenostProvjerePrepreke,
            ~0,
            QueryTriggerInteraction.Ignore))
        {
            if (pogodak.collider.transform.IsChildOf(transform))
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public void ZaustaviKretanje()
    {
        imaCilj = false;
        ciljnaPozicija = transform.position;
        PostaviAnimacijuStajanja();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void IgnorirajSljedeciKlik()
    {
        ignorirajKlikDo = Time.time + 0.15f;
    }

    private void PostaviAnimacijuHodanja()
    {
        if (animator == null) return;

        animator.SetFloat("State", 0f);
        animator.SetFloat("Hor", 0f);
        animator.SetFloat("Vert", 1f);
    }

    private void PostaviAnimacijuStajanja()
    {
        if (animator == null) return;

        animator.SetFloat("State", 0f);
        animator.SetFloat("Hor", 0f);
        animator.SetFloat("Vert", 0f);
    }
}