using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class SonarPulse : MonoBehaviour
{
    [Header("Sonar Ping Prefab")]
    [SerializeField] private Transform sonarPing;

    [Header("Max Range, Speed, and Fade Out Settings")]
    [SerializeField] private float rangeMax;
    [SerializeField] private float rangeSpeed;
    [SerializeField] private float fadeRange;
    private float range;

    [Header("Sonar Energy Cost & Stamina Bar Access")]
    [SerializeField] private float sonarCost;
    [SerializeField] public PlayerMovement stamina; //Ability to access stamina bar

    private Transform pulseTransform;
    private SpriteRenderer pulseSpriteRenderer;
    private Color pulseColor;
    private List<Collider2D> pingedCollidersList;

    private bool sonarEnabled;
    private AudioManager audioManager;

    private void Awake()
    {
        pulseTransform = transform.Find("Pulse");
        pulseSpriteRenderer = pulseTransform.GetComponent<SpriteRenderer>();
        pulseColor = pulseSpriteRenderer.color;
        pingedCollidersList = new List<Collider2D>();
        range = 0f;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) //enable and disable the sonar with the right mouse button
        {
            audioManager.PlaySFX(audioManager.sonarActivation);//play the sonar activation sound

            if (!sonarEnabled)
            {
                setSonarEnabled(true);
                pulseSpriteRenderer.enabled = true;
            }
            else
            {
                setSonarEnabled(false);
                pulseSpriteRenderer.enabled = false;
                range = 0f; //reset the range so it won't appear to start from where it left off if re-enabled
            }
        }

        if (!sonarEnabled)
        {
            pulseSpriteRenderer.enabled = false;
            range = 0f; //reset the range so it won't appear to start from where it left off if re-enabled
        }

        if (stamina.getCurrentStamina() == 0)
        {
            setSonarEnabled(false);
            pulseSpriteRenderer.enabled = false;
            range = 0f; //reset the range so it won't appear to start from where it left off if re-enabled
        }


        if (sonarEnabled)
        {
            range += rangeSpeed * Time.deltaTime;
            if (range > rangeMax)
            {
                range = 0f;
                pingedCollidersList.Clear();
            }
            pulseTransform.localScale = new Vector3(range, range);

            RaycastHit2D[] raycastHit2DArray = Physics2D.CircleCastAll(transform.position, range / 2f, Vector2.zero);
            foreach (RaycastHit2D raycastHit2D in raycastHit2DArray)
            {
                if (raycastHit2D.collider != null)
                {
                    if (!pingedCollidersList.Contains(raycastHit2D.collider))
                    {
                        pingedCollidersList.Add(raycastHit2D.collider);
                        //Debug.Log("Ping! Location: " + raycastHit2D.point);
                        if (raycastHit2D.collider.gameObject.layer != 7)
                        {
                            Transform sonarPingTransform = Instantiate(sonarPing, raycastHit2D.point, Quaternion.identity);
                            SonarPing sonarPingMarker = sonarPingTransform.GetComponent<SonarPing>();
                            sonarPingMarker.setFadeTimer(rangeMax / rangeSpeed);
                            sonarPingMarker.setColor(new Color(0, 1, 0.02951145f));
                        }
                    }
                }
            }

            //make the sonar ring start to fade away as it moves out towards the edge of its range
            if (range > rangeMax - fadeRange)
            {
                pulseColor.a = Mathf.Lerp(0f, 1f, (rangeMax - range) / fadeRange);
            }
            else
            {
                pulseColor.a = 1f;
            }
            pulseSpriteRenderer.color = pulseColor;

            //drain the energy cost from the stamina bar
            stamina.drainStamina(sonarCost);
        }
    }

    [YarnCommand("setSonar")]
    public void setSonarEnabled(bool sonarOn)
    {
        audioManager.PlaySFX(audioManager.sonarActivation);//play the sonar activation sound
        sonarEnabled = sonarOn;
    }

    public void tutorialEnable()
    {
        setSonarEnabled(true);
        pulseSpriteRenderer.enabled = true;
    }

    public void tutorialDisable()
    {
        setSonarEnabled(false);
        pulseSpriteRenderer.enabled = false;
        range = 0f; //reset the range so it won't appear to start from where it left off if re-enabled
    }
}
