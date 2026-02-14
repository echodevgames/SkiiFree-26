// ----- GameManager.cs START -----

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // ================== WORLD SPEED ==================
    [Header("World Speed")]
    public float baseScrollSpeed = 5f;

    [Header("Speed Ramp")]
    public float minSpeed = 4.5f;
    public float maxSpeed = 12f;
    public float speedRampPerSecond = 0.15f;
    public float speedLerp = 4f;

    float targetSpeed;
    float currentSpeed;
    float smoothedHeadingPenalty = 1f;

    public float DisplaySpeed => IsCrashed ? 0f : currentSpeed;

    // ================== SCOOT ==================
    // Holding SAME direction while fully sideways generates periodic scoot "bumps".
    // OPPOSITE direction starts turning back downhill.
    public float ScootIntent { get; private set; }   // -1, 0, +1
    public bool ScootBump { get; private set; }      // true for one frame when a bump fires

    [Header("Scoot Tuning")]
    [Tooltip("Seconds between scoot bumps when holding direction at 90°")]
    public float scootBumpInterval = 0.14f;

    // internal timer for bump cadence
    float scootBumpTimer;

    // ================== HEADING ==================
    [Header("Heading (degrees)")]
    public float[] headingSteps = { 0f, 15f, 45f, 65f, 90f };
    public int HeadingIndex { get; private set; }
    public int HeadingDirection { get; private set; }
    public float HeadingDegrees => headingSteps[HeadingIndex];

    // ================== BOUNCE ==================
    [Header("Bounce")]
    public bool JustBounced { get; private set; }
    float bounceVisualTimer;
    public float bounceVisualDuration = 0.15f;

    // ================== JUMP ==================
    [Header("Jump Tuning")]
    public float normalJumpDuration = 0.8f;
    public float highJumpDuration = 1.3f;
    public float rockBounceExtraTime = 0.35f;
    public float stumpBounceExtraTime = 0.45f;

    float jumpTimer;

    // ================== SPIN ==================
    [Header("Spin")]
    [Tooltip("Degrees per second while holding spin input")]
    public float spinDegreesPerSecond = 360f;

    [Tooltip("Spin sprites in clockwise order starting at 0°")]
    public SpinFrame[] spinFrames;

    float spinAngle;
    float accumulatedSpinAngle;
    public int SpinFrameIndex { get; private set; }

    [Header("Spin Debug")]
    public bool logSpinDebug = true;

    // ================== TIME & SCORE ==================
    [Header("Time")]
    public float RunTime { get; private set; }

    [Header("Speed Scoring")]
    public float SpeedScore { get; private set; }
    [Tooltip("Score gained per second at max speed")]
    public float maxSpeedScoreRate = 50f;

    [Header("Style Scoring")]
    public float StyleScore { get; private set; }
    [Tooltip("Points per full spin")]
    public float pointsPerSpin = 250f;
    [Tooltip("Bonus for clean landing")]
    public float cleanLandingBonus = 150f;

    [Tooltip("How many points per second just for staying alive")]
    public float timeScoreRate = 10f;

    public int TotalScore =>
        Mathf.FloorToInt(SpeedScore + StyleScore + (RunTime * timeScoreRate));

    // ================== STATE ==================
    int cachedHeadingIndex;
    int cachedHeadingDirection;

    float speedMultiplier = 1f;
    float speedMultTimer;

    [Header("Landing Rules")]
    public float cleanLandingMaxAngle = 20f;
    public float sloppyLandingMaxAngle = 45f;
    public float sloppyLandingSlow = 0.6f;
    public float sloppyLandingDuration = 0.6f;

    public enum GameState { Menu, Playing }
    public GameState CurrentGameState { get; private set; } = GameState.Menu;

    public enum JumpState { Grounded, Jumping, HighJump }
    public JumpState CurrentJumpState { get; private set; } = JumpState.Grounded;

    public bool IsCrashed { get; private set; }

    public float CurrentScrollSpeed => currentSpeed;
    public float LateralFactor => HeadingDegrees / 90f;

    // ================== SCORE API ==================
    public void AddStyleScore(float amount) => StyleScore += amount;
    public void AddSpeedScore(float amount) => SpeedScore += amount;

    // ================== UNITY ==================
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (CurrentGameState != GameState.Playing || IsCrashed)
            return;

        // reset each frame
        ScootIntent = 0f;
        ScootBump = false;

        // ---------- TIME ----------
        RunTime += Time.deltaTime;

        // ---------- INPUT ----------
        if (CurrentJumpState == JumpState.Grounded)
        {
            bool leftDown = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
            bool rightDown = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);

            bool leftHeld = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
            bool rightHeld = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

            bool fullySideways = HeadingDegrees >= 90f;

            if (!fullySideways)
            {
                // leaving sideways -> reset scoot cadence
                scootBumpTimer = 0f;

                if (leftDown) StepHeading(-1);
                if (rightDown) StepHeading(1);
            }
            else
            {
                // Fully sideways: opposite press turns downhill.
                // Holding same direction produces periodic bumps.

                if (HeadingDirection >= 0)
                {
                    // sideways-right
                    if (leftDown)
                    {
                        // turn back downhill
                        scootBumpTimer = 0f;
                        StepHeading(-1);
                    }
                    else if (rightHeld)
                    {
                        ScootIntent = 1f;
                        TickScootBump();
                    }
                    else
                    {
                        scootBumpTimer = 0f;
                    }
                }
                else
                {
                    // sideways-left
                    if (rightDown)
                    {
                        scootBumpTimer = 0f;
                        StepHeading(1);
                    }
                    else if (leftHeld)
                    {
                        ScootIntent = -1f;
                        TickScootBump();
                    }
                    else
                    {
                        scootBumpTimer = 0f;
                    }
                }
            }
        }
        else
        {
            // In air: spin input
            float spinInput =
                (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) ? 1f : 0f) -
                (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) ? 1f : 0f);

            if (spinInput != 0f)
                ApplySpin(spinInput);

            scootBumpTimer = 0f;
        }

        // ---------- SPEED ----------
        if (speedMultTimer > 0f)
        {
            speedMultTimer -= Time.deltaTime;
            if (speedMultTimer <= 0f)
                speedMultiplier = 1f;
        }

        targetSpeed = Mathf.Clamp(
            targetSpeed + speedRampPerSecond * Time.deltaTime,
            minSpeed,
            maxSpeed
        );

        float rawPenalty = 1f - (HeadingDegrees / 90f);
        smoothedHeadingPenalty = Mathf.Lerp(
            smoothedHeadingPenalty,
            rawPenalty,
            Time.deltaTime * speedLerp
        );

        currentSpeed = Mathf.Lerp(
            currentSpeed,
            targetSpeed * smoothedHeadingPenalty * speedMultiplier,
            Time.deltaTime * speedLerp
        );

        // ---------- SPEED SCORE ----------
        float speedPercent = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);
        SpeedScore += speedPercent * maxSpeedScoreRate * Time.deltaTime;

        // ---------- JUMP TIMER ----------
        if (CurrentJumpState != JumpState.Grounded)
        {
            jumpTimer -= Time.deltaTime;

            if (JustBounced)
            {
                bounceVisualTimer -= Time.deltaTime;
                if (bounceVisualTimer <= 0f)
                    JustBounced = false;
            }

            if (jumpTimer <= 0f)
                EndJump();

            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            StartJump();
    }

    void TickScootBump()
    {
        scootBumpTimer += Time.deltaTime;

        if (scootBumpTimer >= scootBumpInterval)
        {
            // allow "catch up" but don’t spam multiple bumps in one frame
            scootBumpTimer -= scootBumpInterval;
            ScootBump = true;
        }
    }

    // ================== GAME FLOW ==================
    public void StartGame()
    {
        Time.timeScale = 1f;

        CurrentGameState = GameState.Playing;
        HeadingIndex = 0;
        HeadingDirection = 0;

        spinAngle = 0f;
        accumulatedSpinAngle = 0f;
        SpinFrameIndex = 0;

        CurrentJumpState = JumpState.Grounded;
        IsCrashed = false;

        speedMultiplier = 1f;
        targetSpeed = minSpeed;
        currentSpeed = minSpeed;
        smoothedHeadingPenalty = 1f;

        ScootIntent = 0f;
        ScootBump = false;
        scootBumpTimer = 0f;

        RunTime = 0f;
        SpeedScore = 0f;
        StyleScore = 0f;
    }

    void StepHeading(int dir)
    {
        if (HeadingIndex == 0)
        {
            HeadingDirection = dir;
            HeadingIndex = 1;
            return;
        }

        HeadingIndex += (HeadingDirection == dir) ? 1 : -1;
        HeadingIndex = Mathf.Clamp(HeadingIndex, 0, headingSteps.Length - 1);

        if (HeadingIndex == 0)
            HeadingDirection = 0;
    }

    void ApplySpin(float direction)
    {
        if (spinFrames == null || spinFrames.Length == 0)
            return;

        float delta = direction * spinDegreesPerSecond * Time.deltaTime;

        accumulatedSpinAngle += delta;
        spinAngle = Mathf.Repeat(accumulatedSpinAngle, 360f);

        float degreesPerFrame = 360f / spinFrames.Length;
        SpinFrameIndex = Mathf.FloorToInt(spinAngle / degreesPerFrame);
        SpinFrameIndex = Mathf.Clamp(SpinFrameIndex, 0, spinFrames.Length - 1);
    }

    public void StartJump()
    {
        cachedHeadingIndex = HeadingIndex;
        cachedHeadingDirection = HeadingDirection;

        HeadingIndex = 0;
        HeadingDirection = 0;

        spinAngle = 0f;
        accumulatedSpinAngle = 0f;
        SpinFrameIndex = 0;

        CurrentJumpState = JumpState.Jumping;
        jumpTimer = normalJumpDuration;
    }

    public void StartHighJump()
    {
        StartJump();
        jumpTimer = highJumpDuration;
    }

    public void Bounce(float extraTime)
    {
        if (CurrentJumpState == JumpState.Grounded)
            return;

        jumpTimer += extraTime;
        JustBounced = true;
        bounceVisualTimer = bounceVisualDuration;
    }

    void EndJump()
    {
        CurrentJumpState = JumpState.Grounded;

        HeadingIndex = cachedHeadingIndex;
        HeadingDirection = cachedHeadingDirection;

        spinAngle = 0f;
        accumulatedSpinAngle = 0f;
        SpinFrameIndex = 0;
    }

    public void TriggerSlow(float multiplier, float duration)
    {
        speedMultiplier = Mathf.Clamp(multiplier, 0.05f, 1f);
        speedMultTimer = Mathf.Max(speedMultTimer, duration);
    }

    public void TriggerCrash(float duration)
    {
        if (IsCrashed)
            return;

        IsCrashed = true;
        SpeedScore *= 0.5f;

        ResetSpeedRamp();
        Invoke(nameof(RecoverFromCrash), duration);
    }

    void RecoverFromCrash() => IsCrashed = false;

    void ResetSpeedRamp()
    {
        targetSpeed = minSpeed;
        currentSpeed = minSpeed;
        speedMultiplier = 1f;
        speedMultTimer = 0f;
    }
}

// ----- GameManager.cs END -----
