using UnityEngine;

public class PosklizniSe : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            ScoreManager.instance.DodajBod();
            Debug.Log("NPC se poskliznu! +1 bod");
        }
    }
}