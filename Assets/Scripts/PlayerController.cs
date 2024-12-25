using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;
    public bool enableAirJump = true;
    private bool hasAirJump = false;
    public bool isOnPlatform;
    public bool enableDash = true;
    private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    private bool hasAirDashing = false;
    public float wallSlidingSpeed = 2f;


    Vector2 moveInput;
    TouchingDirection touchingDirection;
    Damageable damageable;
    public Rigidbody2D platformRb;
    public TrailRenderer tr;

    public float currentMoveSpeed
    {
        get
        {
            if (canMove)
            {
                if (isMoving && !touchingDirection.isOnWall)
                {
                    if (touchingDirection.isGrounded)
                    {
                        return moveSpeed;
                    }
                    else
                    {
                        return airWalkSpeed;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }

    [SerializeField]
    private bool _isMoving = false;

    public bool isMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    [SerializeField]
    private bool _isWallSliding = false;

    public bool isWallSliding
    {
        get
        {
            return _isWallSliding;
        }
        private set
        {
            _isWallSliding = value;
            animator.SetBool(AnimationStrings.isWallSliding, value);
        }
    }

    private void wallSlide()
    {

        if (touchingDirection.isOnWall && !touchingDirection.isGrounded && moveInput.x != 0)
        {
            Debug.Log("Wall sliding");
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlidingSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    public bool _isFacingRight = true;

    public bool isFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    public bool canMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }

    Rigidbody2D rb;

    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
        damageable = GetComponent<Damageable>();
        tr = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        wallSlide();
        if (isDashing)
        {
            return;
        }

        if (touchingDirection.isGrounded)
        {
            hasAirJump = false;
            animator.SetBool(AnimationStrings.hasAirJump, false);
            hasAirDashing = false;
        }


    }

    private void FixedUpdate()
    {
        wallSlide();
        if (isDashing)
        {
            return;
        }

        if (!damageable.lockVelocity)
        {
            if (isOnPlatform)
            {
                Debug.Log("onPlatform");
                rb.velocity = new Vector2((moveInput.x * currentMoveSpeed) + platformRb.velocity.x, rb.velocity.y);
            }
            else
                rb.velocity = new Vector2(moveInput.x * currentMoveSpeed, rb.velocity.y);
        }


        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    public void onMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            isMoving = moveInput != Vector2.zero;
            setFacingDirection(moveInput);
        }
        else
        {
            isMoving = false;
        }
    }

    public void onJump(InputAction.CallbackContext context)
    {
        // TODO check if alive & check if have air jump
        if (context.started && touchingDirection.isGrounded && canMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
        else if (context.started && !touchingDirection.isGrounded && canMove && !hasAirJump)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            hasAirJump = true;
            animator.SetBool(AnimationStrings.hasAirJump, true);
        }
    }
    public void onDash(InputAction.CallbackContext context)
    {
        if (context.started && enableDash && canMove)
        {
            if (touchingDirection.isGrounded)
            {
                animator.SetTrigger(AnimationStrings.dashTrigger);
                StartCoroutine(Dash());
            }
            else if (!hasAirDashing)
            {
                animator.SetTrigger(AnimationStrings.dashTrigger);
                StartCoroutine(Dash());
                hasAirDashing = true;
            }
        }
    }

    public void onPickaxe(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.pickaxeTrigger);
        }
    }
    private void setFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            isFacingRight = false;
        }
    }

    public void onHit(int damage, Vector2 knockback)
    {
        if (!isFacingRight)
            rb.velocity = new Vector2(knockback.x, knockback.y);
        else
            rb.velocity = new Vector2(knockback.x * -1, knockback.y);
    }

    private IEnumerator Dash()
    {
        enableDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Tentukan arah dash berdasarkan input
        Vector2 dashDirection = new Vector2(moveInput.x, moveInput.y);

        // Jika tidak ada input diagonal, fallback ke arah horizontal default
        if (dashDirection == Vector2.zero)
        {
            dashDirection = new Vector2(isFacingRight ? 1f : -1f, 0f);
        }

        // Normalisasi arah dash untuk kecepatan konsisten di semua arah
        dashDirection = dashDirection.normalized;

        // Terapkan kecepatan dash
        if (moveInput.y > 0)
            rb.velocity = dashDirection * (dashingPower / 2);
        else if (moveInput.y > 0 && (moveInput.x > 0 || moveInput.x < 0))
            rb.velocity = dashDirection * (dashingPower / 4 * 3);
        else
            rb.velocity = dashDirection * dashingPower;

        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        enableDash = true;
    }



}