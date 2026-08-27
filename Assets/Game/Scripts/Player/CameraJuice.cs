using System.Collections.Generic;
using UnityEngine;

public class CameraJuice : BaseMonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMove player;
    [SerializeField] private List<Camera> cameras = new();

    [Header("Head Bobbing")]
    [SerializeField] private float bobFrequency = 10f;
    [SerializeField] private float bobHorizontalAmplitude = 0.05f;
    [SerializeField] private float bobVerticalAmplitude = 0.05f;

    [Header("Tilt / Lean")]
    [SerializeField] private float tiltAngle = 2.5f;
    [SerializeField] private float tiltSpeed = 8f;

    [Header("FOV Effect")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float dashFOV = 75f;
    [SerializeField] private float fovSpeed = 10f;

    private float timer;
    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;

    private void Start()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;

        ValidateReference(player, nameof(player));
        foreach (var cam in cameras) ValidateReference(cam, nameof(cam));
    }

    private void Update()
    {
        if (player == null) return;

        HandleHeadBob();
        HandleTilt();
        HandleFOV();
    }

    private void HandleHeadBob()
    {
        bool isMoving = player.IsGrounded && player.CurrentMoveInput.sqrMagnitude > 0.1f && !player.IsDashing;

        if (isMoving)
        {
            var speedMultiplier = player.IsSprinting ? 1.4f : 1f;
            timer += Time.deltaTime * bobFrequency * speedMultiplier;
            
            if (timer > Mathf.PI * 4f) timer -= Mathf.PI * 4f;

            var newX = initialLocalPos.x + Mathf.Cos(timer * 0.5f) * bobHorizontalAmplitude;
            var newY = initialLocalPos.y + Mathf.Sin(timer) * bobVerticalAmplitude;

            transform.localPosition = new Vector3(newX, newY, initialLocalPos.z);
        }
        else
        {
            var blendSpeed = 1f - Mathf.Exp(-bobFrequency * Time.deltaTime);
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, blendSpeed);
        }
    }

    private void HandleTilt()
    {
        float targetTilt = -player.HorizontalInput * tiltAngle;
        Quaternion targetRotation = initialLocalRot * Quaternion.Euler(0f, 0f, targetTilt);

        float blendSpeed = 1f - Mathf.Exp(-tiltSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, blendSpeed);
    }

    private void HandleFOV()
    {
        float targetFOV = player.IsDashing ? dashFOV : normalFOV;
        float blendSpeed = 1f - Mathf.Exp(-fovSpeed * Time.deltaTime);

        foreach (var cam in cameras)
        {
            if (cam == null) continue;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, blendSpeed);
        }
    }
}