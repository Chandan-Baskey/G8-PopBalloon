using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    public float upSpeed;
    static int score = 0;          // ← static: shared across ALL balloons
    AudioSource audioSource;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (transform.position.y > 6f)
        {
            score = 0;             // ← reset shared score on scene reload
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(0, upSpeed * Time.deltaTime, 0);
    }

    private void OnMouseDown()
    {
        score++;
        scoreText.text = score.ToString();   // all balloons now read same score
        audioSource.Play();
        ResetPosition();
    }

    private void ResetPosition()
    {
        float randomX = Random.Range(-2.2f, 2.2f);
        transform.position = new Vector2(randomX, -6f);
    }
}