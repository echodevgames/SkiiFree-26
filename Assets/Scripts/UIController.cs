// ----- UIController.cs START -----



using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;   // Panel with Play button

    void Start()
    {
        ShowMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.Menu)
                QuitGame();
            else
                ShowMenu(); // future pause menu hook
        }
    }


    public void OnPlayPressed()
    {
        menuPanel.SetActive(false);
        GameManager.Instance.StartGame();
    }

    void ShowMenu()
    {
        Time.timeScale = 0f;
        menuPanel.SetActive(true);
    }

    void QuitGame()
    {
        Debug.Log("Quit Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
// ----- UIController.cs END -----

