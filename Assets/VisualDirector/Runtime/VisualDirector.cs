using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VisualDirector
{
    public class VisualDirector : MonoBehaviour
    {
        [Header("Scene References")]
        public List<Image> ActorLocationList;
        public GameObject DialoguePanel;
        public TextMeshProUGUI DialogueText;
        public TextMeshProUGUI ActorNameText;
        public TextMeshProUGUI choice1;
        public TextMeshProUGUI choice2;
        public TextMeshProUGUI choice3;
        public TextMeshProUGUI choice4;
        public Animator animator;

        [Header("Settings")]
        public float GlobalFadeDuration = 0.5f;
        public float GlobalTextDelayPerCharacter = 0.03f;

        [Header("Input")]
        public MonoBehaviour InputComponent;
        public IVisualDirectorInputProvider InputProvider => InputComponent as IVisualDirectorInputProvider;

        public void SetChoiceAmount(int choiceAmount)
        {
            if(choiceAmount == 2)
                animator.SetBool("hasChoices", true);
            else if(choiceAmount == 0)
                animator.SetBool("hasChoices", false);
            else
                Debug.LogError($"Unsupported choice amount: {choiceAmount}");
        }
        public async void Execute(VisualDirectorRuntimeGraph runtimeGraph)
        {
            animator.SetBool("active", true);
            
            var setDialogueExecutor = new SetDialogueExecutor();
            var waitForInputExecutor = new WaitForInputExecutor();
            var multiChoiceExecutor = new MultiChoiceExecutor();

            var node = runtimeGraph.Nodes[0];
            while (node != null)
            {
                switch (node)
                {
                    case SetDialogueRuntimeNode dialogueNode:
                        await setDialogueExecutor.ExecuteAsync(dialogueNode, this);
                        node = dialogueNode.Next[0];
                        break;
                    case SetDialogueRuntimeNodeWithPreviousActor dialogueNode:
                        await setDialogueExecutor.ExecuteAsync(dialogueNode, this);
                        node = dialogueNode.Next[0];
                        break;
                    case WaitForInputRuntimeNode waitNode:
                        await waitForInputExecutor.ExecuteAsync(waitNode, this);
                        node = waitNode.Next[0];
                        break;
                    case MultiChoiceRuntimeNode multiChoiceNode:
                        await multiChoiceExecutor.ExecuteAsync(multiChoiceNode, this);
                        node = multiChoiceNode.Next[0];
                        break;
                    default:
                        Debug.LogError($"No executor found for node type: {node.GetType()}");
                        break;
                }
            }

            animator.SetBool("active", false);
        }
    }
}
