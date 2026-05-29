using UnityEngine;

public class SkrivanjeIgraca : MonoBehaviour
{
    public Transform igrac;
    public Transform mjestoSkrivanja;
    public Transform mjestoIzlaska;
    public GameObject modelIgraca;

    public float udaljenostZaSkrivanje = 2f;

    private bool igracJeSakriven = false;
    private KretanjeMisem kretanjeIgraca;
    private Rigidbody rb;

    private void Start()
    {
        kretanjeIgraca = igrac.GetComponent<KretanjeMisem>();
        rb = igrac.GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        float udaljenost = Vector3.Distance(igrac.position, transform.position);

        if (!igracJeSakriven && udaljenost > udaljenostZaSkrivanje)
        {
            Debug.Log("Igrač je predaleko od ormara.");
            return;
        }

        if (!igracJeSakriven)
        {
            SakrijIgraca();
        }
        else
        {
            IzvadiIgracaIzOrmara();
        }
    }

    private void SakrijIgraca()
    {
        igracJeSakriven = true;

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

        igrac.position = mjestoSkrivanja.position;

        if (rb != null)
        {
            rb.position = mjestoSkrivanja.position;
        }

        if (modelIgraca != null)
        {
            modelIgraca.SetActive(false);
        }

        Debug.Log("Igrač se sakrio u ormar.");
    }

    private void IzvadiIgracaIzOrmara()
    {
        igracJeSakriven = false;

        igrac.position = mjestoIzlaska.position;

        if (rb != null)
        {
            rb.position = mjestoIzlaska.position;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (modelIgraca != null)
        {
            modelIgraca.SetActive(true);
        }

        if (kretanjeIgraca != null)
        {
            kretanjeIgraca.enabled = true;
            kretanjeIgraca.ZaustaviKretanje();
            kretanjeIgraca.IgnorirajSljedeciKlik();
        }

        Debug.Log("Igrač je izašao iz ormara.");
    }

    public bool JeLiIgracSakriven()
    {
        return igracJeSakriven;
    }
}