// ----- UIController.cs START -----
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject menuPanel;

    [Header("HUD Text")]
    public TMP_Text timeText;
    public TMP_Text distanceText;
    public TMP_Text speedText;
    public TMP_Text styleText;

    [Header("Distance Tuning")]
    [Tooltip("Meters gained per second at base speed")]
    public float distanceMultiplier = 1f;

    void Start()
    {
        ShowMenu();
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null)
            return;

        // ---------- HUD UPDATE ----------
        if (gm.CurrentGameState == GameManager.GameState.Playing)
        {
            UpdateHUD(gm);
        }

        // ---------- MENU INPUT ----------
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gm.CurrentGameState == GameManager.GameState.Menu)
                QuitGame();
            else
                ShowMenu(); // pause hook later
        }
    }

    void UpdateHUD(GameManager gm)
    {
        // Time
        timeText.text = $"{gm.RunTime:00.0}";

        // Distance (classic SkiFree feel)
        float distance =
            gm.RunTime *
            gm.CurrentScrollSpeed *
            distanceMultiplier;

        distanceText.text = $"{Mathf.FloorToInt(distance)}m";

        // Speed
        speedText.text = $"{gm.DisplaySpeed:0.0} m/s";


        // Style
        styleText.text = $"{Mathf.FloorToInt(gm.StyleScore)}";
    }

    // ---------- BUTTONS ----------
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
