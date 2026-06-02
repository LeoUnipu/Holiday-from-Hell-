using UnityEngine;

public class PokupiSapun : MonoBehaviour
{
    public Transform igrac;
    public float udaljenostZaSkupljanje = 2f;

    private void OnMouseDown()
    {
        if (igrac == null) return;

        float udaljenost = Vector3.Distance(igrac.position, transform.position);

        if (udaljenost > udaljenostZaSkupljanje)
        {
            Debug.Log("Igrač je predaleko od sapuna.");
            return;
        }

        InventoryIgraca inventory = igrac.GetComponent<InventoryIgraca>();

        if (inventory != null)
        {
            inventory.DodajPredmet("Sapun");
            gameObject.SetActive(false);

            Debug.Log("Igrač je pokupio sapun.");
        }
    }
}