using UnityEngine;

public class PlayerCrash : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        var gm = GameManager.Instance;

        // TREE: always crash unless high jump
        if (collision.gameObject.GetComponent<Tree>() != null)
        {
            if (gm.CurrentJumpState == GameManager.JumpState.HighJump)
                return;

            gm.TriggerCrash(1.0f);
            return;
        }

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


        // JUMP PAD
        var pad = collision.gameObject.GetComponent<JumpPad>();
        if (pad != null)
        {
            gm.StartHighJump();

            // Disable collider so it can't retrigger
            var col = collision.gameObject.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

            return;
        }

    }
}
