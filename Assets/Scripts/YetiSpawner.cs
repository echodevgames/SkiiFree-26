// ----- YetiSpawner.cs START -----

using UnityEngine;

public class YetiSpawner : MonoBehaviour
{
    public GameObject yetiPrefab;
    public float spawnAfterSeconds = 40f;
    public float spawnYOffset = 2f;
    public float spawnXOffset = 8f;

    bool spawned;

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null || spawned)
            return;

        if (gm.CurrentGameState != GameManager.GameState.Playing)
            return;

        if (gm.RunTime >= spawnAfterSeconds)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;

            Vector3 pos = new Vector3(
                player.transform.position.x + spawnXOffset,
                player.transform.position.y + spawnYOffset,
                0f
            );

            Instantiate(yetiPrefab, pos, Quaternion.identity);
            spawned = true;
        }
    }
}

// ----- YetiSpawner.cs END -----