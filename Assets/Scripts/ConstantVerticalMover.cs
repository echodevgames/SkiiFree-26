// ----- ConstantVerticalMover.cs START -----

using UnityEngine;

public class ConstantVerticalMover : MonoBehaviour
{
    public enum Direction
    {
        Up,
        Down
    }

    public Direction direction = Direction.Up;
    public float speed = 2f;

    void Update()
    {
        float dir = direction == Direction.Up ? 1f : -1f;
        transform.position += Vector3.up * dir * speed * Time.deltaTime;
    }
}
// ----- ConstantVerticalMover.cs END -----