using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace VisualDirector
{
    [Serializable]
    public class DisableInteractivityRuntimeNode : VisualDirectorRuntimeNode
    {
        public string Tag;
    }

    public class DisableInteractivityExecutor : IVisualDirectorNodeExecutor<DisableInteractivityRuntimeNode>
    {
        public async Task ExecuteAsync(DisableInteractivityRuntimeNode node, VisualDirector ctx)
        {
            Assert.IsNotNull(ctx.currentInteractable, "No interactable to disable.");

            if (node.Tag == "")
                ctx.currentInteractable?.Disable();
            else
                GameObject.FindGameObjectsWithTag(node.Tag).First().GetComponent<IDisabable>().Disable();
        }
    }
}
