using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class ActorManager : MonoBehaviour
{
    Animator pcAnim;
    [SerializeField] Image pcImage;
    [SerializeField] Image npcImage;

    [Space]

    Animator npcAnim;
    [SerializeField] Sprite[] pcSprites;
    [SerializeField] Sprite[] npcSprites;
    private void Awake()
    {
        pcAnim = pcImage.gameObject.GetComponent<Animator>();
        npcAnim = npcImage.gameObject.GetComponent<Animator>();
    }
    void OnEnable()
    {
        Lua.RegisterFunction(nameof(ShowPC), this, SymbolExtensions.GetMethodInfo(() => ShowPC(0)));
        Lua.RegisterFunction(nameof(DisablePC), this, SymbolExtensions.GetMethodInfo(() => DisablePC()));
        Lua.RegisterFunction(nameof(ShowNPC), this, SymbolExtensions.GetMethodInfo(() => ShowNPC(0)));
        Lua.RegisterFunction(nameof(DisableNPC), this, SymbolExtensions.GetMethodInfo(() => DisableNPC()));
    }

    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(ShowPC));
        Lua.UnregisterFunction(nameof(DisablePC));
        Lua.UnregisterFunction(nameof(ShowNPC));
        Lua.UnregisterFunction(nameof(DisableNPC));
    }
    public void ShowPC(double sprite = 0)
    {
        pcAnim.SetTrigger("FadeIn");
        pcImage.sprite = pcSprites[(int)sprite];
        pcImage.enabled = true;
    }
    public void DisablePC()
    {
        pcAnim.SetTrigger("FadeOut");
        pcImage.enabled = false;
    }
    public void ShowNPC(double sprite = 0)
    {
        npcAnim.SetTrigger("FadeIn");
        npcImage.sprite = npcSprites[(int)sprite];
        npcImage.enabled = true;
    }
    public void DisableNPC()
    {
        npcAnim.SetTrigger("FadeOut");
        npcImage.enabled = false;
    }
}
