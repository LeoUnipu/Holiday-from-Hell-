using UnityEngine;

public class PokupiJaje : MonoBehaviour
{
    public Transform igrac;
    public float udaljenostZaSkupljanje = 2f;

    [Header("Zvuk skupljanja")]
    public AudioClip zvukSkupljanja;
    public float jacinaZvuka = 1f;

    private void OnMouseDown()
    {
        if (igrac == null) return;

        float udaljenost = Vector3.Distance(igrac.position, transform.position);

        if (udaljenost > udaljenostZaSkupljanje)
        {
            Debug.Log("Igrač je predaleko od jajeta.");
            return;
        }

        InventoryIgraca inventory = igrac.GetComponent<InventoryIgraca>();

        if (inventory != null)
        {
            inventory.DodajPredmet("Jaje");

            if (zvukSkupljanja != null)
            {
                AudioSource.PlayClipAtPoint(
                    zvukSkupljanja,
                    Camera.main.transform.position,
                    jacinaZvuka
                );
            }

            gameObject.SetActive(false);

            Debug.Log("Igrač je pokupio jaje.");
        }
    }
}