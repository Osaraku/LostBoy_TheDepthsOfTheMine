using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HealthPickup : MonoBehaviour
{
    public int healthRestore = 10;
    public float moveAmount = 0.1f; // Jarak gerakan naik/turun
    public float moveSpeed = 5f; // Kecepatan gerakan


    AudioSource pickupSource;

    private Vector3 startPosition;
    private float timer;

    private void Awake()
    {
        pickupSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position; // Simpan posisi awal
    }

    // Update is called once per frame
    void Update()
    {
        // Hitung posisi baru menggunakan sinus untuk gerakan naik/turun
        timer += Time.deltaTime * moveSpeed;
        float newY = Mathf.Sin(timer) * moveAmount;

        // Update posisi objek
        transform.position = startPosition + new Vector3(0, newY, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable)
        {
            damageable.Heal(healthRestore);

            if (pickupSource)
            {
                AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);
            }

            Destroy(gameObject);
        }
    }
}