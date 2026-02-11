// ----- VerticalWorldMover.cs START-----

using UnityEngine;

public class VerticalWorldMover : MonoBehaviour
{
    public enum VerticalDirection
    {
        Down,   // same as obstacles
        Up      // opposite direction
    }

    public VerticalDirection direction = VerticalDirection.Down;
    public float speedMultiplier = 1f;

    void Update()
    {
        var gm = GameManager.Instance;

        if (gm.IsCrashed)
            return;

        float baseSpeed = gm.CurrentScrollSpeed * speedMultiplier;
        float verticalSpeed = direction == VerticalDirection.Down
            ? baseSpeed
            : -baseSpeed;

        transform.position += Vector3.up * verticalSpeed * Time.deltaTime;
    }
}
// ----- VerticalWorldMover.cs END -----