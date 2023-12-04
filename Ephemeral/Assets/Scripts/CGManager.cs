using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CGManager : MonoBehaviour
{
    [SerializeField] private GalleryManager galleryManager;
    [SerializeField] private Image cg;
    public List<Sprite> cgList;
    [SerializeField] private int savedCG;
    private bool saveLoaded = false;
    private bool luaSet = false;
    private void OnDisable()
    {
        Lua.UnregisterFunction(nameof(SetCG));
    }

    private void Update()
    {
        DeleteTestSaveCGGallery();
        if (luaSet == false && DialogueManager.isConversationActive)
        {
            luaSet = true;
            Lua.RegisterFunction(nameof(SetCG), this, SymbolExtensions.GetMethodInfo(() => SetCG(0)));
        }
        if (saveLoaded == false && DialogueManager.isConversationActive)
        {
            saveLoaded = true;

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
    }

    public void LoadSavedCG()
    {
        saveLoaded = false;
    }

    public void ResetCG()
    {
        luaSet = false;
        cg.sprite = cgList[0];
        cg.enabled = false;
    }

    public void SetCG(double num)
    {
        StartCoroutine(SetCGCor(num));
    }

    private IEnumerator SetCGCor(double num)
    {
        yield return new WaitForSecondsRealtime(1);
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
    public void SaveCGGallery(int cgNum)
    {
        galleryManager.AddSavedCG(cgNum);
    }

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