using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;

    public float fadeDuration = 1.0f; // Duration of the fade in seconds
    public bool startFadedOut = false; // Start with the audio faded out
    bool fading = false;

    private float targetVolume;
    private float currentVolume = 0.0f;
    private void Awake()
    {

        Lua.RegisterFunction(nameof(MusicFadeIn), this, SymbolExtensions.GetMethodInfo(() => MusicFadeIn(1, 0, 0)));
        Lua.RegisterFunction(nameof(MusicFadeOut), this, SymbolExtensions.GetMethodInfo(() => MusicFadeOut(1)));
    }
    void OnEnable()
    {

    }
    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(MusicFadeIn));
        Lua.UnregisterFunction(nameof(MusicFadeOut));
    }
    void Update()
    {
        if (!fading) return;
        float volumeChangeRate = 1.0f / fadeDuration;

        currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, volumeChangeRate * Time.deltaTime);

        audioSource.volume = currentVolume;

        if (currentVolume == targetVolume)
        {
            // Change the target volume to start fading in/out again
            targetVolume = targetVolume == 0.0f ? audioSource.volume : 0.0f;
            fading = false;
        }
    }
    public void MusicFadeIn(float targetVolume, float fadeDuration = 1f, double clipNum = 0)
    {
        this.targetVolume = targetVolume;
        currentVolume = audioSource.volume;
        audioSource.volume = currentVolume;
        audioSource.clip = audioClips[(int)clipNum];
        fading = true;
        this.fadeDuration = fadeDuration;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    public void MusicFadeOut(float fadeDuration = 1f)
    {
        this.targetVolume = 0;
        currentVolume = audioSource.volume;
        fading = true;
        this.fadeDuration = fadeDuration;
    }
}
