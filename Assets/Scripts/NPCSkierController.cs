// ----- NPCSkierController.cs START -----
using UnityEngine;

public class NPCSkierController : MonoBehaviour
{
    [Header("Downhill Movement")]
    public float baseDownhillSpeed = 3.5f;
    public float speedVariance = 0.75f;

    [Header("Lateral Behavior")]
    public float maxLateralSpeed = 2.2f;
    public float steeringResponsiveness = 3.5f;

    [Header("Human Imperfection")]
    public float microJitterStrength = 0.25f;
    public float microJitterSpeed = 3f;

    [Header("Decision Making")]
    public float retargetIntervalMin = 0.6f;
    public float retargetIntervalMax = 1.8f;
    public float maxTargetOffset = 2.5f;

    float downhillSpeed;
    float targetLateral;
    float currentLateral;
    float retargetTimer;
    float jitterSeed;

    void Start()
    {
        downhillSpeed =
            baseDownhillSpeed +
            Random.Range(-speedVariance, speedVariance);

        PickNewTarget();

        jitterSeed = Random.Range(0f, 1000f);
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.CurrentGameState != GameManager.GameState.Playing)
            return;


        // ------------------------------
        // Retarget lateral intent
        // ------------------------------
        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f)
            PickNewTarget();

        // ------------------------------
        // Smooth steering toward target
        // ------------------------------
        currentLateral = Mathf.MoveTowards(
            currentLateral,
            targetLateral,
            steeringResponsiveness * Time.deltaTime
        );

        // ------------------------------
        // Micro jitter (NOT sinusoidal)
        // ------------------------------
        float jitter =
            (Mathf.PerlinNoise(jitterSeed, Time.time * microJitterSpeed) - 0.5f)
            * microJitterStrength;

        float horizontal =
            currentLateral + jitter;

        // ------------------------------
        // World-relative movement
        // ------------------------------
        float vertical =
            downhillSpeed +
            gm.CurrentScrollSpeed * 0.15f; // subtle sync with world

        Vector3 move =
            Vector3.down * vertical * Time.deltaTime +
            Vector3.right * horizontal * Time.deltaTime;

        transform.position += move;
    }

    void PickNewTarget()
    {
        targetLateral = Random.Range(-maxLateralSpeed, maxLateralSpeed);

        retargetTimer = Random.Range(
            retargetIntervalMin,
            retargetIntervalMax
        );
    }
}
// ----- NPCSkierController.cs END -----
