using CI.QuickSave;
using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

public class DialogueSkipper : MonoBehaviour
{
    [SerializeField] private HistoryManager historyManager;
    [SerializeField] private AbstractDialogueUI abstractDialogueUI;
    public static bool isEndSkip = false;

    [TextArea(10, 20)]
    [SerializeField] private string gameHistory;

    private QuickSaveWriter writer;
    private QuickSaveReader reader;
    [SerializeField] private UnityEngine.UI.Toggle skipToggle;

    public bool skipping;

    public void IsSkipping()
    {
        skipping = skipToggle.isOn;
        writer.Write<bool>("SkipRead", skipping).Commit();
        if (skipping)
        {
            isEndSkip = false;
        }
    }

    private void Awake()
    {
        InitializeQuickSave();
    }

    public void InitializeQuickSave()
    {
        writer = QuickSaveWriter.Create("DialogueSkipper");

        LoadGameHistory();
    }

    public void LoadGameHistory()
    {
        if (!writer.Exists("GameHistory")) { writer.Write<string>("GameHistory", gameHistory).Commit(); }
        if (!writer.Exists("SkipRead")) { writer.Write<bool>("SkipRead", skipToggle.isOn).Commit(); }

        reader = QuickSaveReader.Create("DialogueSkipper");
        gameHistory = reader.Read<string>("GameHistory");
        skipping = reader.Read<bool>("SkipRead");
        skipToggle.isOn = skipping;
    }

    public void AddLine()
    {
        string currentDialogue = $"{DialogueManager.currentConversationState.subtitle.formattedText.text}\n";
        currentDialogue = currentDialogue.Replace("\\.", "").Replace("\\,", "");

        if (gameHistory.Contains(currentDialogue)) { return; }

        gameHistory += currentDialogue;
        writer.Write<string>("GameHistory", gameHistory)
        .Commit();

        reader = QuickSaveReader.Create("DialogueSkipper");
    }

    public void CheckIfSkippable()
    {
        StartCoroutine(CheckIfSkippableCor());
    }

    public IEnumerator CheckIfSkippableCor()
    {
        yield return new WaitForSeconds(.1f);

        SkipDialogue();
    }

    public void SkipDialogue()
    {
        Debug.Log("Should skip");
        if (!skipping || !DialogueManager.isConversationActive) { isEndSkip = true; return; }

        string currentDialogue = $"{DialogueManager.currentConversationState.subtitle.formattedText.text}\n";
        currentDialogue = currentDialogue.Replace("\\.", "").Replace("\\,", "");

        if (gameHistory.Contains(currentDialogue))
        {
            StartCoroutine(CheckIfSkippableCor());
            abstractDialogueUI.OnContinue();
            historyManager.AddHistory(gameHistory);
        }
        else
        {
            isEndSkip = true;
        }
    }
}