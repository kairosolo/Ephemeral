using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class BlackOverlay : MonoBehaviour
{

    [SerializeField] Image background;
    float fadeDuration = 1f;
    float currentAlpha = 0.0f;
    float targetAlpha;
    
    bool fadeIn = false;
    bool fading = false;

    void OnEnable()
    {

        Lua.RegisterFunction(nameof(FadeIn), this, SymbolExtensions.GetMethodInfo(() => FadeIn(0f)));
        Lua.RegisterFunction(nameof(FadeSet), this, SymbolExtensions.GetMethodInfo(() => FadeSet(0f, 0f)));
        Lua.RegisterFunction(nameof(FadeOut), this, SymbolExtensions.GetMethodInfo(() => FadeOut(0f)));
    }
    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(FadeIn));
        Lua.UnregisterFunction(nameof(FadeOut));
        Lua.UnregisterFunction(nameof(FadeSet));
    }
    private void Update()
    {
        if (!fading) return;

        float alphaChangeRate = 1.0f / fadeDuration;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, alphaChangeRate * Time.deltaTime);

        background.color = new Color(background.color.r, background.color.g, background.color.b, currentAlpha);

        if (currentAlpha == targetAlpha)
        {
            // Change the target alpha to start fading in/out again
            targetAlpha = targetAlpha == 0.0f ? 1.0f : 0.0f;
            fading = false;
            if(fadeIn == false)
            {
                background.raycastTarget = false;
            }
        }
    }
    public void FadeSet(float fadeInSpeed = 1f, float fadeOutSpeed = 1f)
    {
        StartCoroutine(FadeSetCor(fadeInSpeed, fadeOutSpeed));
    }
    IEnumerator FadeSetCor(float fadeInSpeed, float fadeOutSpeed)
    {
        FadeIn(fadeInSpeed);
        yield return new WaitForSecondsRealtime(2);
        FadeOut(fadeOutSpeed);
    }
    public void FadeIn(float fadeSpeed = 1f) 
    {
        background.raycastTarget = true;
        currentAlpha = 0;
        targetAlpha = 1;
        fading = true;
        fadeIn = true;
        this.fadeDuration = fadeSpeed;
    }
    public void FadeOut(float fadeSpeed = 1f)
    {
        currentAlpha = 1;
        targetAlpha = 0;
        fading = true;
        fadeIn = false;
        this.fadeDuration = fadeSpeed;
    }
        
}
