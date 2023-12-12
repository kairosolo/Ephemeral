using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CGManager : MonoBehaviour
{
    [SerializeField] private GalleryManager galleryManager;
    [SerializeField] private Image cg;
    public List<Sprite> cgList;
    [SerializeField] private int savedCG;
    private bool saveLoaded = false;

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Lua.UnregisterFunction(nameof(SetCG));
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Lua.RegisterFunction(nameof(SetCG), this, SymbolExtensions.GetMethodInfo(() => SetCG(0)));
    }

    private void Update()
    {
        DeleteTestSaveCGGallery();
    }

    public void LoadSavedCG()
    {
        savedCG = DialogueLua.GetVariable("CG").AsInt;

        if (savedCG == 0)
        {
            cg.enabled = false;
        }
        else
        {
            cg.sprite = cgList[savedCG];
            cg.enabled = true;
        }
    }

    public void ResetCG()
    {
        cg.sprite = cgList[0];
        cg.enabled = false;
    }

    public void SetCG(double num)
    {
        if (!DialogueSkipper.isEndSkip)
        {
            SetCGVanilla(num);
        }
        else { StartCoroutine(SetCGCor(num)); }
    }

    public void SetCGVanilla(double num)
    {
        DialogueLua.SetVariable("CG", num);
        if (cgList[(int)num] == null)
        {
            cg.sprite = null;
            cg.enabled = false;
        }
        else
        {
            cg.enabled = true;
            cg.sprite = cgList[(int)num];
            SaveCGGallery((int)num);
        }
    }

    private IEnumerator SetCGCor(double num)
    {
        yield return new WaitForSecondsRealtime(1);
        SetCGVanilla(num);
    }

    public void SaveCGGallery(int cgNum)
    {
        galleryManager.AddSavedCG(cgNum);
    }

    //Delete Later
    public void DeleteTestSaveCGGallery()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            galleryManager.AddSavedCG(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            galleryManager.AddSavedCG(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            galleryManager.AddSavedCG(3);
        }
    }
}