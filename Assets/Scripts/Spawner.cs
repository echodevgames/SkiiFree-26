using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject treePrefab;
    public GameObject rockPrefab;
    public GameObject stumpPrefab;
    public GameObject jumpPadPrefab;


    public float spawnRate = 1f;
    public float spawnWidth = 6f;
    public float spawnY = -6f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            timer = 0f;

            float x = Random.Range(-spawnWidth, spawnWidth);
            Vector3 spawnPos = new Vector3(x, spawnY, 0);

            GameObject prefabToSpawn;
            float roll = Random.value;

            if (roll < 0.55f)
                prefabToSpawn = treePrefab;
            else if (roll < 0.75f)
                prefabToSpawn = rockPrefab;
            else if (roll < 0.9f)
                prefabToSpawn = stumpPrefab;
            else
                prefabToSpawn = jumpPadPrefab;


            Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        }
    }
}
