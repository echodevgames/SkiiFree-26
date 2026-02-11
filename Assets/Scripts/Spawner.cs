using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject stumpPrefab;
    public GameObject jumpPadPrefab;
    public GameObject mogulPrefab;

    [Header("Timing")]
    public float spawnRate = 1f;

    [Header("Positioning")]
    public float spawnWidth = 6f;
    public float spawnY = -6f;

    [Header("Clumping")]
    public float clumpChance = 0.25f;
    public Vector2Int clumpSizeRange = new Vector2Int(3, 6);
    public float clumpSpreadX = 1.2f;
    public float clumpSpreadY = 0.8f;


    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            timer = 0f;

            float x = Random.Range(-spawnWidth, spawnWidth);
            Vector3 spawnPos = new Vector3(x, spawnY, 0);

            GameObject prefabToSpawn = ChoosePrefab();

            if (ShouldSpawnClump(prefabToSpawn))
            {
                int count = Random.Range(clumpSizeRange.x, clumpSizeRange.y + 1);
                SpawnUtility.SpawnClump(
                    prefabToSpawn,
                    spawnPos,
                    count,
                    clumpSpreadX,
                    clumpSpreadY
                );
            }
            else
            {
                Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            }
        }
    }

    GameObject ChoosePrefab()
    {
        float roll = Random.value;

        if (roll < 0.45f)
            return treePrefab;
        else if (roll < 0.65f)
            return mogulPrefab;
        else if (roll < 0.8f)
            return rockPrefab;
        else if (roll < 0.9f)
            return stumpPrefab;
        else
            return jumpPadPrefab;
    }

    bool ShouldSpawnClump(GameObject prefab)
    {
        // Trees + Moguls can clump
        return (prefab == treePrefab || prefab == mogulPrefab)
            && Random.value < clumpChance;
    }
}
