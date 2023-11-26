using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    BlackOverlay blackOverlay;
    MusicManager musicManager;
    private void Awake()
    {
        blackOverlay = FindObjectOfType<BlackOverlay>();
        musicManager = FindObjectOfType<MusicManager>();
    }
    public void PlayGame()
    {
        musicManager.MusicFadeOut(1f);
        blackOverlay.FadeIn(1f);
        StartCoroutine(PlayGameCor());
    }
    IEnumerator PlayGameCor()
    {
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadScene(1);

    }
}
