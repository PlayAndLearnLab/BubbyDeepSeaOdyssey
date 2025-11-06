using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool nextSceneOnEnd = true;
    [SerializeField] private string nextScene = "MainMenu";
    [SerializeField] private LevelLoader lloader;
    [SerializeField] private PlayableDirector cutscene;
    [SerializeField] private PlayerMovement player; //player object so you can halt/enable controls

    // Start is called before the first frame update
    void Start()
    {
        if (playOnStart)
        {
            player.disablePlayerMove(); //make sure the player can't mess with the controls while the timeline plays
            cutscene.Play();
        }
        else
        {
            cutscene.Stop();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((nextSceneOnEnd) && (cutscene.state != PlayState.Playing))
        {
            if (nextScene == "MainMenu")
            {
                lloader.LoadMainMenu();
            }
            else
            {
                lloader.LoadNextLevel();
            }
        }
    }

    public void playCutscene()
    {
        cutscene.Play();
    }

    public void cutsceneReset()
    {
        cutscene.Stop();
        cutscene.time = 0;
        cutscene.Evaluate();
    }
}
