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
    void OnEnable()
    {
        Lua.RegisterFunction(nameof(SetBackground), this, SymbolExtensions.GetMethodInfo(() => SetBackground(0)));
    }

    void OnDisable()
    {
        Lua.UnregisterFunction(nameof(SetBackground));
    }
    public void SetSavedBackground()
    {
        StartCoroutine(SetSavedBackgroundDelay());
    }
    IEnumerator SetSavedBackgroundDelay()
    {
        yield return new WaitForSeconds(.1f);
        savedBackground = DialogueLua.GetVariable("Background").AsInt;
        background.sprite = backgrounds[savedBackground];

        if (backgrounds[savedBackground] == null)
        {
            background.enabled = false;
        }
        else
        {
            background.enabled = true;
        }
        Debug.Log(savedBackground);
    }
    public void ResetBackground()
    {
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
        if(backgrounds[(int)num] == null)
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
