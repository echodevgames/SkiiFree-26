// ----- PlayerCrash.cs START -----

using UnityEngine;

public class PlayerCrash : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        var gm = GameManager.Instance;

        // === MOGUL ===
        var mogul = collision.gameObject.GetComponent<Mogul>();
        if (mogul != null)
        {
            if (gm.CurrentJumpState == GameManager.JumpState.Grounded)
            {
                gm.TriggerSlow(mogul.slowMultiplier, mogul.slowDuration);
            }
            return;
        }

        // === TREE ===
        if (collision.gameObject.GetComponent<Tree>() != null)
        {
            if (gm.CurrentJumpState == GameManager.JumpState.HighJump)
                return;

            gm.TriggerCrash(1.0f);
            return;
        }

        // === ROCK ===
        if (collision.gameObject.GetComponent<Rock>() != null)
        {
            if (gm.CurrentJumpState != GameManager.JumpState.Grounded)
            {
                gm.Bounce(gm.rockBounceExtraTime);
                return;
            }

            gm.TriggerCrash(1.0f);
            return;
        }

        // === STUMP ===
        if (collision.gameObject.GetComponent<Stump>() != null)
        {
            if (gm.CurrentJumpState != GameManager.JumpState.Grounded)
            {
                gm.Bounce(gm.stumpBounceExtraTime);
                return;
            }

            gm.TriggerCrash(1.0f);
            return;
        }

        // === JUMP PAD ===
        var pad = collision.gameObject.GetComponent<JumpPad>();
        if (pad != null)
        {
            gm.StartHighJump();

            var col = collision.gameObject.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var npc = other.GetComponent<NPCSkierController>();
        if (npc != null)
        {
            GameManager.Instance.TriggerCrash(1.0f);
        }
    }
}

// ----- PlayerCrash.cs END -----