using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*Manages inventory, keeps several component references, and any other future control of the game itself you may need*/

public class GameManager : MonoBehaviour
{
    public AudioSource audioSource; //A primary audioSource a large portion of game sounds are passed through
    public DialogueBoxController dialogueBoxController;
    public HUD hud; //A reference to the HUD holding your health UI, coins, dialogue, etc.
    public Inventory inventory = new Inventory();
    private static GameManager instance;
    [SerializeField] public AudioTrigger gameMusic;
    [SerializeField] public AudioTrigger gameAmbience;

    [System.Serializable]
    public class EndingClass
    {
        public string key;
        public Ending val;
        public int bugs;
    }

    [SerializeField]
    private List<EndingClass> endingList = new List<EndingClass>();
    private Dictionary<string, Ending> endingDict = new Dictionary<string, Ending>();
    public Dictionary<string, int> gameCompletion = new Dictionary<string, int>();//0 nie zrobiono, 1 zrobiono
    public Dictionary<string, int> reward = new Dictionary<string, int>();//0 nie zrobiono, 1 zrobiono
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

        foreach (var kvp in endingList)
        {
            endingDict[kvp.key] = kvp.val;
            gameCompletion[kvp.key] = PlayerPrefs.GetInt(kvp.key, 0);
            reward[kvp.key] = kvp.bugs;
        }
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

    public void EndGame(string ending)
    {
        if (!endingDict.ContainsKey(ending))
            Debug.LogError("Wrong ending name: " + ending);
        else
        {
            gameCompletion[ending] = 1;
            if (PlayerPrefs.GetInt(ending) == 0)
                Player.Instance.bugs += reward[ending];

            PlayerPrefs.SetInt(ending, 1);
            EndingPlayer.currentEnding = endingDict[ending];
            SceneManager.LoadScene("EndingScene");
        }

        PlayerPrefs.SetString("CurrendEnding", ending);
    }

    [ContextMenu("ResetEndings")]
    public void ResetEndings()
    {
        foreach (var kvp in endingList)
        {
            gameCompletion[kvp.key] = 0;
        }
    }

}
