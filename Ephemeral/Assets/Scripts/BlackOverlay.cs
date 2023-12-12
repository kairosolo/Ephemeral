using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;

public class BlackOverlay : MonoBehaviour
{
    [SerializeField] private Image background;
    private float fadeDuration = 1f;
    private float currentAlpha = 0.0f;
    private float targetAlpha;

    private bool fadeIn = false;
    private bool fading = false;

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        Lua.UnregisterFunction(nameof(FadeIn));
        Lua.UnregisterFunction(nameof(FadeOut));
        Lua.UnregisterFunction(nameof(FadeSet));
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Lua.RegisterFunction(nameof(FadeIn), this, SymbolExtensions.GetMethodInfo(() => FadeIn(0f)));
        Lua.RegisterFunction(nameof(FadeSet), this, SymbolExtensions.GetMethodInfo(() => FadeSet(0f, 0f, 0f)));
        Lua.RegisterFunction(nameof(FadeOut), this, SymbolExtensions.GetMethodInfo(() => FadeOut(0f)));
    }

    private void Update()
    {
        if (!fading) return;

        float alphaChangeRate = 1.0f / fadeDuration;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, alphaChangeRate * Time.deltaTime);

        background.color = new Color(background.color.r, background.color.g, background.color.b, currentAlpha);

        if (currentAlpha == targetAlpha)
        {
            fading = false;
            if (fadeIn == false)
            {
                background.raycastTarget = false;
            }
        }
    }

    public void FadeSet(float fadeInSpeed = 1f, float fadeOutSpeed = 1f, float waitSpeed = 2)
    {
        if (!DialogueSkipper.isEndSkip) return;
        FadeSetVanilla(fadeInSpeed, fadeOutSpeed, waitSpeed);
    }

    public void FadeSetVanilla(float fadeInSpeed = 1f, float fadeOutSpeed = 1f, float waitSpeed = 2)
    {
        Debug.Log("Should Fade");
        StartCoroutine(FadeSetCor(fadeInSpeed, fadeOutSpeed, waitSpeed));
    }

    private IEnumerator FadeSetCor(float fadeInSpeed, float fadeOutSpeed, float waitSpeed = 2)
    {
        FadeInVanilla(fadeInSpeed);
        yield return new WaitForSecondsRealtime(waitSpeed);
        FadeOutVanilla(fadeOutSpeed);
    }

    public void FadeIn(float fadeSpeed = 1f)
    {
        if (!DialogueSkipper.isEndSkip) return;
        FadeInVanilla(fadeSpeed);
    }

    public void FadeInVanilla(float fadeSpeed = 1f)
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
        if (!DialogueSkipper.isEndSkip) return;
        FadeOutVanilla(fadeSpeed);
    }

    public void FadeOutVanilla(float fadeSpeed = 1f)
    {
        Time.timeScale = 1;
        currentAlpha = 1;
        targetAlpha = 0;
        fading = true;
        fadeIn = false;
        this.fadeDuration = fadeSpeed;
    }
}