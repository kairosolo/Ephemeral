using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.QuickSave;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    [SerializeField] private CGManager cgManager;
    [SerializeField] private GameObject galleryPanel;
    [SerializeField] private Image inspectImage;
    [SerializeField] private List<Image> cgSlotList;
    private List<Sprite> cgList;

    private QuickSaveWriter writer;
    private QuickSaveReader reader;

    private void Awake()
    {
        cgList = cgManager.cgList;
    }

    private void Start()
    {
        writer = QuickSaveWriter.Create("Gallery");
        if (writer.Exists("CG"))
        {
            reader = QuickSaveReader.Create("Gallery");
        }
    }

    public void Open()
    {
        galleryPanel.SetActive(true);

        if (reader == null) return;

        SavedCgs = reader.Read<List<MyType>>("CG");

        foreach (var item in SavedCgs)
        {
            cgSlotList[item.savedcgs - 1].sprite = cgList[item.savedcgs];
        }
    }

    public void Close()
    {
        galleryPanel.SetActive(false);
    }

    public void OpenInspectImage(int num)
    {
        foreach (var item in SavedCgs)
        {
            if (num != item.savedcgs) return;
        }

        inspectImage.gameObject.SetActive(true);
        inspectImage.sprite = cgList[num];
    }

    public void CloseInspectImage()
    {
        inspectImage.gameObject.SetActive(false);
    }

    public void AddSavedCG(int num)
    {
        MyType newSavedCG = new MyType(num);

        foreach (var item in SavedCgs)
        {
            if (num == item.savedcgs) return;
            Debug.Log(num);
        }

        SavedCgs.Add(newSavedCG);

        writer.Write<List<MyType>>("CG", SavedCgs)
        .Commit();

        reader = QuickSaveReader.Create("Gallery");
    }

    public void LoadGallery(List<MyType> SavedCGs)
    {
        foreach (var item in SavedCGs)
        {
            Debug.Log(cgList[item.savedcgs]);
        }
    }

    public List<MyType> SavedCgs = new List<MyType>();

    [System.Serializable]
    public class MyType
    {
        public int savedcgs;

        public MyType(int savedcgs)
        {
            this.savedcgs = savedcgs;
        }
    }
}