using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Balloon Prefabs")]
    public GameObject[] balloonPrefabs;         // Index 0 = balloon 0, Index 1 = balloon 1 ...
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;                 // Assign Win Panel in Inspector

    [Header("Health Settings")]
    public int maxHealth = 3;

    [Header("Win Settings")]
    public int winScore = 10;                   // Player wins when score reaches this

    private int score = 0;
    private int currentHealth;
    private bool isGameOver = false;
    private bool isWin = false;

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

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);   
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
        if (isGameOver || isWin) return;

        score++;
        UpdateUI();
        if (score >= winScore)
            WinGame();
    }

    public void LoseHealth()
    {
        if (isGameOver || isWin) return;

        currentHealth--;
        UpdateUI();

        if (currentHealth <= 0)
            GameOver();
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score + " / " + winScore;

        if (healthText != null)
        {
            string hearts = "";
            for (int i = 0; i < maxHealth; i++)
                hearts += (i < currentHealth) ? "<color=red>♥</color> " : "<color=grey>♥</color> ";
            healthText.text = hearts;
        }
    }

    void WinGame()
    {
        isWin = true;

        
        if (spawner != null)
            spawner.DisableAllBalloons();         

        if (winPanel != null)
            winPanel.SetActive(true);

        Debug.Log("You Win!");
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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextScene < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextScene);
        else
            SceneManager.LoadScene(0);             
    }
}