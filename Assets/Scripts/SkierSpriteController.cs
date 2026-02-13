// ----- SkierSpriteController.cs START -----

using UnityEngine;

public class SkierSpriteController : MonoBehaviour
{
    [Header("Grounded Direction Sprites")]
    public Sprite centerSprite;
    public Sprite[] rightTurnSprites; // ordered outward from center
    public Sprite[] leftTurnSprites;  // ordered outward from center

    [Header("Spin Sprites (Angle-Labeled)")]
    public SpinFrame[] spinFrames; // MUST match GameManager spinFrameCount

    [Header("Special Sprites")]
    public Sprite crashSprite;
    public Sprite bounceSprite;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var gm = GameManager.Instance;

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

        // 🎿 GROUNDED
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
