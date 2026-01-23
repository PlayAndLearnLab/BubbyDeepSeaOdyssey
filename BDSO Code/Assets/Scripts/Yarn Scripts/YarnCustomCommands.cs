using UnityEngine;
using Yarn.Unity;
using System.Collections;

public class YarnCustomCommands : MonoBehaviour
{
    public DialogueRunner dialogueRunner;
    public LineView lineView; // Ensure this is linked in the Inspector!

    void Awake()
    {
        dialogueRunner.AddCommandHandler<float>("hold", HoldDialogue);
    }

    private IEnumerator HoldDialogue(float duration)
    {
        // 1. Manually tell the Line View NOT to finish yet
        lineView.UserRequestedViewAdvancement();

        // 2. Wait the required time
        yield return new WaitForSeconds(duration);

        // 3. The Runner will move to the next line naturally now
    }
}