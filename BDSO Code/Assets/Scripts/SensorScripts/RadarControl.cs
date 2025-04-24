using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class RadarControl : MonoBehaviour
{
    [SerializeField] private float rotationsPerMinute = 10.0f; //how fast 
    [SerializeField] private float radarSpread = 10.0f; //how far can the radar detect out from the player 
    [SerializeField] private float radarCost; //How much energy the radar costs
    [SerializeField] public PlayerMovement stamina; //Ability to access stamina bar
    private LineRenderer lineRend;
    private bool radarEnabled;
    private DialogueRunner dialogueRunner;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        lineRend = gameObject.GetComponent<LineRenderer>();
        lineRend.positionCount = 3;
        radarEnabled = false;
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the triangle to create the radar radius
        transform.Rotate(0, 0, 6.0f * rotationsPerMinute * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Mouse0)) //enable and disable the radar with the left click
        {
            if (!radarEnabled)
            {
                setRadarEnabled(true);
                audioManager.PlaySFX(audioManager.lidarActivation);
            }
            else
            {
                setRadarEnabled(false);
                audioManager.PlaySFX(audioManager.lidarActivation);
            }
        }

        if (stamina.getCurrentStamina() == 0)
        {
            setRadarEnabled(false);
        }
    }

    private void FixedUpdate()
    {
        if (radarEnabled){
            // Cast a ray straight up.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, radarSpread);

            //Draw the radar line
            lineRend.enabled = true;
            Vector2 midpoint = transform.position + transform.up * (radarSpread/2);
            Vector2 end = transform.position + transform.up * radarSpread;
            lineRend.SetPosition(0, transform.position);
            lineRend.SetPosition(1, midpoint);
            lineRend.SetPosition(2, end);

            //drain the energy cost from the stamina bar
            stamina.drainStamina(radarCost);

            // If it hits something...
            if (hit.collider != null)
            {
                //Debug.Log("Hit detected at " + hit.point);
                GameObject radarParticle = ObjectPooler.SharedInstance.GetPooledObject();
                if (radarParticle != null)
                {
                    radarParticle.transform.position = hit.point;
                    radarParticle.transform.rotation = Quaternion.LookRotation(hit.normal);
                    radarParticle.SetActive(true);
                    StartCoroutine(DelayReturn(radarParticle));
                }
            }
        }
        else
        {
            lineRend.enabled = false;
        }
    }

    private IEnumerator DelayReturn(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        obj.SetActive(false);
    }

    public bool getRadarEnabled()
    {
        return radarEnabled;
    }

    [YarnCommand("setLIDAR")]
    public void setRadarEnabled(bool radarOn)
    {
        radarEnabled = radarOn;
        audioManager.PlaySFX(audioManager.lidarActivation);
    }

}
