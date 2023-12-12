using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;

public class ActorManager : MonoBehaviour
{
    private Animator pcAnim;
    [SerializeField] private Image pcImage;
    [SerializeField] private Image npcImage;

    [Space]
    private Animator npcAnim;

    [SerializeField] private Sprite[] pcSprites;
    [SerializeField] private Sprite[] npcSprites;

    [Space]
    [SerializeField] private int savedPCImage;

    [SerializeField] private int savedNPCImage;

    private void Awake()
    {
        pcAnim = pcImage.gameObject.GetComponent<Animator>();
        npcAnim = npcImage.gameObject.GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Lua.UnregisterFunction(nameof(ShowPC));
        Lua.UnregisterFunction(nameof(ShowNPC));
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Lua.RegisterFunction(nameof(ShowPC), this, SymbolExtensions.GetMethodInfo(() => ShowPC(0)));
        Lua.RegisterFunction(nameof(ShowNPC), this, SymbolExtensions.GetMethodInfo(() => ShowNPC(0)));
    }

    public void LoadSavedActors()
    {
        savedPCImage = DialogueLua.GetVariable("PCImage").AsInt;
        savedNPCImage = DialogueLua.GetVariable("NPCImage").AsInt;

        if (savedPCImage == 0)
        {
            pcImage.enabled = false;
        }
        else
        {
            pcImage.sprite = pcSprites[savedPCImage];
            pcImage.enabled = true;
        }

        if (savedNPCImage == 0)
        {
            npcImage.enabled = false;
        }
        else
        {
            npcImage.sprite = npcSprites[savedNPCImage];
            npcImage.enabled = true;
        }
    }

    public void ShowPC(double sprite = 0)
    {
        if (sprite == 0) { pcAnim.SetTrigger("FadeOut"); return; }
        DialogueLua.SetVariable("PCImage", sprite);
        pcAnim.SetTrigger("FadeIn");
        pcImage.sprite = pcSprites[(int)sprite];
        pcImage.enabled = true;
    }

    public void ShowNPC(double sprite = 0)
    {
        if (sprite == 0) { npcAnim.SetTrigger("FadeOut"); return; }
        DialogueLua.SetVariable("NPCImage", sprite);
        npcAnim.SetTrigger("FadeIn");
        npcImage.sprite = npcSprites[(int)sprite];
        npcImage.enabled = true;
    }

    public void ResetActors()
    {
        pcImage.enabled = false;
        npcImage.enabled = false;
    }
}