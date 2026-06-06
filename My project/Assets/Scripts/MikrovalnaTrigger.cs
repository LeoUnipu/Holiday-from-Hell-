using UnityEngine;

public class MikrovalnaTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip mikrovalnaZvuk;
    public PostaviJaje postaviJaje;

    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("NPC") && postaviJaje.postavljeno)
    {
        ScoreManager.instance.DodajBod();
        audioSource.PlayOneShot(mikrovalnaZvuk);
        Debug.Log("NPC prošao pored mikrovalne s jajetom! +1 bod");
    }
}
}