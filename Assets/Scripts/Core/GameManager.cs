using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

/*Manages inventory, keeps several component references, and any other future control of the game itself you may need*/

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public AudioSource audioSource; //A primary audioSource a large portion of game sounds are passed through
    public DialogueBoxController dialogueBoxController; // INFO: dialgoue stuff, don't touch until dialogue graphs are done
    public HUD hud; //A reference to the HUD holding your health UI, coins, dialogue, etc.
    public Inventory inventory = new Inventory();
    [SerializeField] public AudioTrigger gameMusic;
    [SerializeField] public AudioTrigger gameAmbience;

    [System.Serializable]
    private class EndingClass
    {
        public string name;
        public EndingSO val;
        public int bugs;
    }

    [SerializeField]
    private List<EndingClass> endingList = new List<EndingClass>();
    private Dictionary<string, (EndingSO, Status, int)> endingDict = new Dictionary<string, (EndingSO, Status, int)>();
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
        if (Instance != this) Destroy(gameObject);

        foreach (var ending in endingList)
            endingDict[ending.name] = (ending.val, (Status)PlayerPrefs.GetInt(ending.name, 0), ending.bugs);
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
                Player.Instance.bugs += reward;

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
            var (endingSO, _, reward) = endingDict[ending.name];
            endingDict[ending.name] = (endingSO, Status.NotCompleted, reward);
        }
    }

}
