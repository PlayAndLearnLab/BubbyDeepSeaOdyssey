using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class YarnInteractable : MonoBehaviour {
    [SerializeField] public string conversationStartNode;

    private DialogueRunner dialogueRunner;
    private bool interactable = true;
    private bool isCurrentConversation = false;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void Start() {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        //if (interactable && !dialogueRunner.IsDialogueRunning) { - version of the line that doesn't allow dialogue interruption
        if (interactable) {
            if (other.CompareTag("Player"))
            {
                if (dialogueRunner.IsDialogueRunning)
                { 
                    dialogueRunner.Stop(); //end the current dialogue
                }

                PlayerMovement bubby = other.gameObject.GetComponent<PlayerMovement>();

                if (this.CompareTag("SONAR") || this.CompareTag("LIDAR"))
                {
                    bubby.drainStamina(30f); //if the dialogue node is one of the obstacle ones, drain 2 stamina
                    Debug.Log("Energy drained on collision!");
                    audioManager.PlaySFX(audioManager.hitObstacle, true); //play sound effect for hit an obstacle
                }

                if (!DialogueTracker.Instance.nodesSeen.Contains(conversationStartNode)) //if this node hasn't played before
                {
                    DialogueTracker.Instance.addNode(conversationStartNode); //add this conversation node to the list of seen nodes
                    StartConversation();
                } else if (this.CompareTag("LIDAR"))
                {
                    //play the ouch command if it's one of the lidar obstacle nodes
                    conversationStartNode = "OuchLidar";
                    StartConversation();
                }
                else if (this.CompareTag("SONAR"))
                {
                    //play the ouch command if it's one of the sonar obstacle nodes
                    conversationStartNode = "OuchSonar";
                    StartConversation();
                } 
            }
        }
    }

    private void StartConversation() {
        isCurrentConversation = true;
        dialogueRunner.StartDialogue(conversationStartNode);
    }

    private void EndConversation() {
        if (isCurrentConversation) {
            isCurrentConversation = false;
        }
    }

    [YarnCommand("disable")]
    public void DisableConversation() {
        interactable = false;
    }
}