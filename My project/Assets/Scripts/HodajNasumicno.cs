using UnityEngine;
using UnityEngine.AI;

public class HodajNasumicno : MonoBehaviour
{
    public float radiusKretanja = 3f;
    public float vrijemePromjeneSobe = 10f;
    public Vector3[] pozicijePoSobama;

    private NavMeshAgent agent;
    private float timer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        IdiNaNovuPoziciju();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            IdiNaNovuPoziciju();

        timer += Time.deltaTime;
        if (timer >= vrijemePromjeneSobe)
        {
            timer = 0f;
            TeleportirajUSobu();
        }
    }

    void TeleportirajUSobu()
    {
        if (pozicijePoSobama.Length == 0) return;

        Vector3 novaPozicija = pozicijePoSobama[Random.Range(0, pozicijePoSobama.Length)];
        agent.Warp(novaPozicija);
        IdiNaNovuPoziciju();
    }

    void IdiNaNovuPoziciju()
    {
        Vector3 randomPos = Random.insideUnitSphere * radiusKretanja + transform.position;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, radiusKretanja, 1))
            agent.SetDestination(hit.position);
    }
}