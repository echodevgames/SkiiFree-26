// ----- SlalomFlag.cs START -----
using UnityEngine;

public enum SlalomSide
{
    Left,
    Right
}

public class SlalomFlag : MonoBehaviour
{
    public SlalomSide requiredSide;
    public float scoreValue = 150f;
    public float xTolerance = 0.15f;

    bool evaluated;

    void Update()
    {
        if (evaluated)
            return;

        var gm = GameManager.Instance;
        if (gm == null)
            return;

        // Player has passed flag vertically
        if (transform.position.y < gm.transform.position.y - 0.1f)
        {
            Evaluate(gm.transform.position.x);
            evaluated = true;
        }
    }

    void Evaluate(float playerX)
    {
        float dx = playerX - transform.position.x;

        bool success =
            requiredSide == SlalomSide.Left
                ? dx < -xTolerance
                : dx > xTolerance;

        if (success)
            GameManager.Instance.AddStyleScore(scoreValue);
    }
}
// ----- SlalomFlag.cs END -----
