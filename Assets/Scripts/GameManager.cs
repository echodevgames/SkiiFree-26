// ----- GameManager.cs START -----



using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("World Speed")]
    public float baseScrollSpeed = 5f;

    [Header("Heading (degrees)")]
    public float[] headingSteps = { 0f, 15f, 45f, 65f, 90f };
    public int HeadingIndex { get; private set; }
    public int HeadingDirection { get; private set; }
    public float HeadingDegrees => headingSteps[HeadingIndex];

    [Header("Bounce")]
    public bool JustBounced { get; private set; }
    float bounceVisualTimer;
    public float bounceVisualDuration = 0.15f;

    [Header("Jump Tuning")]
    public float normalJumpDuration = 0.8f;
    public float highJumpDuration = 1.3f;
    public float rockBounceExtraTime = 0.35f;
    public float stumpBounceExtraTime = 0.45f;

    [Header("Spin")]
    public int SpinFrameIndex { get; private set; }
    public int spinFrameCount = 12;

    int cachedHeadingIndex;
    int cachedHeadingDirection;

    float speedMultiplier = 1f;
    float speedMultTimer;

    public enum GameState { Menu, Playing }
    public GameState CurrentGameState { get; private set; } = GameState.Menu;

    public enum JumpState { Grounded, Jumping, HighJump }
    public JumpState CurrentJumpState { get; private set; } = JumpState.Grounded;

    float jumpTimer;
    public bool IsCrashed { get; private set; }

    public float CurrentScrollSpeed
    {
        get
        {
            float speed = baseScrollSpeed;

            if (CurrentJumpState == JumpState.Grounded)
            {
                float factor = 1f - (HeadingDegrees / 90f);
                speed *= factor;
            }

            return speed * speedMultiplier;
        }
    }

    public float LateralFactor => HeadingDegrees / 90f;

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

        // TURN INPUT
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            StepHeading(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            StepHeading(1);

        // JUMP STATE
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

        // START JUMP
        if (Input.GetKeyDown(KeyCode.Space))
            StartJump();

        // SLOW TIMER
        if (speedMultTimer > 0f)
        {
            speedMultTimer -= Time.deltaTime;
            if (speedMultTimer <= 0f)
                speedMultiplier = 1f;
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;   // ✅ THIS WAS MISSING

        CurrentGameState = GameState.Playing;
        HeadingIndex = 0;
        HeadingDirection = 0;
        SpinFrameIndex = 0;
        CurrentJumpState = JumpState.Grounded;
        IsCrashed = false;
        speedMultiplier = 1f;
    }


    void StepHeading(int dir)
    {
        if (HeadingIndex == 0)
        {
            HeadingDirection = dir;
            HeadingIndex = 1;
            return;
        }

        if (HeadingDirection == dir)
            HeadingIndex++;
        else
            HeadingIndex--;

        HeadingIndex = Mathf.Clamp(HeadingIndex, 0, headingSteps.Length - 1);

        if (HeadingIndex == 0)
            HeadingDirection = 0;
    }

    public void TriggerCrash(float duration)
    {
        if (IsCrashed)
            return;

        IsCrashed = true;
        Invoke(nameof(RecoverFromCrash), duration);
    }

    void RecoverFromCrash()
    {
        IsCrashed = false;
    }

    public void StartJump()
    {
        cachedHeadingIndex = HeadingIndex;
        cachedHeadingDirection = HeadingDirection;
        CurrentJumpState = JumpState.Jumping;
        jumpTimer = normalJumpDuration;
    }

    public void StartHighJump()
    {
        cachedHeadingIndex = HeadingIndex;
        cachedHeadingDirection = HeadingDirection;
        CurrentJumpState = JumpState.HighJump;
        jumpTimer = highJumpDuration;
    }

    public void Bounce(float extraTime)
    {
        jumpTimer += extraTime;
        JustBounced = true;
        bounceVisualTimer = bounceVisualDuration;
    }

    void EndJump()
    {
        CurrentJumpState = JumpState.Grounded;
        HeadingIndex = cachedHeadingIndex;
        HeadingDirection = cachedHeadingDirection;
    }

    public void TriggerSlow(float multiplier, float duration)
    {
        speedMultiplier = Mathf.Clamp(multiplier, 0.05f, 1f);
        speedMultTimer = Mathf.Max(speedMultTimer, duration);
    }
}

// ----- GameManager.cs END -----

