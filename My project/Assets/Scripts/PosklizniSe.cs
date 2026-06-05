using UnityEngine;

public class PosklizniSe : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            ScoreManager.instance.DodajBod();
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