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

    [Tooltip("How far from center before slalom starts (virtual distance)")]
    public float slalomActivationX = 3.25f;     // more pronounced default

    [Tooltip("How close to center before slalom stops (virtual distance)")]
    public float slalomDeactivateX = 1.25f;     // more pronounced default

    [Tooltip("Offset from slalom center line (world center = 0)")]
    public float slalomOffsetX = 3.5f;          // more pronounced default

    [Tooltip("Spawn Y ahead of camera")]
    public float slalomSpawnY = -10f;

    [Tooltip("Time between slalom flags")]
    public float slalomSpawnInterval = 1.0f;    // slightly faster, feels consistent

    [Header("Virtual Lateral Drift")]
    [Tooltip("How fast virtual X accumulates when steering")]
    public float lateralDriftSpeed = 2.0f;      // more pronounced default

    [Tooltip("Deadzone for steering input (prevents tiny noise)")]
    public float lateralDeadzone = 0.05f;

    [Header("Normal Spawning")]
    public float spawnRate = 1f;
    public float spawnWidth = 6f;
    public float spawnY = -6f;

    [Header("Debug")]
    public bool debugSlalom = false;

    float obstacleTimer;
    float slalomTimer;

    // Virtual lateral "progress" from center. Only changes when you steer.
    float virtualPlayerX;

    bool slalomActive;

    // IMPORTANT: slalom center is always world center (0)
    const float slalomCenterX = 0f;

    SlalomSide nextSlalomSide = SlalomSide.Left;

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null)
            return;

        if (gm.CurrentGameState != GameManager.GameState.Playing)
            return;

        // =============================
        // VIRTUAL LATERAL DRIFT (NO AUTO-RECENTER)
        // =============================
        float lateralIntent = gm.LateralFactor * gm.HeadingDirection; // -1..+1

        // Apply deadzone so tiny intent doesn't "creep"
        if (Mathf.Abs(lateralIntent) >= lateralDeadzone)
        {
            // Use baseScrollSpeed so crashes/speed ramps don't change the feel
            float drift = lateralIntent * lateralDriftSpeed * gm.baseScrollSpeed * Time.deltaTime;
            virtualPlayerX += drift;
        }

        float absX = Mathf.Abs(virtualPlayerX);

        // =============================
        // SLALOM STATE (distance-based)
        // =============================
        if (!slalomActive && absX >= slalomActivationX)
        {
            slalomActive = true;
            slalomTimer = 0f;
            nextSlalomSide = SlalomSide.Left;

            if (debugSlalom)
                Debug.Log($"[SLALOM] Activated | virtualX={virtualPlayerX:F2}");
        }
        else if (slalomActive && absX <= slalomDeactivateX)
        {
            slalomActive = false;

            if (debugSlalom)
                Debug.Log($"[SLALOM] Deactivated | virtualX={virtualPlayerX:F2}");
        }

        if (debugSlalom)
        {
            Debug.Log($"[SLALOM] active={slalomActive} virtualX={virtualPlayerX:F2} abs={absX:F2}");
        }

        // =============================
        // SLALOM SPAWNING
        // =============================
        if (slalomActive)
        {
            slalomTimer += Time.deltaTime;
            if (slalomTimer >= slalomSpawnInterval)
            {
                slalomTimer -= slalomSpawnInterval; // avoids drift / missed frames
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
            obstacleTimer -= spawnRate;
            SpawnObstacle();
        }
    }

    void SpawnSlalomFlag()
    {
        if (leftFlagPrefab == null || rightFlagPrefab == null)
            return;

        GameObject prefab =
            nextSlalomSide == SlalomSide.Left
                ? leftFlagPrefab
                : rightFlagPrefab;

        // Design rule:
        // Left flag MUST be left of center line.
        // Right flag MUST be right of center line.
        float x =
            nextSlalomSide == SlalomSide.Left
                ? slalomCenterX - slalomOffsetX
                : slalomCenterX + slalomOffsetX;

        GameObject flag = Instantiate(
            prefab,
            new Vector3(x, slalomSpawnY, 0f),
            Quaternion.identity
        );

        var sf = flag.GetComponent<SlalomFlag>();
        if (sf != null)
            sf.requiredSide = nextSlalomSide;

        // STRICT alternation (design rule)
        nextSlalomSide =
            nextSlalomSide == SlalomSide.Left
                ? SlalomSide.Right
                : SlalomSide.Left;
    }

    void SpawnObstacle()
    {
        float x = Random.Range(-spawnWidth, spawnWidth);
        Instantiate(ChooseObstacle(), new Vector3(x, spawnY, 0f), Quaternion.identity);
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
