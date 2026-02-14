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

    [Header("Scoring")]
    public float scoreValue = 150f;
    public float xTolerance = 0.15f;

    Transform player;
    bool evaluated;

    void Start()
    {
        // Find player once (cheap + safe)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (evaluated || player == null)
            return;

        // Player has passed flag vertically
        if (player.position.y < transform.position.y - 0.1f)
        {
            Evaluate(player.position.x);
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
        {
            GameManager.Instance.AddStyleScore(scoreValue);
        }
    }
}
// ----- SlalomFlag.cs END -----
