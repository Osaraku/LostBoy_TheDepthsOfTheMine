using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Checkpoint : MonoBehaviour
{
    PlayerController playerController;
    Damageable damageable;
    SpriteRenderer sr;
    AudioSource pickupSource;

    public int healthOnCheckpoint;
    public Sprite passive, active;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        damageable = GameObject.FindGameObjectWithTag("Player").GetComponent<Damageable>();
        sr = GetComponent<SpriteRenderer>();
        pickupSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerController.IsAlive)
        {
            AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);
            healthOnCheckpoint = damageable.Health;
            playerController.checkpointUpdate(transform.position, healthOnCheckpoint);
            sr.sprite = active;
            StartCoroutine(ChangeSpriteBackToPassive());
        }
    }

    private IEnumerator ChangeSpriteBackToPassive()
    {
        // Tunggu selama 3 detik
        yield return new WaitForSeconds(1f);

        // Ubah sprite kembali ke passive
        sr.sprite = passive;
    }
}
