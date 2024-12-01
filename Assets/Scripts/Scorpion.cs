using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection))]
public class Scorpion : MonoBehaviour
{
    public float moveSpeed = 3f;

    Rigidbody2D rb;
    TouchingDirection touchingDirection;

    public enum MoveableDirection { Left, Right };

    private MoveableDirection _moveDirection;
    private Vector2 moveDirectionVector = Vector2.left;
    private bool hasFlipped = false;

    public MoveableDirection moveDirection
    {
        get { return _moveDirection; }
        set {
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
            
            _moveDirection = value; }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
    }

    private void FixedUpdate()
    {
        if (!hasFlipped && touchingDirection.isOnWall && touchingDirection.isGrounded)
        {
            FlipDirection();
            hasFlipped = true;
        }
        else if (!touchingDirection.isOnWall || !touchingDirection.isGrounded)
        {
            hasFlipped = false; // Reset the flag if conditions are not met
        }
        rb.velocity = new Vector2(moveSpeed * moveDirectionVector.x, rb.velocity.y);
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
