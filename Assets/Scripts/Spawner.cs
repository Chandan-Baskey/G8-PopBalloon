using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    private GameObject[] activeBalloons;
    private bool isDisabled = false;        // Prevents respawn after game over / win

    void Start()
    {
        GameManager.Instance.spawner = this;

        int count = GameManager.Instance.balloonPrefabs.Length;
        activeBalloons = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            SpawnBalloon(i);
        }
    }

    void SpawnBalloon(int index)
    {
        GameObject prefab = GameManager.Instance.GetBalloonPrefab(index);
        if (prefab == null) return;

        Vector3 spawnPos = (index < spawnPoints.Length)
            ? spawnPoints[index].position
            : transform.position;

        activeBalloons[index] = Instantiate(prefab, spawnPos, Quaternion.identity);

        Movement movement = activeBalloons[index].GetComponent<Movement>();
        if (movement != null)
            movement.balloonIndex = index;
    }

    // Called by Movement when balloon escapes or clicked → respawn
    public void RespawnBalloon(int index)
    {
        if (isDisabled) return;             // ✅ Don't respawn after game over / win

        if (activeBalloons[index] != null)
            Destroy(activeBalloons[index]);

        SpawnBalloon(index);
    }

    // ✅ Fixed: disable ALL balloons including ones mid-coroutine
    public void DisableAllBalloons()
    {
        isDisabled = true;                  // ✅ Block any future respawns

        // Disable tracked balloons
        for (int i = 0; i < activeBalloons.Length; i++)
        {
            if (activeBalloons[i] != null)
                activeBalloons[i].SetActive(false);
        }

        // ✅ Also find ANY balloon in scene (catches ones mid-coroutine / just respawned)
        Movement[] allBalloons = FindObjectsOfType<Movement>();
        foreach (Movement b in allBalloons)
        {
            if (b != null)
                b.gameObject.SetActive(false);
        }
    }
}