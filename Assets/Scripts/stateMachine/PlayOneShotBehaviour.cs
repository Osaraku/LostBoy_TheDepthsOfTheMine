using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOneShotBehaviour : StateMachineBehaviour
{
    public AudioClip soundToPlay;
    public float volume = 1f;
    public bool playOnEnter = true, playOnExit = false, playAfterDelay = false, loopSound = false;

    // Interval untuk suara looping (dalam detik)
    public float loopInterval = 1f; // Waktu antar suara loop

    // Delayed sound timer
    public float playDelay = 0.25f;
    private float timeSinceEntered = 0;
    private float loopTimer = 0;
    private bool hasDelayedSoundPlayed = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnEnter && !loopSound)
        {
            AudioSource.PlayClipAtPoint(soundToPlay, animator.gameObject.transform.position, volume);
        }

        if (playOnEnter && loopSound)
        {
            // Reset timer untuk loop
            AudioSource.PlayClipAtPoint(soundToPlay, animator.gameObject.transform.position, volume);
            loopTimer = 0f;
        }

        timeSinceEntered = 0f;
        hasDelayedSoundPlayed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playAfterDelay && !hasDelayedSoundPlayed)
        {
            timeSinceEntered += Time.deltaTime;

            if (timeSinceEntered > playDelay)
            {
                if (!loopSound)
                {
                    AudioSource.PlayClipAtPoint(soundToPlay, animator.gameObject.transform.position, volume);
                }
                else
                {
                    // Reset timer untuk loop saat pertama kali diputar setelah delay
                    loopTimer = 0f;
                    AudioSource.PlayClipAtPoint(soundToPlay, animator.gameObject.transform.position, volume);
                }

                hasDelayedSoundPlayed = true;
            }
        }

        if (loopSound)
        {
            // Update timer untuk loop
            loopTimer += Time.deltaTime;

            if (loopTimer >= loopInterval)
            {
                // Mainkan suara baru sesuai interval loop
                AudioSource.PlayClipAtPoint(soundToPlay, animator.gameObject.transform.position, volume);
                loopTimer = 0f; // Reset timer
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnExit && !loopSound)
        {
            AudioSource.PlayClipAtPoint(soundToPlay, animator.gameObject.transform.position, volume);
        }

        // Tidak ada AudioSource yang perlu dihentikan karena kita menggunakan PlayClipAtPoint
    }
}