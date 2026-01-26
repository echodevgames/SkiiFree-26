using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("World Speed")]
    public float baseScrollSpeed = 5f;

    [Header("Heading (degrees)")]
    public float[] headingSteps = { 0f, 15f, 45f, 65f, 90f };
    public int HeadingIndex { get; private set; } = 0;
    public int HeadingDirection { get; private set; } = 0; // -1 left, +1 right
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

    public enum GameState
    {
        Menu,
        Playing
    }
    [Header("Menu")]
    public GameState CurrentGameState { get; private set; } = GameState.Menu;


    public enum JumpState
    {
        Grounded,
        Jumping,
        HighJump
    }
    [Header("Jump")]
    public JumpState CurrentJumpState { get; private set; } = JumpState.Grounded;

    float jumpTimer;
    
    public bool IsCrashed { get; private set; }


    public float CurrentScrollSpeed
    {
        get
        {
            // While jumping, ignore heading slowdown
            if (CurrentJumpState != JumpState.Grounded)
                return baseScrollSpeed;

            float factor = 1f - (HeadingDegrees / 90f);
            return baseScrollSpeed * factor;
        }
    }


    public float LateralFactor => HeadingDegrees / 90f;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // === SIDEWAYS LOCK STATE ===
        bool fullySideways = HeadingDegrees >= 90f;

        if (CurrentGameState != GameState.Playing)
            return;


        if (IsCrashed)
            return;

        // === NORMAL TURNING ===
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            StepHeading(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            StepHeading(1);

        if (CurrentJumpState != JumpState.Grounded)
        {
            jumpTimer -= Time.deltaTime;

            if (JustBounced)
            {
                bounceVisualTimer -= Time.deltaTime;
                if (bounceVisualTimer <= 0f)
                    JustBounced = false;
            }





            // DISCRETE SPIN INPUT (SkiFree-style)
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                AdvanceSpin(-1);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                AdvanceSpin(1);
            }


            if (jumpTimer <= 0f)
                EndJump();

            return; // block steering
        }

        // START JUMP
        if (CurrentJumpState == JumpState.Grounded &&
            Input.GetKeyDown(KeyCode.Space))
        {
            StartJump();
        }


        // DOWN always reduces heading (escape sideways)
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (HeadingIndex > 0)
                HeadingIndex--;

            if (HeadingIndex == 0)
                HeadingDirection = 0;

            return; // prevent other input this frame
        }

        // If fully sideways, special handling
        if (fullySideways)
        {
            // LEFT key pressed
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                if (HeadingDirection == -1)
                {
                    // Scoot LEFT (world moves right)
                    ScootWorld(-1, 0.4f);
                }
                else
                {
                    // Recover heading (same as DOWN)
                    HeadingIndex--;
                }
            }

            // RIGHT key pressed
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                if (HeadingDirection == 1)
                {
                    // Scoot RIGHT (world moves left)
                    ScootWorld(1, 0.4f);
                }
                else
                {
                    // Recover heading (same as DOWN)
                    HeadingIndex--;
                }
            }

            // Clamp + cleanup
            HeadingIndex = Mathf.Clamp(HeadingIndex, 0, headingSteps.Length - 1);

            if (HeadingIndex == 0)
                HeadingDirection = 0;

            return;
        }



    }
    public void StartGame()
    {
        CurrentGameState = GameState.Playing;

        Time.timeScale = 1f;   // Resume time

        // Reset run state
        HeadingIndex = 0;
        HeadingDirection = 0;
        SpinFrameIndex = 0;
        CurrentJumpState = JumpState.Grounded;
        IsCrashed = false;

        Debug.Log("Game Started");
    }

    void StepHeading(int dir)
    {
        // Start turning
        if (HeadingIndex == 0)
        {
            HeadingDirection = dir;
            HeadingIndex = 1;
            return;
        }

        // Same direction → turn further sideways
        if (HeadingDirection == dir)
        {
            HeadingIndex++;
        }
        // Opposite direction → turn back toward downhill
        else
        {
            HeadingIndex--;
        }

        HeadingIndex = Mathf.Clamp(HeadingIndex, 0, headingSteps.Length - 1);

        if (HeadingIndex == 0)
            HeadingDirection = 0;
    }

    void ScootWorld(int dir, float distance)
    {
        foreach (var obs in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            obs.transform.position += Vector3.right * dir * distance;
        }
    }

    public void TriggerCrash(float duration = 1.0f)
    {
        if (IsCrashed)
            return;

        IsCrashed = true;
        Invoke(nameof(RecoverFromCrash), duration);
    }

    public void StartJump()
    {
        cachedHeadingIndex = HeadingIndex;
        cachedHeadingDirection = HeadingDirection;

        CurrentJumpState = JumpState.Jumping;
        SpinFrameIndex = 0;
        jumpTimer = normalJumpDuration;
    }

    public void StartHighJump()
    {
        cachedHeadingIndex = HeadingIndex;
        cachedHeadingDirection = HeadingDirection;

        CurrentJumpState = JumpState.HighJump;
        SpinFrameIndex = 0;
        jumpTimer = highJumpDuration;
    }

    public void Bounce(float extraTime)
    {
        if (CurrentJumpState != JumpState.Grounded)
        {
            jumpTimer += extraTime;
            JustBounced = true;
            bounceVisualTimer = bounceVisualDuration;
            return;
        }

        StartJump();
    }


    void EndJump()
    {
        CurrentJumpState = JumpState.Grounded;

        HeadingIndex = cachedHeadingIndex;
        HeadingDirection = cachedHeadingDirection;

        SpinFrameIndex = 0;
    }


    void RecoverFromCrash()
    {
        IsCrashed = false;
    }
    void AdvanceSpin(int dir)
    {
        SpinFrameIndex += dir;

        int maxFrames = spinFrameCount; // set this via inspector or const

        if (SpinFrameIndex < 0)
            SpinFrameIndex += maxFrames;

        if (SpinFrameIndex >= maxFrames)
            SpinFrameIndex -= maxFrames;
    }




}
