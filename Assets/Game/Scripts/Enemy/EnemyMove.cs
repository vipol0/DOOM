using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Transform player;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player != null && agent != null) agent.SetDestination(player.position);
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}