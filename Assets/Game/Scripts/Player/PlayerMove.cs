using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 8.5f;
    [SerializeField] private float acceleration = 80f;
    [SerializeField] private float deceleration = 70f;

    [Header("Parkour Air Control (WASD)")] 
    [SerializeField] private float airControl = 14f;
    [SerializeField] private float jumpCutoff = 0.5f;

    [Header("Dash")] 
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float dashSpeed = 28f;
    [SerializeField] private float dashTime = 0.15f;
    [SerializeField] private float dashCooldown = 0.7f;

    [Header("Gravity & Jump")] 
    [SerializeField] private float gravity = -28f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;

    [Header("Wall Jump & Slide")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float wallCheckDistance = 0.8f;
    [SerializeField] private float wallSlideSpeed = 2.5f;
    [SerializeField] private float wallJumpUpForce = 9f;
    [SerializeField] private float wallJumpOffForce = 10f;

    [Header("Assist / Coyote & Buffer")] 
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    private CharacterController ch;
    private Vector3 verticalVelocity;
    private Vector3 horizontalVelocity;
    private bool canDash = true;

    private float coyoteTimer;
    private float jumpBufferTimer;

    // Переменные стены
    private bool isTouchingWall;
    private Vector3 wallNormal;
    private bool hasWallJumped;

    public bool IsGrounded { get; private set; }
    public bool IsWallSliding { get; private set; }
    public bool IsDashing { get; private set; }
    public bool IsSprinting { get; private set; }
    public float HorizontalInput { get; private set; }
    public Vector3 CurrentMoveInput { get; private set; }

    private void Awake()
    {
        ch = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (ch == null) return;

        CheckGround();
        CheckWall();
        HandleInputs();

        if (!IsDashing)
        {
            if (IsGrounded)
                ApplyGroundMovement();
            else
                ApplyAirMovement();

            ApplyWallSlide();
            ApplyJumpAndGravity();

            ch.Move((horizontalVelocity + verticalVelocity) * Time.deltaTime);
        }
    }

    private void CheckGround()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (IsGrounded)
        {
            coyoteTimer = coyoteTime;
            hasWallJumped = false; // Сброс прыжка от стены при касании земли

            if (verticalVelocity.y < 0) verticalVelocity.y = -2f;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }

    private void CheckWall()
    {
        if (IsGrounded)
        {
            isTouchingWall = false;
            IsWallSliding = false;
            return;
        }

        // Проверка 4 направлений вокруг игрока на наличие стены
        Vector3[] rayDirections = { transform.forward, transform.right, -transform.right, -transform.forward };
        isTouchingWall = false;

        foreach (var dir in rayDirections)
        {
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, wallCheckDistance, wallMask))
            {
                isTouchingWall = true;
                wallNormal = hit.normal;
                break;
            }
        }

        // Скольжение активируется, если соприкасаемся со стеной и падаем вниз
        IsWallSliding = isTouchingWall && verticalVelocity.y < 0f;
    }

    private void HandleInputs()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");

        IsSprinting = Input.GetKey(KeyCode.LeftShift);

        CurrentMoveInput = (transform.right * HorizontalInput + transform.forward * verticalInput).normalized;

        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        if (Input.GetButtonUp("Jump") && verticalVelocity.y > 0f) verticalVelocity.y *= jumpCutoff;

        if (Input.GetKeyDown(KeyCode.LeftControl) && canDash) StartCoroutine(PerformDash());
    }

    private void ApplyGroundMovement()
    {
        var targetSpeed = IsSprinting ? runSpeed : moveSpeed;
        var targetVelocity = CurrentMoveInput * targetSpeed;

        var rate = CurrentMoveInput.sqrMagnitude > 0.01f ? acceleration : deceleration;
        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, rate * Time.deltaTime);
    }

    private void ApplyAirMovement()
    {
        var targetSpeed = IsSprinting ? runSpeed : moveSpeed;

        if (CurrentMoveInput.sqrMagnitude > 0.01f)
        {
            var currentMaxSpeed = Mathf.Max(horizontalVelocity.magnitude, targetSpeed);
            var targetAirVelocity = CurrentMoveInput * currentMaxSpeed;

            horizontalVelocity = Vector3.Lerp(horizontalVelocity, targetAirVelocity, airControl * Time.deltaTime);
        }
        else
        {
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, 2f * Time.deltaTime);
        }
    }

    private void ApplyWallSlide()
    {
        // Ограничиваем скорость падения при скольжении
        if (IsWallSliding && verticalVelocity.y < -wallSlideSpeed)
        {
            verticalVelocity.y = -wallSlideSpeed;
        }
    }

    private void ApplyJumpAndGravity()
    {
        // Обычный прыжок от земли
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
        // Прыжок от стены (только 1 раз до касания земли)
        else if (jumpBufferTimer > 0f && isTouchingWall && !IsGrounded && !hasWallJumped)
        {
            verticalVelocity.y = wallJumpUpForce;
            horizontalVelocity = wallNormal * wallJumpOffForce;

            hasWallJumped = true;
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        IsDashing = true;
        verticalVelocity = Vector3.zero;

        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");

        Vector3 dashDir;

        if (x == 0 && z >= 0)
            dashDir = cameraTransform != null ? cameraTransform.forward : transform.forward;
        else
            dashDir = CurrentMoveInput;

        if (dashDir.sqrMagnitude < 0.01f)
            dashDir = cameraTransform != null ? cameraTransform.forward : transform.forward;

        dashDir.Normalize();

        var startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            ch.Move(dashDir * dashSpeed * Time.deltaTime);
            yield return null;
        }

        horizontalVelocity = dashDir * (dashSpeed * 0.7f);

        IsDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}