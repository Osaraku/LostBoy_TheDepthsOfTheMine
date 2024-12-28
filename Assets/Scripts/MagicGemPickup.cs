using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MagicGemPickup : MonoBehaviour
{
    public float moveAmount = 0.1f; // Jarak gerakan naik/turun
    public float moveSpeed = 5f; // Kecepatan gerakan
    AudioSource pickupSource;
    PlayerController playerController;

    private Vector3 startPosition;
    private float timer;
    public bool enablingAirJump;
    public bool enablingDash;
    public bool enablingWallSlide;

    private void Awake()
    {
        pickupSource = GetComponent<AudioSource>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
        if (pickupSource)
            AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);

        if (enablingAirJump)
            playerController.enableAirJump = true;
        if (enablingDash)
            playerController.enableDash = true;
        if (enablingWallSlide)
            playerController.enableWallSliding = true;

        Destroy(gameObject);
    }

}
