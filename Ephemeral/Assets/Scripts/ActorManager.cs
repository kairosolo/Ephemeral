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
    bool saveLoaded = false;
    bool luaSet = false;
    private void Awake()
    {
        pcAnim = pcImage.gameObject.GetComponent<Animator>();
        npcAnim = npcImage.gameObject.GetComponent<Animator>();
    }
    void OnEnable()
    {

    }

    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(ShowPC));
        Lua.UnregisterFunction(nameof(ShowNPC));
    }
    private void Update()
    {
        if (luaSet == false && DialogueManager.isConversationActive)
        {
            luaSet = true;
            Lua.RegisterFunction(nameof(ShowPC), this, SymbolExtensions.GetMethodInfo(() => ShowPC(0)));
            Lua.RegisterFunction(nameof(ShowNPC), this, SymbolExtensions.GetMethodInfo(() => ShowNPC(0)));
        }

        if (saveLoaded == false && DialogueManager.isConversationActive)
        {
            saveLoaded = true;
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
    }
    public void LoadSavedActors()
    {
        saveLoaded = false;
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
        luaSet = false;
        pcImage.enabled = false;
        npcImage.enabled = false;
    }
}
