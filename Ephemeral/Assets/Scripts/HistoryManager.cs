using System.Collections;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using TMPro;

public class HistoryManager : MonoBehaviour
{
    [SerializeField] private DialogueSkipper dialogueSkipper;
    [SerializeField] private TMP_Text historyText;
    [SerializeField] private GameObject historyPanel;
    private string saveHistory;

    public void LoadSavedHistory()
    {
        saveHistory = DialogueLua.GetVariable("History").asString;
        Debug.Log(saveHistory);
        historyText.text = saveHistory;
    }

    public void ResetHistory()
    {
        historyText.text = "";
    }

    public void SaveHistory()
    {
        DialogueLua.SetVariable("History", historyText.text);
    }

    public void AddLine()
    {
        StartCoroutine(AddLineDelay());
    }

    private IEnumerator AddLineDelay()
    {
        yield return new WaitForSeconds(.05f);
        string currentDialogue = DialogueManager.currentConversationState.subtitle.formattedText.text;

        currentDialogue = currentDialogue.Replace("\\.", "").Replace("\\,", "");

        string speaker = $"{DialogueManager.currentConversationState.subtitle.speakerInfo.nameInDatabase}:";

        if (historyText.text.Contains(currentDialogue)) { yield break; }
        if (speaker == "Narrator:") { speaker = null; }

        string completeLine = $"{speaker} {currentDialogue}\n\n";
        historyText.text += completeLine;
    }

    public void AddHistory(string gameHistory)
    {
        if (!historyText.text.Contains(gameHistory))
        {
            historyText.text = "";
            historyText.text += gameHistory;
        }
    }

    public void Open()
    {
        historyPanel.SetActive(true);
    }

    public void Close()
    {
        historyPanel.SetActive(false);
    }
}