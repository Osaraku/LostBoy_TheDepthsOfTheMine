using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;

    Transform nextWaypoint;
    int waypointNum = 0;

    public float flightSpeed = 2f;
    public float waypointReachedDistance = 0.1f;
    private bool isReversing = false;
    public DetectionZone biteDetectionZone;
    public List<Transform> waypoints;

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

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nextWaypoint = waypoints[waypointNum];
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = biteDetectionZone.detectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                Flight();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            // fall when death
            rb.gravityScale = 2f;
        }
    }

    private void Flight()
    {
        // Fly to next Waypoint
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        // Check reached waypoint
        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;
        UpdateDirection();

        // Check need to switch waypoints
        if (distance <= waypointReachedDistance)
        {
            if (isReversing)
            {
                // Jika sedang berkurang
                waypointNum--;

                if (waypointNum < 0)
                {
                    // Jika mencapai 0, ubah arah menjadi bertambah
                    waypointNum = 0;
                    isReversing = false;
                }
            }
            else
            {
                // Jika sedang bertambah
                waypointNum++;

                if (waypointNum >= waypoints.Count)
                {
                    // Jika mencapai akhir, ubah arah menjadi berkurang
                    waypointNum = waypoints.Count - 1;
                    isReversing = true;
                }
            }

            // Update next waypoint
            nextWaypoint = waypoints[waypointNum];
        }
    }

    private void UpdateDirection()
    {
        Vector3 locScale = transform.localScale;

        if (transform.localScale.x > 0)
        {
            // Facing right
            if (rb.velocity.x < 0)
            {
                // Flip
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else
        {
            // Facing left
            if (rb.velocity.x > 0)
            {
                // Flip
                transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
    }
}
