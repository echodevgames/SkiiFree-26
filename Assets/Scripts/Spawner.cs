// ----- Spawner.cs START -----

using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Obstacles")]
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject stumpPrefab;
    public GameObject jumpPadPrefab;
    public GameObject mogulPrefab;

    [Header("Slalom")]
    public GameObject leftFlagPrefab;
    public GameObject rightFlagPrefab;
    public float slalomSpawnYOffset = -10f; // 👈 IMPORTANT

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
            GameObject prefabToSpawn = ChoosePrefab();

            float y = spawnY;

            // 🚩 Slalom flags spawn farther ahead
            if (prefabToSpawn == leftFlagPrefab || prefabToSpawn == rightFlagPrefab)
                y = slalomSpawnYOffset;

            Vector3 spawnPos = new Vector3(x, y, 0f);

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
        var gm = GameManager.Instance;
        if (gm == null)
            return treePrefab;

        float lateral = gm.LateralFactor * gm.HeadingDirection;
        float absLat = Mathf.Abs(lateral);

        // =========================
        // BASELINE SLALOM SPAWN
        // =========================
        if (Random.value < 0.18f)
        {
            if (lateral > 0.15f)
                return rightFlagPrefab;

            if (lateral < -0.15f)
                return leftFlagPrefab;

            return Random.value < 0.5f
                ? leftFlagPrefab
                : rightFlagPrefab;
        }

        // =========================
        // STRONG DIRECTION BIAS
        // =========================
        if (lateral > 0.45f && Random.value < 0.35f)
            return rightFlagPrefab;

        if (lateral < -0.45f && Random.value < 0.35f)
            return leftFlagPrefab;

        // =========================
        // CENTER = JUMPS
        // =========================
        if (absLat < 0.2f && Random.value < 0.25f)
            return jumpPadPrefab;

        // =========================
        // FALLBACK RANDOM
        // =========================
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
        // ❌ Flags never clump
        return (prefab == treePrefab || prefab == mogulPrefab)
            && Random.value < clumpChance;
    }
}

// ----- Spawner.cs END -----
