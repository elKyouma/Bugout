using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using VisualDirector;

/*Manages inventory, keeps several component references, and any other future control of the game itself you may need*/

public class GameManager : MonoBehaviour, IGameManager
{
    private static GameManager instance;

    public AudioSource audioSource; //A primary audioSource a large portion of game sounds are passed through
    public Inventory inventory = new();
    [SerializeField] public AudioTrigger gameMusic;
    [SerializeField] public AudioTrigger gameAmbience;
    [SerializeField] private GameObject pauseMenu;

    [HideInInspector] public HUD hud; //A reference to the HUD holding your health UI, coins, dialogue, etc.
    [HideInInspector] public PlayerController newPlayer;
    [HideInInspector] public Postprocess postProcess;

    public uint Bugs;

    [System.Serializable]
    private class EndingClass
    {
        public EndingSO scriptableObject;
        public int bugs;
    }

    [SerializeField]
    private List<EndingClass> endingList = new List<EndingClass>();
    private Dictionary<string, (EndingSO, Status, int)> endingDict = new Dictionary<string, (EndingSO, Status, int)>();
    
    private Dictionary<TeleportTag.Tag, Vector3> teleportLocations = new Dictionary<TeleportTag.Tag, Vector3>();

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<GameManager>();
            return instance;
        }
    }

    void Awake()
    {
        newPlayer = FindFirstObjectByType<PlayerController>();
        hud = FindFirstObjectByType<HUD>();
        postProcess = FindAnyObjectByType<Postprocess>();

        if (Instance != this) Destroy(gameObject);

        foreach (var ending in endingList)
            endingDict[ending.scriptableObject.name] = (ending.scriptableObject, (Status)PlayerPrefs.GetInt(ending.scriptableObject.name, 0), ending.bugs);

        FindObjectsByType<TeleportTag>(FindObjectsSortMode.None)
            .ToList()
            .ForEach(loc =>
            {
                Assert.IsFalse(teleportLocations.ContainsKey(loc.tag), $"Duplicate TeleportTag found: {loc.tag} on {loc.gameObject.name}");
                teleportLocations[loc.tag] = loc.transform.position;
            });
    }

    void Start() => audioSource = GetComponent<AudioSource>();

    public bool TryAddItemToInventory(Item item)
    {
        var (slotID, success) = inventory.TryAddItem(item.type);
        if (!success)
            return false;

        hud.SetInventoryImage(item.UiImage, slotID);
        return true;
    }

    public (ItemType item, bool success) TryGetItemFromInventorySlot(int slotID)
    {
        var (item, success) = inventory.TryGetItemFromSlot(slotID);
        if (!success)
            return (ItemType.None, false);

        return (item, true);
    }
    public bool TryRemoveItemFromInventorySlot(int slotID)
    {
        var success = inventory.TryRemoveItemFromSlot(slotID);
        if (!success)
            return false;

        hud.SetInventoryImage(hud.blankUI, slotID);
        return true;
    }
    public (int slotID, bool success) TryGetItemIventorySlotID(ItemType item) => inventory.TryGetItemSlotID(item);

    public bool IsItemInInventory(ItemType item)
    {
        var (slotID, success) = inventory.TryGetItemSlotID(item);
        return success;
    }

    public bool DoesInventoryHaveTheSameItems(ItemType item) => inventory.AreBothSlotsOccupiedByItem(item);

    public void ClearInventory()
    {
        inventory.TryGetItemFromSlot(0);
        inventory.TryGetItemFromSlot(1);
    }

    public (EndingSO, Status, int) GetEndingData(string endingName) => endingDict[endingName];
    public void EndGame(string endingName)
    {
        if (!endingDict.ContainsKey(endingName))
            Debug.LogError("Wrong ending name: " + endingName);
        else
        {
            var (endingSO, completionStatus, reward) = endingDict[endingName];
            endingDict[endingName] = (endingSO, Status.Completed, reward);

            if (PlayerPrefs.GetInt(endingName) == (int)Status.Completed)
                Bugs += (uint)reward;

            PlayerPrefs.SetInt(endingName, (int)Status.Completed);
            EndingPlayer.currentEnding = endingSO;
            SceneManager.LoadScene("EndingScene");
        }

        PlayerPrefs.SetString("CurrendEnding", endingName);
    }

    [ContextMenu("ResetEndings")]
    public void ResetEndings()
    {
        foreach (var ending in endingList)
        {
            var (endingSO, _, reward) = endingDict[ending.scriptableObject.name];
            endingDict[ending.scriptableObject.name] = (endingSO, Status.NotCompleted, reward);
        }
    }

    public void TeleportPlayerToLocation(TeleportTag.Tag tag) => newPlayer.transform.position = teleportLocations[tag];
    public bool HasItem(IGameManager.ItemType item, int number)
    {
        if (number == 0 || number > 2)
        {
            Debug.LogError("Invalid number of items requested: " + number);
            return false;
        }  
        else if (number == 1)
            return IsItemInInventory((ItemType)item);
        else
            return DoesInventoryHaveTheSameItems((ItemType)item);

    }

    public bool HasBugs(int number) => Bugs >= number;
    public void GiveItem(IGameManager.ItemType item)
    {
        if (!TryAddItemToInventory(new Item((ItemType)item, null)))
            Debug.LogError("Failed to add item to inventory: " + item);
    }
    public void TakeItem(IGameManager.ItemType item)
    {
        var (slotId, succeess )= TryGetItemIventorySlotID((ItemType)item);
        if(succeess)
            TryRemoveItemFromInventorySlot(slotId);
        else
            Debug.LogError("Failed to remove item from inventory: " + item);
    }
    public void TurnOnPauseMenu() => pauseMenu.SetActive(true);
}
