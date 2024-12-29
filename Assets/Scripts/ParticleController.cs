using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] ParticleSystem movementParticle;
    [SerializeField] ParticleSystem fallParticle;
    [SerializeField] ParticleSystem touchParticle;

    [Range(0, 10)]
    [SerializeField] int occurAfterVelocity;

    [Range(0, 0.2f)]
    [SerializeField] float dustFormationPeriod;

    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] TouchingDirection playerTd;
    [SerializeField] PlayerController playerController;
    private bool isFalling;

    float counter;

    private void Update()
    {
        counter += Time.deltaTime;

        if (playerController.isFacingRight)
        {
            movementParticle.transform.localScale = new Vector3(1, 1, 1);
            fallParticle.transform.localScale = new Vector3(1, 1, 1);
            touchParticle.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            movementParticle.transform.localScale = new Vector3(-1, 1, 1);
            fallParticle.transform.localScale = new Vector3(-1, 1, 1);
            touchParticle.transform.localScale = new Vector3(-1, 1, 1);
        }

        if (!playerTd.isGrounded)
        {
            isFalling = true;
        }

        if (!playerController.isOnPlatform && playerTd.isGrounded && Mathf.Abs(playerRb.velocity.x) > occurAfterVelocity)
        {
            if (counter > dustFormationPeriod)
            {
                movementParticle.Play();
                counter = 0;
            }
        }

        if (playerTd.isGrounded && isFalling)
        {
            fallParticle.Play();
            isFalling = false;
        }

        if (playerController.isWallSliding && Mathf.Abs(playerRb.velocity.y) > occurAfterVelocity)
        {
            if (counter > dustFormationPeriod)
            {
                touchParticle.Play();
                counter = 0;
            }
        }
    }


}
