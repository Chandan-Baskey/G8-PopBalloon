using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Balloon Prefabs")]
    public GameObject[] balloonPrefabs;

    [Header("Sound Effects")]
    public AudioClip[] popSounds;           // ✅ Drag all 4-8 sound effects here

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;

    [Header("Panels")]
    public GameObject gameOverPanel;

    [Header("Health Settings")]
    public int maxHealth = 3;

    private int score = 0;
    private int currentHealth;
    private bool isGameOver = false;

    [HideInInspector] public Spawner spawner;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // Called by Spawner to get the correct prefab by index
    public GameObject GetBalloonPrefab(int index)
    {
        if (index >= 0 && index < balloonPrefabs.Length)
            return balloonPrefabs[index];

        Debug.LogWarning("Balloon prefab index out of range: " + index);
        return null;
    }

     
    public AudioClip GetRandomPopSound()
    {
        if (popSounds == null || popSounds.Length == 0)
        {
            Debug.LogWarning("No pop sounds assigned in GameManager!");
            return null;
        }

        int randomIndex = Random.Range(0, popSounds.Length);
        return popSounds[randomIndex];
    }


    public void AddScore()
    {
        if (isGameOver) return;

        score++;
        UpdateUI();
    }

    // Called by balloon when it ESCAPES TOP → -1 Health
    public void LoseHealth()
    {
        if (isGameOver) return;

        currentHealth--;
        UpdateUI();

        if (currentHealth <= 0)
            GameOver();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "" + score;

        if (healthText != null)
        {
            string hearts = "";
            for (int i = 0; i < maxHealth; i++)
                hearts += (i < currentHealth) ? "<color=red>♥</color> " : "<color=grey>♥</color> ";
            healthText.text = hearts;
        }
    }

    void GameOver()
    {
        isGameOver = true;

        if (spawner != null)
            spawner.DisableAllBalloons();


        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("Game Over!");
    }

    // Hook to Restart Button
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void startGame()
    {
        SceneManager.LoadScene(1);
    }
}