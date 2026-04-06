using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class InventorySaveEntry
{
    public Item.PickupType pickupType;
    public int count;
}

[Serializable]
public class GameSession
{
    public float X;
    public float Y;
    public int coins;
    public List<string> pickedItems;
    public List<InventorySaveEntry> inventory;
    public string level;
}

public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance;

    [SerializeField] private string fileName = "gamesession.json";

    private GameSession pendingLoadedSession;

    private string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static StorageManager GetOrCreateInstance()
    {
        if (Instance != null)
        {
            return Instance;
        }

        GameObject storageObject = new GameObject(nameof(StorageManager));
        return storageObject.AddComponent<StorageManager>();
    }

    public void SaveGame(GameSession session)
    {
        if (session == null)
        {
            Debug.LogWarning("Cannot save a null game session.");
            return;
        }

        session.pickedItems ??= new List<string>();
        session.inventory ??= new List<InventorySaveEntry>();

        string json = JsonUtility.ToJson(session, true);
        File.WriteAllText(GetPath(), json);
        Debug.Log("Game saved to: " + GetPath());
    }

    public GameSession LoadGame()
    {
        string path = GetPath();

        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found.");
            return null;
        }

        string json = File.ReadAllText(path);
        GameSession session = JsonUtility.FromJson<GameSession>(json);

        Debug.Log("Game loaded from: " + path);
        return session;
    }

    public bool LoadSavedGameToActiveLevel()
    {
        GameSession session = LoadGame();

        if (session == null || string.IsNullOrWhiteSpace(session.level))
        {
            Debug.LogWarning("Saved game does not contain a valid level.");
            return false;
        }

        pendingLoadedSession = session;
        SceneManager.LoadScene(session.level);
        return true;
    }

    public void LoadLevel(string levelName)
    {
        pendingLoadedSession = null;
        SceneManager.LoadScene(levelName);
    }

    public GameSession ConsumePendingSessionForActiveScene()
    {
        if (pendingLoadedSession == null)
        {
            return null;
        }

        string activeSceneName = SceneManager.GetActiveScene().name;
        if (!string.Equals(activeSceneName, pendingLoadedSession.level, StringComparison.Ordinal))
        {
            return null;
        }

        GameSession session = pendingLoadedSession;
        pendingLoadedSession = null;
        return session;
    }

    public bool HasSaveGame()
    {
        return File.Exists(GetPath());
    }

    public void DeleteSave()
    {
        string path = GetPath();

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save deleted.");
        }
    }
}
