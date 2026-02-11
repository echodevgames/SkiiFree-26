// ----- DogSpawner.cs START-----

using UnityEngine;

public class DogSpawner : MonoBehaviour
{
    public GameObject dogPrefab;

    public float spawnInterval = 12f;
    public float spawnChance = 0.35f;

    public float spawnY = 1.5f;
    public float spawnYVariance = 0.75f;   // NEW
    public float offscreenX = 8f;

    float timer;

    void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.Playing)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            if (Random.value > spawnChance)
                return;

            bool spawnFromLeft = Random.value < 0.5f;

            float x = spawnFromLeft ? -offscreenX : offscreenX;
            int direction = spawnFromLeft ? 1 : -1;

            float y = spawnY + Random.Range(-spawnYVariance, spawnYVariance);

            GameObject dog = Instantiate(
                dogPrefab,
                new Vector3(x, y, 0),
                Quaternion.identity
            );

            dog.GetComponent<DogRunner>().SetDirection(direction);
        }
    }
}

// ----- DogSpawner.cs END-----