using UnityEngine;

public class NPCSpawner : MonoBehaviour
{

    public GameObject[] npcPrefab; 

    public float spawnInterval = 4f;
    public float spawnWidth = 6f;
    public float spawnY = 7f;

    float timer;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.Playing)
            return;

        timer += Time.deltaTime;

        if ( timer >= spawnInterval)
        {
            timer = 0f;

            float x = Random.Range(-spawnWidth, spawnWidth);
            Vector3 pos = new Vector3(x, spawnY, 0f);

            var prefab = npcPrefab[Random.Range(0, npcPrefab.Length)];
            Instantiate(prefab, pos, Quaternion.identity);
        }


    }
}
