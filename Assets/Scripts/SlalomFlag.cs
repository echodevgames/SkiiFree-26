// ----- SlalomFlag.cs START -----

using UnityEngine;

public enum SlalomSide
{
    Left,
    Right
}

public class SlalomFlag : MonoBehaviour
{
    [Header("Slalom Rules")]
    public SlalomSide requiredSide;
    public float scoreValue = 150f;
    public float xTolerance = 0.15f;

    float aliveTime;
    bool evaluated;

    void Update()
    {
        aliveTime += Time.deltaTime;

        // ⏱ ensure flag exists long enough to be seen
        if (evaluated || aliveTime < 5.55f)
            return;

        var gm = GameManager.Instance;
        if (gm == null)
            return;

        // Player passed vertically
        if (transform.position.y < gm.transform.position.y - 0.1f)

        {
            Evaluate(gm.transform.position.x);
            evaluated = true;
            Destroy(gameObject);
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
            Debug.Log($"[SLALOM] Success ({requiredSide}) +{scoreValue}");
        }
        else
        {
            Debug.Log($"[SLALOM] Missed ({requiredSide})");
        }
    }
}

// ----- SlalomFlag.cs END -----
