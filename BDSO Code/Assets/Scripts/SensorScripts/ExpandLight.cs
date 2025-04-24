using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Yarn.Unity;

public class ExpandLight : MonoBehaviour
{
    [SerializeField] private Light2D lt; //The point light object
    [SerializeField] private float maxRange;//The maximum range of the light
    [SerializeField] private float originalRange; //The minimum/original range of the light
    [SerializeField] private float expandSpeed = 0.01f; //Controls the speed at which the light expands/contracts
    [SerializeField] private PlayerMovement player; //Player to drain stamina from
    [SerializeField] private float staminaDrain; //How much stamina expanding the light should drain
    private bool cameraEnabled; //allows the intro animation to turn on the light without needing to hold down the shift key
    private float range; //The current range of the light
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        lt = GetComponent<Light2D>();
        range = originalRange;
        cameraEnabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            audioManager.PlaySFX(audioManager.cameraActivation);
        } else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            audioManager.StopSFX();
        }

            if ((Input.GetKey(KeyCode.LeftShift)) || (cameraEnabled)) //Hold left shift to activate/hold light expansion
        {
            if(player.getCurrentStamina() > 0)
            {
                if (range < maxRange)
                {
                    range += expandSpeed;
                    lt.pointLightOuterRadius = range;
                }
                player.drainStamina(staminaDrain); //drain stamina for as long as the light is expanded
            }
            audioManager.PlaySFX(audioManager.cameraActivation);
        }
        else if (range > originalRange)
        {
            range -= (expandSpeed * 2);//light contracts faster than it expands
            lt.pointLightOuterRadius = range;
        }

    }

    [YarnCommand("cameraEnabled")]
    public void cameraOn(bool enabledState)
    {
        cameraEnabled = enabledState;
    }
}