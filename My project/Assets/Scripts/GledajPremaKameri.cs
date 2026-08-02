using UnityEngine;

public class GledajPremaKameri : MonoBehaviour
{
    private Camera glavnaKamera;

    private void Start()
    {
        glavnaKamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (glavnaKamera == null)
        {
            return;
        }

        transform.rotation =
            glavnaKamera.transform.rotation;
    }
}