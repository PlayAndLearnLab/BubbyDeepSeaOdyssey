using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueTracker : MonoBehaviour
{
    public static DialogueTracker Instance;

    public List<string> nodesSeen; //which dialogue nodes the player has already seen

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        nodesSeen = new List<string>();
    }

    void OnEnable()
    {
        Debug.Log("OnEnable called, scene loaded");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void addNode(string conversationNode)
    {
        nodesSeen.Add(conversationNode);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            nodesSeen.Clear(); //reset the nodes seen when you get to the main menu page 
        }
    }
}
