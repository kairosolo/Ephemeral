using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class BackgroundHandler : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Sprite[] backgrounds;
    [SerializeField] int savedBackground;
    bool saveLoaded = false;
    bool luaSet = false;
    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(SetBackground));
    }
    private void Update()
    {
        if(luaSet == false && DialogueManager.isConversationActive)
        {
            luaSet = true;
            Lua.RegisterFunction(nameof(SetBackground), this, SymbolExtensions.GetMethodInfo(() => SetBackground(0)));
        }

        if (saveLoaded == false && DialogueManager.isConversationActive)
        {
            saveLoaded = true;
            savedBackground = DialogueLua.GetVariable("Background").AsInt;

            if (savedBackground == 0)
            {
                background.enabled = false;
            }
            else
            {
                background.sprite = backgrounds[savedBackground];
                background.enabled = true;
            }

        }
    }
    public void LoadSavedBackground()
    {
        saveLoaded = false;
    }
    public void ResetBackground()
    {
        luaSet = false;
        background.sprite = backgrounds[0];
        background.enabled = false;
    }
    public void SetBackground(double num)
    {
        StartCoroutine(SetBackgroundCor(num));
    }
    IEnumerator SetBackgroundCor(double num)
    {
        yield return new WaitForSecondsRealtime(1);
        DialogueLua.SetVariable("Background", num);
        if (backgrounds[(int)num] == null)
        {
            background.sprite = null;
            background.enabled = false;
        }
        else
        {
            background.enabled = true;
            background.sprite = backgrounds[(int)num];
        }

    }
}
