using UnityEngine;

public class KretanjeMisem : MonoBehaviour
{
    public float brzina = 4f;
    public Camera glavnaKamera;

    public Animator animator;

    // Ovdje povezujem Base_Mesh, odnosno vizualni model lika.
    public Transform modelLika;

    private Vector3 ciljnaPozicija;
    private bool imaCilj = false;

    // Ovo koristim da se ignorira samo klik kojim igrač izlazi iz ormara.
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
            // Ovdje kratko ignoriram klik nakon izlaska iz ormara.
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
                // Ovdje koristim GetComponentInParent zato što su nova vrata možda child objekt.
                VrataKlik vrata = pogodak.collider.GetComponentInParent<VrataKlik>();

                if (vrata != null)
                {
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

        if (Vector3.Distance(rb.position, ciljnaPozicija) < 0.05f)
        {
            imaCilj = false;
            PostaviAnimacijuStajanja();
        }
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