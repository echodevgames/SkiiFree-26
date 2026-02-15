// ----- YetiController.cs START -----
using UnityEngine;

public class YetiController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float speedRamp = 0.4f;

    Transform player;
    float currentSpeed;

    Vector3 moveDir;
    bool hasKilledPlayer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentSpeed = speed;
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null)
            return;

        // Yeti ALWAYS moves — even after crash
        currentSpeed += speedRamp * Time.deltaTime;

        if (!hasKilledPlayer && player != null)
        {
            // Actively chase player
            moveDir = (player.position - transform.position).normalized;
        }
        // else: keep last moveDir forever

        transform.position += moveDir * currentSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!hasKilledPlayer)
        {
            hasKilledPlayer = true;

            // Freeze direction at moment of impact
            moveDir = (player.position - transform.position).normalized;

            GameManager.Instance.TriggerCrash(999f);
        }
    }
}

// ----- YetiController.cs END -----