using UnityEngine;

public class VrataKlik : MonoBehaviour
{
    public Transform igrac;
    public Transform ciljnaPozicija;

    [Header("Postavke vrata")]
    public float udaljenostZaUlaz = 1.5f;

    [Header("Kamera")]
    public KameraPratiKat kameraPratiKat;
    public bool mijenjaKat = false;
    public float yPozicijaKamere = 2.5f;

    [Header("Izlaz iz kuće")]
    [Tooltip(
        "Uključi samo na vratima koja vode izvan kuće. " +
        "Nakon teleportiranja pokrenut će se Game Over."
    )]
    public bool izlazIzKuce = false;

    public GameManager gameManager;

    public void Teleportiraj()
    {
        if (igrac == null || ciljnaPozicija == null)
        {
            return;
        }

        float udaljenost =
            Mathf.Abs(
                igrac.position.x -
                transform.position.x
            );

        if (udaljenost > udaljenostZaUlaz)
        {
            Debug.Log(
                "Igrač je predaleko od vrata."
            );

            return;
        }

        KretanjeMisem kretanje =
            igrac.GetComponent<KretanjeMisem>();

        if (kretanje != null)
        {
            kretanje.ZaustaviKretanje();
        }

        Rigidbody rb =
            igrac.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Vector3 novaPozicija =
            ciljnaPozicija.position;

        igrac.position = novaPozicija;

        if (rb != null)
        {
            rb.position = novaPozicija;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (mijenjaKat &&
            kameraPratiKat != null)
        {
            kameraPratiKat.PomakniNaKat(
                yPozicijaKamere
            );
        }

        if (izlazIzKuce)
        {
            if (gameManager == null)
            {
                gameManager =
                    FindAnyObjectByType<GameManager>();
            }

            if (gameManager != null)
            {
                Debug.Log(
                    "Igrač je izašao iz kuće. Game Over."
                );

                gameManager.GameOver();
            }
            else
            {
                Debug.LogError(
                    "GameManager nije pronađen."
                );
            }
        }
    }
}