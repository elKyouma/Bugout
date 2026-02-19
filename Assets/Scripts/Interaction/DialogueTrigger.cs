using UnityEngine;
using VisualDirector;
/*Triggers a dialogue conversation, passing unique commands and information to the dialogue box and inventory system for fetch quests, etc.*/

public class DialogueTrigger : MonoBehaviour, IDisabable
{

    public VisualDirectorRuntimeGraph vs; //optional reference, if there is use new system instead of legacy dialogue system

    [Header("References")]
    [SerializeField] private GameObject finishTalkingActivateObject; //After completing a conversation, an object can activate. 
    [SerializeField] private Animator iconAnimator; //The E icon animator

    [Header("Trigger")]
    [SerializeField] private bool autoHit; //Does the player need to press the interact button, or will it simply fire automatically?
    public bool completed;
    [SerializeField] private bool repeat; //Set to true if the player should be able to talk again and again to the NPC. 
    [SerializeField] private bool sleeping;

    [Header("Dialogue")]
    [SerializeField] private string characterName; //The character's name shown in the dialogue UI
    [SerializeField] private string dialogueStringA; //The dialogue string that occurs before the fetch quest
    [SerializeField] private string dialogueStringB; //The dialogue string that occurs after fetch quest
    [SerializeField] private AudioClip[] audioLinesA; //The audio lines that occurs before the fetch quest
    [SerializeField] private AudioClip[] audioLinesB; //The audio lines that occur after the fetch quest
    [SerializeField] private AudioClip[] audioChoices; //The audio lines that occur when selecting an audio choice

    [Header("Fetch Quest")]
    [SerializeField] private GameObject deleteGameObject; //If an NPC is holding the object, and gives it to you, this object will destroy
    [SerializeField] private ItemType getWhichItem; //The inventory item given if items is fetched
    [SerializeField] private int getBugsAmount; //Or the amount of coins given if item is fetched
    [SerializeField] private string finishTalkingAnimatorBool; //After completing a conversation, an animation can be fired
    [SerializeField] private string finishTalkingActivateObjectString; //After completing a conversation, an object's name can be searched for and activated.
    [SerializeField] private GameObject activateObjectChoice1;
    [SerializeField] private GameObject activateObjectChoice2;
    [SerializeField] private Sprite getItemSprite; //The sprite of the inventory item given, shown in HUD
    [SerializeField] private AudioClip getSound; //When the player is given an object, this sound will play
    [SerializeField] private bool instantGet; //Player can be immediately given an item the moment the conversation begins
    [SerializeField] private bool removeRequiredItem;
    [SerializeField] private ItemType requiredItem; //The required fetch quest item
    [SerializeField] private int requiredBugs; //Or the required coins (cannot require both an item and coins)
    public Animator useItemAnimator; //If the player uses an item, like a key, an animator can be fired (ie to open a door)
    [SerializeField] private string useItemAnimatorBool; //An animator bool can be set to true once an item is used, like ae key.

    private bool toReset = false;

    public void SetDialgueA(string s) => dialogueStringA = s;
    public void SetDialgueB(string s) => dialogueStringB = s;
    public void SetActivateObject1(GameObject go) => activateObjectChoice1 = go;
    public void SetActivateObject2(GameObject go) => activateObjectChoice2 = go;
    public void ResetDialogue() => toReset = true;

    void OnTriggerStay2D(Collider2D col)
    {
        Cursor.visible = true;
        if (instantGet)
            InstantGet();

        if (col.gameObject == Player.Instance.gameObject && !sleeping && !completed && Player.Instance.grounded)
        {
            iconAnimator.SetBool("active", true);
            if (autoHit || (Input.GetAxis("Submit") > 0))
            {
                iconAnimator.SetBool("active", false);
                if (requiredItem == ItemType.None && requiredBugs == 0 || !GameManager.Instance.IsItemInInventory(requiredItem) && requiredBugs == 0 || (requiredBugs != 0 && Player.Instance.bugs < requiredBugs))
                    if (!vs)
                        GameManager.Instance.dialogueBoxController.Appear(dialogueStringA, characterName, this, false, audioLinesA, audioChoices, finishTalkingAnimatorBool, finishTalkingActivateObject, finishTalkingActivateObjectString, repeat, activateObjectChoice1, activateObjectChoice2);
                    else
                        FindFirstObjectByType<VisualDirector.VisualDirector>().Execute(vs, this, GameManager.Instance);
                else if (requiredBugs == 0 && GameManager.Instance.IsItemInInventory(requiredItem) || (requiredBugs != 0 && Player.Instance.bugs >= requiredBugs))
                {
                    if (dialogueStringB != "")
                        if (!vs)
                            GameManager.Instance.dialogueBoxController.Appear(dialogueStringB, characterName, this, true, audioLinesB, audioChoices, "", null, "", repeat, activateObjectChoice1, activateObjectChoice2);
                        else
                            FindFirstObjectByType<VisualDirector.VisualDirector>().Execute(vs, this, GameManager.Instance);
                    else
                        UseItem();
                }
                sleeping = true;
            }
        }
        else
            iconAnimator.SetBool("active", false);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == Player.Instance.gameObject)
        {
            iconAnimator.SetBool("active", false);
            sleeping = completed;
        }
    }

    public void UseItem()
    {
        if (!completed)
        {
            if (useItemAnimatorBool != "")
                useItemAnimator.SetBool(useItemAnimatorBool, true);

            if (deleteGameObject)
                Destroy(deleteGameObject);

            Collect();

            var (slotID, success) = GameManager.Instance.TryGetItemIventorySlotID(requiredItem);
            if (success)
            {
                if (removeRequiredItem)
                    GameManager.Instance.TryRemoveItemFromInventorySlot(slotID);
            }
            else
                Player.Instance.bugs -= requiredBugs;

            repeat = false;
        }
    }

    public void Collect()
    {
        if (!completed)
        {
            Item prizeItem = new Item(getWhichItem, getItemSprite);
            if (getWhichItem != ItemType.None)
            {
                bool success = GameManager.Instance.TryAddItemToInventory(prizeItem);
                if (!success)
                    return;
            }

            if (getBugsAmount != 0)
                Player.Instance.bugs += getBugsAmount;

            if (getSound != null)
                GameManager.Instance.audioSource.PlayOneShot(getSound);

            completed = true;
        }
    }

    public void InstantGet()
    {
        Item prizeItem = new Item(getWhichItem, getItemSprite);
        bool success = GameManager.Instance.TryAddItemToInventory(prizeItem);
        if (!success) 
            return;
        instantGet = false;
    }

    private void Update()
    {
        if (toReset && completed && sleeping)
        {
            toReset = false;
            sleeping = false;
            completed = false;
        }
    }

    public void Disable()
    {
        completed = true;
    }
}