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

    [Space]

    [SerializeField] int savedPCImage;
    [SerializeField] int savedNPCImage;
    private void Awake()
    {
        pcAnim = pcImage.gameObject.GetComponent<Animator>();
        npcAnim = npcImage.gameObject.GetComponent<Animator>();
    }
    void OnEnable()
    {
        Lua.RegisterFunction(nameof(ShowPC), this, SymbolExtensions.GetMethodInfo(() => ShowPC(0)));
        Lua.RegisterFunction(nameof(ShowNPC), this, SymbolExtensions.GetMethodInfo(() => ShowNPC(0)));
    }

    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(ShowPC));
        Lua.UnregisterFunction(nameof(ShowNPC));
    }
    public void SetSavedActors()
    {
        StartCoroutine(SetSavedActorsDelay());
    }
    IEnumerator SetSavedActorsDelay()
    {
        yield return new WaitForSeconds(.1f);
        savedPCImage = DialogueLua.GetVariable("PCImage").AsInt;
        savedNPCImage = DialogueLua.GetVariable("NPCImage").AsInt;

        pcImage.sprite = pcSprites[savedPCImage];
        npcImage.sprite = npcSprites[savedNPCImage];

        Debug.Log(savedPCImage + savedNPCImage);
        if (savedPCImage == 0)
        {
            pcImage.enabled = false;
        }
        else
        {
            pcImage.enabled = true;
        }

        if (savedNPCImage == 0)
        {
            npcImage.enabled = false;
        }
        else
        {
            npcImage.enabled = true;
        }
    }
    public void ShowPC(double sprite = 0)
    {
        if(sprite == 0) { pcAnim.SetTrigger("FadeOut"); return; }
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
