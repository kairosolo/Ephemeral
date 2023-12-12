using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackgroundHandler : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Sprite[] backgrounds;
    [SerializeField] private int savedBackground;

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Lua.UnregisterFunction(nameof(SetBackground));
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Lua.RegisterFunction(nameof(SetBackground), this, SymbolExtensions.GetMethodInfo(() => SetBackground(0)));
    }

    public void LoadSavedBackground()
    {
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

    public void ResetBackground()
    {
        background.sprite = backgrounds[0];
        background.enabled = false;
    }

    public void SetBackground(double num)
    {
        if (!DialogueSkipper.isEndSkip) { SetBackgroundVanilla(num); }
        else { StartCoroutine(SetBackgroundCor(num)); }
    }

    public void SetBackgroundVanilla(double num)
    {
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

    private IEnumerator SetBackgroundCor(double num)
    {
        yield return new WaitForSecondsRealtime(1);
        SetBackgroundVanilla(num);
    }
}