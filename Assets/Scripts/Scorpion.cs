using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class Scorpion : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float moveStopRate = 0.05f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;

    Rigidbody2D rb;
    TouchingDirection touchingDirection;
    Animator animator;
    Damageable damageable;

    public enum MoveableDirection { Left, Right };

    private MoveableDirection _moveDirection;
    private Vector2 moveDirectionVector = Vector2.left;
    private bool hasFlipped = false;

    public MoveableDirection moveDirection
    {
        get { return _moveDirection; }
        set
        {
            if (_moveDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == MoveableDirection.Left)
                {
                    moveDirectionVector = Vector2.left;
                }
                else if (value == MoveableDirection.Right)
                {
                    moveDirectionVector = Vector2.right;
                }

            }

            _moveDirection = value;
        }
    }

    public bool _hasTarget = false;

    public bool HasTarget
    {
        get
        {
            return _hasTarget;
        }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool canMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        // Debug.Log($"hasFlipped: {hasFlipped}, isOnWall: {touchingDirection.isOnWall}, isGrounded: {touchingDirection.isGrounded}");
        if (!hasFlipped && touchingDirection.isOnWall && touchingDirection.isGrounded)
        {
            FlipDirection();
            hasFlipped = true;
            Debug.Log($"Flip");
        }
        else if (!touchingDirection.isOnWall || !touchingDirection.isGrounded)
        {
            hasFlipped = false; // Reset the flag if conditions are not met
        }
        if (!damageable.lockVelocity)
        {
            if (canMove)
                rb.velocity = new Vector2(moveSpeed * moveDirectionVector.x, rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, moveStopRate), rb.velocity.y);

        }
    }

    private void FlipDirection()
    {
        if (moveDirection == MoveableDirection.Left)
        {
            moveDirection = MoveableDirection.Right;
        }
        else if (moveDirection == MoveableDirection.Right)
        {
            moveDirection = MoveableDirection.Left;
        }
        else
        {
            Debug.LogError("walkable tidak diset right / left");
        }

    }

    public void onHit(int damage, Vector2 knockback)
    {
        if (moveDirection == MoveableDirection.Left)
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        else
            rb.velocity = new Vector2(knockback.x * -1, rb.velocity.y + knockback.y);
    }

    public void onCliffDetected()
    {
        if (touchingDirection.isGrounded)
        {
            FlipDirection();
        }

    }



}
