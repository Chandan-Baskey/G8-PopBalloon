using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    [Header("Settings")]
    public float upSpeed = 2f;
    public int balloonIndex = 0;

    AudioSource audioSource;
    bool isClicked = false;             // Prevents respawn before sound finishes

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Balloon escaped to TOP → lose health + respawn
        if (transform.position.y > 6f)
        {
            GameManager.Instance.LoseHealth();
            GameManager.Instance.spawner.RespawnBalloon(balloonIndex);
        }
    }

    private void FixedUpdate()
    {
        if (!isClicked)                 // Stop moving after click so it doesn't escape while sound plays
            transform.Translate(0, upSpeed * Time.deltaTime, 0);
    }

    private void OnMouseDown()
    {
        if (isClicked) return;          // Ignore double clicks
        isClicked = true;

        GameManager.Instance.AddScore();
        audioSource.Play();             // ✅ Play sound FIRST

        StartCoroutine(WaitThenRespawn()); // ✅ Respawn AFTER sound finishes
    }

    IEnumerator WaitThenRespawn()
    {
        float clipLength = audioSource.clip != null ? audioSource.clip.length : 0.3f;
        yield return new WaitForSeconds(clipLength); // Wait for full sound to play

        GameManager.Instance.spawner.RespawnBalloon(balloonIndex); // Then respawn
    }
}