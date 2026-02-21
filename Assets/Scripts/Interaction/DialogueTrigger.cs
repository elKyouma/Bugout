using UnityEngine;
using VisualDirector;
/*Triggers a dialogue conversation, passing unique commands and information to the dialogue box and inventory system for fetch quests, etc.*/

public class DialogueTrigger : MonoBehaviour, IDisabable, IDialogueController
{
    public VisualDirectorRuntimeGraph vs; //optional reference, if there is use new system instead of legacy dialogue system

    [Header("References")]
    [SerializeField] private GameObject finishTalkingActivateObject; //After completing a conversation, an object can activate. 
    [SerializeField] private Animator iconAnimator; //The E icon animator

    [Header("Trigger")]
    [SerializeField] private bool autoHit; //Does the player need to press the interact button, or will it simply fire automatically?
    public bool completed;
    [SerializeField] private bool sleeping;

    private bool toReset = false;

    public void ResetDialogue() => toReset = true;

    void OnTriggerStay2D(Collider2D col)
    {
        Cursor.visible = true;

        if (col.gameObject == GameManager.Instance.newPlayer.gameObject && !sleeping && !completed)
        {
            iconAnimator.SetBool("active", true);
            if (autoHit || (Input.GetAxis("Submit") > 0))
            {
                iconAnimator.SetBool("active", false);
                FindFirstObjectByType<VisualDirector.VisualDirector>().Execute(vs, this, GameManager.Instance);
                sleeping = true;
            }
        }
        else
            iconAnimator.SetBool("active", false);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == GameManager.Instance.newPlayer.gameObject)
        {
            iconAnimator.SetBool("active", false);
            sleeping = completed;
        }
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

    public void Disable() => completed = true;
    public void UpdateDialogue(VisualDirectorRuntimeGraph vs) => this.vs = vs;
}