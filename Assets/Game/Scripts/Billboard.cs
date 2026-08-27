using System;
using UnityEngine;

public class Billboard : BaseMonoBehaviour
{
    [SerializeField] protected Camera targetCamera;

    protected virtual void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;

        ValidateReference(targetCamera, nameof(targetCamera));
    }

    protected virtual void LateUpdate()
    {
        if (targetCamera == null) return;

        if ((targetCamera.transform.position - transform.position).sqrMagnitude > 0.001f)
            transform.LookAt(targetCamera.transform.position);
    }

    public void ResetPosition()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}