using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueTextSpeed : MonoBehaviour
{
    [SerializeField] TextMeshProTypewriterEffect textMeshProTypewriter;
    void SetTypewriterSpeed(int charsPerSec)
    {
        // Make sure typewriter will always finish by {{end}}:
        charsPerSec = (int)Mathf.Max(charsPerSec, DialogueManager.DisplaySettings.subtitleSettings.subtitleCharsPerSecond);

        textMeshProTypewriter.charactersPerSecond = charsPerSec;
    }
}
