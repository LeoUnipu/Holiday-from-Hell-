using UnityEngine;

public class NPCUhvatiIgraca : MonoBehaviour
{
    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Igrač")]
    public Transform igrac;

    [Header("Hvatanje")]
    [Tooltip("Najveća udaljenost na kojoj NPC može uhvatiti igrača.")]
    public float udaljenostHvatanja = 3f;

    [Tooltip("Visina s koje NPC provjerava postoji li zid.")]
    public float visinaNPCProvjere = 1f;

    [Tooltip("Visina cilja na igraču.")]
    public float visinaIgracaProvjere = 1f;

    [Tooltip("Koliko često se provjerava udaljenost i zid.")]
    public float vrijemeIzmeduProvjera = 0.1f;

    [Header("Debug")]
    public bool prikaziDebugZraku = true;

    private float sljedecaProvjera = 0f;
    private bool igracUhvacen = false;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<GameManager>();
        }

        if (igrac == null)
        {
            GameObject pronadeniIgrac =
                GameObject.FindGameObjectWithTag("Igrac");

            if (pronadeniIgrac != null)
            {
                igrac = pronadeniIgrac.transform;
            }
        }
    }

    private void Update()
    {
        if (igracUhvacen || igrac == null)
        {
            return;
        }

        if (Time.time < sljedecaProvjera)
        {
            return;
        }

        sljedecaProvjera =
            Time.time + vrijemeIzmeduProvjera;

        ProvjeriIgraca();
    }

    private void ProvjeriIgraca()
    {
        Vector3 pocetak =
            transform.position +
            Vector3.up * visinaNPCProvjere;

        Vector3 cilj =
            igrac.position +
            Vector3.up * visinaIgracaProvjere;

        Vector3 smjer =
            cilj - pocetak;

        float udaljenost =
            smjer.magnitude;

        if (udaljenost > udaljenostHvatanja)
        {
            return;
        }

        if (udaljenost <= 0.01f)
        {
            UhvatiIgraca();
            return;
        }

        smjer.Normalize();

        RaycastHit[] pogoci =
            Physics.RaycastAll(
                pocetak,
                smjer,
                udaljenost,
                ~0,
                QueryTriggerInteraction.Ignore
            );

        System.Array.Sort(
            pogoci,
            (a, b) =>
                a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit pogodak in pogoci)
        {
            if (pogodak.collider == null)
            {
                continue;
            }

            Transform pogodeniObjekt =
                pogodak.collider.transform;

            if (pogodeniObjekt == transform ||
                pogodeniObjekt.IsChildOf(transform))
            {
                continue;
            }

            if (JeLiIgrac(pogodeniObjekt))
            {
                if (prikaziDebugZraku)
                {
                    Debug.DrawLine(
                        pocetak,
                        pogodak.point,
                        Color.green,
                        vrijemeIzmeduProvjera
                    );
                }

                UhvatiIgraca();
                return;
            }

            if (prikaziDebugZraku)
            {
                Debug.DrawLine(
                    pocetak,
                    pogodak.point,
                    Color.red,
                    vrijemeIzmeduProvjera
                );
            }

            return;
        }
    }

    private bool JeLiIgrac(Transform objekt)
    {
        if (objekt == null || igrac == null)
        {
            return false;
        }

        if (objekt == igrac)
        {
            return true;
        }

        if (objekt.IsChildOf(igrac))
        {
            return true;
        }

        if (igrac.IsChildOf(objekt))
        {
            return true;
        }

        return objekt.CompareTag("Igrac");
    }

    private void UhvatiIgraca()
    {
        if (igracUhvacen)
        {
            return;
        }

        igracUhvacen = true;

        Debug.Log(
            "NPC je sreo igrača u istoj prostoriji. Game Over."
        );

        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        else
        {
            Debug.LogError(
                "GameManager nije postavljen."
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pocetak =
            transform.position +
            Vector3.up * visinaNPCProvjere;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            pocetak,
            udaljenostHvatanja
        );
    }
}