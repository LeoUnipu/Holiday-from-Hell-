using UnityEngine;

public class RotirajKadSeAktivira : MonoBehaviour
{
    public float kutRotacije = 90f;
    public float brzinaRotacije = 180f;

    private Quaternion pocetnaRotacija;
    private Quaternion ciljnaRotacija;
    private bool rotiraSe = false;

    private void OnEnable()
    {
        pocetnaRotacija = transform.rotation;
        ciljnaRotacija = pocetnaRotacija * Quaternion.Euler(0f, 0f, kutRotacije);
        rotiraSe = true;
    }

    private void Update()
    {
        if (!rotiraSe) return;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            ciljnaRotacija,
            brzinaRotacije * Time.deltaTime
        );

        if (Quaternion.Angle(transform.rotation, ciljnaRotacija) < 0.1f)
        {
            transform.rotation = ciljnaRotacija;
            rotiraSe = false;
        }
    }
}