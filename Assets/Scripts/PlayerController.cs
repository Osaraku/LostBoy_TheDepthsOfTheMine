using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;
    public bool enableAirJump = true;

    Vector2 moveInput;
    TouchingDirection touchingDirection;
    Damageable damageable;

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

    public bool _hasAirJump = false;
    public bool isOnPlatform;
    public Rigidbody2D platformRb;

    Rigidbody2D rb;

    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
        damageable = GetComponent<Damageable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (touchingDirection.isGrounded)
        {
            _hasAirJump = false;
            animator.SetBool(AnimationStrings.hasAirJump, false);
        }
    }

    private void FixedUpdate()
    {
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
        else if (context.started && !touchingDirection.isGrounded && canMove && !_hasAirJump)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            _hasAirJump = true;
            animator.SetBool(AnimationStrings.hasAirJump, true);
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
}