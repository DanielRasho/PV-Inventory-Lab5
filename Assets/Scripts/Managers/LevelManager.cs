using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public List<InventoryEntry> inventory = new List<InventoryEntry>();

    private List<string> pickedItems = new List<string>();

    private readonly Dictionary<Item.PickupType, InventoryEntry> lookup =
        new Dictionary<Item.PickupType, InventoryEntry>();

    private int money;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Item.OnItemCollected += ItemCollected;
    }

    private void OnDisable()
    {
        Item.OnItemCollected -= ItemCollected;
    }

    private void Start()
    {
        GameSession pendingSession = StorageManager.GetOrCreateInstance().ConsumePendingSessionForActiveScene();
        if (pendingSession != null)
        {
            RestoreSession(pendingSession);
            return;
        }

        RefreshUI();
    }

    private void ItemCollected(Item item, string id)
    {
        if (!string.IsNullOrEmpty(id) && !pickedItems.Contains(id))
        {
            pickedItems.Add(id);
        }

        if (item.data.pickupType == Item.PickupType.Money)
        {
            money += (item as MoneyItem).Amount;
            RefreshUI();
            return;
        }

        if (lookup.ContainsKey(item.data.pickupType))
        {
            lookup[item.data.pickupType].count++;
        }
        else
        {
            InventoryEntry entry = new InventoryEntry
            {
                itemName = item.data.name,
                icon = item.data.icon,
                count = 1
            };

            inventory.Add(entry);
            lookup[item.data.pickupType] = entry;
        }

        RefreshUI();
    }

    public void SaveCurrentGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        GameSession session = new GameSession
        {
            X = player.transform.position.x,
            Y = player.transform.position.y,
            coins = money,
            pickedItems = new List<string>(pickedItems),
            inventory = BuildInventorySnapshot(),
            level = SceneManager.GetActiveScene().name
        };

        StorageManager.GetOrCreateInstance().SaveGame(session);
    }

    public void RestoreSession(GameSession session)
    {
        if (session == null)
        {
            return;
        }

        pickedItems = session.pickedItems != null
            ? new List<string>(session.pickedItems)
            : new List<string>();

        money = session.coins;

        Dictionary<Item.PickupType, ItemParams> itemCatalog = BuildItemCatalog();
        RestoreInventory(session.inventory, itemCatalog);
        RestorePlayerPosition(session.X, session.Y);
        RemovePickedSceneItems();
        RefreshUI();
    }

    private void RestoreInventory(List<InventorySaveEntry> savedInventory, Dictionary<Item.PickupType, ItemParams> itemCatalog)
    {
        inventory.Clear();
        lookup.Clear();

        if (savedInventory == null)
        {
            return;
        }

        foreach (InventorySaveEntry entry in savedInventory)
        {
            if (entry == null || entry.count <= 0)
            {
                continue;
            }

            itemCatalog.TryGetValue(entry.pickupType, out ItemParams paramsData);

            InventoryEntry restoredEntry = new InventoryEntry
            {
                itemName = paramsData != null ? paramsData.name : entry.pickupType.ToString(),
                icon = paramsData != null ? paramsData.icon : null,
                count = entry.count
            };

            inventory.Add(restoredEntry);
            lookup[entry.pickupType] = restoredEntry;
        }
    }

    private Dictionary<Item.PickupType, ItemParams> BuildItemCatalog()
    {
        Dictionary<Item.PickupType, ItemParams> catalog = new Dictionary<Item.PickupType, ItemParams>();
        Item[] sceneItems = FindObjectsOfType<Item>(true);

        foreach (Item item in sceneItems)
        {
            if (item == null || item.data == null)
            {
                continue;
            }

            if (!catalog.ContainsKey(item.data.pickupType))
            {
                catalog[item.data.pickupType] = item.data;
            }
        }

        return catalog;
    }

    private void RestorePlayerPosition(float x, float y)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Could not restore player position because no Player-tagged object was found.");
            return;
        }

        player.transform.position = new Vector3(x, y, player.transform.position.z);
    }

    private void RemovePickedSceneItems()
    {
        if (pickedItems.Count == 0)
        {
            return;
        }

        HashSet<string> pickedLookup = new HashSet<string>(pickedItems);
        Item[] sceneItems = FindObjectsOfType<Item>(true);

        foreach (Item item in sceneItems)
        {
            if (item == null || string.IsNullOrEmpty(item.ID))
            {
                continue;
            }

            if (pickedLookup.Contains(item.ID))
            {
                Destroy(item.gameObject);
            }
        }
    }

    private List<InventorySaveEntry> BuildInventorySnapshot()
    {
        List<InventorySaveEntry> snapshot = new List<InventorySaveEntry>(lookup.Count);

        foreach (KeyValuePair<Item.PickupType, InventoryEntry> entry in lookup)
        {
            snapshot.Add(new InventorySaveEntry
            {
                pickupType = entry.Key,
                count = entry.Value.count
            });
        }

        return snapshot;
    }

    private void RefreshUI()
    {
        if (moneyUI.Instance != null)
        {
            moneyUI.Instance.updateCounter(money);
        }

        if (HotbarUI.Instance != null)
        {
            HotbarUI.Instance.UpdateHotbar(inventory);
        }
    }

    private void OnApplicationQuit()
    {
        SaveCurrentGame();
    }
}
