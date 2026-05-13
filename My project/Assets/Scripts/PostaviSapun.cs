using UnityEngine;

public class PostaviSapun : MonoBehaviour
{
    public Transform igrac;
    public GameObject sapunZamka;
    public float udaljenostZaPostavljanje = 1.5f;

    private bool postavljeno = false;

    private void OnMouseDown()
    {
        if (postavljeno) return;

        float udaljenost = Vector3.Distance(igrac.position, transform.position);

        if (udaljenost <= udaljenostZaPostavljanje)
        {
            sapunZamka.SetActive(true);
            postavljeno = true;

            Debug.Log("Sapun je postavljen!");
        }
        else
        {
            Debug.Log("Igrač je predaleko od mjesta za sapun.");
        }
    }
}