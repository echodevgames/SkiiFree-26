// ----- DestroyZone.cs START -----

using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("NPC"))
        {
            Destroy(other.transform.root.gameObject);
        }

    }
}

// ----- DestroyZone.cs END -----

