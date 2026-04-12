using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    [Header("Settings")]
    public float upSpeed = 2f;
    public int balloonIndex = 0;

    AudioSource audioSource;
    bool isClicked = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (transform.position.y > 6f)
        {
            GameManager.Instance.LoseHealth();
            GameManager.Instance.spawner.RespawnBalloon(balloonIndex);
        }
    }

    private void FixedUpdate()
    {
        if (!isClicked)
            transform.Translate(0, upSpeed * Time.deltaTime, 0);
    }

    private void OnMouseDown()
    {
        if (isClicked) return;
        isClicked = true;

        GameManager.Instance.AddScore();

        // ✅ Get a random sound from GameManager and play it
        AudioClip randomClip = GameManager.Instance.GetRandomPopSound();
        if (randomClip != null)
        {
            audioSource.clip = randomClip;
            audioSource.Play();
        }

        StartCoroutine(WaitThenRespawn());
    }

    IEnumerator WaitThenRespawn()
    {
        // ✅ Wait for the random clip length before respawning
        float clipLength = audioSource.clip != null ? audioSource.clip.length : 0.3f;
        yield return new WaitForSeconds(clipLength);

        GameManager.Instance.spawner.RespawnBalloon(balloonIndex);
    }
}