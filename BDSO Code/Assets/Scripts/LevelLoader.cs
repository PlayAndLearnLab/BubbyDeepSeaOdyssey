using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    [YarnCommand("loadNextLevel")]
    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    [YarnCommand("loadMainMenu")]
    public void LoadMainMenu()
    {
        StartCoroutine(LoadLevel(0));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        //Play "complete!" sfx
        if (SceneManager.GetActiveScene().name != "Homecoming")
        {
            audioManager.PlaySFX(audioManager.goalReached, true);
        }

        //Play animation
        transition.SetTrigger("Start");

        //Wait for animation to play
        yield return new WaitForSeconds(transitionTime);

        //Load the next scene
        SceneManager.LoadScene(levelIndex);
    }
}
