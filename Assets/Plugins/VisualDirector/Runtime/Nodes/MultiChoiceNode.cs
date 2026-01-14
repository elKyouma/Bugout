using System;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace VisualDirector
{
    [Serializable]
    public class MultiChoiceRuntimeNode : VisualDirectorRuntimeNode
    {
        public string choide1;
        public string choide2;
        public string choide3;
        public string choide4;
    }

    public class MultiChoiceExecutor : IVisualDirectorNodeExecutor<MultiChoiceRuntimeNode>
    {
        public async Task ExecuteAsync(MultiChoiceRuntimeNode runtimeNode, VisualDirector ctx)
        {
            // Set up choice UI here (not implemented in this snippet)
            ctx.choice1.text = runtimeNode.choide1;
            ctx.choice2.text = runtimeNode.choide2;
            //ctx.choice3.text = runtimeNode.choide3;
            //ctx.choice4.text = runtimeNode.choide4;
            ctx.SetChoiceAmount(runtimeNode.choide4 != "" ? 4 : runtimeNode.choide3 != "" ? 3 : runtimeNode.choide2 != "" ? 2 : 1);
            await ctx.InputProvider.InputDetected();
            ctx.SetChoiceAmount(0); // Hide choices after selection
        }
    }
}
