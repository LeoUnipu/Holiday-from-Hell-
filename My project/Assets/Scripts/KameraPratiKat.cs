using UnityEngine;

public class KameraPratiKat : MonoBehaviour
{
    public Transform kamera;
    public float brzinaPomicanja = 5f;

    private Vector3 ciljnaPozicija;
    private bool pomakniKameru = false;

    private void Start()
    {
        if (kamera == null)
        {
            kamera = Camera.main.transform;
        }

        ciljnaPozicija = kamera.position;
    }

    private void Update()
    {
        if (!pomakniKameru) return;

        kamera.position = Vector3.Lerp(
            kamera.position,
            ciljnaPozicija,
            brzinaPomicanja * Time.deltaTime
        );

        if (Vector3.Distance(kamera.position, ciljnaPozicija) < 0.05f)
        {
            kamera.position = ciljnaPozicija;
            pomakniKameru = false;
        }
    }

    public void PomakniNaKat(float yPozicijaKamere)
    {
        ciljnaPozicija = new Vector3(
            kamera.position.x,
            yPozicijaKamere,
            kamera.position.z
        );

        pomakniKameru = true;
    }
}