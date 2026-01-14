using System;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace VisualDirector
{
    [Serializable]
    public class SetDialogueRuntimeNode : VisualDirectorRuntimeNode
    {
        public string ActorName;
        public Sprite ActorSprite;
        public int LocationIndex;
        public string DialogueText;
    }

    [Serializable]
    public class SetDialogueRuntimeNodeWithPreviousActor : VisualDirectorRuntimeNode
    {
        public string DialogueText;
    }

    public class SetDialogueExecutor :
        IVisualDirectorNodeExecutor<SetDialogueRuntimeNode>,
        IVisualDirectorNodeExecutor<SetDialogueRuntimeNodeWithPreviousActor>
    {
        public async Task ExecuteAsync(SetDialogueRuntimeNode runtimeNode, VisualDirector ctx)
        {
            if (string.IsNullOrEmpty(runtimeNode.DialogueText))
            {
                ctx.DialoguePanel.SetActive(false);
                return;
            }

            ctx.DialoguePanel.SetActive(true);
            ctx.ActorNameText.text = runtimeNode.ActorName;

            foreach (var location in ctx.ActorLocationList)
                location.enabled = false;

            if (runtimeNode.ActorSprite != null)
            {
                var img = ctx.ActorLocationList[runtimeNode.LocationIndex];
                img.enabled = true;
                img.sprite = runtimeNode.ActorSprite;
            }

            await TypeTextWithSkipAsync(runtimeNode.DialogueText, ctx);
        }

        public async Task ExecuteAsync(SetDialogueRuntimeNodeWithPreviousActor runtimeNode, VisualDirector ctx)
        {
            if (string.IsNullOrEmpty(runtimeNode.DialogueText))
            {
                ctx.DialoguePanel.SetActive(false);
                return;
            }

            ctx.DialoguePanel.SetActive(true);
            await TypeTextWithSkipAsync(runtimeNode.DialogueText, ctx);
        }

        static async Task TypeTextWithSkipAsync(string dialogueText, VisualDirector ctx)
        {
            var label = ctx.DialogueText;
            var delayPerCharSeconds = ctx.GlobalTextDelayPerCharacter;
            var inputProvider = ctx.InputProvider;

            label.text = "";
            var builder = new StringBuilder();
            var insideRichTag = false;

            var skipInputDetected = inputProvider.InputDetected();

            foreach (var c in dialogueText)
            {
                if (c == '<')
                    insideRichTag = true;

                builder.Append(c);

                if (c == '>')
                    insideRichTag = false;

                if (insideRichTag || char.IsWhiteSpace(c)) continue;

                label.text = builder.ToString();
                var timer = 0f;
                while (timer < delayPerCharSeconds)
                {
                    if (skipInputDetected.IsCompleted)
                    {
                        label.text = dialogueText;
                        return;
                    }
                    timer += Time.deltaTime;
                    await Task.Yield();
                }
            }

            label.text = dialogueText;
        }
    }
}
