using UnityEngine;

public class NPCUhvatiIgraca : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider drugi)
    {
        if (drugi.CompareTag("Igrac"))
        {
            gameManager.GameOver();
        }
    }
}