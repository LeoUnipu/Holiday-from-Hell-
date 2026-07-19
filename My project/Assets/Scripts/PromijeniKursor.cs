using UnityEngine;

public class PromijeniKursor : MonoBehaviour
{
    [Header("Kursor")]
    public Texture2D kursorInterakcije;

    [Header("Provjera udaljenosti")]
    public Transform igrac;
    public float udaljenostZaKursor = 2f;

    private bool interakcijaAktivna = true;
    private bool misJeIznadObjekta = false;
    private bool posebanKursorJeAktivan = false;

    private void Update()
    {
        if (!interakcijaAktivna || igrac == null || !misJeIznadObjekta)
        {
            PostaviObicniKursor();
            return;
        }

        float udaljenost = Vector3.Distance(
            igrac.position,
            transform.position
        );

        if (udaljenost <= udaljenostZaKursor)
        {
            PostaviPosebanKursor();
        }
        else
        {
            PostaviObicniKursor();
        }
    }

    private void OnMouseEnter()
    {
        misJeIznadObjekta = true;
    }

    private void OnMouseExit()
    {
        misJeIznadObjekta = false;
        PostaviObicniKursor();
    }

    private void OnDisable()
    {
        misJeIznadObjekta = false;
        PostaviObicniKursor();
    }

    public void IskljuciInterakciju()
    {
        interakcijaAktivna = false;
        misJeIznadObjekta = false;
        PostaviObicniKursor();
    }

    public void UkljuciInterakciju()
    {
        interakcijaAktivna = true;
    }

    private void PostaviPosebanKursor()
    {
        if (posebanKursorJeAktivan) return;
        if (kursorInterakcije == null) return;

        Cursor.SetCursor(
            kursorInterakcije,
            Vector2.zero,
            CursorMode.Auto
        );

        posebanKursorJeAktivan = true;
    }

    private void PostaviObicniKursor()
    {
        if (!posebanKursorJeAktivan) return;

        Cursor.SetCursor(
            null,
            Vector2.zero,
            CursorMode.Auto
        );

        posebanKursorJeAktivan = false;
    }
}