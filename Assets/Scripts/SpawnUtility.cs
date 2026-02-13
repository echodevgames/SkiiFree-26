// ----- SpawnUtility.cs START -----

using UnityEngine;

public static class SpawnUtility
{
    public static void SpawnClump(
        GameObject prefab,
        Vector3 center,
        int count,
        float spreadX,
        float spreadY
    )
    {
        for (int i = 0; i < count; i++)
        {
            float offsetX = Random.Range(-spreadX, spreadX);
            float offsetY = Random.Range(-spreadY, spreadY);

            Vector3 pos = center + new Vector3(offsetX, offsetY, 0f);
            Object.Instantiate(prefab, pos, Quaternion.identity);
        }
    }
}
// ----- SpawnUtility.cs END -----