using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;

    private void Start()
    {
        HideMenu();
    }

    public void ToogleMenu()
    {
        if (menuPanel == null)
        {
            Debug.LogWarning("Pause menu panel is not assigned.");
            return;
        }

        if (menuPanel.activeSelf)
        {
            HideMenu();
        }
        else
        {
            ShowMenu();
        }
    }

    private void HideMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }
    
    private void ShowMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
        }
    }

    public void SaveAndReturnToMainMenu()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SaveCurrentGame();
        }

        SceneManager.LoadScene("MainMenu");
    }
}
