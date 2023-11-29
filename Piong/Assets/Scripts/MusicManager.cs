using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;

    [SerializeField] float fadeDuration = 1.0f;
    [SerializeField] bool startFadedOut = false;
    bool fading = false;

    [SerializeField] int savedMusic;
    float targetVolume;
    float currentVolume = 0.0f;
    void OnEnable()
    {
        Lua.RegisterFunction(nameof(MusicFadeIn), this, SymbolExtensions.GetMethodInfo(() => MusicFadeIn(1, 0, 0)));
        Lua.RegisterFunction(nameof(MusicFadeOut), this, SymbolExtensions.GetMethodInfo(() => MusicFadeOut(1)));
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
    public void SetSavedMusic()
    {
        StartCoroutine(SetSavedMusicDelay());
    }
    IEnumerator SetSavedMusicDelay()
    {
        yield return new WaitForSeconds(.1f);
        savedMusic = DialogueLua.GetVariable("Music").AsInt;
        audioSource.clip = audioClips[savedMusic];
        audioSource.volume = 1;
        if (savedMusic == 0)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.Play();
        }
    }
    public void MusicFadeIn(float targetVolume, float fadeDuration = 1f, double clipNum = 0)
    {
        DialogueLua.SetVariable("Music", clipNum);

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
