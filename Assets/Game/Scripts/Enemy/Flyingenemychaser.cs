using UnityEngine;

public class FlyingEnemyChaser : BaseMonoBehaviour
{
    [Header("Target")] [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";

    [Header("Move")] [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float accelSmoothing = 5f;

    [Header("Obstacle")] [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float obstacleCheckDistance = 3f;
    [SerializeField] private float avoidanceForce = 6f;
    [SerializeField] private float agentRadius = 0.5f;

    private Rigidbody rb;
    private Vector3 currentVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        if (target != null) return;
        var playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            target = playerObj.transform;

        ValidateReference(target, nameof(target));
    }

    private void Update()
    {
        if (target == null) return;

        var desiredDirection = (target.position - transform.position).normalized;
        var avoidanceDirection = CalculateAvoidanceDirection(desiredDirection);

        var finalDirection = (desiredDirection + avoidanceDirection).normalized;

        MoveAndRotate(finalDirection);
    }

    private Vector3 CalculateAvoidanceDirection(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude < 0.001f)
            return Vector3.zero;

        if (Physics.SphereCast(transform.position, agentRadius, moveDirection, out var hit, obstacleCheckDistance,
                obstacleMask))
        {
            var slideDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal).normalized;

            var closeness = 1f - Mathf.Clamp01(hit.distance / obstacleCheckDistance);
            var pushAway = hit.normal * closeness * avoidanceForce;

            return slideDirection * avoidanceForce * 0.5f + pushAway;
        }

        return Vector3.zero;
    }

    private void MoveAndRotate(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f) return;

        var targetVelocity = direction * moveSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * accelSmoothing);

        if (rb != null)
            rb.MovePosition(transform.position + currentVelocity * Time.deltaTime);
        else
            transform.position += currentVelocity * Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * obstacleCheckDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, agentRadius);
    }
}