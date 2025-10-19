using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Sahne İsimleri")]
    public string gameSceneName = "SampleScene";

    [Header("UI")]
    public GameObject settingsPanel;

    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnQuitButtonPressed()
    {
#if UNITY_EDITOR
        // Editör modunda oyunu durdur
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Oyunu kapat
        Application.Quit();
#endif
    }

    public void OnSettingsButtonPressed()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void OnCloseSettingsButtonPressed()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}
