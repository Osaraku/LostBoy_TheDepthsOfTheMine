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
    private bool isJumpPressed = false;
    public bool enableAirJump = false;
    private bool hasAirJump = false;
    public bool enableDash = false;
    private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    private bool hasAirDashing = false;
    public bool enableWallSliding = false;
    public float wallSlidingSpeed = 2f;
    public float wallJumpingDirection;
    public float wallJumpingTime = 0.2f;
    public float wallJumpingCounter;
    public float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(5f, 10f);
    public bool isOnPlatform;
    public bool isOnJumper;
    public bool hasLanding;


    Vector2 checkpointPos;
    Vector2 moveInput;
    TouchingDirection touchingDirection;
    Damageable damageable;
    AudioSource pickupSource;
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

    [SerializeField]
    private bool _isWallJumping = false;

    public bool isWallJumping
    {
        get
        {
            return _isWallJumping;
        }
        private set
        {
            _isWallJumping = value;
            animator.SetBool(AnimationStrings.isWallJumping, value);
        }
    }

    private void wallSlide()
    {

        if (touchingDirection.isOnWall && !touchingDirection.isGrounded && moveInput.x != 0 && enableWallSliding)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlidingSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    public void onWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(stopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (isJumpPressed && wallJumpingCounter > 0f)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                if (moveInput.x > 0)
                {
                    isFacingRight = true;
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else if (moveInput.x < 0)
                {
                    isFacingRight = false;
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }

            Invoke(nameof(stopWallJumping), wallJumpingDuration);
        }
    }

    private void stopWallJumping()
    {
        isWallJumping = false;
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
        pickupSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!damageable.IsAlive)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if (isDashing)
        {
            return;
        }

        if (touchingDirection.isGrounded)
        {
            hasAirJump = false;
            animator.SetBool(AnimationStrings.hasAirJump, false);
            hasAirDashing = false;
            if (hasLanding == false)
            {
                hasLanding = true;
                AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);
            }
        }
        else
        {
            hasLanding = false;
        }


        wallSlide();
        onWallJump();
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
            if (!isWallJumping)
            {
                if (isOnPlatform)
                {
                    rb.velocity = new Vector2((moveInput.x * currentMoveSpeed) + platformRb.velocity.x, rb.velocity.y);
                }
                else if (isOnJumper)
                {
                    rb.velocity = new Vector2((moveInput.x * currentMoveSpeed), jumpImpulse + (jumpImpulse / 3));
                }
                else
                    rb.velocity = new Vector2(moveInput.x * currentMoveSpeed, rb.velocity.y);
            }
        }
        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    int healthOnCheckpoint = 100;
    public void checkpointUpdate(Vector2 pos, int health)
    {
        checkpointPos = pos;
        healthOnCheckpoint = health;
    }

    public void onRestart(InputAction.CallbackContext context)
    {
        Debug.Log("Restart");
        if (!IsAlive && context.started)
        {
            rb.simulated = false;
            damageable.Health = healthOnCheckpoint;
            damageable.IsAlive = true;
            transform.position = checkpointPos;
            rb.simulated = true;
        }
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
        if (context.started)
        {
            isJumpPressed = true;
        }
        else if (context.canceled)
        {
            isJumpPressed = false;
        }

        if (context.started && touchingDirection.isGrounded && canMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
        else if (context.started && !touchingDirection.isGrounded && !touchingDirection.isOnWall && canMove && !hasAirJump && enableAirJump)
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
        animator.SetBool(AnimationStrings.canMove, false);

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
        animator.SetBool(AnimationStrings.canMove, true);
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        enableDash = true;
    }



}