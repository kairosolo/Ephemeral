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
    bool luaSet = false;
    void OnEnable()
    {


    }
    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(FadeIn));
        Lua.UnregisterFunction(nameof(FadeOut));
        Lua.UnregisterFunction(nameof(FadeSet));
    }
    private void Update()
    {
        if (luaSet == false && DialogueManager.isConversationActive)
        {
            luaSet = true;
            Lua.RegisterFunction(nameof(FadeIn), this, SymbolExtensions.GetMethodInfo(() => FadeIn(0f)));
            Lua.RegisterFunction(nameof(FadeSet), this, SymbolExtensions.GetMethodInfo(() => FadeSet(0f, 0f, 0f)));
            Lua.RegisterFunction(nameof(FadeOut), this, SymbolExtensions.GetMethodInfo(() => FadeOut(0f)));
        }
        if (!fading) return;

        float alphaChangeRate = 1.0f / fadeDuration;
            
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, alphaChangeRate * Time.deltaTime);

        background.color = new Color(background.color.r, background.color.g, background.color.b, currentAlpha);

        if (currentAlpha == targetAlpha)
        {
            fading = false;
            if(fadeIn == false)
            {
                background.raycastTarget = false;
            }
        }
    }

    public void FadeSet(float fadeInSpeed = 1f, float fadeOutSpeed = 1f, float waitSpeed = 2)
    {
        StartCoroutine(FadeSetCor(fadeInSpeed, fadeOutSpeed));
    }
    IEnumerator FadeSetCor(float fadeInSpeed, float fadeOutSpeed, float waitSpeed = 2)
    {
        FadeIn(fadeInSpeed);
        yield return new WaitForSecondsRealtime(waitSpeed);
        FadeOut(fadeOutSpeed);
    }
    public void FadeIn(float fadeSpeed = 1f) 
    {
        Time.timeScale = 1;
        background.raycastTarget = true;
        currentAlpha = 0;
        targetAlpha = 1;
        fading = true;
        fadeIn = true;
        this.fadeDuration = fadeSpeed;
    }
    public void FadeOut(float fadeSpeed = 1f)
    {
        Time.timeScale = 1;
        currentAlpha = 1;
        targetAlpha = 0;
        fading = true;
        fadeIn = false;
        this.fadeDuration = fadeSpeed;
    }
    public void ResetBlackOverlay()
    {
        luaSet = false;
    }
}
