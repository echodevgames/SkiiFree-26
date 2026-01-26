using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;   // Panel with Play button

    void Start()
    {
        Time.timeScale = 0f;
        ShowMenu();
    }

    void Update()
    {
        // Quit on Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void OnPlayPressed()
    {
        menuPanel.SetActive(false);
        GameManager.Instance.StartGame();
    }

    void ShowMenu()
    {
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
