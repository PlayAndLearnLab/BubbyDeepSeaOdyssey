using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonarPing : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float fadeTimer;
    private float fadeTimerMax;
    private Color color;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        fadeTimer = 0f;
        fadeTimerMax = 2f;
        color = new Color(1, 1, 1, 1f);
    }

    private void Update()
    {
        fadeTimer += Time.deltaTime;

        color.a = Mathf.Lerp(fadeTimerMax, 0f, fadeTimer / fadeTimerMax);
        spriteRenderer.color = color;

        if (fadeTimer >= fadeTimerMax)
        {
            Destroy(gameObject);
        }
    }

    public void setColor(Color c)
    {
        this.color = c;
    }

    public void setFadeTimer(float fadeTimeMax)
    {
        this.fadeTimerMax = fadeTimeMax;
        fadeTimer = 0f;
    }
}
