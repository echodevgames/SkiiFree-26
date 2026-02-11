// -----DogRunner.cs START-----



using UnityEngine;

public class DogRunner : MonoBehaviour
{
    public float horizontalSpeed = 5f;
    public float verticalDrift = 0.3f;

    [Header("World Influence")]
    public float speedInfluence = 0.4f; // how much player speed affects dog

    int direction; // -1 = left, +1 = right

    public void SetDirection(int dir)
    {
        direction = Mathf.Clamp(dir, -1, 1);

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = direction < 0;
    }

    void Update()
    {
        var gm = GameManager.Instance;

        float worldSpeed = gm.CurrentScrollSpeed * speedInfluence;

        float xMove = direction * (horizontalSpeed + worldSpeed) * Time.deltaTime;
        float yMove = -verticalDrift * Time.deltaTime;

        transform.position += new Vector3(xMove, yMove, 0f);
    }
}



// -----DogRunner.cs END-----