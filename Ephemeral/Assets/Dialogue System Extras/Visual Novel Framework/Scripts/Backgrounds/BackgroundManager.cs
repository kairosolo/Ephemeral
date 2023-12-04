using UnityEngine;
using System.Collections;
using System;

namespace PixelCrushers.DialogueSystem.VisualNovelFramework
{

    /// <summary>
    /// Manages the current background image.
    /// </summary>
    public class BackgroundManager : MonoBehaviour
    {

        public UnityEngine.UI.Image background;
        public string backgroundVariable = "Background";
        public string backgroundFadeDurationVariable = "BackgroundFadeDuration";

        [Tooltip("Seconds to wait after fading to black before fading in again. If zero, no fade. If non-zero, will use Dialogue Manager's Scene Transition Manager to fade.")]
        public float fadeDuration = 0;

        private static BackgroundManager m_instance = null;

        private void Awake()
        {
            m_instance = this;
            if (background != null) background = GetComponentInChildren<UnityEngine.UI.Image>();
            Lua.RegisterFunction("BackgroundFadeDuration", this, SymbolExtensions.GetMethodInfo(() => BackgroundFadeDuration((double)0)));
        }

        private void OnDestroy()
        {
            m_instance = null;
        }

        private void Start()
        {
            UpdateBackgroundFromVariable();
        }

        private void OnApplyPersistentData()
        {
            UpdateBackgroundFromVariable();
        }

        public void BackgroundFadeDuration(double duration)
        {
            fadeDuration = (float)duration;
            DialogueLua.SetVariable(backgroundFadeDurationVariable, duration);
        }

        public static void UpdateBackgroundFromVariable()
        {
            if (m_instance == null) return;
            SetBackgroundImage(DialogueLua.GetVariable(m_instance.backgroundVariable).AsString);
            if (DialogueLua.DoesVariableExist(m_instance.backgroundFadeDurationVariable))
            {
                 m_instance.fadeDuration = DialogueLua.GetVariable(m_instance.backgroundFadeDurationVariable).asFloat;
            }
        }

        private static string m_backgroundName;

        public static void SetBackgroundImage(string backgroundName)
        {
            if (string.IsNullOrEmpty(backgroundName) || string.Equals(backgroundName, "nil")) return;
            m_backgroundName = backgroundName;
            if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: Setting background image to '" + backgroundName + "'.");
            DialogueLua.SetVariable(m_instance.backgroundVariable, backgroundName);
            DialogueManager.LoadAsset(backgroundName, typeof(Sprite), OnAssetLoaded);
        }

        private static void OnAssetLoaded(UnityEngine.Object asset)
        {
            var image = asset as Sprite;
            if (image == null && asset is Texture2D)
            {
                var texture = asset as Texture2D;
                image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
            }
            if (image == null)
            {
#if USE_ADDRESSABLES
                UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Sprite>(m_backgroundName).Completed += OnSpriteLoaded;
#else
                Debug.LogWarning("Dialogue System: Can't load background image '" + m_backgroundName + "'. Is the name correct?");
#endif
            }
            else
            {
                m_instance.StartCoroutine(m_instance.SetBackgroundImageCoroutine(image));
            }
        }

#if USE_ADDRESSABLES
        private static void OnSpriteLoaded(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> obj)
        {
            var image = obj.Result as Sprite;
            if (image == null)
            {
                Debug.LogWarning("Dialogue System: Can't load background image '" + m_backgroundName + "'. Is the name correct?");
            }
            else
            {
                m_instance.StartCoroutine(m_instance.SetBackgroundImageCoroutine(image));
            }
        }
#endif

        private IEnumerator SetBackgroundImageCoroutine(Sprite image)
        {
            if (Mathf.Approximately(0, fadeDuration))
            {
                m_instance.background.sprite = image;
                yield break;
            }
            var sceneTransitionManager = DialogueManager.instance.GetComponentInChildren<StandardSceneTransitionManager>();
            if (sceneTransitionManager == null)
            {
                m_instance.background.sprite = image;
                yield break;
            }
            else
            {
                sceneTransitionManager.leaveSceneTransition.TriggerAnimation();
                yield return new WaitForSeconds(sceneTransitionManager.leaveSceneTransition.animationDuration + fadeDuration);
                m_instance.background.sprite = image;
                sceneTransitionManager.enterSceneTransition.TriggerAnimation();
            }
        }

    }
}