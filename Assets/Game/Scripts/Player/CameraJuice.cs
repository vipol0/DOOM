using UnityEngine;

public class CameraJuice : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMove player;
    [SerializeField] private Camera cam;

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

    private void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
        initialLocalPos = transform.localPosition;
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
        if (player.IsGrounded && player.CurrentMoveInput.sqrMagnitude > 0.1f && !player.IsDashing)
        {
            float currentSpeedMultiplier = player.IsSprinting ? 1.4f : 1f;
            timer += Time.deltaTime * bobFrequency * currentSpeedMultiplier;

            float newX = initialLocalPos.x + Mathf.Cos(timer / 2) * bobHorizontalAmplitude;
            float newY = initialLocalPos.y + Mathf.Sin(timer) * bobVerticalAmplitude;

            transform.localPosition = new Vector3(newX, newY, initialLocalPos.z);
        }
        else
        {
            timer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos, Time.deltaTime * bobFrequency);
        }
    }

    private void HandleTilt()
    {
        float targetTilt = -player.HorizontalInput * tiltAngle;
        
        Quaternion targetRotation = Quaternion.Euler(
            transform.localRotation.eulerAngles.x,
            transform.localRotation.eulerAngles.y,
            targetTilt
        );

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }

    private void HandleFOV()
    {
        float targetFOV = player.IsDashing ? dashFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
    }
}