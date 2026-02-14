// ----- LiftSpawner.cs START -----
using UnityEngine;

public class LiftSpawner : MonoBehaviour
{
    public GameObject liftSegmentPrefab;

    public float spawnInterval = 6f;
    public float spawnX = 3.5f;   // fixed horizontal location
    public float spawnY = -6f;

    
    float distanceAccumulator;

    void Update()
    {
        var gm = GameManager.Instance;

        if (gm == null || gm.CurrentGameState != GameManager.GameState.Playing)
            return;

        if (gm.CurrentScrollSpeed <= 0f)
            return;

        distanceAccumulator += gm.CurrentScrollSpeed * Time.deltaTime;

        // meters between lift segments
        float metersPerLift = 14f;

        if (distanceAccumulator >= metersPerLift)
        {
            distanceAccumulator -= metersPerLift;

            Vector3 pos = new Vector3(spawnX, spawnY, 0);
            Instantiate(liftSegmentPrefab, pos, Quaternion.identity);
        }
    }


}
// ----- LiftSpawner.cs END -----