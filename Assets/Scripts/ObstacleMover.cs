// ----- ObstacleMover.cs START -----



using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float lateralSpeedMultiplier = 1f;

    void Update()
    {
        var gm = GameManager.Instance;

        float verticalSpeed = gm.CurrentScrollSpeed;

        float horizontalSpeed = 0f;

        if (GameManager.Instance.IsCrashed)
            return;

        // Only apply continuous lateral drift while NOT fully sideways
        if (gm.HeadingDegrees < 90f)
        {
            horizontalSpeed =
                gm.baseScrollSpeed *
                gm.LateralFactor *
                gm.HeadingDirection *
                lateralSpeedMultiplier;
        }


        Vector3 move =
            Vector3.up * verticalSpeed * Time.deltaTime +
            Vector3.right * horizontalSpeed * Time.deltaTime;

        transform.position += move;
    }
}
// ----- ObstacleMover.cs END -----

