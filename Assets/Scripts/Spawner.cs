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
    public float slalomSpawnInterval = 1f;

    [Header("Virtual Lateral Drift")]
    public float lateralDriftSpeed = 2f;
    public float lateralDeadzone = 0.05f;
    public float lateralReturnSpeed = 0.6f;
    public float maxVirtualX = 8f;

    [Header("Normal Spawning")]
    public float spawnRate = 1f;
    public float spawnWidth = 6f;
    public float spawnY = -6f;

    float obstacleTimer;
    float slalomTimer;
    float virtualPlayerX;
    bool slalomActive;

    const float slalomCenterX = 0f;
    SlalomSide nextSlalomSide = SlalomSide.Left;

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.CurrentGameState != GameManager.GameState.Playing)
            return;

        bool sideways = gm.HeadingDegrees >= 90f;

        float lateralIntent = gm.LateralFactor * gm.HeadingDirection;

        if (Mathf.Abs(lateralIntent) >= lateralDeadzone)
            virtualPlayerX += lateralIntent * lateralDriftSpeed * gm.baseScrollSpeed * Time.deltaTime;
        else
            virtualPlayerX = Mathf.MoveTowards(
                virtualPlayerX, 0f,
                lateralReturnSpeed * gm.baseScrollSpeed * Time.deltaTime
            );

        virtualPlayerX = Mathf.Clamp(virtualPlayerX, -maxVirtualX, maxVirtualX);

        float absX = Mathf.Abs(virtualPlayerX);

        if (!slalomActive && absX >= slalomActivationX)
        {
            slalomActive = true;
            slalomTimer = 0f;
            nextSlalomSide = SlalomSide.Left;
        }
        else if (slalomActive && absX <= slalomDeactivateX)
        {
            slalomActive = false;
        }

        if (slalomActive)
        {
            slalomTimer += Time.deltaTime;
            if (slalomTimer >= slalomSpawnInterval)
            {
                slalomTimer -= slalomSpawnInterval;
                SpawnSlalomFlag();
            }
        }

        if (!sideways)
        {
            obstacleTimer += Time.deltaTime;
            if (obstacleTimer >= spawnRate)
            {
                obstacleTimer -= spawnRate;
                SpawnObstacle();
            }
        }
    }

    void SpawnSlalomFlag()
    {
        GameObject prefab = nextSlalomSide == SlalomSide.Left ? leftFlagPrefab : rightFlagPrefab;
        float x = nextSlalomSide == SlalomSide.Left ? slalomCenterX - slalomOffsetX : slalomCenterX + slalomOffsetX;

        Instantiate(prefab, new Vector3(x, slalomSpawnY, 0f), Quaternion.identity);
        nextSlalomSide = nextSlalomSide == SlalomSide.Left ? SlalomSide.Right : SlalomSide.Left;
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
