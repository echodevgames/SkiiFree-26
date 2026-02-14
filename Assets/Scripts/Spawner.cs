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

    public float slalomActivationX = 3.25f;
    public float slalomDeactivateX = 1.25f;
    public float slalomOffsetX = 3.5f;
    public float slalomSpawnY = -10f;
    public float slalomSpawnInterval = 1f; // (unused now, ok to keep)

    [Header("Virtual Lateral Drift")]
    public float lateralDriftSpeed = 2f;
    public float lateralDeadzone = 0.05f;
    public float lateralReturnSpeed = 0.6f;
    public float maxVirtualX = 8f;

    [Header("Normal Spawning")]
    public float spawnWidth = 6f;
    public float spawnY = -6f;

    [Header("Clumping")]
    [Range(0f, 1f)]
    public float clumpChance = 0.35f;

    public int clumpMinCount = 3;
    public int clumpMaxCount = 6;

    public float clumpSpreadX = 2.8f;   // WIDE horizontally
    public float clumpSpreadY = 0.6f;   // TIGHT vertically

    float distanceAccumulator;
    float slalomDistanceAccumulator;

    float virtualPlayerX;
    bool slalomActive;

    const float slalomCenterX = 0f;
    SlalomSide nextSlalomSide = SlalomSide.Left;

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.CurrentGameState != GameManager.GameState.Playing)
            return;

        if (gm.CurrentScrollSpeed <= 0f)
            return;

        bool sideways = gm.HeadingDegrees >= 90f;

        // Compute once, reuse everywhere
        float speedPercent =
            Mathf.InverseLerp(gm.minSpeed, gm.maxSpeed, gm.CurrentScrollSpeed);

        // =============================
        // VIRTUAL LATERAL DRIFT
        // =============================
        float lateralIntent = gm.LateralFactor * gm.HeadingDirection;

        if (Mathf.Abs(lateralIntent) >= lateralDeadzone)
            virtualPlayerX += lateralIntent * lateralDriftSpeed * gm.baseScrollSpeed * Time.deltaTime;
        else
            virtualPlayerX = Mathf.MoveTowards(
                virtualPlayerX,
                0f,
                lateralReturnSpeed * gm.baseScrollSpeed * Time.deltaTime
            );

        virtualPlayerX = Mathf.Clamp(virtualPlayerX, -maxVirtualX, maxVirtualX);
        float absX = Mathf.Abs(virtualPlayerX);

        // =============================
        // SLALOM STATE (YOU WERE MISSING THIS BLOCK)
        // =============================
        if (!slalomActive && absX >= slalomActivationX)
        {
            slalomActive = true;
            slalomDistanceAccumulator = 0f;
            nextSlalomSide = SlalomSide.Left;
        }
        else if (slalomActive && absX <= slalomDeactivateX)
        {
            slalomActive = false;
            slalomDistanceAccumulator = 0f;
        }

        // =============================
        // SLALOM SPAWNING (DISTANCE-BASED)
        // =============================
        if (slalomActive)
        {
            slalomDistanceAccumulator += gm.CurrentScrollSpeed * Time.deltaTime;

            float metersPerFlag = Mathf.Lerp(12f, 7.5f, speedPercent);

            while (slalomDistanceAccumulator >= metersPerFlag)
            {
                slalomDistanceAccumulator -= metersPerFlag;
                SpawnSlalomFlag();
            }
        }

        // =============================
        // DISTANCE-BASED OBSTACLE SPAWNING
        // =============================
        if (sideways)
            return;

        distanceAccumulator += gm.CurrentScrollSpeed * Time.deltaTime;

        float metersPerSpawn = Mathf.Lerp(6f, 4f, speedPercent);

        while (distanceAccumulator >= metersPerSpawn)
        {
            distanceAccumulator -= metersPerSpawn;
            SpawnBeat();
        }
    }

    // =============================
    // SPAWN BEAT (single or clump)
    // =============================
    void SpawnBeat()
    {
        float x = Random.Range(-spawnWidth, spawnWidth);
        Vector3 center = new Vector3(x, spawnY, 0f);

        GameObject prefab = ChooseObstacle();

        bool canClump =
            prefab == treePrefab ||
            prefab == mogulPrefab;

        if (canClump && Random.value < clumpChance)
        {
            int count = Random.Range(clumpMinCount, clumpMaxCount + 1);

            SpawnUtility.SpawnClump(
                prefab,
                center,
                count,
                clumpSpreadX,
                clumpSpreadY
            );
        }
        else
        {
            Instantiate(prefab, center, Quaternion.identity);
        }
    }

    void SpawnSlalomFlag()
    {
        GameObject prefab =
            nextSlalomSide == SlalomSide.Left
                ? leftFlagPrefab
                : rightFlagPrefab;

        float x =
            nextSlalomSide == SlalomSide.Left
                ? slalomCenterX - slalomOffsetX
                : slalomCenterX + slalomOffsetX;

        Instantiate(prefab, new Vector3(x, slalomSpawnY, 0f), Quaternion.identity);

        nextSlalomSide =
            nextSlalomSide == SlalomSide.Left
                ? SlalomSide.Right
                : SlalomSide.Left;
    }

    GameObject ChooseObstacle()
    {
        float roll = Random.value;
        if (roll < 0.45f) return treePrefab;
        if (roll < 0.65f) return mogulPrefab;
        if (roll < 0.8f) return rockPrefab;
        if (roll < 0.9f) return stumpPrefab;
        return jumpPadPrefab;
    }
}
// ----- Spawner.cs END -----
