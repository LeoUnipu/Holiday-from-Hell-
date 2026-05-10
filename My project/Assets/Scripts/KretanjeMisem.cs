using UnityEngine;

public class KretanjeMisem : MonoBehaviour
{
    public float brzina = 4f;
    public Camera glavnaKamera;

    private Vector3 ciljnaPozicija;
    private bool imaCilj = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (glavnaKamera == null)
        {
            glavnaKamera = Camera.main;
        }

        ciljnaPozicija = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray zraka = glavnaKamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] pogoci = Physics.RaycastAll(
                zraka,
                100f,
                ~0,
                QueryTriggerInteraction.Collide
            );

            foreach (RaycastHit pogodak in pogoci)
            {
                Debug.Log("Pogođen: " + pogodak.collider.gameObject.name);

                VrataKlik vrata = pogodak.collider.GetComponent<VrataKlik>();

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
                    1.1f,
                    0f
                );

                imaCilj = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (!imaCilj) return;

        Vector3 novaPozicija = Vector3.MoveTowards(
            rb.position,
            ciljnaPozicija,
            brzina * Time.fixedDeltaTime
        );

        rb.MovePosition(novaPozicija);

        if (Vector3.Distance(rb.position, ciljnaPozicija) < 0.05f)
        {
            imaCilj = false;
        }
    }

    public void ZaustaviKretanje()
    {
        imaCilj = false;
        ciljnaPozicija = transform.position;
    }
}