using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuPersistenceUI : MonoBehaviour
{
    [SerializeField] private string newGameScene = "lvl_1";
    [SerializeField] private Button loadButton;

    private void Start()
    {
        RefreshLoadButtonState();
    }

    public void LoadSavedGame()
    {
        StorageManager.GetOrCreateInstance().LoadSavedGameToActiveLevel();
    }

    public void StartNewGame()
    {
        StorageManager.GetOrCreateInstance().LoadLevel(newGameScene);
    }

    public void DeleteSavedGame()
    {
        StorageManager.GetOrCreateInstance().DeleteSave();
        RefreshLoadButtonState();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RefreshLoadButtonState()
    {
        if (loadButton != null)
        {
            loadButton.interactable = StorageManager.GetOrCreateInstance().HasSaveGame();
        }
    }
}
