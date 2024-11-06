using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sun : MonoBehaviour
{
    public float fadeDuration = 5f;
    private SpriteRenderer spriteRenderer;
    private float fadeTimer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        fadeTimer += Time.deltaTime;

        float fadeAmount = fadeTimer / fadeDuration;

        Color spriteColor = spriteRenderer.color;
        spriteColor.a = Mathf.Clamp01(1 - fadeAmount);
        spriteRenderer.color = spriteColor;

        if (fadeAmount >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
