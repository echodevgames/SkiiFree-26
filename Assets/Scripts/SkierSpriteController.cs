// ----- SkierSpriteController.cs START -----

using UnityEngine;

public class SkierSpriteController : MonoBehaviour
{
    public Sprite[] directionSprites;

    public Sprite[] spinSprites;

    public Sprite crashSprite;

    public Sprite bounceSprite;


    SpriteRenderer sr;
    const int CENTER_INDEX = 4;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var gm = GameManager.Instance;

        if (gm.IsCrashed)
        {
            sr.sprite = crashSprite;
            transform.rotation = Quaternion.identity;
            return;
        }
        if (gm.CurrentJumpState != GameManager.JumpState.Grounded)
        {
            if (gm.JustBounced && bounceSprite != null)
                sr.sprite = bounceSprite;
            else
                sr.sprite = spinSprites[gm.SpinFrameIndex];

            return;
        }


        transform.rotation = Quaternion.identity;

        int spriteIndex =
            gm.HeadingIndex == 0 || gm.HeadingDirection == 0
            ? CENTER_INDEX
            : CENTER_INDEX + gm.HeadingIndex * gm.HeadingDirection;

        spriteIndex = Mathf.Clamp(spriteIndex, 0, directionSprites.Length - 1);
        sr.sprite = directionSprites[spriteIndex];
    }
}
// ----- SkierSpriteController.cs START -----
