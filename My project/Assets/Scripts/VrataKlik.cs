using UnityEngine;

public class VrataKlik : MonoBehaviour
{
    public Transform igrac;
    public Transform ciljnaPozicija;

    [Header("Postavke vrata")]
    public float udaljenostZaUlaz = 1.5f;

    public void Teleportiraj()
    {
        if (igrac == null || ciljnaPozicija == null) return;

        float udaljenost = Vector3.Distance(igrac.position, transform.position);

        if (udaljenost > udaljenostZaUlaz)
        {
            Debug.Log("Igrač je predaleko od vrata.");
            return;
        }

        KretanjeMisem kretanje = igrac.GetComponent<KretanjeMisem>();

        if (kretanje != null)
        {
            kretanje.ZaustaviKretanje();
        }

        Rigidbody rb = igrac.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Vector3 novaPozicija = ciljnaPozicija.position;
        novaPozicija.y = 1.1f;
        novaPozicija.z = 0f;

        igrac.position = novaPozicija;

        if (rb != null)
        {
            rb.position = novaPozicija;
        }
    }
}