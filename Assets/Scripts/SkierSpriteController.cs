// ----- SkierSpriteController.cs START -----

using UnityEngine;

public class SkierSpriteController : MonoBehaviour
{
    [Header("Grounded Direction Sprites")]
    public Sprite centerSprite;
    public Sprite[] rightTurnSprites; // ordered outward from center
    public Sprite[] leftTurnSprites;  // ordered outward from center

    [Header("Scoot Sprites")]
    public Sprite rightScootSprite;
    public Sprite leftScootSprite;

    [Tooltip("How long the scoot sprite shows per bump")]
    public float scootSpriteDuration = 0.07f;

    [Header("Spin Sprites (Angle-Labeled)")]
    public SpinFrame[] spinFrames;

    [Header("Special Sprites")]
    public Sprite crashSprite;
    public Sprite bounceSprite;

    SpriteRenderer sr;

    float scootSpriteTimer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null)
            return;

        // 💥 CRASH
        if (gm.IsCrashed)
        {
            sr.sprite = crashSprite;
            return;
        }

        // 🌀 AIR / SPIN
        if (gm.CurrentJumpState != GameManager.JumpState.Grounded)
        {
            if (gm.JustBounced && bounceSprite != null)
            {
                sr.sprite = bounceSprite;
            }
            else
            {
                sr.sprite = GetSpinSprite(gm.SpinFrameIndex);
            }
            return;
        }

        // ⛷️ SCOOT BUMP TRIGGER
        if (gm.ScootBump)
        {
            scootSpriteTimer = scootSpriteDuration;
        }


        // ⛷️ SCOOT SPRITE DISPLAY
        if (scootSpriteTimer > 0f)
        {
            scootSpriteTimer -= Time.deltaTime;

            if (gm.HeadingDirection > 0 && rightScootSprite != null)
            {
                sr.sprite = rightScootSprite;
            }
            else if (gm.HeadingDirection < 0 && leftScootSprite != null)
            {
                sr.sprite = leftScootSprite;
            }
            else
            {
                sr.sprite = GetGroundedSprite(gm);
            }

            return;
        }

        // 🎿 NORMAL GROUNDED
        sr.sprite = GetGroundedSprite(gm);
    }

    Sprite GetSpinSprite(int spinIndex)
    {
        if (spinFrames == null || spinFrames.Length == 0)
            return null;

        spinIndex = Mathf.Clamp(spinIndex, 0, spinFrames.Length - 1);
        return spinFrames[spinIndex].sprite;
    }

    Sprite GetGroundedSprite(GameManager gm)
    {
        if (gm.HeadingIndex == 0 || gm.HeadingDirection == 0)
            return centerSprite;

        int index = gm.HeadingIndex - 1;

        if (gm.HeadingDirection > 0)
        {
            if (index < rightTurnSprites.Length)
                return rightTurnSprites[index];
        }
        else
        {
            if (index < leftTurnSprites.Length)
                return leftTurnSprites[index];
        }

        return centerSprite;
    }
}

// ----- SkierSpriteController.cs END -----
