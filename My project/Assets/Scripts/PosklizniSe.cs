using UnityEngine;

public class PosklizniSe : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip posklizniZvuk;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            ScoreManager.instance.DodajBod();
            audioSource.PlayOneShot(posklizniZvuk);
            HodajNasumicno hodanje = other.GetComponent<HodajNasumicno>();
            if (hodanje != null)
                foreach (Animator anim in hodanje.npcAnimatori)
                    anim.SetBool("isSlipping", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            HodajNasumicno hodanje = other.GetComponent<HodajNasumicno>();
            if (hodanje != null)
                foreach (Animator anim in hodanje.npcAnimatori)
                    anim.SetBool("isSlipping", false);
        }
    }
}