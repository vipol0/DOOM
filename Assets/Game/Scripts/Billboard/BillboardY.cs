using UnityEngine;

public class BillboardY : Billboard
{
    protected override void LateUpdate()
    {
        if (targetCamera == null) return;

        var targetPosition = new Vector3(
            targetCamera.transform.position.x,
            transform.position.y,
            targetCamera.transform.position.z
        );

        if ((targetPosition - transform.position).sqrMagnitude > 0.001f) transform.LookAt(targetPosition);
    }
}