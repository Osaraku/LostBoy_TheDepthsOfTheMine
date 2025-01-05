using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; // Tambahkan namespace Audio untuk menggunakan Audio Mixer

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

    // Referensi ke Audio Mixer Group
    public AudioMixerGroup outputMixerGroup; // Tambahkan opsi untuk Audio Mixer Group

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnEnter && !loopSound)
        {
            PlaySound(animator);
        }

        if (playOnEnter && loopSound)
        {
            // Reset timer untuk loop
            PlaySound(animator);
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
                    PlaySound(animator);
                }
                else
                {
                    // Reset timer untuk loop saat pertama kali diputar setelah delay
                    loopTimer = 0f;
                    PlaySound(animator);
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
                PlaySound(animator);
                loopTimer = 0f; // Reset timer
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnExit && !loopSound)
        {
            PlaySound(animator);
        }
    }

    // Fungsi untuk memutar suara dengan mendukung Audio Mixer
    private void PlaySound(Animator animator)
    {
        GameObject tempAudioSource = new GameObject("TempAudio");
        tempAudioSource.transform.position = animator.gameObject.transform.position;

        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.clip = soundToPlay;
        audioSource.volume = volume;

        // Atur output Audio Mixer Group jika tersedia
        if (outputMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = outputMixerGroup;
        }

        audioSource.Play();

        // Hancurkan GameObject setelah suara selesai diputar
        GameObject.Destroy(tempAudioSource, soundToPlay.length);
    }
}