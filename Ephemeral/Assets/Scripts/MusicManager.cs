using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private bool startFadedOut = false;
    private bool fading = false;

    [SerializeField] private int savedMusic;
    [SerializeField] private int savedMusicVolume;
    private float targetVolume;
    private float currentVolume = 0.0f;

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        Lua.UnregisterFunction(nameof(MusicFadeIn));
        Lua.UnregisterFunction(nameof(MusicFadeOut));
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Lua.RegisterFunction(nameof(MusicFadeIn), this, SymbolExtensions.GetMethodInfo(() => MusicFadeIn(1, 0, 0)));
        Lua.RegisterFunction(nameof(MusicFadeOut), this, SymbolExtensions.GetMethodInfo(() => MusicFadeOut(1)));
    }

    private void Update()
    {
        if (!fading) return;

        float volumeChangeRate = 1.0f / fadeDuration;

        currentVolume = Mathf.MoveTowards(currentVolume, targetVolume, volumeChangeRate * Time.deltaTime);

        audioSource.volume = currentVolume;

        if (currentVolume == targetVolume)
        {
            DialogueLua.SetVariable("MusicVolume", audioSource.volume);
            fading = false;
        }
    }

    public void LoadSavedMusic()
    {
        savedMusic = DialogueLua.GetVariable("Music").AsInt;
        audioSource.volume = DialogueLua.GetVariable("MusicVolume").asFloat;

        if (savedMusic == 0)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.clip = audioClips[savedMusic];
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