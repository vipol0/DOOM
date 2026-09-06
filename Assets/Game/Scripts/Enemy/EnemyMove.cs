using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : BaseMonoBehaviour
{
    [SerializeField] private Transform player;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        ValidateReference(player, nameof(player));
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