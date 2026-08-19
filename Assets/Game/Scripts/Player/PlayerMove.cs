using System.Collections;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float runSpeed = 6f;

    private float currentSpeed;
    private float x;
    private float z;

    [Header("Dash")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool isDashing;
    private bool canDash = true;

    [Header("Gravity")]
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController ch;
    private Vector3 verticalVelocity;
    private bool isGrounded;
    
    public bool IsGrounded => isGrounded;
    public bool IsDashing => isDashing;
    public bool IsSprinting { get; private set; }
    public float HorizontalInput => x;
    public Vector3 CurrentMoveInput { get; private set; }
    
    public event Action<float> PlayerVelocityChanged;

    private void Awake()
    {
        ch = GetComponent<CharacterController>();
        currentSpeed = moveSpeed;
    }

    private void Update()
    {
        PlayerVelocityChanged?.Invoke(ch.velocity.magnitude);
        
        if (isDashing || ch == null || groundCheck == null) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && verticalVelocity.y < 0) verticalVelocity.y = -2f;

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        IsSprinting = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = IsSprinting ? runSpeed : moveSpeed;

        CurrentMoveInput = (transform.right * x + transform.forward * z).normalized;

        if (Input.GetButtonDown("Jump") && isGrounded) verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        if (Input.GetKeyDown(KeyCode.LeftControl) && canDash)
        {
            StartCoroutine(PerformDash(CurrentMoveInput));
            return;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        var finalVelocity = CurrentMoveInput * currentSpeed + verticalVelocity;
        ch.Move(finalVelocity * Time.deltaTime);
    }

    private IEnumerator PerformDash(Vector3 moveDirection)
    {
        canDash = false;
        isDashing = true;
        verticalVelocity.y = 0f;

        if (moveDirection.sqrMagnitude == 0 || z > 0)
        {
            if (cameraTransform != null)
            {
                moveDirection = cameraTransform.forward; 
            }
            else
            {
                moveDirection = transform.forward;
            }
        }

        var startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            ch.Move(moveDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}