// ----- GameFlowController.cs START -----


using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameFlowController : MonoBehaviour
{
    [Header("Timing")]
    public float teamSplashTime = 3.0f;
    //public float gameSplashTime = 6.0f;

    void Awake()
    {
        if (FindObjectsOfType<GameFlowController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(FlowRoutine());
    }

    IEnumerator FlowRoutine()
    {
        yield return LoadSceneAndWait("TeamSplash", teamSplashTime);
       // yield return LoadSceneAndWait("GameSplash", gameSplashTime);

        SceneManager.LoadScene("Main");
    }

    IEnumerator LoadSceneAndWait(string sceneName, float duration)
    {
        SceneManager.LoadScene(sceneName);

        // Prevent key carryover
        yield return WaitForKeyRelease();

        float timer = 0f;

        while (timer < duration)
        {
            if (Input.anyKeyDown)
                break;

            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator WaitForKeyRelease()
    {
        // Wait until no keys are held
        while (Input.anyKey)
            yield return null;
    }
}
// ----- GameFlowController.cs END -----