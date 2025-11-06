using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;
using Yarn.Unity;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 7f; //Controls player speed
    [SerializeField] private SpriteRenderer bubby;

    public bool canMove = true; //Enables the player to move or not
    [SerializeField] private Transform spawn; //Sets the spawn point for the player
    [SerializeField] private Transform finish;
    private Rigidbody2D rb; //Rigidbody component

    public float acceleration; //Controls how fast the player accelerates
    public float decceleration; //Controls how fast the player deccelerates
    public float velPower;
    private Vector3 lastPosition; // Keeps track of the position the player was in last frame

    private bool facingRight = true;  // For determining which way the player is currently facing.

    [SerializeField] private float rotationSpeed = .001f; //Determines how fast the player rotates to face direction of movement
    private float currentRotation = 0;

    [SerializeField] public Image staminaBar;
    [SerializeField] private float staminaRegen;
    private float stamina, maxStamina;
    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private Coroutine regen;

    [SerializeField] private Light2D globalLight; //The global light object
    private float highLightLevel = 0.04f;
    private float lowLightLevel = 0.005f;
    private float currentLightLevel;
    private float maxDistance;

    [SerializeField] private GameObject spaceMessage;

    private int treasureCount;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canMove = false;
        lastPosition = transform.position;
        transform.position = spawn.transform.position;
        //timerOn = true;
        maxStamina = 10;
        stamina = 10;
        currentLightLevel = 0.07f;
        maxDistance = Vector2.Distance(spawn.position, finish.position);
        treasureCount = 0;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            float movementX = CalculateMovement(moveHorizontal, rb.velocity.x);
            float movementY = CalculateMovement(moveVertical, rb.velocity.y);
            rb.AddForce(movementX * Vector2.right);
            rb.AddForce(movementY * Vector2.up);

            if ((moveVertical > 0.3))
            {
                if (facingRight)
                {
                    currentRotation += rotationSpeed;
                }
                else if (!facingRight)
                {
                    currentRotation -= rotationSpeed;
                }
            }
            else if ((moveVertical < -0.3))
            {
                if (facingRight)
                {
                    currentRotation -= rotationSpeed;
                }
                else if (!facingRight)
                {
                    currentRotation += rotationSpeed;
                }
            }
            else
            {
                if (currentRotation != 0)
                {
                    if (currentRotation > 2)
                    {
                        currentRotation -= rotationSpeed;
                    }
                    else if (currentRotation < -2)
                    {
                        currentRotation += rotationSpeed;
                    }
                }
            }
            currentRotation = Mathf.Clamp(currentRotation, -45f, 45f);
            bubby.transform.localRotation = Quaternion.Euler(0f, 0f, currentRotation);
        }
    }

    void Update()
    {
        //flip Bubby so he's facing the direction of movement
        if (transform.position.x > lastPosition.x && !facingRight)
        {
            Flip();
        }
        else if (transform.position.x < lastPosition.x && facingRight)
        {
            Flip();
        }

        //change light level as Bubby moves into deeper/shallower water
        float distanceFromSpawn = Vector2.Distance(transform.position, spawn.position);
        currentLightLevel = Mathf.Lerp(lowLightLevel, highLightLevel, 1 - distanceFromSpawn / maxDistance);
        globalLight.intensity = currentLightLevel;


        var currentPosition = transform.position;
        lastPosition = currentPosition;

        if ((stamina/maxStamina) > .2f)
        {
            staminaBar.color = new Color(0, 1, 0.02951145f); //energy bar green above 20% energy left
        }
        else
        {
            staminaBar.color = new Color(0.7264151f, 0, 0.02070081f); //energy bar red at low levels
            audioManager.PlaySFX(audioManager.energyLow);//play the low energy level sound
        }
    }

    private float CalculateMovement(float value, float velocityVal)
    {
        float targetSpeed = value * speed;
        float speedDiff = targetSpeed - velocityVal;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);
        return movement;
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        if (facingRight)
        {
            facingRight = false;
        }
        else
        {
            facingRight = true;
        }

        // Multiply the sprite's x local scale by -1 to flip them
        Vector3 theScale = bubby.transform.localScale;
        theScale.x *= -1;
        bubby.transform.localScale = theScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Finish")
        {
            Debug.Log("Congratulations, you made it to the end!");
        }
        else if (other.tag == "Signpost")
        {
            other.gameObject.transform.GetChild(2).gameObject.SetActive(true);
            Debug.Log("Added signpost to map");
        }
    }

    [YarnCommand("playerCanMove")]
    public void enablePlayerMove()
    {
        canMove = true;

    }
    [YarnCommand("playerCantMove")]
    public void disablePlayerMove()
    {
        canMove = false;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
    }

    public float getCurrentStamina()
    {
        return stamina;
    }

    public void drainStamina(float staminaDrainAmount)
    {
        stamina -= staminaDrainAmount * Time.deltaTime;
        if (stamina < 0) stamina = 0;
        staminaBar.fillAmount = stamina / maxStamina;
        if (regen != null)
        {
            StopCoroutine(regen);
        }
        regen = StartCoroutine(RegenStamina());
    }

    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(5f);

        while (stamina < maxStamina)
        {
            stamina += staminaRegen;
            if (stamina > maxStamina) stamina = maxStamina;
            staminaBar.fillAmount = stamina / maxStamina;
            yield return regenTick;
        }
        regen = null;
    }

    [YarnCommand("foundTreasure")]
    public void foundTreasure()
    {
        treasureCount += 1;
    }

    [YarnCommand("enableFinish")]
    public void enableFinish()
    {
        finish.transform.GetChild(0).gameObject.SetActive(true);
    }
}