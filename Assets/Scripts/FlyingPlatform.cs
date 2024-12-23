using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPlatform : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    PlayerController playerController;


    Transform nextWaypoint;
    int waypointNum = 0;

    public float flightSpeed = 2f;
    public float waypointReachedDistance = 0.1f;
    private bool isReversing = false;
    public List<Transform> waypoints;

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
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nextWaypoint = waypoints[waypointNum];
    }

    // Update is called once per frame
    private void FixedUpdate()
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

    private void Flight()
    {
        // Fly to next Waypoint
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        // Check reached waypoint
        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerController.isOnPlatform = true;
        playerController.platformRb = rb;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        playerController.isOnPlatform = false;
    }
}
