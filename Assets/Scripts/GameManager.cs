using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Balloon Prefabs")]
    public GameObject[] balloonPrefabs;    // Index 0 = balloon 0, Index 1 = balloon 1 ...

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;
    public GameObject gameOverPanel;

    [Header("Health Settings")]
    public int maxHealth = 3;

    private int score = 0;
    private int currentHealth;
    private bool isGameOver = false;

    // Spawner will register itself here
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

    // Called by balloon when CLICKED → +1 Score
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
            TriggerGameOver();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (healthText != null)
        {
            string hearts = "";
            for (int i = 0; i < maxHealth; i++)
                hearts += (i < currentHealth) ? "<color=red>♥</color> " : "<color=grey>♥</color> ";

            healthText.text = hearts;
        }
    }

    void TriggerGameOver()
    {
        isGameOver = true;

        // Stop all active balloons via Spawner
        if (spawner != null)
            spawner.DisableAllBalloons();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log("Game Over!");
    }

    // Hooked to Restart Button onClick
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
