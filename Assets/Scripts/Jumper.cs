using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    Animator animator;
    PlayerController playerController;
    AudioSource pickupSource;
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
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            pickupSource = GetComponent<AudioSource>();
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HasTarget = true;
        playerController.isOnJumper = true;
        AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        HasTarget = false;
        playerController.isOnJumper = false;
    }
}
