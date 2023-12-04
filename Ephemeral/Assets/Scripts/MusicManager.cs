using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

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
    private bool saveLoaded = false;
    private bool luaSet = false;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction(nameof(MusicFadeIn));
        Lua.UnregisterFunction(nameof(MusicFadeOut));
    }

    private void Update()
    {
        if (luaSet == false && DialogueManager.isConversationActive)
        {
            luaSet = true;
            Lua.RegisterFunction(nameof(MusicFadeIn), this, SymbolExtensions.GetMethodInfo(() => MusicFadeIn(1, 0, 0)));
            Lua.RegisterFunction(nameof(MusicFadeOut), this, SymbolExtensions.GetMethodInfo(() => MusicFadeOut(1)));
        }

        if (saveLoaded == false && DialogueManager.isConversationActive)
        {
            saveLoaded = true;
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
        saveLoaded = false;
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

    public void ResetMusic()
    {
        luaSet = false;
    }
}