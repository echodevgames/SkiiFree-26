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

    [Tooltip("How far from center before slalom starts")]
    public float slalomActivationX = 2.5f;

    [Tooltip("How close to center before slalom stops")]
    public float slalomDeactivateX = 1.0f;

    [Tooltip("Offset from slalom center line")]
    public float slalomOffsetX = 2.5f;

    [Tooltip("Spawn Y ahead of camera")]
    public float slalomSpawnY = -10f;

    [Tooltip("Time between slalom flags")]
    public float slalomSpawnInterval = 1.25f;

    [Header("Virtual Lateral Drift")]
    public float lateralDriftSpeed = 1.0f;
    public float recenterSpeed = 0.6f;

    [Header("Normal Spawning")]
    public float spawnRate = 1f;
    public float spawnWidth = 6f;
    public float spawnY = -6f;

    float obstacleTimer;
    float slalomTimer;

    // 🔑 Virtual lateral position (NOT player transform)
    float virtualPlayerX;

    bool slalomActive;
    float slalomCenterX;
    SlalomSide nextSlalomSide = SlalomSide.Left;

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null)
            return;

        // =============================
        // VIRTUAL LATERAL DRIFT
        // =============================
        float lateralIntent =
            gm.LateralFactor *
            gm.HeadingDirection; // -1 → +1 intent

        // Steering drift
        virtualPlayerX +=
            lateralIntent *
            lateralDriftSpeed *
            gm.CurrentScrollSpeed *
            Time.deltaTime;

        // Natural recentering when not steering
        if (Mathf.Abs(lateralIntent) < 0.05f)
        {
            virtualPlayerX = Mathf.MoveTowards(
                virtualPlayerX,
                0f,
                recenterSpeed * gm.CurrentScrollSpeed * Time.deltaTime
            );
        }

        float absX = Mathf.Abs(virtualPlayerX);

        // =============================
        // SLALOM STATE
        // =============================
        if (!slalomActive && absX >= slalomActivationX)
        {
            slalomActive = true;
            slalomCenterX = virtualPlayerX;
            slalomTimer = 0f;
            nextSlalomSide = SlalomSide.Left;

            Debug.Log($"[SLALOM] Activated at X={slalomCenterX:F2}");
        }
        else if (slalomActive && absX <= slalomDeactivateX)
        {
            slalomActive = false;
            Debug.Log("[SLALOM] Deactivated");
        }

        // =============================
        // SLALOM SPAWNING
        // =============================
        if (slalomActive)
        {
            slalomTimer += Time.deltaTime;
            if (slalomTimer >= slalomSpawnInterval)
            {
                slalomTimer = 0f;
                SpawnSlalomFlag();
            }
            return;
        }

        // =============================
        // NORMAL OBSTACLES
        // =============================
        obstacleTimer += Time.deltaTime;
        if (obstacleTimer >= spawnRate)
        {
            obstacleTimer = 0f;
            SpawnObstacle();
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

        GameObject flag = Instantiate(
            prefab,
            new Vector3(x, slalomSpawnY, 0f),
            Quaternion.identity
        );

        flag.GetComponent<SlalomFlag>().requiredSide = nextSlalomSide;

        // STRICT alternation (design rule)
        nextSlalomSide =
            nextSlalomSide == SlalomSide.Left
                ? SlalomSide.Right
                : SlalomSide.Left;
    }

    void SpawnObstacle()
    {
        float x = Random.Range(-spawnWidth, spawnWidth);
        Instantiate(
            ChooseObstacle(),
            new Vector3(x, spawnY, 0f),
            Quaternion.identity
        );
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
