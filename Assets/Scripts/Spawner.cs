using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;        // Index 0 = spawn point for balloon 0, etc.

    // Tracks all active spawned balloon instances
    private GameObject[] activeBalloons;

    void Start()
    {
        // Register this spawner with GameManager
        GameManager.Instance.spawner = this;

        int count = GameManager.Instance.balloonPrefabs.Length;
        activeBalloons = new GameObject[count];

        // Spawn each balloon prefab at its matching spawn point
        for (int i = 0; i < count; i++)
        {
            SpawnBalloon(i);
        }
    }

    void SpawnBalloon(int index)
    {
        GameObject prefab = GameManager.Instance.GetBalloonPrefab(index);

        if (prefab == null) return;

        // Use matching spawn point, or fallback to this object's position
        Vector3 spawnPos = (index < spawnPoints.Length)
            ? spawnPoints[index].position
            : transform.position;

        // Instantiate the balloon at the spawn point
        activeBalloons[index] = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Tell the balloon which index it is (for respawning)
        Movement movement = activeBalloons[index].GetComponent<Movement>();
        if (movement != null)
            movement.balloonIndex = index;
    }

    // Called by Movement when balloon escapes → respawn it
    public void RespawnBalloon(int index)
    {
        if (activeBalloons[index] != null)
            Destroy(activeBalloons[index]);

        SpawnBalloon(index);
    }

    // Called by GameManager on Game Over → stop all balloons
    public void DisableAllBalloons()
    {
        foreach (GameObject balloon in activeBalloons)
        {
            if (balloon != null)
                balloon.SetActive(false);
        }
    }
}