// ----- ObstacleMover.cs START -----

using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    [Header("Lateral Movement")]
    public float lateralSpeedMultiplier = 1f;

    [Header("Scoot")]
    [Tooltip("World-units moved per scoot bump")]
    public float scootStepDistance = 0.35f;

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.IsCrashed)
            return;

        // Normal vertical scrolling
        float verticalSpeed = gm.CurrentScrollSpeed;
        float horizontalSpeed = 0f;

        // Normal carve drift (not sideways)
        if (gm.HeadingDegrees < 90f)
        {
            horizontalSpeed =
                gm.baseScrollSpeed *
                gm.LateralFactor *
                gm.HeadingDirection *
                lateralSpeedMultiplier;

            transform.position +=
                Vector3.up * verticalSpeed * Time.deltaTime +
                Vector3.right * horizontalSpeed * Time.deltaTime;

            return;
        }

        // Sideways: still scroll vertically, but lateral only bumps
        transform.position += Vector3.up * verticalSpeed * Time.deltaTime;

        if (gm.ScootBump && gm.ScootIntent != 0f)
        {
            transform.position += Vector3.right * (gm.ScootIntent * scootStepDistance);
        }
    }
}

// ----- ObstacleMover.cs END -----
