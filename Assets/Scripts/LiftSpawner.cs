// ----- LiftSpawner.cs START -----
using UnityEngine;

public class LiftSpawner : MonoBehaviour
{
    public GameObject liftSegmentPrefab;

    public float spawnInterval = 6f;
    public float spawnX = 3.5f;   // fixed horizontal location
    public float spawnY = -6f;

    float timer;

    void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.Playing)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            Vector3 pos = new Vector3(spawnX, spawnY, 0);
            Instantiate(liftSegmentPrefab, pos, Quaternion.identity);
        }
    }
}
// ----- LiftSpawner.cs END -----