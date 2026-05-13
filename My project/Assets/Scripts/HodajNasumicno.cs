using UnityEngine;
using UnityEngine.AI;

public class HodajNasumicno : MonoBehaviour
{
    public float radiusKretanja = 3f;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        IdiNaNovuPoziciju();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            IdiNaNovuPoziciju();
    }

    void IdiNaNovuPoziciju()
    {
        Vector3 randomPos = Random.insideUnitSphere * radiusKretanja + transform.position;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, radiusKretanja, 1))
            agent.SetDestination(hit.position);
    }
}