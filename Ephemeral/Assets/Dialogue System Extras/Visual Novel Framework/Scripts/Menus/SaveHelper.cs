using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCrushers.DialogueSystem.VisualNovelFramework
{
    /// <summary>
    /// Provides methods for slot-based saving and loading of games.
    /// Saves in PlayerPrefs. You can make a subclass and override
    /// the methods to save elsewhere.
    ///
    /// - Dialogue System variable: CurrentStage
    /// </summary>
    public class SaveHelper : MonoBehaviour
    {
        [SerializeField] private LoadGamePanel loadGamePanel;
        [SerializeField] private BlackOverlay blackOverlay;
        [SerializeField] private BackgroundHandler backgroundHandler;
        [SerializeField] private ActorManager actorManager;
        [SerializeField] private MusicManager musicManager;
        [SerializeField] private CGManager cgManager;
        [SerializeField] private HistoryManager historyManager;

        [ConversationPopup(true)]
        public string conversation = "Start Conversation";

        public string emptySlotText = "-empty-";
        public string slotText = "Slot";
        public string timeText = "Time";

        public int mainMenuScene = 0;
        public string firstGameplaySceneName = "Gameplay";

        public int currentSlotNum { get; set; }

        protected bool m_startConversationAfterLoadingScene = false;
        protected bool m_hasInitialized = false;

        protected virtual void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected virtual void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (m_startConversationAfterLoadingScene && !DialogueManager.IsConversationActive)
            {
                DialogueManager.StartConversation(conversation);
            }
            m_startConversationAfterLoadingScene = false;
            //if (m_hasInitialized)
            {
                var menus = GetComponent<Menus>();
                var isStartScene = scene.buildIndex == mainMenuScene;
                var isCreditsScene = string.Equals(scene.name, menus.creditsScene);
                menus.startPanel.gameObject.SetActive(isStartScene);
                var showGameplayPanel = !(isStartScene || isCreditsScene);
                menus.gameplayPanel.SetActive(showGameplayPanel);
            }
            m_hasInitialized = true;
        }

        protected string GetLastSavedGameKey()
        {
            return "savedgame_lastSlotNum";
        }

        protected string GetSlotSummaryKey(int slotNum)
        {
            return "savedgame_" + slotNum + "_summary";
        }

        protected string GetSlotDetailsKey(int slotNum)
        {
            return "savedgame_" + slotNum + "_details";
        }

        /*        protected string GetSlotDataKey(int slotNum)
                {
                    return "savedgame_" + slotNum + "_data";
                }
        */

        public virtual bool IsGameSavedInSlot(int slotNum)
        {
            return SaveSystem.HasSavedGameInSlot(slotNum);
        }

        public virtual string GetLocalizedText(string text)
        {
            return DialogueManager.GetLocalizedText(text);
        }

        public virtual string GetEmptySlotText()
        {
            return GetLocalizedText(emptySlotText);
        }

        public virtual string GetSlotSummary(int slotNum)
        {
            return SaveSystem.HasSavedGameInSlot(slotNum) ? PlayerPrefs.GetString(GetSlotSummaryKey(slotNum)) : emptySlotText;
        }

        public virtual string GetSlotDetails(int slotNum)
        {
            return SaveSystem.HasSavedGameInSlot(slotNum) ? PlayerPrefs.GetString(GetSlotDetailsKey(slotNum)) : string.Empty;
        }

        public virtual string GetCurrentSummary(int slotNum)
        {
            return GetLocalizedText(slotText) + " " + slotNum + "\n" + GetLocalizedText(timeText) + ": " + System.DateTime.Now;
        }

        public virtual string GetCurrentDetails(int slotNum)
        {
            var details = GetCurrentSummary(slotNum);
            if (DialogueLua.DoesVariableExist("CurrentStage"))
            {
                details += "\n" + DialogueLua.GetVariable("CurrentStage").AsString;
            }
            return details;
        }

        public virtual bool HasLastSavedGame()
        {
            return PlayerPrefs.HasKey(GetLastSavedGameKey());
        }

        public virtual void SaveGame(int slotNum)
        {
            historyManager.SaveHistory();
            SaveSystem.SaveToSlot(slotNum);
            PlayerPrefs.SetString(GetSlotSummaryKey(slotNum), GetCurrentSummary(slotNum));
            PlayerPrefs.SetString(GetSlotDetailsKey(slotNum), GetCurrentDetails(slotNum));
            PlayerPrefs.SetInt(GetLastSavedGameKey(), slotNum);
        }

        public virtual void LoadGame(int slotNum)
        {
            blackOverlay.FadeSetVanilla(.5f, 1, 1);
            StartCoroutine(LoadGameCor(slotNum));
        }

        public IEnumerator LoadGameCor(int slotNum)
        {
            yield return new WaitForSeconds(.5f);
            loadGamePanel.Close();
            SceneManager.LoadScene("LoadingScene");
            SaveSystem.LoadFromSlot(slotNum);
        }

        public void LoadSaves()
        {
            backgroundHandler.LoadSavedBackground();
            actorManager.LoadSavedActors();
            musicManager.LoadSavedMusic();
            cgManager.LoadSavedCG();
            historyManager.LoadSavedHistory();
        }

        public virtual void LoadLastSavedGame()
        {
            if (HasLastSavedGame())
            {
                LoadGame(PlayerPrefs.GetInt(GetLastSavedGameKey()));
            }
        }

        public virtual void LoadCurrentSlot()
        {
            LoadGame(currentSlotNum);
        }

        public virtual void RestartGame()
        {
            ResetGame();
            blackOverlay.FadeSetVanilla(.5f, .5f, 1);
            StartCoroutine(RestartGameDelay());
        }

        private IEnumerator RestartGameDelay()
        {
            yield return new WaitForSeconds(1f);
            //SaveSystem.RestartGame(firstGameplaySceneName);
            m_startConversationAfterLoadingScene = true;
            SceneManager.LoadScene("LoadingScene");
            Tools.LoadLevel(firstGameplaySceneName);
#if USE_INK
            DialogueManager.AddDatabase(FindObjectOfType<PixelCrushers.DialogueSystem.InkSupport.DialogueSystemInkIntegration>().database);
            FindObjectOfType<PixelCrushers.DialogueSystem.InkSupport.DialogueSystemInkIntegration>().ResetStories();
#endif
        }

        public virtual void DeleteSavedGame(int slotNum)
        {
            SaveSystem.DeleteSavedGameInSlot(slotNum);
            PlayerPrefs.DeleteKey(GetSlotSummaryKey(slotNum));
            PlayerPrefs.DeleteKey(GetSlotDetailsKey(slotNum));
            var lastSlotNum = PlayerPrefs.GetInt(GetLastSavedGameKey());
            if (lastSlotNum == slotNum) PlayerPrefs.DeleteKey(GetLastSavedGameKey());
        }

        public virtual void ReturnToMenu()
        {
            musicManager.MusicFadeOut(.5f);
            blackOverlay.FadeSetVanilla(.5f, .5f, 1);
            DialogueManager.StopConversation();
            DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
            StartCoroutine(ReturnToMenuDelay());
        }

        private IEnumerator ReturnToMenuDelay()
        {
            yield return new WaitForSeconds(.5f);
            Tools.LoadLevel(mainMenuScene);
            var menus = FindObjectOfType<Menus>();
            menus.startPanel.Open();
        }

        private void ResetGame()
        {
            DialogueSkipper.isEndSkip = false;
            backgroundHandler.ResetBackground();
            actorManager.ResetActors();
            cgManager.ResetCG();
            historyManager.ResetHistory();
        }
    }
}